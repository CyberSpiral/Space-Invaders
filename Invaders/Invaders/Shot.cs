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
    class Shot
    {
        // Public properties
        public Vector2 Position { get; set; }
        public float Speed { get; set; }
        public Texture2D Texture { get; private set; }
        public Color Color { get; private set; }
        public Rectangle HitBox { get { return new Rectangle((int)Position.X - (Texture.Width / 2),
            (int)Position.Y - (Texture.Height / 2), Texture.Width, Texture.Height); } }
        public Vector2 Direction { get; set; }
        public float Rotation { get; set; }
        public Vector2 Target { get; set; }

        // Public booleans
        public bool dead = false;

        // Constructor(s)
        public Shot(Vector2 origin, float speed, Texture2D texture, Color color, Vector2 direction, float rotation, Vector2 target)
        {
            this.Position = origin;
            this.Speed = speed;
            this.Texture = texture;
            this.Color = color;
            this.Direction = direction;
            this.Rotation = rotation;
            this.Target = target;
        }
        public Shot(Vector2 origin, float speed, Texture2D texture, Color color, Vector2 direction, float rotation)
        {
            this.Position = origin;
            this.Speed = speed;
            this.Texture = texture;
            this.Color = color;
            this.Direction = direction;
            this.Rotation = rotation;
        }

        // Method(s)
        public void Update(GraphicsDevice graphicsDevice, bool Targeted)
        {
            if (this.Position.Y < 50) { this.dead = true; }
            if (Targeted)
                this.Position += this.Direction * this.Speed;
            else
                this.Position = new Vector2(this.Position.X, this.Position.Y - this.Speed);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this.Texture, this.Position, null, Color, Rotation + (float)(Math.PI * 0.5f), new Vector2(this.Texture.Width / 2, this.Texture.Height / 2), 1f, SpriteEffects.None, 1);
        }
    }
}
