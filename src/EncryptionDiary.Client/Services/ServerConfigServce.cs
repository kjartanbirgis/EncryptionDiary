using EncryptionDiary.Shared.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EncryptionDiary.Client.Services
{
    public class ServerConfigService
    {
        private readonly string _filePath;

        public ServerConfigService()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var folder = Path.Combine(appData, "EncryptionDiary");
            Directory.CreateDirectory(folder);
            _filePath = Path.Combine(folder, "servers.json");
        }

        public Dictionary<string, ServerConnection> Load()
        {
            if (!File.Exists(_filePath)) return new Dictionary<string, ServerConnection>();
            var json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<Dictionary<string, ServerConnection>>(json) ?? new Dictionary<string, ServerConnection>();
        }

        public void Save(Dictionary<string, ServerConnection> servers)
        {
            var json = JsonSerializer.Serialize(servers);
            File.WriteAllText(_filePath, json);
        }
    }
}
