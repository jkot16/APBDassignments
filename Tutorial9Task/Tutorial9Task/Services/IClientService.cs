namespace Tutorial9Task.Services;

public interface IClientService
{
    Task<bool> DeleteClientAsync(int idClient, CancellationToken cancellationToken);
}