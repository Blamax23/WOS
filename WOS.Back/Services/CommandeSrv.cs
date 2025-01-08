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
    public class CommandeSrv : ICommandeSrv
    {
        private readonly WOSDbContext _context;
        private readonly IGlobalDataSrv _globalDataSrv;

        public CommandeSrv(WOSDbContext context, IGlobalDataSrv globalDataSrv)
        {
            _context = context;
            _globalDataSrv = globalDataSrv;
        }

        public Commande GetCommandeById(int id)
        {
            Commande commande = _globalDataSrv.Commandes.FirstOrDefault(c => c.Id == id);

            commande.Client = _globalDataSrv.Clients.FirstOrDefault(c => c.Id == commande.ClientId);
            commande.AdresseLivraison = _globalDataSrv.Adresses.FirstOrDefault(a => a.Id == commande.AdresseLivraisonId);
            commande.Statut = _globalDataSrv.StatutsCommande.FirstOrDefault(s => s.Id == commande.StatutId);
            commande.LignesCommande = _globalDataSrv.LignesCommande.Where(lc => lc.CommandeId == commande.Id).ToList();

            return commande;
        }

        public List<Commande> GetCommandesByClientId(int id)
        {
            List<Commande> commandes = _globalDataSrv.Commandes.Where(c => c.ClientId == id).ToList();

            foreach (Commande commande in commandes)
            {
                commande.Client = _globalDataSrv.Clients.FirstOrDefault(c => c.Id == commande.ClientId);
                commande.AdresseLivraison = _globalDataSrv.Adresses.FirstOrDefault(a => a.Id == commande.AdresseLivraisonId);
                commande.Statut = _globalDataSrv.StatutsCommande.FirstOrDefault(s => s.Id == commande.StatutId);
                commande.LignesCommande = _globalDataSrv.LignesCommande.Where(lc => lc.CommandeId == commande.Id).ToList();
            }

            return commandes;
        }

        public Commande GetCommandeByNumberOrder(string number)
        {
            Commande commande = _globalDataSrv.Commandes.FirstOrDefault(c => c.NumeroCommande == number);

            commande.Client = _globalDataSrv.Clients.FirstOrDefault(c => c.Id == commande.ClientId);
            commande.AdresseLivraison = _globalDataSrv.Adresses.FirstOrDefault(a => a.Id == commande.AdresseLivraisonId);
            commande.Statut = _globalDataSrv.StatutsCommande.FirstOrDefault(s => s.Id == commande.StatutId);
            commande.LignesCommande = _globalDataSrv.LignesCommande.Where(lc => lc.CommandeId == commande.Id).ToList();

            return commande;
        }

        public void AddCommande(Commande commande)
        {
            _context.Adresses.Add(commande.AdresseLivraison);
            _context.SaveChanges();

            _context.Commandes.Add(commande);
            _context.SaveChanges();

            _globalDataSrv.RefreshCacheAsync(typeof(Adresse));
            _globalDataSrv.RefreshCacheAsync(typeof(Commande));

            Commande commandeSaved = _globalDataSrv.Commandes.FirstOrDefault(c => c.NumeroCommande == commande.NumeroCommande);
            Adresse adresseCommande = _globalDataSrv.Adresses.FirstOrDefault(a => a.CodePostal == commande.AdresseLivraison.CodePostal && a.ClientId == commande.ClientId);

            _context.Commandes.FirstOrDefault(c => c == commandeSaved).AdresseLivraisonId = adresseCommande.Id;

            foreach (LigneCommande ligneCommande in commande.LignesCommande)
            {
                LigneCommande lignetoAdd = ligneCommande;
                lignetoAdd.Id = 0;
                lignetoAdd.CommandeId = commandeSaved.Id;
                _context.LignesCommande.Add(lignetoAdd);
            }

            _context.SaveChanges();
        }

        public void UpdateStatus(int idCommande)
        {
            Commande commande = _context.Commandes.FirstOrDefault(c => c.Id == idCommande);

            commande.StatutId = commande.StatutId++;

            _context.SaveChanges();

            _globalDataSrv.RefreshCacheAsync(typeof(Commande));
        }
    }
}
