using System.Drawing;

namespace SecurIT_Memory.Logic
{
    /// <summary>
    /// Classe représentant une carte individuelle du Memory.
    /// </summary>
    public class Carte
    {
        // Propriétés encapsulées (Lecture seule pour l'ID et les images)
        public int ID { get; private set; }
        public Image ImageFace { get; private set; }
        public Image ImageDos { get; private set; }
        public EtatCarte Etat { get; set; }

        public Carte(int id, Image imageFace, Image imageDos)
        {
            this.ID = id;
            this.ImageFace = imageFace;
            this.ImageDos = imageDos;
            this.Etat = EtatCarte.Cachee;
        }

        /// <summary>
        /// Retourne l'image appropriée en fonction de l'état actuel de la carte.
        /// </summary>
        /// <returns>L'image de face ou de dos.</returns>
        public Image GetImageAffichee()
        {
            if (Etat == EtatCarte.Cachee)
            {
                return ImageDos;
            }
            return ImageFace;
        }

        /// <summary>
        /// Change l'état de la carte vers Révélée.
        /// </summary>
        public void Reveler()
        {
            if (Etat == EtatCarte.Cachee)
                Etat = EtatCarte.Revelee;
        }

        /// <summary>
        /// Remet la carte face cachée.
        /// </summary>
        public void Cacher()
        {
            if (Etat == EtatCarte.Revelee)
                Etat = EtatCarte.Cachee;
        }
    }
}
