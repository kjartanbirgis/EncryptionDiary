using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncryptionDiary.Shared.Models
{
    public class RegisterUser
    {
        public string Username { get; set; }
        public byte[] ClientHash { get; set; }
    }
}
