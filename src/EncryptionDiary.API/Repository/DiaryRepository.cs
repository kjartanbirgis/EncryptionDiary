using EncryptionDiary.Shared.Models;

namespace EncryptionDiary.API.Repository
{
    public class DiaryRepository : BaseRepository
    {
        public DiaryRepository(string connectionString) : base(connectionString)
        {
        }

        public async Task<List<Diary>> GetAllDiary(Guid UserID)
        {
            throw new NotImplementedException();
        }
        public async Task SoftDeleteDiary(Guid diaryID)
        {
            throw new NotImplementedException();
        }
        public async Task<Diary> InsertDiary(Diary diary)
        {
            throw new NotImplementedException();
        }
        public async Task<Diary> ModifyDiary(Diary diary)
        {
            throw new NotImplementedException();
        }
    }
}
