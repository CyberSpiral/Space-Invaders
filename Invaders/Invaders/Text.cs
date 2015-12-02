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
    class Text
    {
        // Public properties
        public Vector2 Position { get; set; }
        public SpriteFont SpriteFont { get; set; }
        public Color Color { get; private set; }
        public string TextString { get; set; }
        public Vector2 Movement { get; set; }

        // Public booleans
        public bool dead = false;

        // Private members
        private int duration;

        // Constructor(s)
        public Text(Vector2 position, SpriteFont spriteFont, Color color, string text, int duration, Vector2 movement)
        {
            this.Position = position;
            this.SpriteFont = spriteFont;
            this.Color = color;
            this.TextString = text;
            this.duration = duration;
            this.Movement = movement;
        }

        // Method(s)
        public void Draw(SpriteBatch spriteBatch)
        {
            this.Position += this.Movement;
            if (duration > 0)
            {
                spriteBatch.DrawString(SpriteFont, TextString, Position, Color);
                duration--;
            }
            else { dead = true; }
        }
    }
}
