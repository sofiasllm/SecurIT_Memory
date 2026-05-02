namespace SecurIT_Memory.Logic
{
    /// <summary>
    /// Définit les trois états possibles d'une carte dans le jeu.
    /// </summary>
    public enum EtatCarte
    {
        Cachee,   // La carte est face contre table
        Revelee,  // La carte est retournée mais pas encore validée
        Trouvee   // La paire a été trouvée, la carte reste visible
    }
}
