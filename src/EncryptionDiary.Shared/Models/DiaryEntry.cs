using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncryptionDiary.Shared.Models
{
    internal class DiaryEntry
    {
        public Guid? ID { get; set; }
        public Guid? KeyID { get; set; }

        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime EntryDate { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
    }
}
