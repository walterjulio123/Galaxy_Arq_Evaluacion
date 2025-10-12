namespace Walter.Evaluacion.ApiPedidos.Services
{
    public interface IHttpClientService
    {
        Task<T?> PostAsync<T>(string url, object data);
    }
}
