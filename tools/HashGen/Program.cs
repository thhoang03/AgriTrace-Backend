using System;
using System.Security.Cryptography;

namespace HashGen;

class Program
{
    static void Main(string[] args)
    {
        string password = args.Length > 0 ? args[0] : "Admin@123";
        byte[] salt = RandomNumberGenerator.GetBytes(16);
        byte[] key = Rfc2898DeriveBytes.Pbkdf2(password, salt, 100_000, HashAlgorithmName.SHA256, 32);
        string saltB64 = Convert.ToBase64String(salt);
        string keyB64 = Convert.ToBase64String(key);
        string hash = string.Concat("100000.", saltB64, ".", keyB64);
        Console.WriteLine(hash);
    }
}
