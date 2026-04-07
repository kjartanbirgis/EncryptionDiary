using EncryptionDiary.API.Repository;
using EncryptionDiary.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EncryptionDiary.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]

    public class DiariesController : Controller
    {
        private readonly DiaryRepository _diaryRepository;
        

        public DiariesController(DiaryRepository diaryRepository)
        {
            _diaryRepository = diaryRepository;
        }

        [HttpGet("all")]
        public async Task<IActionResult> RetreveKeys(Guid userID)
        {
            return Ok(await _diaryRepository.GetAllDiaries(userID));
        }

        [HttpGet("{diaryID}")]
        public async Task<IActionResult> GetKeys(Guid keyID)
        {
            return Ok(await _diaryRepository.GetDiaryByID(keyID));
        }

        [HttpPost]
        public async Task<IActionResult> InsertKey([FromBody] Diary diaryData)
        {
            return Ok(await _diaryRepository.InsertDiary(diaryData));
        }

        [HttpPut]
        public async Task<IActionResult> ModifyKey([FromBody] Diary diaryData)
        {
            return Ok(await _diaryRepository.ModifyDiaryByID(diaryData));
        }
        [HttpDelete("{diaryID}")]
        public async Task<IActionResult> DeleteKey(Guid diaryID)
        {
            var returnValue = await _diaryRepository.SoftDeleteDiaryByID(diaryID);
            return returnValue ? Ok() : NotFound();

        }
    }
}
