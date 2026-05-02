using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace SecurIT_Memory.Logic
{
    /// <summary>
    /// Gère la persistance des scores dans un fichier local.
    /// </summary>
    public static class ScoreManager
    {
        private static string filePath = "highscores.txt";

        /// <summary>
        /// Enregistre une performance à la fin d'une partie.
        /// </summary>
        public static void EnregistrerScore(int secondes, int tentatives)
        {
            try
            {
                string log = $"{DateTime.Now:G}|{secondes}|{tentatives}";
                File.AppendAllLines(filePath, new[] { log });
            }
            catch (Exception) { /* Ignorer les erreurs d'écriture en mode démo */ }
        }

        /// <summary>
        /// Récupère le meilleur temps enregistré.
        /// </summary>
        public static string GetMeilleurScore()
        {
            try
            {
                if (!File.Exists(filePath)) return "Aucun record";
                var lines = File.ReadAllLines(filePath);
                if (lines.Length == 0) return "Aucun record";

                var best = lines
                    .Select(s => s.Split('|'))
                    .Where(parts => parts.Length > 1)
                    .Select(parts => int.Parse(parts[1]))
                    .Min();

                return $"{best}s";
            }
            catch { return "---"; }
        }
    }
}
