namespace Authentication.Contracts.Config;

public class TwilioSettings
{
    public string AccountSid { get; set; } = null!;
    public string ApiKey { get; set; } = null!;
    public string ApiSecret { get; set; } = null!;
    public string FromPhoneNumber { get; set; } = null!;
}