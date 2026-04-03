using Npgsql;
using EncryptionDiary.Shared.Models;
using Npgsql.Replication.PgOutput.Messages;

namespace EncryptionDiary.API.Repository
{
    public class UserRepository : BaseRepository
    {
        public UserRepository (string connectionString) : base(connectionString)
        {
        }
        
        public async Task<User?> GetByUsername(string username)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new NpgsqlCommand(
                "SELECT id, username, password_hash, pw_salt, pw_iter, created, updated, deleted FROM users WHERE username = @username",
                conn);
            cmd.Parameters.AddWithValue("username", username);
            cmd.Connection = conn;

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new User
                {
                    ID = reader.GetGuid(0),
                    Username = reader.GetString(1),
                    PasswordHash = (byte[])reader[2],
                    PasswordSalt = (byte[])reader[3],
                    PasswordIteration = reader.GetInt32(4),
                    Created = reader.GetDateTime(5),
                    Updated = reader.GetDateTime(6),
                    Deleted = reader.IsDBNull(7) ? null : reader.GetDateTime(7)
                };
            }
            return null;
        }

        public async Task<User?> RegisterUser(User user)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();
            using var cmd = new NpgsqlCommand("insert into users(username,password_hash,pw_salt,pw_iter) values(@username,@password_hash,@pw_salt,@pw_iter)");
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("username", user.Username);
            cmd.Parameters.AddWithValue("password_hash",user.PasswordHash);
            cmd.Parameters.AddWithValue("pw_salt", user.PasswordSalt);
            cmd.Parameters.AddWithValue("pw_iter",user.PasswordIteration);
            cmd.Connection= conn;
            await cmd.ExecuteNonQueryAsync();
            
            return await GetByUsername(user.Username);

        }
    }
}

