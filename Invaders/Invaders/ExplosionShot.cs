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
    class ExplosionShot
    {
        // Public properties
        public Vector2 Position { get; set; }
        public List<Texture2D> Textures { get; set; }
        public Color Color { get; private set; }
        public Rectangle HitBox { get { return new Rectangle((int)Position.X - 30, (int)Position.Y - 30, 60, 60); } }

        // Public booleans
        public bool dead = false;

        // Private members
        private int frame = 0;
        private int animationTimer = 0;

        // Constructor(s)
        public ExplosionShot(Vector2 position, List<Texture2D> textures, Color color)
        {
            this.Position = position;
            this.Textures = textures;
            this.Color = color;
        }

        // Method(s)
        public void Update()
        {
            animationTimer++;
            if (animationTimer == 8)
            {
                animationTimer = 0;
                if (frame == 6)
                {
                    this.dead = true;
                }
                else
                    frame++;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this.Textures[frame], new Rectangle((int)Position.X - 30, (int)Position.Y - 30, 60, 60), Color);
        }
    }
}
