using Authentication.DAL.Models;
using Microsoft.AspNetCore.Http;

namespace Authentication.Services.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(string userId, string role);
    void SetTokenCookie(HttpResponse response, string cookieName, string token, int expiryMinutes);
}