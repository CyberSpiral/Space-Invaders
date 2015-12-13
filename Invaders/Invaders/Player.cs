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
    class Player
    {
        // Public properties
        public Vector2 Position { get; set; }
        public float Speed { get; set; }
        public Directions Direction { get; set; }
        public List<Texture2D> Texture { get; private set; }
        public Color Color { get; private set; }
        public List<Shot> Shots { get; set; }
        public List<ExplosionShot> Explosions { get; set; }
        public Rectangle HitBox { get { return new Rectangle((int)Position.X, (int)Position.Y, Texture[0].Width, Texture[0].Height); } }
        public float Rotation { get; set; }
        public Vector2 BulletPlus45Rotation { get; set; }
        public Vector2 BulletMinus45Rotation { get; set; }
        public Vector2 BulletRotation { get; set; }
        public Vector2 Origin { get; set; }
        public List<Texture2D> ExplosionsTextures { get; set; }
        public List<PowerUpsPlayer1> PowerUps { get; set; }


        // Public booleans
        public bool dead = false;

        // Private members
        private Shot shotPrototype;
        private int shotDelayTimer = 0;
        private int shotDelay;
        private int frame = 0;
        private int animationTimer = 0;

        // Constructor(s)
        public Player(GraphicsDevice graphicsDevice, List<Texture2D> texture, Color color, Shot shotPrototype, int shotSpeed, List<Texture2D> textures)
        {
            this.Texture = texture;
            this.Color = color;
            this.Rotation = -(float)(Math.PI / 2);
            this.Direction = Directions.None;
            this.Position = new Vector2((graphicsDevice.Viewport.Width / 2) - (this.Texture[0].Width / 2), graphicsDevice.Viewport.Height - (this.Texture[0].Height * 2));
            this.shotPrototype = shotPrototype;
            this.Shots = new List<Shot>();
            this.Explosions = new List<ExplosionShot>();
            this.shotDelay = shotSpeed;
            this.Origin = new Vector2(this.Position.X + this.Texture[0].Width / 2, this.Position.X + this.Texture[0].Height / 2);
            this.ExplosionsTextures = textures;
        }

        // Method(s)
        public void Update(GraphicsDevice graphicsDevice, Vector2 rotation, SoundEffect explode)
        {
            foreach (ExplosionShot ex in Explosions)
            {
                ex.Update();
            }
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

            for (int i = Shots.Count - 1; i >= 0; i--)
            {
                Shots[i].Update(graphicsDevice, true);
                if (Vector2.Distance(Shots[i].Position, Shots[i].Target) < 5)
                {
                    Explosions.Add(new ExplosionShot(Shots[i].Position, ExplosionsTextures, Color.White));
                    Game1.screenShake += 30.0f;
                    Shots[i].dead = true;
                }
                else if ((Shots[i].Position.Y < Shots[i].Target.Y) && (Shots[i].Position.Y < this.Position.Y - 20))
                {
                    Explosions.Add(new ExplosionShot(Shots[i].Position, ExplosionsTextures, Color.White));
                    Game1.screenShake += 30.0f;
                    Shots[i].dead = true;
                }
                if (Shots[i].dead)
                {
                    explode.Play();
                    Shots.RemoveAt(i);
                    i--;
                }
            }
            for (int i = 0; i < Explosions.Count; i++)
            {
                if (Explosions[i].dead)
                {
                    Explosions.RemoveAt(i);
                    i--;
                }
            }
            float direction = (float)Math.Atan2(rotation.Y, rotation.X);
            this.Rotation = direction;
            this.BulletRotation = new Vector2((float)Math.Cos(Rotation), (float)Math.Sin(Rotation));
            this.BulletMinus45Rotation = new Vector2((float)Math.Cos(Rotation - 0.1), (float)Math.Sin(Rotation - 0.1));
            this.BulletPlus45Rotation = new Vector2((float)Math.Cos(Rotation + 0.1), (float)Math.Sin(Rotation + 0.1));
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

        public void GetKeyboardInput(KeyboardState keyboardState, MouseState oldMouse, MouseState newMouse, GamePadState gamepadState, SoundEffect fireSound)
        {
            if (keyboardState.IsKeyDown(Keys.Left) || gamepadState.IsButtonDown(Buttons.DPadLeft)) { this.Direction = Directions.Left; }
            if (keyboardState.IsKeyDown(Keys.Right) || gamepadState.IsButtonDown(Buttons.DPadRight)) { this.Direction = Directions.Right; }
            if ((keyboardState.IsKeyUp(Keys.Left) && keyboardState.IsKeyUp(Keys.Right)) &&
                (gamepadState.IsButtonUp(Buttons.DPadLeft) && gamepadState.IsButtonUp(Buttons.DPadRight)))
            { this.Direction = Directions.None; }

            if ((oldMouse.LeftButton == ButtonState.Released && newMouse.LeftButton == ButtonState.Pressed) && this.shotDelayTimer == 0)
            {
                fireSound.Play(0.5f, 0.0f, 0.0f);
                this.Shots.Add(new Shot(new Vector2(this.Position.X, this.Position.Y), this.shotPrototype.Speed, this.shotPrototype.Texture, this.shotPrototype.Color, this.BulletRotation, this.Rotation, new Vector2(newMouse.X, newMouse.Y)));
                /*this.Shots.Add(new Shot(new Vector2(this.Position.X, this.Position.Y), this.shotPrototype.Speed, this.shotPrototype.Texture, this.shotPrototype.Color, this.BulletPlus45Rotation, this.Rotation + 0.2f, new Vector2(newMouse.X + 50, newMouse.Y)));
                this.Shots.Add(new Shot(new Vector2(this.Position.X, this.Position.Y), this.shotPrototype.Speed, this.shotPrototype.Texture, this.shotPrototype.Color, this.BulletMinus45Rotation, this.Rotation - 0.2f, new Vector2(newMouse.X - 50, newMouse.Y)));*/
                this.shotDelayTimer = this.shotDelay;
            }
            if ((oldMouse.LeftButton == ButtonState.Released && newMouse.LeftButton == ButtonState.Pressed) && this.shotDelayTimer == 0)
            {
                fireSound.Play(0.5f, 0.0f, 0.0f);
                this.Shots.Add(new Shot(new Vector2(this.Position.X, this.Position.Y), this.shotPrototype.Speed, this.shotPrototype.Texture, this.shotPrototype.Color, this.BulletRotation, this.Rotation, new Vector2(newMouse.X, newMouse.Y)));
                /*this.Shots.Add(new Shot(new Vector2(this.Position.X, this.Position.Y), this.shotPrototype.Speed, this.shotPrototype.Texture, this.shotPrototype.Color, this.BulletPlus45Rotation, this.Rotation + 0.2f, new Vector2(newMouse.X + 50, newMouse.Y)));
                this.Shots.Add(new Shot(new Vector2(this.Position.X, this.Position.Y), this.shotPrototype.Speed, this.shotPrototype.Texture, this.shotPrototype.Color, this.BulletMinus45Rotation, this.Rotation - 0.2f, new Vector2(newMouse.X - 50, newMouse.Y)));*/
                this.shotDelayTimer = this.shotDelay;
            }

            if (shotDelayTimer > 0) { shotDelayTimer--; }
        }

        public void Draw(SpriteBatch spriteBatch, bool playerFlash)
        {
            foreach (Shot s in Shots)
            {
                s.Draw(spriteBatch);
            }
            foreach (ExplosionShot s in Explosions)
            {
                s.Draw(spriteBatch);
            }
            if (!playerFlash)
            {
                spriteBatch.Draw(this.Texture[frame], this.Position, null, Color, Rotation + (float)(Math.PI * 0.5f), new Vector2(this.Texture[0].Width / 2, this.Texture[0].Height / 2), 1f, SpriteEffects.None, 1);
            }
        }

        public void AddPower(PowerUpsPlayer1 power)
        {
            PowerUps.Add(power);
        }
    }
}
