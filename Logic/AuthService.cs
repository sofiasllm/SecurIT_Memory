using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SecurIT_Memory.Logic
{
    /// <summary>
    /// Service de gestion des utilisateurs (Simule une base de données SQL pour l'instant).
    /// </summary>
    public static class AuthService
    {
        private static List<Utilisateur> _utilisateurs = new List<Utilisateur>();
        private static string _dbPath = "users_db.txt";

        static AuthService()
        {
            ChargerUtilisateurs();
        }

        public static Utilisateur? UtilisateurConnecte { get; private set; }

        public static bool Connexion(string nom)
        {
            var user = _utilisateurs.FirstOrDefault(u => u.Nom.Equals(nom, StringComparison.OrdinalIgnoreCase));
            if (user != null)
            {
                UtilisateurConnecte = user;
                return true;
            }
            return false;
        }

        public static void Inscription(string nom)
        {
            if (!_utilisateurs.Any(u => u.Nom.Equals(nom, StringComparison.OrdinalIgnoreCase)))
            {
                var newUser = new Utilisateur(nom);
                _utilisateurs.Add(newUser);
                SauvegarderUtilisateurs();
                UtilisateurConnecte = newUser;
            }
        }

        private static void ChargerUtilisateurs()
        {
            if (!File.Exists(_dbPath)) return;
            try
            {
                var lines = File.ReadAllLines(_dbPath);
                foreach (var line in lines)
                {
                    var parts = line.Split('|');
                    if (parts.Length >= 3)
                    {
                        var u = new Utilisateur(parts[0]);
                        u.MeilleurTemps = int.Parse(parts[1]);
                        u.TotalEssais = int.Parse(parts[2]);
                        _utilisateurs.Add(u);
                    }
                }
            }
            catch { /* Logique de secours */ }
        }

        public static void SauvegarderUtilisateurs()
        {
            try
            {
                var lines = _utilisateurs.Select(u => $"{u.Nom}|{u.MeilleurTemps}|{u.TotalEssais}");
                File.WriteAllLines(_dbPath, lines);
            }
            catch { }
        }
    }
}
