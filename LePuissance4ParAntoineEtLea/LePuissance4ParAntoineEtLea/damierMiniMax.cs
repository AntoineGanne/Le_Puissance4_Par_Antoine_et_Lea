using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LePuissance4ParAntoineEtLea
{
    class damierMiniMax
    {
        private const int VX = 7;
        private const int VY = 6;
        private byte[,] damier;
        //Vector2 dernierePositionjoue;
        private int colonneJouee;
        private byte gagnant;
        private bool estFeuille ;

        

        public damierMiniMax(byte[,] damier_input)
        {
            //damier = new byte[VY,VX];
            this.Damier = new byte[VY, VX];
            for(int x= 0;x < VX; x++){
                for(int y = 0; y < VY; y++)
                {
                    damier[y,x] = damier_input[y,x];
                }
            }
            gagnant = 0;
            estFeuille = false;
           
        }

        public List<damierMiniMax> GetSuccesseurs(byte joueur)
        {
            List<damierMiniMax> resultatJeux = new List<damierMiniMax>();
            for (int x = 0; x < 7; x++)
            {
                if (damier[0, x] == 0)
                {
                    damierMiniMax nouveauDamier = new damierMiniMax(damier);
                    //si cela est possible,on joue un pion a la colonne x 
                    //et on ajoute le demier aux resultats;
                    if (nouveauDamier.posePion(x, joueur))
                    {
                        nouveauDamier.colonneJouee = x;
                        resultatJeux.Add(nouveauDamier);
                    }
                }
            }
            return resultatJeux;
        }

        private bool posePion(int colonne,byte joueur)
        {
            if (damier[0, colonne] == 0)
            {
                int y = 0;
                while (y < VY && damier[y, colonne] == 0)
                {
                    y++;
                }
                damier[y - 1, colonne] = joueur;

                //on fait le test de feuille maintenant, pour se simplifier la tache apres
                estFeuille=testFeuille(colonne, joueur);
                return true;
                
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// teste si la configutation actuelle de damier est une feuille (==partie finie)
        /// </summary>
        /// <param name="colonne"></param>
        /// <param name="joueur"></param>
        /// <returns></returns>
        public bool testFeuille(int colonne,byte joueur)
        {
            bool resultat = false;
            int y = -1;
            while (y + 1 < VY && damier[y + 1, colonne] == 0)
            {
                y++;
            }
            int xOffset = 0;
            int yOffset = 1;
            int nbPions = Bot.comptePionsDirection(colonne, y, xOffset, yOffset, joueur, damier);
            xOffset = 1;
            for (yOffset = -1; yOffset <= 1; yOffset++)
            {
                int nbPionsTemp = Bot.comptePionsDirection(colonne, y, xOffset, yOffset, joueur, damier);
                nbPions = (nbPions > nbPionsTemp ? nbPions : nbPionsTemp); //on prend le max de toutes les directions possibles
            }

            if (nbPions >= 4)
            {
                gagnant = joueur;
                resultat = true;
            }

            if (Puissance4.testDamierRemplit(this.damier)) resultat = true;
            return resultat;
        }

        /// <summary>
        /// retourne une valeur pour le damier.
        /// n'a de sens que si le damier est une feuille.
        /// </summary>
        /// <param name="numBot"></param>
        /// <returns></returns>
        public int Valeur(byte numBot)
        {
            if (estFeuille)
            {
                if (gagnant == 0) { return 0; }
                else
                {
                    return (gagnant == numBot ? 100 : -100);
                }
            }
            else
                return 0;
        }

        public byte[,] Damier
        {
            get
            {
                return damier;
            }

            set
            {
                damier = value;
            }
        }

        public byte Gagnant
        {
            get
            {
                return gagnant;
            }

            set
            {
                gagnant = value;
            }
        }

        public bool EstFeuille
        {
            get
            {
                return estFeuille;
            }

            set
            {
                estFeuille = value;
            }
        }

        public int ColonneJouee
        {
            get
            {
                return colonneJouee;
            }

            set
            {
                colonneJouee = value;
            }
        }

      
    }
}
