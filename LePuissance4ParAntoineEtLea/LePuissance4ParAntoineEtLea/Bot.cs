using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LePuissance4ParAntoineEtLea
{
    class Bot
    {
        private String difficulte;
        private static String[] niveaux = { "facile", "intermediaire", "difficile" };
        private byte joueurBot; //stocke le numero du bot 
        public Bot()
        {
            difficulte =niveaux[1];
            joueurBot = 2;
        }

        public int choixColonne(byte[,] damier)
        {
            int dimX = damier.GetLength(1);
            int dimY = damier.GetLength(0);
            Random aleatoire = new Random();
            if (difficulte == "facile")
            {

                return aleatoire.Next(dimX);
            }

            switch (difficulte)
            {
                case "facile":
                    return aleatoire.Next(dimX);
                    break;
                case "intermediaire":
                    return choixBotIntermediaire(damier);
                    break;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="damier"></param>
        /// <returns></returns>
        private int choixBotIntermediaire(byte[,] damier)
        {
            Random aleatoire = new Random();
            int dimX = damier.GetLength(1);
            int dimY = damier.GetLength(0);
            int nbPionsBot = 0; //permet de compter les pions du bot (pour chaque colonne)
            int nbPionsAdv = 0; //permet de compter les pions de l'adversaire
            byte adversaire = (byte)(joueurBot == 1 ? 2 : 1);
            int colonneBloquageAdv = -1;  //si l'on peut bloquer l'adversaire en une certaine colonne, on la stocke ici

            for (int colonne=0; colonne < dimX; ++colonne)
            {
                int y = -1;
                while (y+1 < dimY && damier[y+1, colonne] == 0)
                {
                    y++;
                }
                int xOffset = 0;
                int yOffset = 1;
                nbPionsBot = comptePionsDirection(colonne, y, xOffset, yOffset, joueurBot,damier);
                nbPionsAdv= comptePionsDirection(colonne, y, xOffset, yOffset, adversaire, damier);
                xOffset = 1;
                for (yOffset = -1; yOffset <= 1; yOffset++)
                {
                    int nbPionsTemp = comptePionsDirection(colonne, y, xOffset, yOffset,joueurBot,damier);
                    nbPionsBot = (nbPionsBot > nbPionsTemp ? nbPionsBot : nbPionsTemp); //on prend le max de toutes les directions possibles

                    nbPionsTemp = comptePionsDirection(colonne, y, xOffset, yOffset, adversaire, damier);
                    nbPionsAdv = (nbPionsAdv > nbPionsTemp ? nbPionsAdv : nbPionsTemp); //on prend le max de toutes les directions possibles
                }

                if (nbPionsBot >= 4) return colonne;  //si on peut gagner, on a pas besoin de calculer les autres possibilitées
                if (nbPionsAdv >= 4) colonneBloquageAdv = colonne;
            }
            if (colonneBloquageAdv != -1) return colonneBloquageAdv;
            else
            {
                return aleatoire.Next(dimX);
            }
        }

        /// <summary>
        /// compte les pions d'un meme joueur autours d'une position donnée et selon une direction donnée par xOffset et yOffset
        /// </summary>
        /// <param name="xIN"></param>
        /// <param name="yIN"></param>
        /// <param name="xOffset"></param>
        /// <param name="yOffset"></param>
        /// <param name="joueur"></param>
        /// <param name="damier"></param>
        /// <returns></returns>
        private int comptePionsDirection(int xIN, int yIN, int xOffset, int yOffset, byte joueur, byte[,] damier)
        {
            int nbPions = 1; //commence a 1 car le pion posé est compté d'office

            int dimX = damier.GetLength(1);
            int dimY = damier.GetLength(0);
            for (int sens = -1; sens <= 1; sens += 2)
            {
                int x = xIN + xOffset;
                int y = yIN + yOffset;
                while (x >= 0 && x < dimX && y >= 0 && y < dimY && damier[y, x] == joueur)
                {
                    nbPions++;
                    x += xOffset;
                    y += yOffset;
                }
                xOffset *= -1;
                yOffset *= -1;
            }

            return nbPions;
        }



    }
}
