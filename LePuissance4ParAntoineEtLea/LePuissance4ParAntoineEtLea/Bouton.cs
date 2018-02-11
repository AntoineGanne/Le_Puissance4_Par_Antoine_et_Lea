using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LePuissance4ParAntoineEtLea
{
    class Bouton: ObjetPuissance4  //hérite de ObjetPuissance4
    {
        private string texte;  //contient le texte a afficher sur le bouton + permet de retrouver l'effet de bouton
        private bool menu;  //est vrai si le bouton fait partie du menu (et donc est faux lorsqu'il doit etre affiché durant le jeu)
        public Bouton(string texte_input,bool menu_input,Texture2D texture_input, Vector2 position_input, Vector2 size_input):base(texture_input,position_input,size_input)
        {
            texte = texte_input;
            menu = menu_input;
        }
        
        public string Texte
        {
            get
            {
                return texte;
            }

            set
            {
                texte = value;
            }
        }

        
        public bool Menu
        {
            get
            {
                return menu;
            }

            set
            {
                menu = value;
            }
        }
        
        /// <summary>
        /// retourne vrai si la position donnée en paramètre est au dessus du bouton
        /// </summary>
        /// <param name="coord"></param>
        /// <returns></returns>
        public bool isOver(Vector2 coord)
        {
            return !(coord.X < Position.X || coord.X > Position.X + Size.X
                     || coord.Y < Position.Y || coord.Y > Position.Y + Size.Y);
        }

        /// <summary>
        /// dessine le bouton: on dessine la texture du bouton puis on ajoute le texte par dessus
        /// </summary>
        /// <param name="sprt"></param>
        /// <param name="textFont"></param>
        /// <param name="menuActif"></param>
        public void draw(SpriteBatch sprt,SpriteFont textFont, bool menuActif)
        {
            // on dessine le bouton dans 2 cas:
            //1) le bouton fait partie du menu et le menu est actif (les deux sont vrais)
            //2) le bouton ne fait pas partie du menu et et le menu n'est pas acitf (les deux sont faux)
            if (menu==menuActif)
            {
                sprt.Draw(Texture, Position, Color.White);
                string messageFin = string.Format(Texte);
                Vector2 pos = new Vector2(Position.X + (Size.X - 11 * Texte.Length) / 2, Position.Y + 10);
                sprt.DrawString(textFont, messageFin, pos, Color.Black);
            }
        }

    }
}
