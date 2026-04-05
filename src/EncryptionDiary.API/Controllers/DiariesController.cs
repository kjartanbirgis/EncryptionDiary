using EncryptionDiary.API.Repository;
using Microsoft.AspNetCore.Mvc;

namespace EncryptionDiary.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class DiariesController : Controller
    {
        private readonly DiaryRepository _diaryRepository;
        

        public DiariesController(DiaryRepository diaryRepository)
        {
            _diaryRepository = diaryRepository;
        }

    }
}
