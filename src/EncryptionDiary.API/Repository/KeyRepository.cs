using Microsoft.AspNetCore.Mvc;
using Npgsql;
using NpgsqlTypes;
using EncryptionDiary.Shared.Models;
using Microsoft.AspNetCore.DataProtection.KeyManagement;


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
                    EncKey = (byte[])reader[2],
                    KeyNonce = (byte[])reader[3],
                    KeyTag = (byte[])reader[4],
                    Description = reader.GetString(5),
                    Shared = reader.IsDBNull(6)? null: reader.GetBoolean(6),
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
            cmd.Parameters.AddWithValue("id", NpgsqlDbType.Uuid, key.ID!=null?(object)key.ID:DBNull.Value);
            cmd.Parameters.AddWithValue("user_id", key.UserID);
            
            cmd.Parameters.AddWithValue("enc_key", NpgsqlDbType.Bytea ,key.EncKey!=null?(object)key.EncKey:DBNull.Value);
            cmd.Parameters.AddWithValue("enc_nonce", NpgsqlDbType.Bytea, key.KeyNonce != null ? (object)key.KeyNonce : DBNull.Value);
            cmd.Parameters.AddWithValue("enc_tag", NpgsqlDbType.Bytea, key.KeyTag != null ? (object)key.KeyTag : DBNull.Value);
            
            cmd.Parameters.AddWithValue("description",NpgsqlDbType.Varchar, key.Description!= null? (object)key.Description:DBNull.Value);
            cmd.Parameters.AddWithValue("shared", NpgsqlDbType.Boolean, key.Shared != null? (object)key.Shared:DBNull.Value);
            
            cmd.Parameters.AddWithValue("created", NpgsqlDbType.TimestampTz, key.Created != null ? (object)key.Created : DBNull.Value);
            cmd.Parameters.AddWithValue("updated", NpgsqlDbType.TimestampTz, key.Updated != null ? (object)key.Updated : DBNull.Value);
            cmd.Parameters.AddWithValue("deleted", NpgsqlDbType.TimestampTz, key.Deleted != null ? (object)key.Deleted : DBNull.Value);

            cmd.Connection = conn;
            var keyID = await cmd.ExecuteScalarAsync();
            if(keyID != DBNull.Value)
            { return null; }

            return await GetKeyByID((Guid)keyID);

        }

        private async Task<Key?> GetKeyByID(Guid keyID)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();
            List<Key> keys = new List<Key>();
            using var cmd = new NpgsqlCommand(
                "SELECT id, user_id, enc_key, key_nonce, key_tag, description, shared, created, updated, deleted FROM keys WHERE id = @id",
                conn);
            cmd.Parameters.AddWithValue("id", keyID);
            cmd.Connection = conn;

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Key
                {
                    ID = reader.GetGuid(0),
                    EncKey = (byte[])reader[1],
                    KeyNonce = (byte[])reader[2],
                    KeyTag = (byte[])reader[3],
                    Description = reader.GetString(4),
                    Shared = reader.IsDBNull(5) ? null : reader.GetBoolean(5),
                    Created = reader.GetDateTime(6),
                    Updated = reader.GetDateTime(7),
                    Deleted = reader.IsDBNull(8) ? null : reader.GetDateTime(8)
                };
            }
            return null;
        }

        private async Task<Key?> ModifyKeyByID(Key key)
        {
            if (key.ID == null) { return null; }
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();
            using var cmd = new NpgsqlCommand("update keys set " +
                                                    "user_id = @user_id, " +
                                                    "enc_key = @enc_key, " +
                                                    "key_nonce = @key_nonce, " +
                                                    "key_tag = @key_tag," +
                                                    "description =  @description, " +
                                                    "shared = @shared," +
                                                    "updated = @updated" +
                                                "where id = @id");

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("id", NpgsqlDbType.Uuid, key.ID.Value );
            cmd.Parameters.AddWithValue("user_id", key.UserID);

            cmd.Parameters.AddWithValue("enc_key", NpgsqlDbType.Bytea, key.EncKey != null ? (object)key.EncKey : DBNull.Value);
            cmd.Parameters.AddWithValue("key_nonce", NpgsqlDbType.Bytea, key.KeyNonce != null ? (object)key.KeyNonce : DBNull.Value);
            cmd.Parameters.AddWithValue("key_tag", NpgsqlDbType.Bytea, key.KeyTag != null ? (object)key.KeyTag : DBNull.Value);

            cmd.Parameters.AddWithValue("description", NpgsqlDbType.Varchar, key.Description != null ? (object)key.Description : DBNull.Value);
            cmd.Parameters.AddWithValue("shared", NpgsqlDbType.Boolean, key.Shared != null ? (object)key.Shared : DBNull.Value);

            cmd.Parameters.AddWithValue("updated", NpgsqlDbType.TimestampTz, key.Updated != null ? (object)key.Updated : DBNull.Value);


            cmd.Connection = conn;
            await cmd.ExecuteNonQueryAsync();

            return await GetKeyByID(key.ID.Value);
        }

        private void SoftDeleteKeyByID(Key KeyID) { 
        }

    }
}
