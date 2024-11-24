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

        public CommandeSrv(WOSDbContext context)
        {
            _context = context;
        }

        public List<Commande> GetCommandes()
        {
            List<Commande> commandes = _context.Commandes.ToList();

            foreach (Commande commande in commandes)
            {
                commande.Client = _context.Clients.FirstOrDefault(c => c.Id == commande.ClientId);
                commande.AdresseLivraison = _context.Adresses.FirstOrDefault(a => a.Id == commande.AdresseLivraisonId);
                commande.Statut = _context.StatutsCommande.FirstOrDefault(s => s.Id == commande.StatutId);
                commande.LignesCommande = _context.LignesCommande.Where(lc => lc.CommandeId == commande.Id).ToList();
            }

            return commandes;
        }

        public Commande GetCommandeById(int id)
        {
            Commande commande = _context.Commandes.FirstOrDefault(c => c.Id == id);

            commande.Client = _context.Clients.FirstOrDefault(c => c.Id == commande.ClientId);
            commande.AdresseLivraison = _context.Adresses.FirstOrDefault(a => a.Id == commande.AdresseLivraisonId);
            commande.Statut = _context.StatutsCommande.FirstOrDefault(s => s.Id == commande.StatutId);
            commande.LignesCommande = _context.LignesCommande.Where(lc => lc.CommandeId == commande.Id).ToList();

            return commande;
        }

        public List<Commande> GetCommandesByClientId(int id)
        {
            List<Commande> commandes = _context.Commandes.Where(c => c.ClientId == id).ToList();

            foreach (Commande commande in commandes)
            {
                commande.Client = _context.Clients.FirstOrDefault(c => c.Id == commande.ClientId);
                commande.AdresseLivraison = _context.Adresses.FirstOrDefault(a => a.Id == commande.AdresseLivraisonId);
                commande.Statut = _context.StatutsCommande.FirstOrDefault(s => s.Id == commande.StatutId);
                commande.LignesCommande = _context.LignesCommande.Where(lc => lc.CommandeId == commande.Id).ToList();
            }

            return commandes;
        }

        public void AddCommande(Commande commande)
        {
            _context.Commandes.Add(commande);
            foreach (LigneCommande ligneCommande in commande.LignesCommande)
            {
                _context.LignesCommande.Add(ligneCommande);
            }
            _context.Adresses.Add(commande.AdresseLivraison);
            _context.StatutsCommande.Add(commande.Statut);
            _context.SaveChanges();
        }
    }
}
