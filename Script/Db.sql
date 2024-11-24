-- Création des tables de référence
CREATE TABLE CATEGORIE (
    id INTEGER PRIMARY KEY ,
    nom VARCHAR(100) NOT NULL,
    description TEXT,
    IdMarque INT NOT NULL,
    IsHome BOOLEAN NOT NULL
);

CREATE TABLE MARQUE (
    id INTEGER PRIMARY KEY ,
    nom VARCHAR(100) NOT NULL,
    description TEXT,
    IsHome BOOLEAN NOT NULL
);

CREATE TABLE STATUT_COMMANDE (
    id INTEGER PRIMARY KEY ,
    libelle VARCHAR(50) NOT NULL
);

-- Table principale des produits
CREATE TABLE PRODUIT (
    id INTEGER PRIMARY KEY ,
    nom VARCHAR(200) NOT NULL,
    description TEXT,
    date_creation TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    actif BOOLEAN DEFAULT true,
    IsTendance BOOLEAN DEFAULT false,
    categorie_id INTEGER REFERENCES CATEGORIE(id),
    marque_id INTEGER REFERENCES MARQUE(id)
);

-- Tables de variants produits
CREATE TABLE PRODUIT_TAILLE (
    id INTEGER PRIMARY KEY ,
    produit_id INTEGER REFERENCES PRODUIT(id),
    taille VARCHAR(10) NOT NULL,
    stock INTEGER NOT NULL DEFAULT 0,
    prix DECIMAL(10,2) NOT NULL,
    CONSTRAINT unique_produit_taille UNIQUE (produit_id, taille)
);

CREATE TABLE PRODUIT_COULEUR (
    id INTEGER PRIMARY KEY ,
    produit_id INTEGER REFERENCES PRODUIT(id),
    couleur VARCHAR(50) NOT NULL,
    code_hex VARCHAR(7),
    CONSTRAINT unique_produit_couleur UNIQUE (produit_id, couleur)
);

CREATE TABLE PRODUIT_IMAGE (
    id INTEGER PRIMARY KEY ,
    produit_id INTEGER REFERENCES PRODUIT(id),
    url VARCHAR(255) NOT NULL,
    principale BOOLEAN DEFAULT false
);

-- Gestion des clients et adresses
CREATE TABLE CLIENT (
    id INTEGER PRIMARY KEY ,
    email VARCHAR(255) NOT NULL UNIQUE,
    mot_de_passe VARCHAR(255) NOT NULL,
    nom VARCHAR(100) NOT NULL,
    prenom VARCHAR(100) NOT NULL,
    date_inscription TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE ADRESSE (
    id INTEGER PRIMARY KEY ,
    client_id INTEGER REFERENCES CLIENT(id),
    rue VARCHAR(255) NOT NULL,
    ville VARCHAR(100) NOT NULL,
    code_postal VARCHAR(10) NOT NULL,
    pays VARCHAR(100) NOT NULL,
    principale BOOLEAN DEFAULT false
);

-- Gestion des commandes
CREATE TABLE COMMANDE (
    id INTEGER PRIMARY KEY ,
    client_id INTEGER REFERENCES CLIENT(id),
    adresse_livraison_id INTEGER REFERENCES ADRESSE(id),
    montant_total DECIMAL(10,2) NOT NULL,
    date_commande TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    statut_id INTEGER REFERENCES STATUT_COMMANDE(id)
);

CREATE TABLE LIGNE_COMMANDE (
    id INTEGER PRIMARY KEY ,
    commande_id INTEGER REFERENCES COMMANDE(id),
    produit_id INTEGER REFERENCES PRODUIT(id),
    produit_taille_id INTEGER REFERENCES PRODUIT_TAILLE(id),
    produit_couleur_id INTEGER REFERENCES PRODUIT_COULEUR(id),
    quantite INTEGER NOT NULL,
    prix_unitaire DECIMAL(10,2) NOT NULL
);

-- Système d'avis
CREATE TABLE AVIS (
    id INTEGER PRIMARY KEY ,
    client_id INTEGER REFERENCES CLIENT(id),
    produit_id INTEGER REFERENCES PRODUIT(id),
    note INTEGER NOT NULL CHECK (note BETWEEN 1 AND 5),
    commentaire TEXT,
    date_avis TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Création des index pour optimiser les performances
CREATE INDEX idx_produit_categorie ON PRODUIT(categorie_id);
CREATE INDEX idx_produit_marque ON PRODUIT(marque_id);
CREATE INDEX idx_produit_taille_produit ON PRODUIT_TAILLE(produit_id);
CREATE INDEX idx_produit_couleur_produit ON PRODUIT_COULEUR(produit_id);
CREATE INDEX idx_produit_image_produit ON PRODUIT_IMAGE(produit_id);
CREATE INDEX idx_commande_client ON COMMANDE(client_id);
CREATE INDEX idx_commande_statut ON COMMANDE(statut_id);
CREATE INDEX idx_ligne_commande_commande ON LIGNE_COMMANDE(commande_id);
CREATE INDEX idx_avis_produit ON AVIS(produit_id);
CREATE INDEX idx_avis_client ON AVIS(client_id);

-- Insertion des données de base pour les statuts de commande
INSERT INTO STATUT_COMMANDE (libelle) VALUES
    ('En attente de paiement'),
    ('Payée'),
    ('En préparation'),
    ('Expédiée'),
    ('Livrée'),
    ('Annulée');