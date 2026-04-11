using EncryptionDiary.API.Repository;
using EncryptionDiary.Shared.Helper;
using EncryptionDiary.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


using System.Text.Json;
using System.Text;

namespace EncryptionDiary.API.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]

    public class ArchiveController : Controller
    {
        DiaryRepository _diaryRepository;
        public ArchiveController(DiaryRepository diaryRepository)
        {
            _diaryRepository = diaryRepository;
        }
        [HttpPost]
        public async Task<IActionResult> GetDiaries(Archive archive)
        {
            //sækjum allar dulkóðaðar færslur
            var diaries = await _diaryRepository.GetAllDiariesByKeyID(archive.KeyID);

            //búum til lykil úr splittunum
            var key = ShamirService.ReconstructSecret(archive.Shares);
            //Búum til stað til að geyma færslurnar //ef það er eitthvað annað hvernig getum við ekki geymt það í minni.
            List<DiaryEntry> diaryEntries = new List<DiaryEntry>();
            //telja hvað tekst mikið til og hvað klúðrast. stoppa þegar hlutfallið er 10:1 eða eitthvað svoleiðis???
            int error = 0, success = 0;
            foreach (var entry in diaries)
            {
                try
                {
                    var plainBytes = CryptoServices.DecryptAES_GCM(key, entry.DiaryNonce, entry.EncDiaryData, entry.DiaryTag);
                    var json = Encoding.UTF8.GetString(plainBytes);
                    var tmpentry = JsonSerializer.Deserialize<DiaryEntry>(json);

                    diaryEntries.Add(tmpentry);
                     success++;
                }
                catch (Exception ex)
                {
                    error++;
                }
            }
            return Ok(new { Entries = diaryEntries, Success = success, Error = error });

        }
    }
}
