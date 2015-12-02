using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Invaders
{
    class Explosion
    {
        // Public properties 
        public Vector2 Position { get; set; }
        public List<Texture2D> Textures { get; set; }
        public Color Color { get; private set; }

        // Public booleans
        public bool dead = false;

        // Private members
        private int frame = 0;
        private int animationDelayTimer = 0;
        private int animationDelay = 10;

        // Constructor(s)
        public Explosion(Vector2 position, List<Texture2D> textures, Color color)
        {
            this.Position = position;
            this.Textures = textures;
            this.Color = color;
        }

        // Method(s)
        public void Draw(SpriteBatch spriteBatch)
        {
            if (animationDelayTimer == 0)
            {
                animationDelayTimer = animationDelay;
                if (this.frame < this.Textures.Count - 1) { this.frame++; }
                else { this.dead = true; }
            }
            else { animationDelayTimer--; }

            spriteBatch.Draw(this.Textures[frame], this.Position, Color);
        }
    }
}
