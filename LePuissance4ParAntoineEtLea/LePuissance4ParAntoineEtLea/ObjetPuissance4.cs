using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LePuissance4ParAntoineEtLea
{
    class ObjetPuissance4
    {
        private Texture2D texture;
        private Vector2 position;
        private Vector2 size;

        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public Vector2 Size
        {
            get { return size; }
            set { size = value; }
        }

        public ObjetPuissance4(Texture2D texture_input,Vector2 position_input,Vector2 size_input)
        {
            this.texture = texture_input;
            this.position = position_input;
            this.size = size_input;
        }

    }
}
