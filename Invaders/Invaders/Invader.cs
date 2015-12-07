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
    class Invader
    {
        // Public properties
        public Vector2 Position { get; set; }
        public List<Texture2D> Textures { get; set; }
        public Color Color { get; private set; }
        public int MoveRange { get; set; }
        public Directions Direction { get; set; }
        public int DiveChance { get; set; }
        public Rectangle HitBox { get { return new Rectangle((int)Position.X, (int)Position.Y, Textures[frame].Width, Textures[frame].Height); } }
        public Vector2 DirectionAgainst { get; set; }
        public 

        // Public booleans
        public bool dead = false;
        public bool diving = false;
        public bool starting = true;
        public bool divingAgainst = false;

        // Private members
        private int frame = 0;
        private int animationDelayTimer = 0;
        private int animationDelay = 20;
        private int moveDelayTimer;
        private int moveDelay;
        private Color startColor;
        private int movesX = 1;
        private int movesY = 0;
        private int startRange = 0;

        // Constructor(s)
        public Invader(Vector2 position, int moveRange, int speed, List<Texture2D> textures, Color color)
        {
            this.Position = position;
            this.MoveRange = moveRange;
            this.moveDelay = speed;
            this.moveDelayTimer = speed;
            this.Textures = textures;
            this.Color = color;
            this.Direction = Directions.Right;
            this.DiveChance = 4;
            this.startColor = color;
        }

        // Method(s)
        public void Update(GraphicsDevice graphicsDevice)
        {
            if (!diving)
            {
                if (startRange < 240)
                {
                    this.Position = new Vector2(this.Position.X, this.Position.Y + 2);
                    startRange++;
                }
                else
                {
                    starting = false;
                    if (moveDelayTimer == 0)
                    {
                        moveDelayTimer = moveDelay;
                        if (Direction == Directions.Right && movesX < 3) { this.Position = new Vector2(this.Position.X + this.MoveRange, this.Position.Y); }
                        if (Direction == Directions.Left && movesX < 3) { this.Position = new Vector2(this.Position.X - this.MoveRange, this.Position.Y); }
                        this.movesX++;
                    }
                    else { moveDelayTimer--; }

                    if (movesX == 4)
                    {
                        movesX = 0;
                        movesY++;
                        this.Position = new Vector2(this.Position.X, this.Position.Y + MoveRange);
                        if (Direction == Directions.Left) { Direction = Directions.Right; }
                        else { Direction = Directions.Left; }
                    }
                }
            }

            if (Position.Y > graphicsDevice.Viewport.Height - 200 && !diving) { DiveChance = 100; }

            if (diving && divingAgainst == false)
            {
                if (Direction == Directions.Left) { Position = new Vector2(Position.X - 2, Position.Y + 2); }
                if (Direction == Directions.Right) { Position = new Vector2(Position.X + 2, Position.Y + 2); }
                if (Direction == Directions.None) { Position = new Vector2(Position.X, Position.Y + 2); }
            }

            if (diving && (this.Position.X < 60 || this.Position.X > graphicsDevice.Viewport.Width - 60) && divingAgainst == false)
            {
                if (Direction == Directions.Left) { Direction = Directions.Right; }
                else if (Direction == Directions.Right) { Direction = Directions.Left; }
                else { Direction = Directions.None; }
            }

            if (diving && divingAgainst)
            {
                this.Position -= this.DirectionAgainst * 4;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (animationDelayTimer == 0)
            {
                animationDelayTimer = animationDelay;
                if (this.frame < this.Textures.Count - 1) { this.frame++; }
                else { this.frame = 0; }
            }
            else { animationDelayTimer--; }

            if (diving)
            {
                if (Position.Y % 20 == 1) { Color = Color.Red; }
                if (Position.Y % 20 == 0) { Color = startColor; }
            }
            spriteBatch.Draw(this.Textures[frame], this.Position, Color);
        }
    }
}
