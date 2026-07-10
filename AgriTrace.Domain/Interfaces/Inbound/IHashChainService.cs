namespace AgriTrace.Domain.Interfaces.Inbound;


public interface IHashChainService
{

    string ComputeHash(
        string previousHash,
        string eventData);

}