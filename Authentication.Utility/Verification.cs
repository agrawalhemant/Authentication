namespace Authentication.Utility;

public static class Verification
{
    public static string GenerateVerificationCode()
    {
        return (new Random().Next(100000, 999999)).ToString();
    }
}