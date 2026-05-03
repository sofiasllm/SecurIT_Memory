using System;
using System.Collections.Generic;
using System.Drawing;

namespace SecurIT_Memory.Logic
{
    /// <summary>
    /// Gestionnaire principal de la logique du jeu Memory.
    /// </summary>
    public class JeuMemory
    {
        public List<Carte> Cartes { get; private set; }
        public int Score { get; private set; }
        public int Tentatives { get; private set; }
        private Random rng = new Random();

        public JeuMemory()
        {
            Cartes = new List<Carte>();
            Score = 0;
            Tentatives = 0;
        }

        /// <summary>
        /// Initialise une nouvelle partie avec un nombre de paires donné.
        /// </summary>
        public void InitialiserPartie(int nbPaires, List<Image> icons, Image dos)
        {
            Cartes.Clear();
            Score = 0;
            Tentatives = 0;

            if (icons.Count < nbPaires)
                throw new ArgumentException($"Pas assez d'icônes pour {nbPaires} paires.");

            for (int i = 0; i < nbPaires; i++)
            {
                // On utilise la même image pour les deux cartes de la paire
                Image facePartagee = icons[i];
                string symb = "Symbole_" + i;

                Cartes.Add(new Carte(i, symb, facePartagee, dos));
                Cartes.Add(new Carte(i, symb, facePartagee, dos));
            }

            Melanger();
        }

        private void Melanger()
        {
            int n = Cartes.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                Carte temp = Cartes[k];
                Cartes[k] = Cartes[n];
                Cartes[n] = temp;
            }
        }

        /// <summary>
        /// Vérifie si deux cartes forment une paire.
        /// </summary>
        public bool VerifierPaire(Carte c1, Carte c2)
        {
            Tentatives++;

            // Comparaison par ID pour s'assurer que c'est la même paire
            if (c1.ID == c2.ID)
            {
                c1.Etat = EtatCarte.Trouvee;
                c2.Etat = EtatCarte.Trouvee;
                Score++;
                return true;
            }

            return false;
        }

        public bool EstPartieTerminee(int nbPairesCibles)
        {
            return Score >= nbPairesCibles;
        }
    }
}