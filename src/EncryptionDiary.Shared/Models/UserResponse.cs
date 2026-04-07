using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncryptionDiary.Shared.Models
{
    public class UserResponse
    {
        public UserResponse() { }

        public UserResponse(User user)
        {
            ID = user.ID;
            Username = user.Username;
            Created = user.Created;
            Updated = user.Updated;
            Deleted = user.Deleted;
        }

        public Guid ID { get; set; }
        public string Username { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public DateTime? Deleted { get; set; }
    }
}
