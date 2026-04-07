using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncryptionDiary.Client.Services
{
    public  class ServerManager
    {
        private List< ApiService> _services = new List< ApiService>();

        public void AddServer(ApiService service)
        {
            _services. Add(service);
        }

        public List<ApiService> GetServices()
        {
            return _services;
        }
    }
}
