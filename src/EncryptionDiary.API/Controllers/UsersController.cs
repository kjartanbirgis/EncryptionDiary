using EncryptionDiary.API.Middleware;
using EncryptionDiary.API.Repository;
using EncryptionDiary.Shared.Helper;
using EncryptionDiary.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;


namespace EncryptionDiary.API.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly UserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly TokenService _tokenService;

        public UsersController (UserRepository userRepository, IConfiguration configuration, TokenService tokenService)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _tokenService = tokenService;
        }
        

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser(AuthUser user)
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
            var tempresponse = await _userRepository.RegisterUser(registerUser);
            if (tempresponse == null)
            {
                return BadRequest();
            }
            var userResponse = new UserResponse(tempresponse);
            return Ok(new { Token = _tokenService.GenerateToken(userResponse.ID, userResponse.Username), User = userResponse });
        }
        [HttpPost("login")]
        public async Task<IActionResult> LoginUser(AuthUser user)
        {

            var pepper = _configuration.GetValue<string>("PasswordSettings:Pepper");
            var tempUser = await _userRepository.GetByUsername(user.Username);
            if (tempUser == null)
            {
                /*
                 * Hægum á ´kóðanum til að draga úr side attack timing
                 */
                var dummySalt = new byte[32];
                var iteration = _configuration.GetValue<int>("PasswordSettings:Iterations");
                var temphash = PasswordHelper.HashPassword(user.ClientHash, dummySalt, iteration, pepper);
                _= CryptographicOperations.FixedTimeEquals(user.ClientHash, temphash);
                return Unauthorized();
            }

            var hash = PasswordHelper.HashPassword(user.ClientHash, tempUser.PasswordSalt, tempUser.PasswordIteration, pepper);
            
            if (!CryptographicOperations.FixedTimeEquals(hash,tempUser.PasswordHash)) {
                return Unauthorized();
            }
            return Ok(new { Token = _tokenService.GenerateToken(tempUser.ID, tempUser.Username), User = new UserResponse(tempUser) });
        }

    }
}
