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
using Meth = System.Math;


namespace Invaders
{
    class PlayerTwo
    {
        // Public properties
        public Vector2 Position { get; set; }
        public float Speed { get; set; }
        public float ShotDelay { get; set; }
        public float DiagonalSpeed { get; set; }
        public Directions Direction { get; set; }
        public List<Texture2D> Texture { get; private set; }
        public Color Color { get; private set; }
        public List<Shot> Shots { get; set; }
        public Rectangle HitBox { get { return new Rectangle((int)Position.X, (int)Position.Y, Texture[0].Width, Texture[0].Height); } }
        public float Rotation { get; set; }
        public Vector2 BulletRotation { get; set; }
        public Vector2 Origin { get; set; }


        // Public booleans
        public bool dead = false;

        // Private members
        private Shot shotPrototype;
        private int shotDelayTimer = 0;
        private int frame = 0;
        private int animationTimer = 0;
        private List<PowerUpsPlayer2> powerUps;

        // Constructor(s)
        public PlayerTwo(GraphicsDevice graphicsDevice, List<Texture2D> texture, Color color, float speed, Shot shotPrototype, int shotSpeed)
        {
            this.Texture = texture;
            this.Color = color;
            this.Speed = speed;
            this.DiagonalSpeed = (float)Math.Sin(45) * speed;
            this.Rotation = 0;
            this.Direction = Directions.None;
            this.Position = new Vector2((graphicsDevice.Viewport.Width / 2) - (this.Texture[0].Width / 2), graphicsDevice.Viewport.Height - (this.Texture[0].Height * 4));
            this.shotPrototype = shotPrototype;
            this.Shots = new List<Shot>();
            this.powerUps = new List<PowerUpsPlayer2>();
            this.ShotDelay = shotSpeed;
            this.Origin = new Vector2(this.Position.X + this.Texture[0].Width / 2, this.Position.X + this.Texture[0].Height / 2);
        }

        // Method(s)
        public void Update(GraphicsDevice graphicsDevice)
        {
            if (Direction == Directions.Left) { Position = new Vector2(Position.X - Speed, Position.Y); }
            if (Direction == Directions.Right) { Position = new Vector2(Position.X + Speed, Position.Y); }
            if (Direction == Directions.Up) { Position = new Vector2(Position.X, Position.Y - Speed); }
            if (Direction == Directions.Down) { Position = new Vector2(Position.X, Position.Y + Speed); }
            if (Direction == Directions.NorthEast) { Position = new Vector2(Position.X + DiagonalSpeed, Position.Y - DiagonalSpeed); }
            if (Direction == Directions.NorthWest) { Position = new Vector2(Position.X - DiagonalSpeed, Position.Y - DiagonalSpeed); }
            if (Direction == Directions.SouthEast) { Position = new Vector2(Position.X + DiagonalSpeed, Position.Y + DiagonalSpeed); }
            if (Direction == Directions.SouthWest) { Position = new Vector2(Position.X - DiagonalSpeed, Position.Y + DiagonalSpeed); }
            if (Direction == Directions.None) { Position = new Vector2(Position.X, Position.Y); }


            if (this.Position.X - this.Speed < 0)
            {
                this.Position = new Vector2(this.Position.X + this.Speed, this.Position.Y);
                this.Direction = Directions.None;
            }

            if (this.Position.X + this.Speed > graphicsDevice.Viewport.Width - this.Texture[0].Width)
            {
                this.Position = new Vector2(this.Position.X - this.Speed, this.Position.Y);
                this.Direction = Directions.None;
            }
            if (this.Position.Y - this.Speed < 0)
            {
                this.Position = new Vector2(this.Position.X, this.Position.Y + this.Speed);
                this.Direction = Directions.None;
            }

            if (this.Position.Y + this.Speed > graphicsDevice.Viewport.Height - this.Texture[0].Height)
            {
                this.Position = new Vector2(this.Position.X, this.Position.Y - this.Speed);
                this.Direction = Directions.None;
            }

            for (int i = Shots.Count - 1; i >= 0; i--)
            {
                Shots[i].Update(graphicsDevice, false);
                if (Shots[i].dead)
                {
                    Shots.RemoveAt(i);
                    i--;
                }
            }
            this.BulletRotation = new Vector2((float)Math.Cos(Rotation), (float)Math.Sin(Rotation));
            animationTimer++;
            if (animationTimer == 5)
            {
                animationTimer = 0;
                if (frame == 2)
                {
                    frame = 0;
                }
                else
                    frame++;
            }
        }

        public void GetKeyboardInput(KeyboardState keyboardState, KeyboardState oldKey, GamePadState gamepadState, SoundEffect fireSound)
        {
            if (keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A)) { this.Direction = Directions.Left; }
            if (keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D)) { this.Direction = Directions.Right; }
            if (keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W)) { this.Direction = Directions.Up; }
            if (keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S)) { this.Direction = Directions.Down; }
            if ((keyboardState.IsKeyDown(Keys.Right) && keyboardState.IsKeyDown(Keys.Up)) || (keyboardState.IsKeyDown(Keys.D) && keyboardState.IsKeyDown(Keys.W)))
                { this.Direction = Directions.NorthEast; }
            if ((keyboardState.IsKeyDown(Keys.Left) && keyboardState.IsKeyDown(Keys.Up)) || (keyboardState.IsKeyDown(Keys.A) && keyboardState.IsKeyDown(Keys.W)))
                { this.Direction = Directions.NorthWest; }
            if ((keyboardState.IsKeyDown(Keys.Right) && keyboardState.IsKeyDown(Keys.Down)) || (keyboardState.IsKeyDown(Keys.D) && keyboardState.IsKeyDown(Keys.S)))
                { this.Direction = Directions.SouthEast; }
            if ((keyboardState.IsKeyDown(Keys.Left) && keyboardState.IsKeyDown(Keys.Down)) || (keyboardState.IsKeyDown(Keys.A) && keyboardState.IsKeyDown(Keys.S)))
                { this.Direction = Directions.SouthWest; }
            if (keyboardState.IsKeyUp(Keys.Left) && keyboardState.IsKeyUp(Keys.Right) && keyboardState.IsKeyUp(Keys.Down) && keyboardState.IsKeyUp(Keys.Up) &&
                keyboardState.IsKeyUp(Keys.A) && keyboardState.IsKeyUp(Keys.D) && keyboardState.IsKeyUp(Keys.S) && keyboardState.IsKeyUp(Keys.W))
                { Direction = Directions.None; }

            if (keyboardState.IsKeyDown(Keys.Space) && this.shotDelayTimer == 0 && !powerUps.Contains(PowerUpsPlayer2.Duoshot) && !powerUps.Contains(PowerUpsPlayer2.TripleShot))
            {
                fireSound.Play(0.5f, 0.0f, 0.0f);
                this.Shots.Add(new Shot(new Vector2(this.Position.X, this.Position.Y), this.shotPrototype.Speed, this.shotPrototype.Texture, this.shotPrototype.Color, this.BulletRotation, this.Rotation - (float)(Math.PI * 0.5f)));
                this.shotDelayTimer = (int)Math.Round(this.ShotDelay);
            }
            else if (keyboardState.IsKeyDown(Keys.Space) && oldKey.IsKeyUp(Keys.Space) && this.shotDelayTimer == 0 && powerUps.Contains(PowerUpsPlayer2.TripleShot))
            {
                fireSound.Play(0.5f, 0.0f, 0.0f);
                this.Shots.Add(new Shot(new Vector2(this.Position.X - (Texture[0].Width / 2), this.Position.Y), this.shotPrototype.Speed, this.shotPrototype.Texture, this.shotPrototype.Color, this.BulletRotation, this.Rotation - (float)(Math.PI * 0.5f)));
                this.Shots.Add(new Shot(new Vector2(this.Position.X, this.Position.Y), this.shotPrototype.Speed, this.shotPrototype.Texture, this.shotPrototype.Color, this.BulletRotation, this.Rotation - (float)(Math.PI * 0.5f)));
                this.Shots.Add(new Shot(new Vector2(this.Position.X + (Texture[0].Width / 2), this.Position.Y), this.shotPrototype.Speed, this.shotPrototype.Texture, this.shotPrototype.Color, this.BulletRotation, this.Rotation - (float)(Math.PI * 0.5f)));
                this.shotDelayTimer = (int)Math.Round(this.ShotDelay);
            }
            else if ((keyboardState.IsKeyDown(Keys.Space)) && this.shotDelayTimer == 0 && powerUps.Contains(PowerUpsPlayer2.Duoshot))
            {
                fireSound.Play(0.5f, 0.0f, 0.0f);
                this.Shots.Add(new Shot(new Vector2(this.Position.X - (Texture[0].Width / 2), this.Position.Y), this.shotPrototype.Speed, this.shotPrototype.Texture, this.shotPrototype.Color, this.BulletRotation, this.Rotation - (float)(Math.PI * 0.5f)));
                this.Shots.Add(new Shot(new Vector2(this.Position.X + (Texture[0].Width / 2), this.Position.Y), this.shotPrototype.Speed, this.shotPrototype.Texture, this.shotPrototype.Color, this.BulletRotation, this.Rotation - (float)(Math.PI * 0.5f)));
                this.shotDelayTimer = (int)Math.Round(this.ShotDelay);
            }

            if (shotDelayTimer > 0) { shotDelayTimer--; }
        }

        public void Draw(SpriteBatch spriteBatch, bool playerFlash)
        {
            foreach (Shot s in Shots)
            {
                s.Draw(spriteBatch);
            }
            if (!playerFlash)
            {
                spriteBatch.Draw(this.Texture[frame], this.Position, null, Color, Rotation, new Vector2(this.Texture[0].Width / 2, this.Texture[0].Height / 2), 1f, SpriteEffects.None, 1);
            }
        }
        public void AddPower(PowerUpsPlayer2 power)
        {
            if (power == PowerUpsPlayer2.Delay)
            {
                ShotDelay *= 0.95f;
            }
            else if (power == PowerUpsPlayer2.Speed)
            {
                Speed += 0.3f;
                DiagonalSpeed = (float)Math.Sin(45) * Speed;
            }
            else if (power == PowerUpsPlayer2.SpeedDown)
            {
                Speed -= 0.3f;
                DiagonalSpeed = (float)Math.Sin(45) * Speed;
            }
            else
            {
                ShotDelay /= 0.85f;
                powerUps.Add(power);
            }
        }
    }
}
