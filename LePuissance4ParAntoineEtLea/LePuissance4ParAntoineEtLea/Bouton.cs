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
        private string texte;
        private Boolean visible;
        public Bouton(string texte_input,bool visible_input,Texture2D texture_input, Vector2 position_input, Vector2 size_input):base(texture_input,position_input,size_input)
        {
            texte = texte_input;
            visible = visible_input;
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

        public bool Visible
        {
            get
            {
                return visible;
            }

            set
            {
                visible = value;
            }
        }

        public bool isOver(Vector2 coord)
        {
            return !(coord.X < Position.X || coord.X > Position.X + Size.X
                     || coord.Y < Position.Y || coord.Y > Position.Y + Size.Y);
        }

    }
}
