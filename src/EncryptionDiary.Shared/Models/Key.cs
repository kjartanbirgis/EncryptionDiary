using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncryptionDiary.Shared.Models
{
    public class Key
    {
        public Guid ID { get; set; }
        public byte[] EncKey { get; set; }
        public byte[] KeyNonce { get; set; }
        public byte[] KeyTag { get; set; }
        public string? Description { get; set; } //hum þetta ætti kanski ekki að vera í gagnagrunninum heldur í clientinum???
        public bool? Sheared { get; set; } //sama hér??
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public DateTime? Deleted { get; set; }
    }
}
