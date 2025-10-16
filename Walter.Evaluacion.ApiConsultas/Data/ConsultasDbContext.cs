using MongoDB.Driver;
using Walter.Evaluacion.ApiConsultas.Models;

namespace Walter.Evaluacion.ApiConsultas.Data
{
    public class ConsultasDbContext
    {
        private readonly IMongoDatabase _database;

        public ConsultasDbContext(IMongoClient mongoClient, string databaseName)
        {
            _database = mongoClient.GetDatabase(databaseName);
        }

        public IMongoCollection<Consulta> Consultas => _database.GetCollection<Consulta>("Consultas");

    }
}
