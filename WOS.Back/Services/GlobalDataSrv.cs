using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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
    public class GlobalDataSrv : IGlobalDataSrv
    {
        public List<Categorie> Categories { get; set; }
        public List<Marque> Marques { get; set; }
        public List<Produit> Produits { get; set; }
        public List<ProduitTaille> ProduitTailles { get; set; }
        public List<ProduitCouleur> ProduitCouleurs { get; set; }
        public List<ProduitImage> ProduitImages { get; set; }
        public List<Client> Clients { get; set; }
        public List<Adresse> Adresses { get; set; }
        public List<Commande> Commandes { get; set; }
        public List<LigneCommande> LignesCommande { get; set; }
        public List<StatutCommande> StatutsCommande { get; set; }
        public List<Avis> Avis { get; set; }
        public List<Admin> Admins { get; set; }
        public List<Question> Questions { get; set; }
        public List<ModeLivraison> ModeLivraisons { get; set; }

        private readonly IServiceProvider _serviceProvider;


        public GlobalDataSrv(IServiceProvider serviceProvider)
        {
            Produits = new List<Produit>();
            Categories = new List<Categorie>();
            Marques = new List<Marque>();
            Questions = new List<Question>();
            Clients = new List<Client>();
            Commandes = new List<Commande>();
            Avis = new List<Avis>();
            Admins = new List<Admin>();
            ModeLivraisons = new List<ModeLivraison>();
            ProduitTailles = new List<ProduitTaille>();
            ProduitCouleurs = new List<ProduitCouleur>();
            ProduitImages = new List<ProduitImage>();
            _serviceProvider = serviceProvider;
        }

        public async Task RefreshCacheAsync(Type type)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<WOSDbContext>();

                if (type == typeof(Produit))
                {
                    Produits = await context.Produits
                                             .Include(p => p.ProduitImages)
                                             .Include(p => p.ProduitCouleurs)
                                             .Include(p => p.ProduitTailles)
                                             .Include(p => p.Avis)
                                             .Include(p => p.Marque)
                                             .Include(p => p.Categorie)
                                             .ToListAsync();
                }
                else if (type == typeof(Categorie))
                {
                    Categories = await context.Categories.ToListAsync();
                }
                else if (type == typeof(Admin))
                {
                    Admins = await context.Admins.ToListAsync();
                }
                else if (type == typeof(Adresse))
                {
                    Adresses = await context.Adresses.ToListAsync();
                }
                else if (type == typeof(Avis))
                {
                    Avis = await context.Avis.ToListAsync();
                }
                else if (type == typeof(Client))
                {
                    Clients = await context.Clients.ToListAsync();
                }
                else if (type == typeof(Commande))
                {
                    Commandes = await context.Commandes
                                              .Include(c => c.Client)
                                              .Include(c => c.AdresseLivraison)
                                              .Include(c => c.Statut)
                                              .Include(c => c.LignesCommande)
                                              .ToListAsync();
                }
                else if (type == typeof(Marque))
                {
                    Marques = await context.Marques.ToListAsync();
                }
                else if (type == typeof(ModeLivraison))
                {
                    ModeLivraisons = await context.ModeLivraisons.ToListAsync();
                }
                else if (type == typeof(ProduitImage))
                {
                    ProduitImages = await context.ProduitImages.ToListAsync();
                }
                else if (type == typeof(ProduitCouleur))
                {
                    ProduitCouleurs = await context.ProduitCouleurs.ToListAsync();
                }
                else if (type == typeof(ProduitTaille))
                {
                    ProduitTailles = await context.ProduitTailles.ToListAsync();
                }
                else if (type == typeof(Question))
                {
                    Questions = await context.Questions.ToListAsync();
                }
            }
        }

    }

    public class GlobalDataInitializer
    {
        private readonly WOSDbContext _context;
        private readonly IGlobalDataSrv _globalDataSrv;

        public GlobalDataInitializer(WOSDbContext context, IGlobalDataSrv globalDataSrv)
        {
            _context = context;
            _globalDataSrv = globalDataSrv;
        }

        public async Task InitializeDataAsync()
        {
            // Charger les données à partir de la base de données
            _globalDataSrv.Categories = await _context.Categories.ToListAsync();
            _globalDataSrv.Marques = await _context.Marques.ToListAsync();
            _globalDataSrv.Produits = await _context.Produits.ToListAsync();
            _globalDataSrv.Clients = await _context.Clients.ToListAsync();

            _globalDataSrv.ProduitTailles = GetProduitTaille();
            _globalDataSrv.ProduitCouleurs = GetProduitCouleurs();
            _globalDataSrv.ProduitImages = GetProduitImages();
            _globalDataSrv.Produits = GetProduits();
            _globalDataSrv.Categories = GetAllCategories();
            _globalDataSrv.Marques = GetAllMarques();
            _globalDataSrv.Questions = GetAllQuestions();
            _globalDataSrv.Clients = GetAllClients();
            _globalDataSrv.Commandes = GetCommandes();
            _globalDataSrv.ModeLivraisons = GetModeLivraisons();
            _globalDataSrv.Avis = GetAllAvis();
            _globalDataSrv.Admins = GetAllAdmins();
        }

        public List<Admin> GetAllAdmins()
        {
            return _context.Admins.ToList();
        }

        public List<Avis> GetAllAvis()
        {
            return _context.Avis.ToList();
        }

        public List<Categorie> GetAllCategories()
        {
            List<Categorie> Categories = _context.Categories.ToList();
            return Categories;
        }

        public List<Client> GetAllClients()
        {
            return _context.Clients.ToList();
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

        public List<Marque> GetAllMarques()
        {
            List<Marque> marques = _context.Marques.ToList();
            return marques;
        }

        public List<ModeLivraison> GetModeLivraisons()
        {
            return _context.ModeLivraisons.ToList();
        }

        public List<Produit> GetProduits()
        {
            List<Produit> produits = _context.Produits.ToList();

            foreach (Produit produit in produits)
            {
                produit.ProduitImages = _context.ProduitImages.Where(pi => pi.ProduitId == produit.Id).ToList();
                produit.ProduitTailles = _context.ProduitTailles.Where(pt => pt.ProduitId == produit.Id).ToList();
                produit.ProduitCouleurs = _context.ProduitCouleurs.Where(pc => pc.ProduitId == produit.Id).ToList();
                produit.Avis = _context.Avis.Where(a => a.ProduitId == produit.Id).ToList();
                produit.Marque = _context.Marques.FirstOrDefault(m => m.Id == produit.MarqueId);
                produit.Categorie = _context.Categories.FirstOrDefault(c => c.Id == produit.CategorieId);
            }

            return produits;
        }

        public List<ProduitImage> GetProduitImages()
        {
            return _context.ProduitImages.ToList();
        }

        public List<ProduitCouleur> GetProduitCouleurs()
        {
            return _context.ProduitCouleurs.ToList();
        }

        public List<ProduitTaille> GetProduitTaille()
        {
            return _context.ProduitTailles.ToList();
        }

        public List<Question> GetAllQuestions()
        {
            // Code pour récupérer tous les Actualites
            var questions = _context.Questions.ToList();

            return questions;
        }
    }
}
