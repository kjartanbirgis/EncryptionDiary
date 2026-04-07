using Microsoft.AspNetCore.Mvc;
using Npgsql;
using NpgsqlTypes;
using EncryptionDiary.Shared.Models;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;


namespace EncryptionDiary.API.Repository
{

    public class KeyRepository : BaseRepository
    {
        public KeyRepository(string connectionString) : base(connectionString)
        {
        }

        public async Task<List<Key>>GetAllKeys(Guid userID)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();
            List<Key> keys = new List<Key>();
            using var cmd = new NpgsqlCommand(
                "SELECT id, user_id, enc_key, key_nonce, key_tag, description, shared, created, updated, deleted FROM keys WHERE user_id = @user_id",
                conn);
            cmd.Parameters.AddWithValue("user_id", userID);
            cmd.Connection = conn;

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var tmpkey = new Key
                {
                    ID = reader.GetGuid(0),
                    UserID = reader.GetGuid(1),
                    EncKey = reader.IsDBNull(2)?null:(byte[])reader[2],
                    KeyNonce = reader.IsDBNull(3)?null:(byte[])reader[3],
                    KeyTag = reader.IsDBNull(4) ? null : (byte[])reader[4],
                    Description = reader.GetString(5),
                    Shared = reader.GetBoolean(6),
                    Created = reader.GetDateTime(7),
                    Updated = reader.GetDateTime(8),
                    Deleted = reader.IsDBNull(9) ? null : reader.GetDateTime(9)
                };
                keys.Add(tmpkey);
            }
            return keys;
        }

        public async Task<Key?> InsertKey(Key key)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();
            using var cmd = new NpgsqlCommand("insert into keys(id,user_id,enc_key,key_nonce,key_tag,description,shared,created,updated,deleted) "+
                                                "values(@id, @user_id, @enc_key, @key_nonce, @key_tag, @description, @shared, @created, @updated, @deleted) returning id");
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("id", NpgsqlDbType.Uuid, key.ID != null ? key.ID : Guid.NewGuid());
            cmd.Parameters.AddWithValue("user_id", key.UserID);
            
            cmd.Parameters.AddWithValue("enc_key", NpgsqlDbType.Bytea ,key.EncKey!=null?(object)key.EncKey:DBNull.Value);
            cmd.Parameters.AddWithValue("key_nonce", NpgsqlDbType.Bytea, key.KeyNonce != null ? (object)key.KeyNonce : DBNull.Value);
            cmd.Parameters.AddWithValue("key_tag", NpgsqlDbType.Bytea, key.KeyTag != null ? (object)key.KeyTag : DBNull.Value);
            
            cmd.Parameters.AddWithValue("description",NpgsqlDbType.Varchar, key.Description!= null? (object)key.Description:DBNull.Value);
            cmd.Parameters.AddWithValue("shared", NpgsqlDbType.Boolean, key.Shared != null? (object)key.Shared:DBNull.Value);
            
            cmd.Parameters.AddWithValue("created", NpgsqlDbType.TimestampTz, key.Created != null ? key.Created : DateTime.UtcNow);
            cmd.Parameters.AddWithValue("updated", NpgsqlDbType.TimestampTz, key.Updated != null ? key.Updated : DateTime.UtcNow);
            cmd.Parameters.AddWithValue("deleted", NpgsqlDbType.TimestampTz, key.Deleted != null ? (object)key.Deleted : DBNull.Value);

            cmd.Connection = conn;
            var keyID = await cmd.ExecuteScalarAsync();
            if(keyID == null||keyID == DBNull.Value)
            { return null; }

            return await GetKeyByID((Guid)keyID);

        }

        public async Task<Key?> GetKeyByID(Guid keyID)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();
            List<Key> keys = new List<Key>();
            using var cmd = new NpgsqlCommand(
                "SELECT id, enc_key, key_nonce, key_tag, description, shared, created, updated, deleted FROM keys WHERE id = @id",
                conn);
            cmd.Parameters.AddWithValue("id", keyID);
            cmd.Connection = conn;

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Key
                {
                    ID = reader.GetGuid(0),
                    EncKey = reader.IsDBNull(1) ? null : (byte[])reader[1],
                    KeyNonce = reader.IsDBNull(2) ? null : (byte[])reader[2],
                    KeyTag = reader.IsDBNull(3) ? null : (byte[])reader[3],
                    Description = reader.GetString(4),
                    Shared = reader.GetBoolean(5),
                    Created = reader.GetDateTime(6),
                    Updated = reader.GetDateTime(7),
                    Deleted = reader.IsDBNull(8) ? null : reader.GetDateTime(8)
                };
            }
            return null;
        }

        public async Task<bool> SoftDeleteKeyByID(Guid keyID)
        {

            using var connn = new NpgsqlConnection(_connectionString);
            await connn.OpenAsync();
            using var cmd = new NpgsqlCommand("update keys set " +
                                                    "enc_key = null, " +
                                                    "key_nonce = null, " +
                                                    "key_tag = null," +
                                                    "deleted = @deleted "+
                                                "where id = @id");
            cmd.Connection = connn;
            cmd.Parameters.AddWithValue("id", keyID);
            cmd.Parameters.AddWithValue("deleted", DateTime.UtcNow);
            var rows= await cmd.ExecuteNonQueryAsync();
            return rows >0;

        }

        public async Task<Key?> ModifyKeyByID(Key key)
        {
            if (key.ID == null || key == null) { return null; }
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();
            using var cmd = new NpgsqlCommand("update keys set " +
                                                    "enc_key = @enc_key, " +
                                                    "key_nonce = @key_nonce, " +
                                                    "key_tag = @key_tag," +
                                                    "description =  @description, " +
                                                    "shared = @shared," +
                                                    "updated = @updated " +
                                                "where id = @id");

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("id", NpgsqlDbType.Uuid, key.ID.Value );

            cmd.Parameters.AddWithValue("enc_key", NpgsqlDbType.Bytea, key.EncKey != null ? (object)key.EncKey : DBNull.Value);
            cmd.Parameters.AddWithValue("key_nonce", NpgsqlDbType.Bytea, key.KeyNonce != null ? (object)key.KeyNonce : DBNull.Value);
            cmd.Parameters.AddWithValue("key_tag", NpgsqlDbType.Bytea, key.KeyTag != null ? (object)key.KeyTag : DBNull.Value);

            cmd.Parameters.AddWithValue("description", NpgsqlDbType.Varchar, key.Description != null ? (object)key.Description : DBNull.Value);
            cmd.Parameters.AddWithValue("shared", NpgsqlDbType.Boolean, key.Shared );

            cmd.Parameters.AddWithValue("updated", NpgsqlDbType.TimestampTz, key.Updated != null ? key.Updated : DateTime.UtcNow);


            cmd.Connection = conn;
            await cmd.ExecuteNonQueryAsync();

            return await GetKeyByID(key.ID.Value);
        }



    }
}
