using Microsoft.AspNetCore.Mvc;
using Npgsql;
using EncryptionDiary.Shared.Models;

namespace EncryptionDiary.API.Repository
{

    public class KeyRepository : BaseRepository
    {
        public KeyRepository(string connectionString) : base(connectionString)
        {
        }

        public async Task<List<Key>>getAllKeys(Guid userID)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();
            List<Key> keys = new List<Key>();
            using var cmd = new NpgsqlCommand(
                "SELECT id, enc_key, key_nonce, key_tag, description, sheared, created, updated, deleted FROM keys WHERE user_id = @user_id",
                conn);
            cmd.Parameters.AddWithValue("user_id", userID);
            cmd.Connection = conn;

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var tmpkey = new Key
                {
                    ID = reader.GetGuid(0),
                    EncKey = (byte[])reader[1],
                    KeyNonce = (byte[])reader[2],
                    KeyTag = (byte[])reader[3],
                    Description = reader.GetString(4),
                    Sheared = reader.IsDBNull(5)? null: reader.GetBoolean(5),
                    Created = reader.GetDateTime(6),
                    Updated = reader.GetDateTime(7),
                    Deleted = reader.IsDBNull(8) ? null : reader.GetDateTime(8)
                };
                keys.Add(tmpkey);
            }
            return keys;
        }


    }
}
