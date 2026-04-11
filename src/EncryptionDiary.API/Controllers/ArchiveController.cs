using EncryptionDiary.API.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    }
}
