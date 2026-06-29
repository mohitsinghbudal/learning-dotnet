using System.Security.Cryptography;
using System.Text;

public static class PaymentHelper
{
    public static string GenerateSignature(string message, string secretKey)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
        return Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(message)));
    }
}