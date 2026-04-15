using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncryptionDiary.Shared.Models
{
    public class AuthUser
    {
        public Guid? ID { get; set; }
        [Required]
        public string Username { get; set; }
        
        [Required]
        public byte[] ClientHash { get; set; }
    }
}
