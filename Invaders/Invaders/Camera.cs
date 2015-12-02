using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Invaders
{
    class Camera
    {
        protected Vector2 position;
        protected Matrix viewMatrix;

        public Matrix ViewMatrix
        {
            get { return viewMatrix; }
        }
        public Vector2 Position
        {
            get { return position; }
        }
        public int ScreenWidth
        {
            get { return GraphicsDeviceManager.DefaultBackBufferWidth; }
        }
        public int ScreenHeight
        {
            get { return GraphicsDeviceManager.DefaultBackBufferHeight; }
        }
        public void Update(Vector2 playerPosition)
        {
            position.X = playerPosition.X - (ScreenWidth / 2);
            position.Y = playerPosition.Y - (ScreenHeight / 2);



            viewMatrix = Matrix.CreateTranslation(new Vector3(-position, 0));
        }

        
    }
}
