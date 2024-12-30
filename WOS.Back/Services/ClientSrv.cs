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
        private readonly IGlobalDataSrv _globalDataSrv;

        public ClientSrv(WOSDbContext context, IGlobalDataSrv globalDataSrv)
        {
            _context = context;
            _globalDataSrv = globalDataSrv;
        }

        public void AddClient(Client client)
        {
            _context.Clients.Add(client);
            _context.SaveChanges();
            _globalDataSrv.RefreshCacheAsync(typeof(Client));
        }

        public void DeleteClient(Client client)
        {
            _context.Clients.Remove(client);
            _context.SaveChanges();
            _globalDataSrv.RefreshCacheAsync(typeof(Client));
        }

        public void UpdateClient(Client client) {


            _context.Clients.Update(client);
            _context.SaveChanges();
            _globalDataSrv.RefreshCacheAsync(typeof(Client));
        }

        public Client GetClientById(int id)
        {
            return _globalDataSrv.Clients.Find(c => c.Id == id);
        }

        public Client GetClientByEmail(string email)
        {
            return _globalDataSrv.Clients.Find(c => c.Email.Equals(email));
        }

        public Client GetClient(string email, string password)
        {
            try
            {
                Client client = _globalDataSrv.Clients.FirstOrDefault(c => c.Email.ToLower() == email.ToLower() && c.MotDePasse == password);
                return client;
            }
            catch(Exception e)
            {
                return null;
            }
        }

        public bool ClientExists(string email)
        {
            return _globalDataSrv.Clients.Any(c => c.Email == email);
        }
    }
}
