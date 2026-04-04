using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncryptionDiary.Shared.Models
{
    public class DiaryAssets
    {
        public Guid ID { get; set; }
        public Guid DiaryID { get; set; }
        public Guid KeyID { get; set; }

        public byte[] EncMetaData { get; set; }
        public byte[] MetaDataTag { get; set; }
        public byte[] MetaDataNonce { get; set; }

        public byte[] EncContent { get; set; }
        public byte[] ContentTag { get; set; }
        public byte[] ContentNonce { get; set; }

        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public DateTime? Deleted { get; set; }
    }
}
