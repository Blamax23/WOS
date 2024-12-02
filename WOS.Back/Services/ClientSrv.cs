using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WOS.Dal.Context;
using WOS.Dal.Interfaces;
using WOS.Model;

namespace WOS.Back.Services
{
    public class ClientSrv : IClientSrv
    {
        private readonly WOSDbContext _context;

        public ClientSrv(WOSDbContext context)
        {
            _context = context;
        }

        public void AddClient(Client client)
        {
            _context.Clients.Add(client);
            _context.SaveChanges();
        }

        public void DeleteClient(Client client)
        {
            _context.Clients.Remove(client);
            _context.SaveChanges();
        }

        public void UpdateClient(Client client) {


            _context.Clients.Update(client);
            _context.SaveChanges();
        }

        public Client GetClientById(int id)
        {
            return _context.Clients.Find(id);
        }

        public Client GetClientByEmail(string email)
        {
            return _context.Clients.FirstOrDefault(c => c.Email.Equals(email));
        }

        public List<Client> GetAllClients()
        {
            return _context.Clients.ToList();
        }

        public Client GetClient(string email, string password)
        {
            try
            {
                Client client = _context.Clients.FirstOrDefault(c => c.Email.ToLower() == email.ToLower() && c.MotDePasse == password);
                return client;
            }
            catch(Exception e)
            {
                return null;
            }
        }

        public bool ClientExists(string email)
        {
            return _context.Clients.Any(c => c.Email == email);
        }
    }
}
