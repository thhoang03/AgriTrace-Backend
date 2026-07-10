using System.Security.Cryptography;
using System.Text;
using AgriTrace.Domain.Interfaces.Inbound;


namespace AgriTrace.Domain.Services;


public class HashChainService : IHashChainService
{


    public string ComputeHash(
        string previousHash,
        string eventData)
    {


        var rawData =
            $"{previousHash}{eventData}";


        using var sha256 =
            SHA256.Create();



        var bytes =
            Encoding.UTF8.GetBytes(rawData);



        var hash =
            sha256.ComputeHash(bytes);



        return Convert.ToHexString(hash);

    }

}