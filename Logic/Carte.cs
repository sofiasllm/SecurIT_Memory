using System.Drawing;

namespace SecurIT_Memory.Logic
{
    /// <summary>
    /// Classe représentant une carte individuelle du Memory.
    /// </summary>
    public class Carte
    {
        public int ID { get; private set; }
        public Image ImageFace { get; private set; }
        public Image ImageDos { get; private set; }
        public string Symbole { get; private set; }
        public EtatCarte Etat { get; set; }
 
        public Carte(int id, string symbole, Image imageFace, Image imageDos)
        {
            this.ID = id;
            this.ImageFace = imageFace;
            this.ImageDos = imageDos;
            this.Symbole = symbole;
            this.Etat = EtatCarte.Cachee;
        }
 
        public Image GetImageAffichee()
        {
            return (Etat == EtatCarte.Cachee) ? ImageDos : ImageFace;
        }
 
        public void Reveler()
        {
            if (Etat == EtatCarte.Cachee)
                Etat = EtatCarte.Revelee;
        }
 
        public void Cacher()
        {
            if (Etat == EtatCarte.Revelee)
                Etat = EtatCarte.Cachee;
        }
    }
}