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
    class Star
    {
        // Public properties
        public Vector2 Position { get; set; }
        public Vector2 Movement { get; set; }
        public Texture2D Texture { get; set; }
        public Color Color { get; private set; }

        // Public booleans
        public bool dead = false;

        // Constructor(s)
        public Star(Texture2D texture, GraphicsDevice graphicsDevice)
        {
            Random r = new Random();
            this.Position = new Vector2(r.Next(0, graphicsDevice.Viewport.Width), -10);
            this.Movement = new Vector2(0, r.Next(1, 4));
            this.Texture = texture;
            this.Color = Color.White;

            int i = r.Next(0, 3);
            if (i == 0) { this.Color = Color.Gray; }
            if (i == 1) { this.Color = Color.GhostWhite; }
            if (i == 2) { this.Color = Color.DarkGray; }
        }

        // Method(s)
        public void Update(GraphicsDevice graphicsDevice)
        {
            this.Position += this.Movement;
            if (this.Position.Y > graphicsDevice.Viewport.Height) { this.dead = true; }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this.Texture, this.Position, Color);
        }
    }
}
