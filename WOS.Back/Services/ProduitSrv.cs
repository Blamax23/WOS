﻿using System;
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

        public ProduitSrv(WOSDbContext context)
        {
            _context = context;
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

        public List<Produit> GetProduitsTendance()
        {
            List<Produit> produits = _context.Produits.Where(p => p.IsTendance).ToList();

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

        public List<Produit> GetProduitsByMarque(int idMarque)
        {
            List<Produit> produits = _context.Produits.Where(p => p.MarqueId == idMarque).ToList();

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

        public List<Produit> GetProduitsByCat(int idCat)
        {
            List<Produit> produits = _context.Produits.Where(p => p.CategorieId == idCat).ToList();

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

        public Produit GetProduitById(int id)
        {
            Produit produit = _context.Produits.FirstOrDefault(p => p.Id == id);

            produit.ProduitImages = _context.ProduitImages.Where(pi => pi.ProduitId == produit.Id).ToList();
            produit.ProduitTailles = _context.ProduitTailles.Where(pt => pt.ProduitId == produit.Id).ToList();
            produit.ProduitCouleurs = _context.ProduitCouleurs.Where(pc => pc.ProduitId == produit.Id).ToList();
            produit.Avis = _context.Avis.Where(a => a.ProduitId == produit.Id).ToList();
            produit.Marque = _context.Marques.FirstOrDefault(m => m.Id == produit.MarqueId);
            produit.Categorie = _context.Categories.FirstOrDefault(c => c.Id == produit.CategorieId);

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
        }

        public List<double> GetAllTailles()
        {
            List<double> tailles = new List<double>() { 33, 33.3, 33.5, 33.7, 34, 34.3, 34.5, 34.7, 35, 35.3, 35.5, 35.7, 36, 36.3, 36.5, 36.7, 37, 37.3, 37.5, 37.7, 38, 38.3, 38.5, 38.7, 39, 39.3, 39.5, 39.7, 40, 40.3, 40.5, 40.7, 41, 41.3, 41.5, 41.7, 42, 42.3, 42.5, 42.7, 43, 43.3, 43.5, 43.7, 44, 44.3, 44.5, 44.7, 45, 45.3, 45.5, 45.7, 46, 46.3, 46.5, 46.7, 47, 47.3, 47.5, 47.7, 48, 48.3, 48.5, 48.7, 49, 49.3, 49.5, 49.7, 50 };

            return tailles;
        }

        public void UpdateProduit(Produit produit)
        {
            Produit produitToUpdate = _context.Produits.FirstOrDefault(p => p.Id == produit.Id);

            produitToUpdate.Nom = produit.Nom;
            produitToUpdate.Description = produit.Description;
            produitToUpdate.Actif = produit.Actif;
            produitToUpdate.MarqueId = produit.MarqueId;
            produitToUpdate.CategorieId = produit.CategorieId;
            produitToUpdate.ProduitImages = produit.ProduitImages;
            produitToUpdate.ProduitTailles = produit.ProduitTailles;
            produitToUpdate.ProduitCouleurs = produit.ProduitCouleurs;

            _context.SaveChanges();
        }
    }
}
