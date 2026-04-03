using EncryptionDiary.API.Repository;
using Microsoft.AspNetCore.Mvc;

namespace EncryptionDiary.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly UserRepository _userRepository;

        public UsersController (UserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        [HttpGet("{Username}")]
        public async Task<IActionResult> GetUser(string username)
        {
            var user = await _userRepository.GetByUsername(username);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }
    }
}
