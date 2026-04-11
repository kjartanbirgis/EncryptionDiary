using EncryptionDiary.Shared.Models;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Npgsql;
using NpgsqlTypes;

namespace EncryptionDiary.API.Repository
{
    public class DiaryRepository : BaseRepository
    {
        public DiaryRepository(string connectionString) : base(connectionString)
        {
        }

        public async Task<List<Diary>> GetAllDiaries(Guid userID)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();
            List<Diary> diaries = new List<Diary>();
            using var cmd = new NpgsqlCommand(
                "SELECT id, user_id, key_id, enc_diary_data, diary_nonce, diary_tag, created, updated, deleted FROM diary WHERE user_id = @user_id",
                conn);
            cmd.Parameters.AddWithValue("user_id", userID);
            cmd.Connection = conn;

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var tmpDiary = new Diary
                {
                    ID = reader.GetGuid(0),
                    UserID = reader.GetGuid(1),
                    KeyID = reader.GetGuid(2),
                    EncDiaryData = (byte[])reader[3],
                    DiaryNonce = (byte[])reader[4],
                    DiaryTag = (byte[])reader[5],

                    Created = reader.GetDateTime(6),
                    Updated = reader.GetDateTime(7),
                    Deleted = reader.IsDBNull(8) ? null : reader.GetDateTime(8)
                };
                diaries.Add(tmpDiary);
            }
            return diaries;
        }
       
        //þurfum að passa að eyða assests á undan 
        public async Task<bool> SoftDeleteDiaryByID(Guid diaryID)
        {

            using var connn = new NpgsqlConnection(_connectionString);
            await connn.OpenAsync();
            using var cmd = new NpgsqlCommand("update diary set " +
                                                    "enc_diary_data = null, " +
                                                    "diary_nonce = null, " +
                                                    "diary_tag = null," +
                                                    "deleted = @deleted " +
                                                "where id = @id");
            cmd.Connection = connn;
            cmd.Parameters.AddWithValue("id", diaryID);
            cmd.Parameters.AddWithValue("deleted", DateTime.UtcNow);
            var result = await cmd.ExecuteNonQueryAsync();
            return result < 0;
        }
        public async Task<Diary?> InsertDiary(Diary diary)
        {
            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();
                using var cmd = new NpgsqlCommand("insert into diary(id,user_id,enc_diary_data,diary_nonce,diary_tag,created,updated,deleted,key_id) " +
                                                    "values(@id, @user_id, @enc_diary_data, @diary_nonce, @diary_tag, @created, @updated, @deleted,@key_id) returning id");
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("id", NpgsqlDbType.Uuid, diary.ID != null ? (object)diary.ID : DBNull.Value);
                cmd.Parameters.AddWithValue("user_id", diary.UserID);
                cmd.Parameters.AddWithValue("key_id", diary.KeyID);

                cmd.Parameters.AddWithValue("enc_diary_data", NpgsqlDbType.Bytea, diary.EncDiaryData != null ? (object)diary.EncDiaryData : DBNull.Value);
                cmd.Parameters.AddWithValue("diary_nonce", NpgsqlDbType.Bytea, diary.DiaryNonce != null ? (object)diary.DiaryNonce : DBNull.Value);
                cmd.Parameters.AddWithValue("diary_tag", NpgsqlDbType.Bytea, diary.DiaryTag != null ? (object)diary.DiaryTag : DBNull.Value);


                cmd.Parameters.AddWithValue("created", NpgsqlDbType.TimestampTz, diary.Created != null ? diary.Created : DateTime.UtcNow);
                cmd.Parameters.AddWithValue("updated", NpgsqlDbType.TimestampTz, diary.Updated != null ? diary.Updated : DateTime.UtcNow);
                cmd.Parameters.AddWithValue("deleted", NpgsqlDbType.TimestampTz, diary.Deleted != null ? (object)diary.Deleted : DBNull.Value);

                cmd.Connection = conn;
                var keyID = await cmd.ExecuteScalarAsync();
                if (keyID == null || keyID == DBNull.Value)
                { return null; }

                return await GetDiaryByID((Guid)keyID);
            }catch(Exception e)
                {
                int i = 1 + 2;
            }
            return null;
        }
        
        public async Task<Diary?> ModifyDiaryByID(Diary diary)
        {
            try
            {
                if (diary == null || diary.ID == null) { return null; }
                using var conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();
                using var cmd = new NpgsqlCommand("update diary set " +
                                                        "enc_diary_data = @enc_diary_data, " +
                                                        "diary_nonce = @diary_nonce, " +
                                                        "diary_tag = @diary_tag," +
                                                        "updated = @updated " +
                                                    "where id = @id");

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("id", NpgsqlDbType.Uuid, diary.ID.Value);

                cmd.Parameters.AddWithValue("enc_diary_data", NpgsqlDbType.Bytea, diary.EncDiaryData != null ? (object)diary.EncDiaryData : DBNull.Value);
                cmd.Parameters.AddWithValue("diary_nonce", NpgsqlDbType.Bytea, diary.DiaryNonce != null ? (object)diary.DiaryNonce : DBNull.Value);
                cmd.Parameters.AddWithValue("diary_tag", NpgsqlDbType.Bytea, diary.DiaryTag != null ? (object)diary.DiaryTag : DBNull.Value);

                cmd.Parameters.AddWithValue("updated", NpgsqlDbType.TimestampTz, diary.Updated != null ? (object)diary.Updated : DBNull.Value);


                cmd.Connection = conn;
                await cmd.ExecuteNonQueryAsync();

                return await GetDiaryByID(diary.ID.Value);
            }catch(Exception e)
            {
                int i = 1 + 2;
            }
            return null;
        }
        //spurning um að vera með líka filter á noandum en ekki bara Guid á dagbókarfærslunni!!!
        public async Task<Diary?> GetDiaryByID(Guid diaryID)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new NpgsqlCommand(
                "SELECT id, enc_diary_data, diary_nonce, diary_tag, created, updated, deleted FROM diary WHERE id = @id",
                conn);
            cmd.Parameters.AddWithValue("id", diaryID);
            cmd.Connection = conn;

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Diary
                {
                    ID = reader.GetGuid(0),
                    EncDiaryData = (byte[])reader[1],
                    DiaryNonce = (byte[])reader[2],
                    DiaryTag = (byte[])reader[3],
                    Created = reader.GetDateTime(4),
                    Updated = reader.GetDateTime(5),
                    Deleted = reader.IsDBNull(6) ? null : reader.GetDateTime(6)
                };
            }
            return null;
        }
    }
}
