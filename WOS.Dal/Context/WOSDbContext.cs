using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WOS.Dal.Mapping;
using WOS.Model;

namespace WOS.Dal.Context
{
    public class WOSDbContext : DbContext
    {
        public WOSDbContext(DbContextOptions<WOSDbContext> options) : base(options)
        {
        }

        public DbSet<Categorie> Categories { get; set; }
        public DbSet<Marque> Marques { get; set; }
        public DbSet<Produit> Produits { get; set; }
        public DbSet<ProduitTaille> ProduitTailles { get; set; }
        public DbSet<ProduitCouleur> ProduitCouleurs { get; set; }
        public DbSet<ProduitImage> ProduitImages { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Adresse> Adresses { get; set; }
        public DbSet<Commande> Commandes { get; set; }
        public DbSet<LigneCommande> LignesCommande { get; set; }
        public DbSet<StatutCommande> StatutsCommande { get; set; }
        public DbSet<Avis> Avis { get; set; }
        public DbSet<Admin> Admins { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CategorieMap());
            modelBuilder.ApplyConfiguration(new MarqueMap());
            modelBuilder.ApplyConfiguration(new ProduitMap());
            modelBuilder.ApplyConfiguration(new ProduitTailleMap());
            modelBuilder.ApplyConfiguration(new ProduitCouleurMap());
            modelBuilder.ApplyConfiguration(new ProduitImageMap());
            modelBuilder.ApplyConfiguration(new ClientMap());
            modelBuilder.ApplyConfiguration(new AdresseMap());
            modelBuilder.ApplyConfiguration(new CommandeMap());
            modelBuilder.ApplyConfiguration(new LigneCommandeMap());
            modelBuilder.ApplyConfiguration(new StatutCommandeMap());
            modelBuilder.ApplyConfiguration(new AvisMap());
            modelBuilder.ApplyConfiguration(new AdminMap());
        }
    }
}
