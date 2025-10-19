namespace Walter.Evaluacion.ApiPedidos.Services
{
    public interface IHttpClientService
    {
        //Task<T?> PostAsync<T>(string url, object data);

        //// Nueva sobrecarga: permite enviar token de autorización del cliente
        Task<T?> PostAsync<T>(string url, object data, string token);
    }
}
