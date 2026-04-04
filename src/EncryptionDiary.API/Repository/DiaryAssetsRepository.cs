using EncryptionDiary.Shared.Models;
using Microsoft.AspNetCore.SignalR;

namespace EncryptionDiary.API.Repository
{
    public class DiaryAssetsRepository : BaseRepository
    {
        public DiaryAssetsRepository(string connectionString) : base(connectionString)
        {
        }

        public async Task<List<DiaryAssets>> GetAllDiaryAssets(Guid DiaryID)
        {
            throw new NotImplementedException();
        }
        public async Task SoftDeleteDiaryAssets (Guid diaryAssetsID)
        {  
            throw new NotImplementedException();
        }
        public async Task<DiaryAssets> InsertDiaryAssets(DiaryAssets diaryAssets)
        {
            throw new NotImplementedException();
        }
        public async Task<DiaryAssets> ModifyDiaryAssets(DiaryAssets diaryAssets)
        {  
            throw new NotImplementedException(); 
        }
    }
}
