using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LePuissance4ParAntoineEtLea
{
    /// <summary>
    /// Modelise un pion attiré par la gravité , très basique
    /// </summary>
    class ObjetAnime
    {
        Vector2 destination; //point de l'ecran vers lequel l'objet doit finir son déplacement
        ObjetPuissance4 sprite;
        public ObjetAnime(int colonne_input,Byte [,] damier, Texture2D texture_input, Vector2 position_input, Vector2 size_input)
        {
            sprite = new ObjetPuissance4(texture_input, position_input, size_input);
        }

    }
}
