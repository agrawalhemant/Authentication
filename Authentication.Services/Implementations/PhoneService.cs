using Authentication.Contracts.Config;
using Authentication.DAL.Interfaces;
using Authentication.DAL.Models;
using Authentication.Services.Interfaces;
using Authentication.Utility;
using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Authentication.Services.Implementations;

public class PhoneService : IPhoneService
{
    private readonly IPhoneVerificationRepository _phoneVerificationRepository;
    private readonly IUserRepository _userRepository;
    private readonly TwilioSettings _twilioSettings;
    
    public PhoneService(IPhoneVerificationRepository phoneVerificationRepository, IUserRepository userRepository, IOptions<TwilioSettings> twilioSettings)
    {
        _phoneVerificationRepository = phoneVerificationRepository;
        _userRepository = userRepository;
        _twilioSettings = twilioSettings.Value;
        TwilioClient.Init(_twilioSettings.ApiKey, _twilioSettings.ApiSecret, _twilioSettings.AccountSid);
    }
    
    public async Task SendVerificationSmsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetUserByIdAsync(userId, cancellationToken);
        if(user is null)
            throw new Exception("User not found.");
        if(user.phonenumber is null)
            throw new Exception("User does not have a phone number.");
        
        var code = Verification.GenerateVerificationCode();
        var expiresAt = DateTime.UtcNow.AddMinutes(10);
        var phoneVerification = new PhoneVerification
        {
            id = Guid.NewGuid(),
            userid = userId,
            otp = code,
            expiresat = expiresAt,
            isused = false,
            createdat = DateTime.UtcNow
        };
        await _phoneVerificationRepository.AddPhoneVerificationAsync(phoneVerification, cancellationToken);
        await SendSmsAsync($"Your verification code is {code}\nThis will expire in 10 minutes.", user.phonenumber);
    }

    public async Task<(bool status, string message)> VerifyPhoneAsync(Guid userId, string code, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetUserByIdAsync(userId, cancellationToken);
        if(user is null)
            return (false, "User not found.");
        if(user.phonenumber is null)
            throw new Exception("User does not have a phone number.");
        
        var phoneVerification = await _phoneVerificationRepository.GetPhoneVerificationAsync(userId, code, cancellationToken);
        if(phoneVerification is null)
            return (false, "Verification code is invalid.");
        if(phoneVerification.isused)
            return (false, "Verification code is already used.");
        if(phoneVerification.expiresat < DateTime.UtcNow)
            return (false, "Verification code has expired.");
        
        phoneVerification.isused = true;
        phoneVerification.expiresat = DateTime.UtcNow.AddMinutes(-1);
        await _phoneVerificationRepository.UpdatePhoneVerificationAsync(phoneVerification, cancellationToken);
        
        user.isphoneverified = true;
        await _userRepository.UpdateUserAsync(user, cancellationToken);
        return (true, "Phone number verified successfully.");
    }

    private async Task SendSmsAsync(string message, string toPhoneNumber)
    {
        await MessageResource.CreateAsync(
            body: message,
            from: new PhoneNumber(_twilioSettings.FromPhoneNumber),
            to: new PhoneNumber(toPhoneNumber)
        );
    }
}