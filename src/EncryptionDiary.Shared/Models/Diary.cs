using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncryptionDiary.Shared.Models
{
    public class Diary
    {
        public Guid? ID { get; set; }
        public Guid UserID { get; set; }
        public Guid KeyID { get; set; }
        
        public byte[]? EncDiaryData { get; set; }
        public byte[]? DiaryTag { get; set; }
        public byte[]? DiaryNonce { get; set; }

        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }
        public DateTime? Deleted { get; set; }
    }
}
