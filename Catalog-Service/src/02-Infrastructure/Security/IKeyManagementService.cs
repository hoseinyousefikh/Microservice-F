namespace Catalog_Service.src._02_Infrastructure.Security
{
    public interface IKeyManagementService
    {
        Task<string> GenerateKeyAsync(string keyId);
        Task<bool> ValidateKeyAsync(string keyId, string key);
        Task RevokeKeyAsync(string keyId);
        Task<bool> IsKeyRevokedAsync(string keyId);
    }
}
