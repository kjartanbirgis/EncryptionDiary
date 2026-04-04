using EncryptionDiary.API.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace EncryptionDiary.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KeysController : Controller
    {
        private readonly KeyRepository _keyRepository;

        public KeysController(KeyRepository keyRepository)
        {
            _keyRepository = keyRepository;
        }
    }
}
