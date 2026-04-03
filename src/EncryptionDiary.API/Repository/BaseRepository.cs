namespace EncryptionDiary.API.Repository
{
    public class BaseRepository
    {
        protected readonly string _connectionString;
        
        public BaseRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected Npgsql.NpgsqlConnection CreateConnection()
        {
            return new Npgsql.NpgsqlConnection(_connectionString);
        }
    }
}
