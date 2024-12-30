using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WOS.Model;
using WOS.Dal.Context;
using WOS.Dal.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace WOS.Back.Services
{
    public class AdresseSrv : IAdresseSrv
    {
        private readonly WOSDbContext _context;
        private readonly IGlobalDataSrv _globalDataSrv;

        public AdresseSrv(WOSDbContext context, IGlobalDataSrv globalDataSrv)
        {
            _context = context;
            _globalDataSrv = globalDataSrv;
        }

        public List<Adresse> GetAdresseByUserId(int id)
        {
            return _globalDataSrv.Adresses.FindAll(a => a.ClientId == id);
        }

        public Adresse GetAdressePrincipaleByUserId(int id)
        {
            return _globalDataSrv.Adresses.Find(a => a.ClientId == id && a.Principale == true);
        }

        public Adresse GetAdresseById(int id)
        {
            return _globalDataSrv.Adresses.Find(a => a.Id == id);
        }

        public void UpdateAdresse(Adresse adresse)
        {
            if (adresse != null)
            {
                _context.Adresses.Update(adresse);
                _context.SaveChanges();
                _globalDataSrv.RefreshCacheAsync(typeof(Adresse));
            }
        }

        public void AddAdresse(Adresse adresse)
        {
            if (adresse != null)
            {
                _context.Adresses.Add(adresse);
                _context.SaveChanges();
                _globalDataSrv.RefreshCacheAsync(typeof(Adresse));
            }
        }

        public void ChangeAdressePrincipale()
        {
            _context.Adresses.Where(a => a.PointRelais == false).ForEachAsync(adresse => { adresse.Principale = false; });

            _context.SaveChanges();
            _globalDataSrv.RefreshCacheAsync(typeof(Adresse));
        }
    }
}
