using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncryptionDiary.Shared.Models
{
    public class Archive
    {
        public Guid KeyID { get; set; }
        public List<string> Shares { get; set; }
    }
}
