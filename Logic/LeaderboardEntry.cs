using System;

namespace SecurIT_Memory.Logic
{
    public class LeaderboardEntry
    {
        public int Rang { get; set; }
        public string Nom { get; set; } = string.Empty;
        public int Temps { get; set; }
        public int Essais { get; set; }
        public DateTime DatePartie { get; set; }
    }
}
