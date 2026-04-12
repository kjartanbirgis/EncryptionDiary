using EncryptionDiary.API.Repository;
using EncryptionDiary.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        //svona loka á að það se ekki hægt að gera þetta skipta um userID
        [HttpGet("all")]
        public async Task<IActionResult> RetreveDaries(Guid userID)
        {
            var tokenUserId = GetUserIdFromToken();
            if (tokenUserId != userID)
            { 
            return Forbid(); // 403 - tampering detected //spurning um að logga þetta svo hægt væri að loka á iptölur sem eru í svona vinnu.
        }
            return Ok(await _diaryRepository.GetAllDiaries(userID));
        }

        [HttpGet("{diaryID}")]
        public async Task<IActionResult> GetDiary(Guid keyID)
        {
            return Ok(await _diaryRepository.GetDiaryByID(keyID));
        }

        [HttpPost]
        public async Task<IActionResult> InsertDiary([FromBody] Diary diaryData)
        {
            return Ok(await _diaryRepository.InsertDiary(diaryData));
        }

        [HttpPut]
        public async Task<IActionResult> ModifyDiary([FromBody] Diary diaryData)
        {
            return Ok(await _diaryRepository.ModifyDiaryByID(diaryData));
        }
        [HttpDelete("{diaryID}")]
        public async Task<IActionResult> DeleteDiary(Guid diaryID)
        {
            var returnValue = await _diaryRepository.SoftDeleteDiaryByID(diaryID);
            return returnValue ? Ok() : NotFound();

        }
        private Guid GetUserIdFromToken()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return Guid.Parse(claim.Value);
        }
    }
}
