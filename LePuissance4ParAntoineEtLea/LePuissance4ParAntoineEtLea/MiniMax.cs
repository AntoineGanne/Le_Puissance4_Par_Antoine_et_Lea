using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LePuissance4ParAntoineEtLea
{
    class MiniMax
    {

        private const int _MAXVAL = 1000;   // sert a initiliser les valeurs des min et max.
        private static int jeuxCalcules = 0;  //sert a compter le nombre d'itérations des fonctions min max

        /// <summary>
        /// appelle l'algorithme min max sur le damier passé en paramètre.
        /// Avant cela, On verifie les successeurs pour le cas ou l'on peut gagner tout de suite ou le cas où
        /// l'adversaire peut gagner au prochain tour
        /// </summary>
        /// <param name="damier"></param>
        /// <param name="numBot"></param>
        /// <param name="profondeur"></param>
        /// <returns></returns>
        public int getMeilleureColonne(damierMiniMax damier, byte numBot,int profondeur)
        {
            List<damierMiniMax> resultats = new List<damierMiniMax>();
            int max = -_MAXVAL;
            jeuxCalcules = 0;

            List<damierMiniMax> successeurs = successeursReduit(damier, numBot);

            

            foreach (damierMiniMax damierActuel in successeurs)
            {
                int valeurFils = this.maxiVal(damierActuel, numBot,profondeur,-_MAXVAL,_MAXVAL); //recursif
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
        /// get the mini value.
        /// C'est la partie min de l'algo min max.
        /// 
        /// </summary>
        /// <param name="damierMM"></param>
        /// <param name="numBot"></param>
        /// <returns></returns>
        private int MiniVal(damierMiniMax damierMM, byte numBot,int profondeur,int alpha,int beta)
        {
            byte numHumain = (byte)(numBot == 1 ? 2 : 1);
            List<damierMiniMax> successeurs = damierMM.GetSuccesseurs(numHumain);
            if (damierMM.EstFeuille || profondeur <= 0) return damierMM.Valeur(numBot,numHumain,profondeur,successeurs);
            else
            {
                if (damierMM.nePeutQuePerdre(numHumain,successeurs))
                {
                    return profondeur;
                }
                else
                {
                    int min = _MAXVAL;

                    foreach (damierMiniMax currentDamier in successeurs)
                    {
                        ++jeuxCalcules;
                         profondeur--;
                        int valeurFils = this.maxiVal(currentDamier, numBot, profondeur, alpha, beta);
                        
                        min = Math.Min(min, valeurFils);
                        
                        //coupure alpha
                        if (alpha >= valeurFils)
                        {
                           return valeurFils;
                        }
                        
                        beta = Math.Min(beta, valeurFils);
                    }
                    return min;
                }
            }
        }

        /// <summary>
        /// get the maxi value.
        /// C'est la partie max de l'algo min max.
        /// </summary>
        /// <param name="damierMM"></param>
        /// <param name="numBot"></param>
        /// <param name="profondeur"></param>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        /// <returns></returns>
        private int maxiVal(damierMiniMax damierMM,byte numBot, int profondeur, int alpha, int beta)
        {
            byte numHumain = (byte)(numBot == 1 ? 2 : 1);
            List<damierMiniMax> successeurs = damierMM.GetSuccesseurs(numBot);
            if (damierMM.EstFeuille || profondeur<=0) return damierMM.Valeur(numBot,numBot,profondeur,successeurs);
            else
            {
                if (damierMM.nePeutQuePerdre(numBot, successeurs))
                {
                    return -profondeur;
                }
                else
                {
                    int max = -_MAXVAL;
                    foreach (damierMiniMax currentDamier in successeurs)
                    {
                        ++jeuxCalcules;
                        profondeur--;
                        int valeurFils = this.MiniVal(currentDamier, numBot, profondeur, alpha, beta);
                        max = Math.Max(max, valeurFils);

                        //coupure beta
                        if (valeurFils >= beta)
                        {
                            return valeurFils;
                        }
                        alpha = Math.Max(alpha, valeurFils);

                    }
                    return max;
                }
            }
        }

        /// <summary>
        /// renvoit la liste des successeurs de damier
        /// cependant si le bot peut gagner a ce tour alors il ne renvoit que le coup gagnant
        /// et si l'adversaire peut gagner au prochain tour, il renvoir seulement les
        /// successeurs qui empechentl'adversaire de gagner
        /// 
        /// cela permets de fortement reduire le nombre de coups calculés par l'algorithme min/max
        /// </summary>
        /// <param name="damier"></param>
        /// <param name="numBot"></param>
        /// <returns></returns>
        private List<damierMiniMax> successeursReduit(damierMiniMax damier, byte numBot)
        {
            List<damierMiniMax> successeurs = damier.GetSuccesseurs(numBot);
            List<damierMiniMax> resultat = new List<damierMiniMax>(successeurs);
            int nbElementsEnMoins = 0; 
            for(int i=0;i<successeurs.Count;i++)
            {
                if (successeurs[i].EstFeuille && successeurs[i].Gagnant == numBot)
                {
                    resultat.Clear();
                    resultat.Add(successeurs[i]);
                    return resultat;
                }
                else
                {
                    byte numHumain = (byte)(numBot == 1 ? 2 : 1);
                    damierMiniMax copieFils = new damierMiniMax(successeurs[i].Damier);
                    List<damierMiniMax> nextCoupsHumain = successeurs[i].GetSuccesseurs(numHumain);
                    for (int j = 0; j < nextCoupsHumain.Count; j++)
                    {
                        if (nextCoupsHumain[j].EstFeuille && nextCoupsHumain[j].Gagnant == numHumain)
                        {
                            resultat.RemoveAt(i-nbElementsEnMoins);
                            nbElementsEnMoins++;

                            break;
                        }
                    }
                }
            }
            //si toutes les positions sont perdantes
            if (resultat.Count == 0) { resultat = successeurs; }


            return resultat;
        }


    }
}
