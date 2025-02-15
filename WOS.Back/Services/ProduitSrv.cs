using Microsoft.EntityFrameworkCore;
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
    public class ProduitSrv : IProduitSrv
    {
        private readonly WOSDbContext _context;
        private readonly IGlobalDataSrv _globalDataSrv;

        public ProduitSrv(WOSDbContext context, IGlobalDataSrv globalDataSrv)
        {
            _context = context;
            _globalDataSrv = globalDataSrv;
        }

        public List<Produit> GetProduitsTendance()
        {
            List<Produit> produits = _globalDataSrv.Produits.Where(p => p.IsTendance).ToList();

            foreach (Produit produit in produits)
            {
                produit.ProduitImages = _globalDataSrv.ProduitImages.Where(pi => pi.ProduitId == produit.Id).ToList();
                produit.ProduitTailles = _globalDataSrv.ProduitTailles.Where(pt => pt.ProduitId == produit.Id).ToList();
                produit.ProduitCouleurs = _globalDataSrv.ProduitCouleurs.Where(pc => pc.ProduitId == produit.Id).ToList();
                produit.Avis = _globalDataSrv.Avis.Where(a => a.ProduitId == produit.Id).ToList();
                produit.Marque = _globalDataSrv.Marques.FirstOrDefault(m => m.Id == produit.MarqueId);
                produit.Categorie = _globalDataSrv.Categories.FirstOrDefault(c => c.Id == produit.CategorieId);
            }

            return produits;
        }

        public List<Produit> GetProduitsByMarque(int idMarque)
        {
            List<Produit> produits = _globalDataSrv.Produits.Where(p => p.MarqueId == idMarque).ToList();

            foreach (Produit produit in produits)
            {
                produit.ProduitImages = _globalDataSrv.ProduitImages.Where(pi => pi.ProduitId == produit.Id).ToList();
                produit.ProduitTailles = _globalDataSrv.ProduitTailles.Where(pt => pt.ProduitId == produit.Id).ToList();
                produit.ProduitCouleurs = _globalDataSrv.ProduitCouleurs.Where(pc => pc.ProduitId == produit.Id).ToList();
                produit.Avis = _globalDataSrv.Avis.Where(a => a.ProduitId == produit.Id).ToList();
                produit.Marque = _globalDataSrv.Marques.FirstOrDefault(m => m.Id == produit.MarqueId);
                produit.Categorie = _globalDataSrv.Categories.FirstOrDefault(c => c.Id == produit.CategorieId);
            }

            return produits;
        }

        public List<Produit> GetProduitsByCat(int idCat)
        {
            List<Produit> produits = _globalDataSrv.Produits.Where(p => p.CategorieId == idCat).ToList();

            foreach (Produit produit in produits)
            {
                produit.ProduitImages = _globalDataSrv.ProduitImages.Where(pi => pi.ProduitId == produit.Id).ToList();
                produit.ProduitTailles = _globalDataSrv.ProduitTailles.Where(pt => pt.ProduitId == produit.Id).ToList();
                produit.ProduitCouleurs = _globalDataSrv.ProduitCouleurs.Where(pc => pc.ProduitId == produit.Id).ToList();
                produit.Avis = _globalDataSrv.Avis.Where(a => a.ProduitId == produit.Id).ToList();
                produit.Marque = _globalDataSrv.Marques.FirstOrDefault(m => m.Id == produit.MarqueId);
                produit.Categorie = _globalDataSrv.Categories.FirstOrDefault(c => c.Id == produit.CategorieId);
            }

            return produits;
        }

        public Produit GetProduitById(int id)
        {
            Produit produit = _globalDataSrv.Produits.FirstOrDefault(p => p.Id == id);

            produit.ProduitImages = _globalDataSrv.ProduitImages.Where(pi => pi.ProduitId == produit.Id).ToList();
            produit.ProduitTailles = _globalDataSrv.ProduitTailles.Where(pt => pt.ProduitId == produit.Id).ToList();
            produit.ProduitCouleurs = _globalDataSrv.ProduitCouleurs.Where(pc => pc.ProduitId == produit.Id).ToList();
            produit.Avis = _globalDataSrv.Avis.Where(a => a.ProduitId == produit.Id).ToList();
            produit.Marque = _globalDataSrv.Marques.FirstOrDefault(m => m.Id == produit.MarqueId);
            produit.Categorie = _globalDataSrv.Categories.FirstOrDefault(c => c.Id == produit.CategorieId);

            foreach(var avis in produit.Avis)
            {
                avis.Client = _globalDataSrv.Clients.FirstOrDefault(c => c.Id == avis.ClientId);
            }

            return produit;
        }

        public void AddProduit(Produit produit)
        {
            foreach (ProduitImage produitImage in produit.ProduitImages)
            {
                produitImage.ProduitId = produit.Id;
                _context.ProduitImages.Add(produitImage);
            }
            foreach (ProduitTaille produitTaille in produit.ProduitTailles)
            {
                produitTaille.ProduitId = produit.Id;
                _context.ProduitTailles.Add(produitTaille);
            }
            foreach (ProduitCouleur produitCouleur in produit.ProduitCouleurs)
            {
                produitCouleur.ProduitId = produit.Id;
                _context.ProduitCouleurs.Add(produitCouleur);
            }

            _context.Produits.Add(produit);
            _context.SaveChanges();

            _globalDataSrv.RefreshCacheAsync(typeof(Produit));
        }

        public List<double> GetAllTailles()
        {
            List<double> tailles = new List<double>() { 33, 33.3, 33.5, 33.7, 34, 34.3, 34.5, 34.7, 35, 35.3, 35.5, 35.7, 36, 36.3, 36.5, 36.7, 37, 37.3, 37.5, 37.7, 38, 38.3, 38.5, 38.7, 39, 39.3, 39.5, 39.7, 40, 40.3, 40.5, 40.7, 41, 41.3, 41.5, 41.7, 42, 42.3, 42.5, 42.7, 43, 43.3, 43.5, 43.7, 44, 44.3, 44.5, 44.7, 45, 45.3, 45.5, 45.7, 46, 46.3, 46.5, 46.7, 47, 47.3, 47.5, 47.7, 48, 48.3, 48.5, 48.7, 49, 49.3, 49.5, 49.7, 50 };

            return tailles;
        }

        public void UpdateProduit(Produit produit)
        {
            // Récupérer l'entité directement à partir du contexte
            Produit produitToUpdate = _context.Produits.FirstOrDefault(p => p.Id == produit.Id);

            if (produitToUpdate == null)
                throw new Exception("Produit introuvable");

            // Appliquer les modifications directement sur l'entité suivie
            produitToUpdate.Nom = produit.Nom;
            produitToUpdate.Description = produit.Description;
            produitToUpdate.Actif = produit.Actif;
            produitToUpdate.MarqueId = produit.MarqueId;
            produitToUpdate.CategorieId = produit.CategorieId;
            produitToUpdate.ProduitImages = produit.ProduitImages;
            produitToUpdate.ProduitTailles = produit.ProduitTailles;
            produitToUpdate.ProduitCouleurs = produit.ProduitCouleurs;
            produitToUpdate.IsTendance = produit.IsTendance;

            // Sauvegarder les changements
            _context.SaveChanges();

            // Recharger le cache global
            _globalDataSrv.RefreshCacheAsync(typeof(Produit));

            // On vide produitTuUpdate pour éviter les problèmes de références
            produitToUpdate = null;
        }

        public void UpdateProduitTaille(ProduitTaille produitTaille)
        {
            ProduitTaille produitTailleToUpdate = _context.ProduitTailles.FirstOrDefault(pt => pt.Id == produitTaille.Id);

            if (produitTailleToUpdate == null)
                throw new Exception("ProduitTaille introuvable");

            produitTailleToUpdate.Taille = produitTaille.Taille;
            produitTailleToUpdate.Stock = produitTaille.Stock;
            produitTailleToUpdate.Prix = produitTaille.Prix;
            produitTailleToUpdate.PrixPromo = produitTaille.PrixPromo;

            _context.SaveChanges();
            _globalDataSrv.RefreshCacheAsync(typeof(ProduitTaille));
        }

        public void AddProduitTaille(ProduitTaille produitTaille, int idProduit)
        {
            Produit produit = _context.Produits.FirstOrDefault(p => p.Id == idProduit);

            if (produit == null)
                throw new Exception("Produit introuvable");

            produitTaille.ProduitId = idProduit;

            _context.ProduitTailles.Add(produitTaille);
            _context.SaveChanges();
            _globalDataSrv.RefreshCacheAsync(typeof(Produit));
        }

        public ProduitTaille GetProduitTailleById(int id)
        {
            ProduitTaille produitTaille = _globalDataSrv.ProduitTailles.FirstOrDefault(pt => pt.Id == id);

            return produitTaille;
        }

        public void UpdateActiveProduit(int id, bool actif)
        {
            Produit produit = _context.Produits.FirstOrDefault(p => p.Id == id);

            if (produit == null)
                throw new Exception("Produit introuvable");

            produit.Actif = actif;

            _context.SaveChanges();
            _globalDataSrv.RefreshCacheAsync(typeof(Produit));
            _globalDataSrv.RefreshCacheAsync(typeof(ProduitTaille));
        }

        public void UpdateTendanceProduit(int id, bool tendance)
        {
            Produit produit = _context.Produits.FirstOrDefault(p => p.Id == id);

            if (produit == null)
                throw new Exception("Produit introuvable");

            produit.IsTendance = tendance;

            _context.SaveChanges();
            _globalDataSrv.RefreshCacheAsync(typeof(Produit));
        }

        public void DeleteProduit(int id)
        {
            Produit produit = _context.Produits.FirstOrDefault(p => p.Id == id);

            _context.ProduitImages.RemoveRange(_context.ProduitImages.Where(pi => pi.ProduitId == produit.Id));
            _context.ProduitTailles.RemoveRange(_context.ProduitTailles.Where(pt => pt.ProduitId == produit.Id));
            _context.ProduitCouleurs.RemoveRange(_context.ProduitCouleurs.Where(pc => pc.ProduitId == produit.Id));
            _context.Avis.RemoveRange(_context.Avis.Where(a => a.ProduitId == produit.Id));

            _context.Produits.Remove(produit);
            _context.SaveChanges();
            _globalDataSrv.RefreshCacheAsync(typeof(Produit));
        }

        public void AddAvis(Avis avis)
        {
            _context.Avis.Add(avis);
            _context.SaveChanges();

            _globalDataSrv.RefreshCacheAsync(typeof(Avis));
        }
    }
}
