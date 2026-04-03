using EncryptionDiary.API.Repository;
using EncryptionDiary.Shared.Helper;
using EncryptionDiary.Shared.Models;
using Microsoft.AspNetCore.Mvc;


namespace EncryptionDiary.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly UserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public UsersController (UserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
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

        [HttpPost]
        public async Task<IActionResult> RegisterUser(RegisterUser user)
        {
            if (user == null) { return BadRequest(); }

            var iteration = _configuration.GetValue<int>("PasswordSettings:Iterations");
            var pepper = _configuration.GetValue<string>("PasswordSettings:Pepper");
            var salt = PasswordHelper.GenerateSalt();
            var hash = PasswordHelper.HashPassword(user.ClientHash, salt, iteration, pepper);

            var registerUser = new User()
            {
                Username = user.Username,
                PasswordHash= hash,
                PasswordSalt= salt,
                PasswordIteration= iteration
            };
            return Ok(await _userRepository.RegisterUser(registerUser));
        }
    }
}
