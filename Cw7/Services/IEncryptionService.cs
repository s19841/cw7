namespace Cw7.Services
{
    public interface IEncryptionService
    {
        string Encrypt(string value);
        bool Verify(string candidatePlainString, string existingHashString);
    }
}