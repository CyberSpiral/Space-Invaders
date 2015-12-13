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
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        #region parameters
        // Numeric game parameters (see method ResetGame to change starting parameters)
        int invaderSpeed;
        int score;
        int wave;
        int flashTimer;
        int introTimer;
        int kills;
        int numberOfStars;
        int maxWaves;
        int playerShotSpeed;
        int playerTwoShotSpeed;
        int player1life;
        int player2life;
        int invulnerableTimer;
        int message;
        Camera camera;
        Vector2 cameraPosition;

        // Boolean game parameters (see method ResetGame to change starting parameters)
        bool flash;
        bool win;
        bool perfectRound;
        bool paused;
        bool playerFlash;
        bool shotHimself;

        bool gameHasBegun;
        bool player1IsIn;
        bool player2IsIn;

        bool betweenRound;
        bool notSelected = true;
        private bool startNextRound;

        List<PowerUpsPlayer1> selectablePowers1;
        List<PowerUpsPlayer2> selectablePowers2;

        PowerUpsPlayer1 selectedPower1;
        PowerUpsPlayer2 selectedPower2;

        bool player1HasSelected;
        bool player2HasSelected;


        // Object members
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Player player;
        PlayerTwo player2;
        Texture2D logo;
        List<Invader> invaderList;
        List<Texture2D> invader_01;
        List<Texture2D> player2D;
        List<Texture2D> playerTwo2D;
        List<Explosion> explosionList;
        List<Texture2D> explosions;
        List<Texture2D> explosion;
        List<Text> textList;
        List<Color> invaderColors;
        List<Star> stars;
        Song backgroundMusic;
        List<SoundEffect> SFX;
        SpriteFont font;
        Texture2D interfaceBackground;
        Texture2D playerLife;
        Random r = new Random();

        MouseState oldMouse;
        KeyboardState oldKey;

        Vector2 direction;

        public static float screenShake = 0;

        //Crosshair
        Texture2D crosshairTexture;

        Texture2D powers1;
        Texture2D powers2;
        #endregion
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            // Reset gama parameters
            ResetGame();

            // Load music & sound effects
            backgroundMusic = Content.Load<Song>("game");
            MediaPlayer.Volume = 0.5f;
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(backgroundMusic);
            SFX = new List<SoundEffect>();
            SFX.Add(Content.Load<SoundEffect>("dive"));
            SFX.Add(Content.Load<SoundEffect>("explosion"));
            SFX.Add(Content.Load<SoundEffect>("fire"));
            SFX.Add(Content.Load<SoundEffect>("explosion2"));

            // Load logotype and interface graphics
            logo = Content.Load<Texture2D>("logo");
            font = Content.Load<SpriteFont>("font");
            // Create new texture for the interface
            interfaceBackground = new Texture2D(GraphicsDevice, 1, 1);
            Color[] c = new Color[1];
            c[0] = Color.White;
            interfaceBackground.SetData(c);

            // Initialize game object lists
            explosionList = new List<Explosion>();
            invaderList = new List<Invader>();
            textList = new List<Text>();
            stars = new List<Star>();

            // Fill texture list for invaders
            invader_01 = new List<Texture2D>();
            invader_01.Add(Content.Load<Texture2D>("invader01_01"));
            invader_01.Add(Content.Load<Texture2D>("invader01_02"));
            invader_01.Add(Content.Load<Texture2D>("invader01_03"));

            // Fill texture list for explosions
            explosion = new List<Texture2D>();
            explosion.Add(Content.Load<Texture2D>("explosion_01"));
            explosion.Add(Content.Load<Texture2D>("explosion_02"));
            explosion.Add(Content.Load<Texture2D>("explosion_03"));

            // Fill player textures
            player2D = new List<Texture2D>();
            player2D.Add(Content.Load<Texture2D>("player_01"));
            player2D.Add(Content.Load<Texture2D>("player_02"));
            player2D.Add(Content.Load<Texture2D>("player_03"));

            playerTwo2D = new List<Texture2D>();
            playerTwo2D.Add(Content.Load<Texture2D>("player2_01"));
            playerTwo2D.Add(Content.Load<Texture2D>("player2_02"));
            playerTwo2D.Add(Content.Load<Texture2D>("player2_03"));

            // Fill texture list for explosions
            explosions = new List<Texture2D>();
            explosions.Add(Content.Load<Texture2D>("explosionRetro_1"));
            explosions.Add(Content.Load<Texture2D>("explosionRetro_2"));
            explosions.Add(Content.Load<Texture2D>("explosionRetro_3"));
            explosions.Add(Content.Load<Texture2D>("explosionRetro_4"));
            explosions.Add(Content.Load<Texture2D>("explosionRetro_5"));
            explosions.Add(Content.Load<Texture2D>("explosionRetro_6"));
            explosions.Add(Content.Load<Texture2D>("explosionRetro_7"));

            // Fill color list for invaders
            invaderColors = new List<Color>();
            invaderColors.Add(Color.White);
            invaderColors.Add(Color.LightPink);
            invaderColors.Add(Color.PaleVioletRed);
            invaderColors.Add(Color.LightGreen);
            invaderColors.Add(Color.GreenYellow);
            invaderColors.Add(Color.Lime);
            invaderColors.Add(Color.Aquamarine);


            //Fill texture for crosshair
            crosshairTexture = Content.Load<Texture2D>("crosshair");

            //Life texture
            playerLife = Content.Load<Texture2D>("player");

            // Initialize graphics and screen
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
            camera = new Camera();

            // Initialize players
            player = new Player(GraphicsDevice, player2D, Color.White, new Shot(new Vector2(0, 0), 3f, Content.Load<Texture2D>("player_shot"), Color.Gold, new Vector2(0, 0), 0, new Vector2(0, 0)), playerShotSpeed, explosions);
            player2 = new PlayerTwo(GraphicsDevice, playerTwo2D, Color.White, 3.85f, new Shot(new Vector2(0, 0), 6.5f, Content.Load<Texture2D>("player_shot"), Color.Gold, new Vector2(0, 0), 0), playerTwoShotSpeed);

            // Initialize starting screen with first wave of invaders and starting stars
            textList.Add(new Text(new Vector2(350, 380), font, Color.White, "Moddad av mig!", 60, Vector2.Zero));
            GenerateStartingStars();

            selectablePowers1 = new List<PowerUpsPlayer1>();
            selectablePowers2 = new List<PowerUpsPlayer2>();

            powers1 = Content.Load<Texture2D>("multi");
            powers2 = Content.Load<Texture2D>("multi2");

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void UnloadContent()
        {
        }

        // Main game loop
        protected override void Update(GameTime gameTime)
        {
            if (!gameHasBegun)
            {

                if (Keyboard.GetState().IsKeyDown(Keys.Space))
                {
                    player2IsIn = true;
                }
                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    player1IsIn = true;
                }
                if (player1IsIn && player2IsIn)
                {
                    SpawnInvaders(invaderSpeed);
                    gameHasBegun = true;
                }
                if ((Keyboard.GetState().IsKeyDown(Keys.Enter) && player1IsIn && !player2IsIn) || (Keyboard.GetState().IsKeyDown(Keys.Enter) && player2IsIn && !player1IsIn))
                {
                    SpawnInvaders(invaderSpeed);
                    gameHasBegun = true;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Escape)) { this.Exit(); }
            }
            else if (betweenRound)
            {
                if (player1IsIn && player2IsIn)
                {
                    while (notSelected)
                    {
                        selectablePowers1.Add((PowerUpsPlayer1)r.Next(0, 5));
                        selectablePowers1.Add((PowerUpsPlayer1)r.Next(0, 5));
                        selectablePowers1.Add((PowerUpsPlayer1)r.Next(0, 5));
                        selectablePowers2.Add((PowerUpsPlayer2)r.Next(0, 5));
                        selectablePowers2.Add((PowerUpsPlayer2)r.Next(0, 5));
                        selectablePowers2.Add((PowerUpsPlayer2)r.Next(0, 5));
                        if (((selectablePowers2[0] != selectablePowers2[1]) && (selectablePowers2[1] != selectablePowers2[2]) && (selectablePowers2[0] != selectablePowers2[2])) &&
                            ((selectablePowers1[0] != selectablePowers1[1]) && (selectablePowers1[1] != selectablePowers1[2]) && (selectablePowers1[0] != selectablePowers1[2])))
                            notSelected = false;
                        else
                        {
                            selectablePowers1.Clear(); selectablePowers2.Clear();
                        }
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.I))
                    {
                        selectedPower1 = selectablePowers1[0];
                        player1HasSelected = true;
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.O))
                    {
                        selectedPower1 = selectablePowers1[1];
                        player1HasSelected = true;
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.P))
                    {
                        selectedPower1 = selectablePowers1[2];
                        player1HasSelected = true;
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.D1))
                    {
                        selectedPower2 = selectablePowers2[0];
                        player2HasSelected = true;
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.D2))
                    {
                        selectedPower2 = selectablePowers2[1];
                        player2HasSelected = true;
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.D3))
                    {
                        selectedPower2 = selectablePowers2[2];
                        player2HasSelected = true;
                    }
                    if (player1HasSelected)
                    {
                        startNextRound = true;
                    }
                }
                else if (player1IsIn && !player2IsIn)
                {
                    while (notSelected)
                    {
                        selectablePowers2.Add((PowerUpsPlayer2)r.Next(0, 5));
                        selectablePowers2.Add((PowerUpsPlayer2)r.Next(0, 5));
                        selectablePowers2.Add((PowerUpsPlayer2)r.Next(0, 5));
                        if ((selectablePowers2[0] != selectablePowers2[1]) && (selectablePowers2[1] != selectablePowers2[2]) && (selectablePowers2[0] != selectablePowers2[2]))
                            notSelected = false;
                        else
                            selectablePowers2.Clear();
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.D1))
                    {
                        selectedPower2 = selectablePowers2[0];
                        player1HasSelected = true;
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.D2))
                    {
                        selectedPower2 = selectablePowers2[1];
                        player1HasSelected = true;
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.D3))
                    {
                        selectedPower2 = selectablePowers2[2];
                        player1HasSelected = true;
                    }
                    if (player1HasSelected)
                    {
                        startNextRound = true;
                    }
                }
                else if (!player1IsIn && player2IsIn)
                {
                    while (notSelected)
                    {
                        selectablePowers2.Add((PowerUpsPlayer2)r.Next(0, 5));
                        selectablePowers2.Add((PowerUpsPlayer2)r.Next(0, 5));
                        selectablePowers2.Add((PowerUpsPlayer2)r.Next(0, 5));
                        if ((selectablePowers2[0] != selectablePowers2[1]) && (selectablePowers2[1] != selectablePowers2[2]) && (selectablePowers2[0] != selectablePowers2[2]))
                            notSelected = false;
                        else
                            selectablePowers2.Clear();
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.D1))
                    {
                        selectedPower2 = selectablePowers2[0];
                        player2HasSelected = true;
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.D2))
                    {
                        selectedPower2 = selectablePowers2[1];
                        player2HasSelected = true;
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.D3))
                    {
                        selectedPower2 = selectablePowers2[2];
                        player2HasSelected = true;
                    }
                    if (player2HasSelected)
                    {
                        startNextRound = true;
                    }
                }

                if (Keyboard.GetState().IsKeyDown(Keys.Escape)) { this.Exit(); }
            }
            else
            {
                invulnerableTimer++;
                if (invulnerableTimer < 121)
                {
                    if (invulnerableTimer % 10 == 0)
                    {
                        if (playerFlash)
                        {
                            playerFlash = false;
                        }
                        else
                        {
                            playerFlash = true;
                        }
                    }
                }

                if (Keyboard.GetState().IsKeyDown(Keys.Escape)) { this.Exit(); }

                if (!player.dead)
                {
                    direction = new Vector2(Mouse.GetState().X, Mouse.GetState().Y) - player.Position;
                    if (direction != Vector2.Zero)
                        direction.Normalize();
                    if (player1IsIn && player1IsIn && player1life > 0)
                    {
                        player.GetKeyboardInput(Keyboard.GetState(), oldMouse, Mouse.GetState(), GamePad.GetState(PlayerIndex.One), SFX[2]);
                        player.Update(GraphicsDevice, direction, SFX[1]);
                    }
                    if (player2IsIn && player2IsIn && player2life > 0)
                    {
                        player2.GetKeyboardInput(Keyboard.GetState(), oldKey, GamePad.GetState(PlayerIndex.One), SFX[2]);
                        player2.Update(GraphicsDevice);
                    }
                    oldMouse = Mouse.GetState();
                    oldKey = Keyboard.GetState();
                }

                if (player.dead)
                {
                    if (backgroundMusic == Content.Load<Song>("game"))
                    {
                        backgroundMusic = Content.Load<Song>("dead");
                        MediaPlayer.Play(backgroundMusic);
                    }

                    if (win)
                    {
                        if (backgroundMusic == Content.Load<Song>("dead"))
                        {
                            backgroundMusic = Content.Load<Song>("win");
                            MediaPlayer.Play(backgroundMusic);
                        }
                    }

                    if (Keyboard.GetState().IsKeyDown(Keys.Enter) || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Start))
                    {
                        backgroundMusic = Content.Load<Song>("game");
                        MediaPlayer.Play(backgroundMusic);
                        ResetGame();
                        player = new Player(GraphicsDevice, player2D, Color.White, new Shot(new Vector2(0, 0), 3f, Content.Load<Texture2D>("player_shot"), Color.Gold, new Vector2(0, 0), 0, new Vector2(0, 0)), playerShotSpeed, explosions);
                        player2 = new PlayerTwo(GraphicsDevice, playerTwo2D, Color.White, 3.85f, new Shot(new Vector2(0, 0), 6.5f, Content.Load<Texture2D>("player_shot"), Color.Gold, new Vector2(0, 0), 0), playerTwoShotSpeed);
                        invaderList = new List<Invader>();
                        textList.Add(new Text(new Vector2(350, 380), font, Color.White, "Moddad av mig", 60, Vector2.Zero));
                    }
                }

                if (Keyboard.GetState().IsKeyDown((Keys)72))
                {
                    player2.PowerUps.Add(PowerUpsPlayer2.Duoshot);
                }

                Dive();
                foreach (ExplosionShot ex in player.Explosions)
                {
                    if (invulnerableTimer > 120)
                    {
                        if (player1IsIn)
                        {
                            if (ex.HitBox.Intersects(player.HitBox) && player1IsIn && player1life > 0) { player1life--; invulnerableTimer = 0; shotHimself = true; message = r.Next(8); perfectRound = false; }
                        }
                        if (player2IsIn)
                        {
                            if (ex.HitBox.Intersects(player2.HitBox) && player2IsIn && player2life > 0) { player2life--; invulnerableTimer = 0; shotHimself = true; message = r.Next(8); perfectRound = false; }
                        }
                    }
                }

                foreach (Invader i in invaderList)
                {
                    i.Update(GraphicsDevice);
                    if (i.Position.Y > GraphicsDevice.Viewport.Height - i.Textures[0].Height / 2 && !player.dead)
                    {
                        perfectRound = false;
                        score -= (50 * wave);
                        textList.Add(new Text(new Vector2(i.Position.X, i.Position.Y - 20), font, Color.Red, "-" + (50 * wave).ToString(), 100, new Vector2(0, -0.5f)));
                        i.dead = true;
                    }
                    if (i.target == PlayerEnum.Player1)
                    {
                        Vector2 tempDirection = i.Position - player.Position;
                        if (tempDirection != Vector2.Zero)
                            tempDirection.Normalize();
                        i.DirectionAgainst = tempDirection;
                    }
                    else if (i.target == PlayerEnum.Player2)
                    {
                        Vector2 tempDirection = i.Position - player2.Position;
                        if (tempDirection != Vector2.Zero)
                            tempDirection.Normalize();
                        i.DirectionAgainst = tempDirection;
                    }
                    if (i.Position.X < 0 && !player.dead)
                    {
                        if (player1IsIn && player2IsIn && player1life > 0 && player2life > 0)
                        {
                            int temp = r.Next(2);
                            if (temp == 1)
                            {
                                i.target = PlayerEnum.Player1;
                            }
                            else
                            {
                                i.target = PlayerEnum.Player2;
                            }
                            i.divingAgainst = true;
                        }
                        else if (player1IsIn && !player2IsIn || player2life == 0)
                        {
                            i.target = PlayerEnum.Player1;
                            i.divingAgainst = true;
                        }
                        else if (player2IsIn && !player1IsIn || player1life == 0)
                        {
                            i.target = PlayerEnum.Player2;
                            i.divingAgainst = true;
                        }
                    }
                    if (i.Position.X > GraphicsDevice.Viewport.Width - i.Textures[0].Width && !player.dead)
                    {
                        if (player1IsIn && player2IsIn && player1life > 0 && player2life > 0)
                        {
                            int temp = r.Next(2);
                            if (temp == 1)
                            {
                                i.target = PlayerEnum.Player1;
                            }
                            else
                            {
                                i.target = PlayerEnum.Player2;
                            }
                            i.divingAgainst = true;
                        }
                        else if (player1IsIn && !player2IsIn || player2life == 0)
                        {
                            i.target = PlayerEnum.Player1;
                            i.divingAgainst = true;
                        }
                        else if (player2IsIn && !player1IsIn || player1life == 0)
                        {
                            i.target = PlayerEnum.Player2;
                            i.divingAgainst = true;
                        }
                    }
                    if (i.Position.Y < -600)
                    {
                        perfectRound = false;
                        score -= (50 * wave);
                        textList.Add(new Text(new Vector2(i.Position.X, i.Position.Y - 20), font, Color.Red, "-" + (50 * wave).ToString(), 100, new Vector2(0, -0.5f)));
                        i.dead = true;
                    }
                    if (i.diving && i.divingAgainst == false)
                    {
                        int temp = r.Next(100);
                        if (temp < 2)
                        {
                            if (player1IsIn && player2IsIn && player1life > 0 && player2life > 0)
                            {

                                temp = r.Next(2);
                                if (temp == 1)
                                {
                                    i.target = PlayerEnum.Player1;
                                    i.divingAgainst = true;
                                }
                                else
                                {
                                    i.target = PlayerEnum.Player2;
                                    i.divingAgainst = true;
                                }
                            }
                            else if (player1IsIn && !player2IsIn || player2life == 0)
                            {
                                i.target = PlayerEnum.Player1;
                                i.divingAgainst = true;
                            }
                            else if (player2IsIn && !player1IsIn || player1life == 0)
                            {
                                i.target = PlayerEnum.Player2;
                                i.divingAgainst = true;
                            }
                        }


                    }

                    foreach (Shot s in player2.Shots)
                    {
                        if (s.HitBox.Intersects(i.HitBox) && !player.dead)
                        {
                            i.dead = true;
                            s.dead = true;
                            kills++;
                            if (i.diving)
                            {
                                score += 50 * wave;
                                textList.Add(new Text(i.Position, font, Color.Aquamarine, "" + (50 * wave), 80, new Vector2(0.5f, -0.5f)));
                            }
                            else
                            {
                                score += 10 * wave;
                                textList.Add(new Text(i.Position, font, Color.Aquamarine, "" + (10 * wave), 80, new Vector2(0.5f, -0.5f)));
                            }
                            SFX[1].Play();
                            explosionList.Add(new Explosion(i.Position, explosion, Color.IndianRed));
                        }
                    }
                    foreach (ExplosionShot ex in player.Explosions)
                    {
                        if (ex.HitBox.Intersects(i.HitBox) && !player.dead)
                        {
                            i.dead = true;
                            kills++;
                            if (i.diving)
                            {
                                score += 50 * wave;
                                textList.Add(new Text(i.Position, font, Color.Aquamarine, "" + (50 * wave), 80, new Vector2(0.5f, -0.5f)));
                            }
                            else
                            {
                                score += 10 * wave;
                                textList.Add(new Text(i.Position, font, Color.Aquamarine, "" + (10 * wave), 80, new Vector2(0.5f, -0.5f)));
                            }
                            //SFX[1].Play();
                            explosionList.Add(new Explosion(i.Position, explosion, Color.IndianRed));
                        }
                    }
                    if (invulnerableTimer > 120)
                    {
                        if (player1IsIn)
                        {
                            if (i.HitBox.Intersects(player.HitBox) && player1life > 0) { player1life--; invulnerableTimer = 0; shotHimself = false; message = r.Next(8); perfectRound = false;
                                i.dead = true;
                            }
                        }
                        if (player2IsIn)
                        {
                            if (i.HitBox.Intersects(player2.HitBox) && player2life > 0) { player2life--; invulnerableTimer = 0; shotHimself = false; message = r.Next(8); perfectRound = false;
                                i.dead = true;
                            }
                        }
                    }

                    if (player2life == 0)
                    {
                        player2.Position = new Vector2(50, GraphicsDevice.Viewport.Height - 70);
                    }
                    if (player1IsIn && player2IsIn)
                    {
                        if (player1life == 0 && player2life == 0) { player.dead = true; flash = true; }
                    }
                    else if (player1IsIn && !player2IsIn)
                    {
                        if (player1life == 0) { player.dead = true; flash = true; }
                    }
                    else if (!player1IsIn && player2IsIn)
                    {
                        if (player2life == 0) { player.dead = true; flash = true; }
                    }


                }
            }
            if (invaderList.Count == 0 && !player.dead && gameHasBegun && betweenRound == false)
            {
                if (wave < maxWaves)
                    betweenRound = true;
                else
                    startNextRound = true;
            }
            if (startNextRound)
            {
                if (wave < maxWaves)
                {
                    wave++;
                    SpawnInvaders(invaderSpeed);
                    if (perfectRound)
                    {
                        score += 1000;
                        textList.Add(new Text(new Vector2(245, 80), font, Color.Gold, "PERFECT! +1000 POINTS", 120, new Vector2(0, 0.5f)));
                    }
                    if (player1life < 3)
                    {
                        textList.Add(new Text(new Vector2(r.Next(400), 120), font, Color.Green, "+1UP", 120, new Vector2(0, 0.5f)));
                        player1life++;
                    }
                    if (player2life < 3)
                    {
                        textList.Add(new Text(new Vector2(r.Next(400), 120), font, Color.Green, "+1UP", 120, new Vector2(0, 0.5f)));
                        player2life++;
                    }
                    perfectRound = true;
                }
                else
                {
                    player.dead = true;
                    win = true;
                    textList.Add(new Text(new Vector2(248, 500), font, Color.Gold, "+ 50 000 WIN BONUS!", 600, new Vector2(0, -0.25f)));
                    score += 50000;
                }

                textList.Add(new Text(new Vector2(350, 200), font, Color.LightBlue, "WAVE:" + wave.ToString().PadLeft(2, '0'), 120, Vector2.Zero));
                if (wave == maxWaves) { textList.Add(new Text(new Vector2(330, 230), font, Color.White, "LAST WAVE!", 120, Vector2.Zero)); }
                invaderSpeed = (int)(0.93 * invaderSpeed);
                if (player1IsIn)
                {
                    player1HasSelected = false;
                    selectablePowers1.Clear();
                    player.AddPower(selectedPower1);
                }
                if (player2IsIn)
                {
                    player2HasSelected = false;
                    selectablePowers2.Clear();
                    player2.AddPower(selectedPower2);
                }
                notSelected = true;
                betweenRound = false;
                startNextRound = false;
            }

            foreach (Star s in stars) { s.Update(GraphicsDevice); }
            if (stars.Count < numberOfStars)
            {
                stars.Add(new Star(Content.Load<Texture2D>("star"), GraphicsDevice));
            }

            RemoveObjects();


            float frameX = (float)(r.NextDouble() - 0.5) * 2f * screenShake;
            float frameY = (float)(r.NextDouble() - 0.5) * 2f * screenShake;


            screenShake *= 0.9f;

            screenShake = screenShake < 0.01f ? 0 : screenShake;
            camera.Update(cameraPosition + new Vector2(frameX, frameY));

            base.Update(gameTime);
        }

        // Update and a game graphics
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend,
                null, null, null, null, camera.ViewMatrix);

            if (paused) { spriteBatch.End(); return; }

            foreach (Star s in stars) { s.Draw(spriteBatch); }

            if (!player.dead && !betweenRound)
            {
                if (player1IsIn && player1life > 0)
                {
                    player.Draw(spriteBatch, playerFlash);
                }
                else if (!player1IsIn && !gameHasBegun)
                {
                    spriteBatch.DrawString(font, "Player 1\n Click the \n left mouse \n button to join", new Vector2(50, 50), Color.White);
                }
                if (player2IsIn && player2life > 0)
                {
                    player2.Draw(spriteBatch, playerFlash);
                }
                else if (!player2IsIn && !gameHasBegun)
                {
                    spriteBatch.DrawString(font, "Player 2\n Press the \n spacebar \n to join", new Vector2(600, 50), Color.White);
                }
                if ((player1IsIn && !gameHasBegun) || (player2IsIn && !gameHasBegun))
                {
                    spriteBatch.DrawString(font, "Press the\nenter key\nto start\nsingle player", new Vector2(320, 50), Color.White);
                }
            }
            if (!player1HasSelected && betweenRound && selectablePowers1.Count == 3)
            {
                spriteBatch.Draw(powers1, new Vector2(160, 100), new Rectangle(80 * (int)selectablePowers1[0], 0, 80, 160), Color.White);
                spriteBatch.Draw(powers1, new Vector2(320, 100), new Rectangle(80 * (int)selectablePowers1[1], 0, 80, 160), Color.White);
                spriteBatch.Draw(powers1, new Vector2(480, 100), new Rectangle(80 * (int)selectablePowers1[2], 0, 80, 160), Color.White);
            }
            if (!player2HasSelected && betweenRound && selectablePowers2.Count == 3)
            {
                spriteBatch.Draw(powers2, new Vector2(160, 300), new Rectangle(80 * (int)selectablePowers2[0], 0, 80, 160), Color.White);
                spriteBatch.Draw(powers2, new Vector2(320, 300), new Rectangle(80 * (int)selectablePowers2[1], 0, 80, 160), Color.White);
                spriteBatch.Draw(powers2, new Vector2(480, 300), new Rectangle(80 * (int)selectablePowers2[2], 0, 80, 160), Color.White);
            }

            foreach (Invader i in invaderList) { i.Draw(spriteBatch); }
            foreach (Explosion e in explosionList) { e.Draw(spriteBatch); }
            foreach (Text t in textList) { t.Draw(spriteBatch); }

            if (flash) { Flash(); }


            if (introTimer > 0)
            {
                spriteBatch.Draw(logo, new Vector2(150, 60), Color.White);
                introTimer--;
            }
            DrawInterface();

            spriteBatch.End();


            base.Draw(gameTime);
        }

        // Game procedures
        public void ResetGame()
        {
            invaderSpeed = 200; // Lower number means faster invader movement
            score = 0;
            wave = 1;
            maxWaves = 20;
            flashTimer = 50; // Lower number means faster flash when player is hit
            introTimer = 60; // How long the logo will stay on the first screen (in game cycles)
            kills = 0;
            numberOfStars = 60;
            playerShotSpeed = 25; // Lower number means faster shooting
            playerTwoShotSpeed = 15;
            flash = false;
            win = false;
            perfectRound = true;
            paused = false;
            player1life = 3;
            player2life = 3;
            invulnerableTimer = 120;
            playerFlash = false;
            message = 10;
            gameHasBegun = false;
            player1IsIn = false;
            player2IsIn = false;
            cameraPosition = new Vector2(GraphicsDevice.Viewport.Width / 2, (GraphicsDevice.Viewport.Height / 2) - 50);
        }

        public void DrawInterface()
        {
            if (score < 0) { score = 0; }

            if (paused) { spriteBatch.DrawString(font, "GAME PAUSED", new Vector2(314, 280), Color.White); }

            if (player1IsIn || !gameHasBegun)
            {
                spriteBatch.Draw(crosshairTexture, new Rectangle(Mouse.GetState().X - 15, Mouse.GetState().Y - 15, 30, 30), Color.White);
            }
            if (player1IsIn)
            {
                for (int i = 0; i < player1life; i++)
                {
                    spriteBatch.Draw(playerLife, new Rectangle(50 + (40 * i), 50, 39, 39), Color.White);
                }
            }
            if (player2IsIn)
            {
                for (int i = 0; i < player2life; i++)
                {
                    spriteBatch.Draw(playerLife, new Rectangle(600 + (40 * i), 50, 39, 39), Color.White);
                }
            }
            spriteBatch.DrawString(font, "SCORE:" + score.ToString().PadLeft(6, '0'), new Vector2(20, 20), Color.White);
            spriteBatch.DrawString(font, "WAVE:" + wave.ToString().PadLeft(2, '0') + "/" + maxWaves.ToString().PadLeft(2, '0'), new Vector2(640, 20), Color.White);

            if (invulnerableTimer < 120 && shotHimself)
            {
                switch (message)
                {
                    case 0:
                        spriteBatch.DrawString(font, "WHY DID YOU SHOOT YOURSELF, YOU FOOL?!", new Vector2(100, 50), Color.White);
                        break;
                    case 1:
                        spriteBatch.DrawString(font, "WHY ARE YOU  HITTING YOURSELF?", new Vector2(100, 50), Color.White);
                        break;
                    case 2:
                        spriteBatch.DrawString(font, "OOPS!", new Vector2(100, 50), Color.White);
                        break;
                    case 3:
                        spriteBatch.DrawString(font, "CLUMSY!", new Vector2(100, 50), Color.White);
                        break;
                    case 4:
                        spriteBatch.DrawString(font, "AIM AT THE ENEMY!", new Vector2(100, 50), Color.White);
                        break;
                    case 5:
                        spriteBatch.DrawString(font, "GIT GUD!", new Vector2(100, 50), Color.White);
                        break;
                    case 6:
                        spriteBatch.DrawString(font, "YOU WERE SUPPOSED TO SAVE THE EARTH!", new Vector2(100, 50), Color.White);
                        break;
                    case 7:
                        spriteBatch.DrawString(font, "OH COME ON!", new Vector2(100, 50), Color.White);
                        break;
                    default:
                        break;
                }
            }
            else if (invulnerableTimer < 120 && !shotHimself)
            {
                switch (message)
                {
                    case 0:
                        spriteBatch.DrawString(font, "DODGE!", new Vector2(100, 50), Color.White);
                        break;
                    case 1:
                        spriteBatch.DrawString(font, "YOU SHOULD AVOID TAKING DAMAGE!", new Vector2(100, 50), Color.White);
                        break;
                    case 2:
                        spriteBatch.DrawString(font, "WELL, THERE GOES THAT PERFECT SCORE!", new Vector2(100, 50), Color.White);
                        break;
                    case 3:
                        spriteBatch.DrawString(font, "SHOOT THEM!", new Vector2(100, 50), Color.White);
                        break;
                    case 4:
                        spriteBatch.DrawString(font, "DON'T LET THEM TOUCH YOU!", new Vector2(100, 50), Color.White);
                        break;
                    case 5:
                        spriteBatch.DrawString(font, "GIT GUD!", new Vector2(100, 50), Color.White);
                        break;
                    case 6:
                        spriteBatch.DrawString(font, "YOU WERE SUPPOSED TO SAVE THE EARTH!", new Vector2(100, 50), Color.White);
                        break;
                    case 7:
                        spriteBatch.DrawString(font, "YOU'RE DEAD, NOT BIG SUPRISE!", new Vector2(100, 50), Color.White);
                        break;
                    default:
                        break;
                }
            }

            if (player.dead)
            {
                if (win)
                {
                    spriteBatch.DrawString(font, "You have defended the Earth\r\nagainst the alien invaders!", new Vector2(190, 100), Color.Gold);
                }
                spriteBatch.Draw(interfaceBackground, new Rectangle(286, 176, 240, 158), Color.Gray);
                spriteBatch.Draw(interfaceBackground, new Rectangle(290, 180, 232, 150), Color.Black);
                spriteBatch.DrawString(font, "GAME OVER!", new Vector2(326, 200), Color.Pink);
                spriteBatch.DrawString(font, "FINAL SCORE:", new Vector2(312, 220), Color.White);
                spriteBatch.DrawString(font, score.ToString().PadLeft(6, '0'), new Vector2(352, 240), Color.White);
                spriteBatch.DrawString(font, "Press enter to restart...", new Vector2(212, 520), Color.White);
                spriteBatch.Draw(Content.Load<Texture2D>("invader01_01"), new Vector2(360, 280), Color.White);
                spriteBatch.DrawString(font, ":" + kills, new Vector2(396, 284), Color.White);
            }
        }

        public void GenerateStartingStars()
        {
            Random r = new Random();
            for (int i = 0; i < numberOfStars; i++)
            {
                stars.Add(new Star(Content.Load<Texture2D>("star"), GraphicsDevice));
                stars[i].Position = new Vector2(r.Next(0, GraphicsDevice.Viewport.Width), r.Next(0, GraphicsDevice.Viewport.Height));
            }
        }

        public void Dive()
        {
            foreach (Invader i in invaderList)
            {
                if (r.Next(0, 10000) < (i.DiveChance + wave) && i.starting == false)
                {
                    if (!player.dead) { SFX[0].Play(); }
                    i.diving = true;
                    int d = r.Next(0, 3);
                    if (d == 0) { i.Direction = Directions.None; }
                    if (d == 1) { i.Direction = Directions.Right; }
                    if (d == 2) { i.Direction = Directions.Left; }
                    return;
                }
            }
        }

        public void SpawnInvaders(int invaderSpeed)
        {
            int iX = 90;
            int iY = -400;
            int iWidth = 50;
            int iHeight = 50;

            int rows = (int)(wave * 0.4);
            if (rows < 1) { rows = 1; }
            if (rows > 7) { rows = 7; }

            Random r = new Random();
            for (int i = 0; i < 12; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    invaderList.Add(new Invader(new Vector2(iX + (i * iWidth), iY + (j * iHeight)), 40, invaderSpeed, invader_01, invaderColors[r.Next(invaderColors.Count)]));
                }
            }
        }

        public void RemoveObjects()
        {
            for (int i = invaderList.Count - 1; i >= 0; i--) { if (invaderList[i].dead) { invaderList.RemoveAt(i); } }
            for (int i = player.Shots.Count - 1; i >= 0; i--) { if (player.Shots[i].dead) { player.Shots.RemoveAt(i); } }
            for (int i = explosionList.Count - 1; i >= 0; i--) { if (explosionList[i].dead) { explosionList.RemoveAt(i); } }
            for (int i = textList.Count - 1; i >= 0; i--) { if (textList[i].dead) { textList.RemoveAt(i); } }
            for (int i = stars.Count - 1; i >= 0; i--) { if (stars[i].dead) { stars.RemoveAt(i); } }
        }

        public void Flash()
        {
            Color c = Color.Black;
            if (flashTimer > 0)
            {
                if (flashTimer % 5 == 1) { c = Color.Red; }
                if (flashTimer % 5 == 0) { c = Color.Black; }
                spriteBatch.Draw(interfaceBackground, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), c);
                flashTimer--;
            }
            else { flash = false; }
        }
    }
}