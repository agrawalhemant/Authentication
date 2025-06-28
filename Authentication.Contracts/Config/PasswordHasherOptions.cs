namespace Authentication.Contracts.Config;

public class PasswordHasherOptions
{
    public int Iterations { get; set; } = 10000;
    public int SaltSize { get; set; } = 16; // bytes
    public int HashSize { get; set; } = 32; // bytes
}