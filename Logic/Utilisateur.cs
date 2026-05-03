using System;

namespace SecurIT_Memory.Logic
{
    /// <summary>
    /// Représente un joueur avec ses statistiques (POO : Encapsulation).
    /// </summary>
    public class Utilisateur
    {
        private string _nom;
        private int _meilleurTemps;
        private int _totalEssais;

        public Utilisateur(string nom)
        {
            _nom = nom;
            _meilleurTemps = int.MaxValue;
            _totalEssais = 0;
        }

        public string Nom
        {
            get => _nom;
            set => _nom = string.IsNullOrWhiteSpace(value) ? "Inconnu" : value;
        }

        public int MeilleurTemps
        {
            get => _meilleurTemps;
            set { if (value < _meilleurTemps) _meilleurTemps = value; }
        }

        public int TotalEssais
        {
            get => _totalEssais;
            set => _totalEssais = value;
        }

        public override string ToString() => $"Joueur: {_nom} | Record: {(_meilleurTemps == int.MaxValue ? "N/A" : _meilleurTemps + "s")}";
    }
}
