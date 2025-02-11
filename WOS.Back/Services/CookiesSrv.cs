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
    public class CookiesSrv : ICookiesSrv
    {
        private readonly WOSDbContext _context;
        private readonly IGlobalDataSrv _globalDataSrv;

        public CookiesSrv(WOSDbContext context, IGlobalDataSrv globalDataSrv)
        {
            _context = context;
            _globalDataSrv = globalDataSrv;
        }

        public void SaveCookies(UserCookies userCookies)
        {
            _context.UserCookies.Add(userCookies);
            _context.SaveChanges();

            _globalDataSrv.RefreshCacheAsync(typeof(UserCookies));
        }
    }
}
