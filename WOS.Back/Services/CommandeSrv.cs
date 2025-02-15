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

        public Commande GetCommandeByNumberOrderDelivery(string number)
        {

           Commande commande = _globalDataSrv.Commandes.FirstOrDefault(c => c.NumeroCommandeLivreur == number);

            commande.Client = _globalDataSrv.Clients.FirstOrDefault(c => c.Id == commande.ClientId);
            commande.AdresseLivraison = _globalDataSrv.Adresses.FirstOrDefault(a => a.Id == commande.AdresseLivraisonId);
            commande.Statut = _globalDataSrv.StatutsCommande.FirstOrDefault(s => s.Id == commande.StatutId);
            commande.LignesCommande = _globalDataSrv.LignesCommande.Where(lc => lc.CommandeId == commande.Id).ToList();

            return commande;
        }

        public void AddCommande(Commande commande)
        {
            _context.Commandes.Add(commande);
            _context.SaveChanges();

            _globalDataSrv.RefreshCacheAsync(typeof(Commande));

            Commande commandeSaved = _globalDataSrv.Commandes.FirstOrDefault(c => c.NumeroCommande == commande.NumeroCommande);
            Adresse adresseCommande = _globalDataSrv.Adresses.FirstOrDefault(a => a.CodePostal == commande.AdresseLivraison.CodePostal && a.ClientId == commande.ClientId);

            _context.Commandes.FirstOrDefault(c => c == commandeSaved).AdresseLivraisonId = adresseCommande.Id;

            //foreach (LigneCommande ligneCommande in commande.LignesCommande)
            //{
            //    LigneCommande lignetoAdd = ligneCommande;
            //    lignetoAdd.Id = 0;
            //    lignetoAdd.CommandeId = commandeSaved.Id;
            //    _context.LignesCommande.Add(lignetoAdd);
            //}

            _context.SaveChanges();
            _globalDataSrv.RefreshCacheAsync(typeof(LigneCommande));
            _globalDataSrv.RefreshCacheAsync(typeof(Adresse));
        }

        public void UpdateStatus(int idCommande)
        {
            Commande commande = _context.Commandes.FirstOrDefault(c => c.Id == idCommande);

            commande.StatutId++;

            _context.SaveChanges();

            _globalDataSrv.RefreshCacheAsync(typeof(Commande));
        }

        public void UpdateCommande(Commande commande)
        {
            Commande commandeToUpdate = _context.Commandes.FirstOrDefault(c => c.Id == commande.Id);

            commandeToUpdate.StatutId = commande.StatutId;
            commandeToUpdate.NumeroCommandeLivreur = commande.NumeroCommandeLivreur;
            commandeToUpdate.LinkSuivi = commande.LinkSuivi;
            commandeToUpdate.ModeLivraisonId = commande.ModeLivraisonId;
            commandeToUpdate.BinaryEtiquette = commande.BinaryEtiquette;
            commandeToUpdate.BinaryFacture = commande.BinaryFacture;

            _context.SaveChanges();

            _globalDataSrv.RefreshCacheAsync(typeof(Commande));
        }

        public void AddCodePromo(CodePromo codePromo)
        {
            // On vérifie que le code promo n'existe pas déjà
            if (_context.CodePromos.Any(c => c.Nom == codePromo.Nom))
            {
                throw new Exception("Code promo déjà existant");
            }

            _context.CodePromos.Add(codePromo);
            _context.SaveChanges();

            _globalDataSrv.RefreshCacheAsync(typeof(CodePromo));
        }

        public void DeleteCodePromo(int id)
        {
            CodePromo codePromo = _context.CodePromos.FirstOrDefault(c => c.Id == id);

            _context.CodePromos.Remove(codePromo);
            _context.SaveChanges();

            _globalDataSrv.RefreshCacheAsync(typeof(CodePromo));
        }

        public void UpdateCodePromo(int id, bool validity)
        {
            CodePromo codePromoToUpdate = _context.CodePromos.FirstOrDefault(c => c.Id == id);

            codePromoToUpdate.IsValid = validity;

            _context.SaveChanges();

            _globalDataSrv.RefreshCacheAsync(typeof(CodePromo));
        }
    }
}
