using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Data.Sqlite;

namespace SecurIT_Memory.Logic
{
    /// <summary>
    /// Gère la persistance des scores dans un fichier local.
    /// </summary>
    public static class ScoreManager
    {
        private static readonly string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "leaderboard.db");
        private static readonly string connectionString = $"Data Source={dbPath}";

        static ScoreManager()
        {
            InitialiserBase();
        }

        /// <summary>
        /// Enregistre une performance à la fin d'une partie.
        /// </summary>
        public static void EnregistrerScore(int secondes, int tentatives)
        {
            EnregistrerScore("Agent", secondes, tentatives);
        }

        public static void EnregistrerScore(string nom, int secondes, int tentatives)
        {
            try
            {
                InitialiserBase();

                using var connection = new SqliteConnection(connectionString);
                connection.Open();

                using var command = connection.CreateCommand();
                command.CommandText = @"
                    INSERT INTO Scores (Nom, Temps, Essais, DatePartie)
                    VALUES ($nom, $temps, $essais, $datePartie);";
                command.Parameters.AddWithValue("$nom", string.IsNullOrWhiteSpace(nom) ? "Agent" : nom.Trim());
                command.Parameters.AddWithValue("$temps", secondes);
                command.Parameters.AddWithValue("$essais", tentatives);
                command.Parameters.AddWithValue("$datePartie", DateTime.Now.ToString("O"));
                command.ExecuteNonQuery();
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
                var best = GetLeaderboard(1).FirstOrDefault();

                return best == null ? "Aucun record" : $"{best.Temps}s par {best.Nom}";
            }
            catch { return "---"; }
        }

        public static List<LeaderboardEntry> GetLeaderboard(int limite = 10)
        {
            var scores = new List<LeaderboardEntry>();

            try
            {
                InitialiserBase();

                using var connection = new SqliteConnection(connectionString);
                connection.Open();

                using var command = connection.CreateCommand();
                command.CommandText = @"
                    SELECT Nom, Temps, Essais, DatePartie
                    FROM Scores
                    ORDER BY Temps ASC, Essais ASC, DatePartie ASC
                    LIMIT $limite;";
                command.Parameters.AddWithValue("$limite", limite);

                using var reader = command.ExecuteReader();
                int rang = 1;
                while (reader.Read())
                {
                    scores.Add(new LeaderboardEntry
                    {
                        Rang = rang++,
                        Nom = reader.GetString(0),
                        Temps = reader.GetInt32(1),
                        Essais = reader.GetInt32(2),
                        DatePartie = DateTime.TryParse(reader.GetString(3), out DateTime datePartie) ? datePartie : DateTime.MinValue
                    });
                }
            }
            catch { }

            return scores;
        }

        private static void InitialiserBase()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);

            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Scores (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Nom TEXT NOT NULL,
                    Temps INTEGER NOT NULL,
                    Essais INTEGER NOT NULL,
                    DatePartie TEXT NOT NULL
                );";
            command.ExecuteNonQuery();
        }
    }
}
