using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LePuissance4ParAntoineEtLea
{
    class MiniMax
    {

        private const int _MAXVAL = 10000;
        private static int jeuxCalcules = 0;

        public int getMeilleureColonne(damierMiniMax damier, byte numBot,int nbTours)
        {
            List<damierMiniMax> resultats = new List<damierMiniMax>();
            int max = -_MAXVAL;
            jeuxCalcules = 0;
            foreach(damierMiniMax damierActuel in damier.GetSuccesseurs(numBot))
            {
                int valeurFils = this.MiniVal(damierActuel, numBot,nbTours); //recursif
                if (valeurFils > max) //on prend la plus haute valeur
                {
                    resultats.Clear();
                    max = valeurFils;
                }
                if (valeurFils == max) resultats.Add(damierActuel);
            }

            //a la fin on renvoit un damier aléatoire parmis les meilleurs possibilitées
            Random rdm = new Random();
            int index = rdm.Next(resultats.Count);
            Console.WriteLine("jeux calculés= " + jeuxCalcules);
            return resultats[index].ColonneJouee;
        }

        /// <summary>
        /// get the mini value
        /// </summary>
        /// <param name="damierMM"></param>
        /// <param name="numBot"></param>
        /// <returns></returns>
        private int MiniVal(damierMiniMax damierMM, byte numBot,int nbTour)
        {
            if(damierMM.EstFeuille)return damierMM.Valeur(numBot);
            else
            {
                if (nbTour <= 0)
                {
                    return 0;
                }
                else
                {
                    int min = _MAXVAL;
                    byte numHumain = (byte)(numBot == 1 ? 2 : 1);
                    foreach (damierMiniMax currentDamier in damierMM.GetSuccesseurs(numHumain))
                    {
                        ++jeuxCalcules;
                        int valeurFils = this.maxiVal(currentDamier, numBot, nbTour--);
                        min = Math.Min(min, valeurFils);
                    }
                    return min;
                }
            }
        }

        private int maxiVal(damierMiniMax damierMM,byte numBot, int nbTour)
        {
            if (damierMM.EstFeuille) return damierMM.Valeur(numBot);
            else
            {
                if (nbTour <= 0)
                {
                    return 0;
                }
                else
                {
                    int max = -_MAXVAL;
                    foreach (damierMiniMax currentDamier in damierMM.GetSuccesseurs(numBot))
                    {
                        ++jeuxCalcules;
                        int valeurFils = this.MiniVal(currentDamier, numBot, nbTour--);
                        max = Math.Max(max, valeurFils);
                    }
                    return max;
                }
            }
        }
    }
}
