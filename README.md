# SecurIT Memory - Salon de l'Innovation Tech

## 🛡️ Présentation du Projet
**SecurIT Memory** est un mini-jeu de cartes interactif conçu pour sensibiliser les visiteurs du Salon de l'Innovation Tech aux thématiques de la cybersécurité. Le jeu met au défi la mémoire des utilisateurs en leur demandant de retrouver des paires d'icônes liées à la sécurité informatique (pare-feu, virus, mots de passe, etc.).

Ce projet a été réalisé en binôme dans le cadre du module de développement C# WinForms.

## 🚀 Fonctionnalités
- **Thématique Cyber** : Interface immersive avec un thème sombre et des accents néon.
- **Grille Dynamique** : Génération automatique de la grille de jeu en fonction de la difficulté (4x4 ou 6x6).
- **Logique POO** : Architecture robuste séparant la logique métier (`Logic`) de l'interface utilisateur (`Forms`).
- **Graphismes Procéduraux** : Utilisation des classes `Bitmap` et `Graphics` pour dessiner des icônes de sécurité en temps réel.
- **Leaderboard SQL** : Sauvegarde automatique des scores dans une base SQLite locale avec nom du joueur, temps et nombre d'essais.
- **Classement Dynamique** : Affichage du top 10 depuis le menu principal, trié par meilleur temps puis par nombre d'essais.
- **Gestion du Temps** : Chronomètre précis et délai de mémorisation via des Timers.

## 🛠️ Architecture Technique
Le projet suit une structure organisée pour faciliter la maintenance :

### Logic (Le Cœur)
- `Carte.cs` : Modèle de données pour une carte (ID, état, images).
- `JeuMemory.cs` : Gestionnaire de la logique (mélange de Fisher-Yates, vérification des paires).
- `ScoreManager.cs` : Gestion de la persistance SQL des scores avec `Microsoft.Data.Sqlite`.
- `LeaderboardEntry.cs` : Modèle représentant une ligne du classement.

### Forms (L'Interface)
- `MainForm.cs` : Menu principal avec affichage du record et du leaderboard SQL dynamique.
- `OptionsForm.cs` : Sélection du niveau de difficulté.
- `GameForm.cs` : Moteur de jeu gérant la grille dynamique et les animations de victoire.

## 🎨 Notions Clés Utilisées
- **Programmation Orientée Objet** : Encapsulation des données et utilisation de classes spécialisées.
- **Manipulation d'Images** : Dessin vectoriel avec `System.Drawing`.
- **Événements & Timers** : Gestion de l'asynchronisme visuel (délai de retournement).
- **Persistance SQL Locale** : Utilisation de SQLite pour stocker les performances dans `leaderboard.db`.

## 🏆 Leaderboard SQL
La partie bonus **Leaderboard SQL** est intégrée au jeu :

- **Base locale** : un fichier `leaderboard.db` est créé automatiquement dans le dossier de sortie de l'application.
- **Table `Scores`** : chaque victoire enregistre le nom du joueur, le temps en secondes, le nombre d'essais et la date de la partie.
- **Classement** : le menu principal affiche les 10 meilleurs scores, classés par temps croissant puis par nombre d'essais.
- **Actualisation** : le classement se recharge automatiquement après une partie et peut aussi être actualisé avec le bouton `RAFRAÎCHIR`.

## 💻 Installation & Lancement
1. Ouvrir le fichier de solution `.sln` dans **Visual Studio**.
2. S'assurer que le projet cible le framework **.NET 6.0 ou supérieur**.
3. Restaurer les packages NuGet si nécessaire.
4. Compiler et lancer (`F5`).

Package utilisé pour la base SQL locale :

- `Microsoft.Data.Sqlite`

---
*Réalisé pour le projet SecurIT Memory - 2026*
