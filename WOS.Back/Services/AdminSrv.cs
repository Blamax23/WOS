using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WOS.Dal.Context;
using WOS.Dal.Interfaces;
using WOS.Model;

namespace WOS.Back.Services
{
    public class AdminSrv : IAdminSrv
    {
        private readonly WOSDbContext _context;
        private readonly IGlobalDataSrv _globalDataSrv;

        public AdminSrv(WOSDbContext context, IGlobalDataSrv globalDataSrv)
        {
            _context = context;
            _globalDataSrv = globalDataSrv;
        }

        public Admin GetAdmin(string email, string password)
        {
            Admin admin = _globalDataSrv.Admins.FirstOrDefault(a => a.Email.ToLower() == email.ToLower() && a.MotDePasse == password);

            if (admin != null)
                return admin;
            else
                return null;
        }

        public Admin GetAdminById(int id)
        {
            Admin admin = _globalDataSrv.Admins.Find(a => a.Id == id);

            if (admin != null)
                return admin;
            else
                return null;
        }

        public Admin GetAdminByEmail(string email)
        {
            Admin admin = _globalDataSrv.Admins.FirstOrDefault(a => a.Email == email);

            if (admin != null)
                return admin;
            else
                return null;
        }

        public void UpdateAdmin(Admin admin)
        {
            _context.Admins.Update(admin);
            _context.SaveChanges();

            _globalDataSrv.RefreshCacheAsync(typeof(Admin));
        }
    }
}
