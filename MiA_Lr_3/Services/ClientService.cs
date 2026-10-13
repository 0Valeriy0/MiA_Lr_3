using System.Text.Json;
using MiA_Lr_3.Models;
using MiA_Lr_3.Services.Interface;

namespace MiA_Lr_3.Services
{
    public class ClientService : IClientService
    {
        private readonly string filePath =Path.Combine("Storage", "clients.json");
        public ClientService()
        {
            if (!File.Exists(filePath))
            {
                Directory.CreateDirectory("Storage");
                File.WriteAllText(filePath, "[]");
            }
        }
        private List<Client> ReadClients()
        {
            string json = File.ReadAllText(filePath);

            return JsonSerializer.Deserialize<List<Client>>(json)
                   ?? new List<Client>();
        }
        private void WriteClients(List<Client> clients)
        {
            string json = JsonSerializer.Serialize(
                clients,
                new JsonSerializerOptions { WriteIndented = true });

            File.WriteAllText(filePath, json);
        }
        //CRUD operations
        public List<Client> GetAll()
        {
            return ReadClients();
        }

        public Client? GetById(int id)
        {
            return ReadClients().FirstOrDefault(c => c.Id == id);
        }
        public Client Create(Client client)
        {
            var clients = ReadClients();

            client.Id = clients.Count == 0 ? 1 : clients.Max(c => c.Id) + 1;

            clients.Add(client);
            WriteClients(clients);

            return client;
        }
        public bool Update(int id, Client client)
        {
            var clients = ReadClients();
            int index = clients.FindIndex(c => c.Id == id);

            if (index == -1)
                return false;

            client.Id = id;
            clients[index] = client;

            WriteClients(clients);
            return true;
        }
        public bool Delete(int id)
        {
            var clients = ReadClients();
            var client = clients.FirstOrDefault(c => c.Id == id);

            if (client == null)
                return false;

            clients.Remove(client);
            WriteClients(clients);

            return true;
        }
    }
}
