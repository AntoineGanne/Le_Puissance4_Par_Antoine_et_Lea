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
                y--;
                damier[y, colonne] = joueur;

                //on fait le test de feuille maintenant, pour se simplifier la tache apres
                estFeuille=testFeuille(colonne,y, joueur);
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
        public bool testFeuille(int colonne,int y,byte joueur)
        {
            bool resultat = false;
            
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
        /// a peu de sens que si le damier n'est pas une feuille. -> a ameliorer 
        /// 
        /// profondeur= profondeur de l'arbre pour l'algo min/max, va en descendant
        /// ainsi une victoire apres peu de coups => grande profondeur => plus grande valeur
        /// </summary>
        /// <param name="numBot"></param>
        /// <returns></returns>
        public int Valeur(byte numBot,byte numJoueurActuel, int profondeur, List<damierMiniMax> listeFils)
        {
            // la valeur des feuilles n'est pas sensé etre utile, elle est laissée pour l'instant pour eviter des instabilitées.
            if (estFeuille)
            {
                return 0;
                /*
                if (gagnant == 0) { return 0; }
                else
                {
                    // on cherche avant tout a recompenser les victoires assurées
                    return(int)(gagnant == numBot ? 0 : -10*profondeur);
                    
                }
                */
            }
            else
            {
                //ici, on rentre deans le if si c'est au tour du bot de jouer
                if (numJoueurActuel == numBot)
                {
                    byte numHumain = (byte)(numBot == 1 ? 2 : 1);
                    //si le bot perds qu'importe le pion joué
                    if (nePeutQuePerdre(numHumain, listeFils)) return -profondeur; 
                }
                else
                {
                    if (nePeutQuePerdre(numBot, listeFils)) return profondeur;
                }
            }
            return 0;
        }


        /// <summary>
        /// compte le nombre de pions joués
        /// utlisée par la fonction d'évaluation (plus maintenant)
        /// </summary>
        /// <returns></returns>
        private int nbPionsPlayed()
        {
            int resultat = 0;
            foreach(byte b in this.Damier)
            {
                if (b != 0) resultat++;
            }
            return resultat;
        }


        /// <summary>
        /// renvoit vrai si le joueur(qui joue contre numAdversaire) perd qu'importe le pion qu'il joue
        /// </summary>
        /// <param name="numJoueur"></param>
        /// <returns></returns>
        public bool nePeutQuePerdre(byte numAdversaire, List<damierMiniMax> listeFils)
        {
            //si on trouve au moins deux colonnes perdants alors il ne peut pas empecher sa defaite
            int nbColonnesPerdantes = 0;
            //byte numAdversaire = (byte)(numJoueur == 1 ? 2 : 1);
            foreach(damierMiniMax fils in listeFils)
            {
                if (fils.estFeuille && fils.gagnant == numAdversaire)
                {
                    nbColonnesPerdantes++;
                    if (nbColonnesPerdantes >= 2) return true;
                }
            }

            return false;
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
