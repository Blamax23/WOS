using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WOS.Model;

namespace WOS.Dal.Interfaces
{
    public interface IAdminSrv
    {
        Admin GetAdmin(string email, string password);

        void UpdateAdmin(Admin admin);

        Admin GetAdminById(int id);

        Admin GetAdminByEmail(string email);
    }
}
