using EncryptionDiary.API.Repository;
using EncryptionDiary.Shared.Helper;
using EncryptionDiary.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql.Replication.PgOutput.Messages;
using System.Security.Cryptography;

namespace EncryptionDiary.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class KeysController : Controller
    {
        private readonly KeyRepository _keyRepository;

        public KeysController(KeyRepository keyRepository)
        {
            _keyRepository = keyRepository;
        }

        [HttpGet("all")]
        public async Task<IActionResult> RetreveKeys(Guid userID)
        {
            return Ok(await _keyRepository.GetAllKeys(userID));
        }

        [HttpGet("{keyID}")]
        public async Task<IActionResult> GetKeys(Guid keyID)
        {
            return Ok(await _keyRepository.GetKeyByID(keyID));
        }

        [HttpPost]
        public async Task<IActionResult> InsertKey([FromBody] Key keyData)
        {
            return Ok(await _keyRepository.InsertKey(keyData));
        }

        [HttpPut]
        public async Task<IActionResult> ModifyKey([FromBody] Key keyData)
        {
            return Ok(await _keyRepository.ModifyKeyByID(keyData));
        }
        [HttpDelete("{keyID}")]
        public async Task<IActionResult> DeleteKey(Guid keyID)
        {
            var returnValue = await _keyRepository.SoftDeleteKeyByID(keyID);
            return returnValue ? Ok() : NotFound();

        }
        [HttpPut("share/{keyID}")]
        public async Task<IActionResult> MarkedShare(Guid keyID)
        {
            var result = await _keyRepository.MarkAsShared(keyID);
            if (!result) return NotFound();
            return Ok();
        }

    }
}
