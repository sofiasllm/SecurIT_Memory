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
                throw new ArgumentException("Pas assez d'icônes pour le nombre de paires demandé.");

            for (int i = 0; i < nbPaires; i++)
            {
                // On crée deux instances de cartes avec le même ID pour former une paire
                Cartes.Add(new Carte(i, icons[i], dos));
                Cartes.Add(new Carte(i, icons[i], dos));
            }

            Melanger();
        }

        /// <summary>
        /// Mélange les cartes de manière aléatoire (Algorithme de Fisher-Yates).
        /// </summary>
        private void Melanger()
        {
            int n = Cartes.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                Carte value = Cartes[k];
                Cartes[k] = Cartes[n];
                Cartes[n] = value;
            }
        }

        /// <summary>
        /// Vérifie si deux cartes forment une paire.
        /// </summary>
        /// <returns>True si c'est un match, False sinon.</returns>
        public bool VerifierPaire(Carte c1, Carte c2)
        {
            Tentatives++;

            if (c1.ID == c2.ID)
            {
                c1.Etat = EtatCarte.Trouvee;
                c2.Etat = EtatCarte.Trouvee;
                Score++;
                return true;
            }
            else
            {
                // Les cartes seront remises en état "Cachée" par le contrôleur après un délai
                c1.Cacher();
                c2.Cacher();
                return false;
            }
        }

        /// <summary>
        /// Vérifie si toutes les paires ont été trouvées.
        /// </summary>
        public bool EstPartieTerminee(int nbPairesCibles)
        {
            return Score == nbPairesCibles;
        }
    }
}
