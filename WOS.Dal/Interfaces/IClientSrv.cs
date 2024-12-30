using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WOS.Model;

namespace WOS.Dal.Interfaces
{
    public interface IClientSrv
    {
        void AddClient(Client client);

        void UpdateClient(Client client);

        void DeleteClient(Client client);

        Client GetClientById(int id);

        Client GetClientByEmail(string email);

        Client GetClient(string email, string password);

        bool ClientExists(string email);
    }
}
