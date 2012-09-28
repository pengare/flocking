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
//using Microsoft.Xna.Framework.Net;
//using Microsoft.Xna.Framework.Storage;

namespace flocking {
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {

        #region members
        public Camera camera;
        private FPS fps;
        public Kinect kinect;
        #endregion members
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Scene scene;

        BasicEffect effect;

        Model monster = null;

        public static int thickness = 256;
        public static int width = 0;
        public static int height = 0;

        //vertical placement of view on cylinder
        private float distortionShift = 0.0260000024f;
        //float shiftAngleLocal = 40.5f;
        //private float RotationYWorld = 0.0f;
        //private Matrix gameWorldRotationViewport;
        Distortion distortion;
        BasicEffect distortionEffect;
        RenderTarget2D viewport;
        Matrix View, Projection;
        Texture2D calibrationTexture;

        //Used to control the color of bubble
        private double intervalTime = 0;
        private const double timeThreshold = 8000; //8 second
        private int colorIndex = 0; //indicate which color to choose
        private List<Model> colorPool = new List<Model>();  //save all color model

        //Used to control embrace gesture
        public static Boolean bInEmbrace = false;
        //private double intervalTimeEmbrace = 0;
        //private const double timeThresholdEmbrace = 5000; //5 second
        public static Vector2 screenCenter = Vector2.Zero;

        public Game1() {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //graphics.IsFullScreen = true;
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            IsFixedTimeStep = false;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize() {
            // TODO: Add your initialization logic here

            this.camera = new Camera(this, new Vector3(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2, 1000),
                                    new Vector3(1, 0, 0),
                                    new Vector3(0, 1, 0),
                                    new Vector3(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2, -500));
            this.Components.Add(this.camera);
            this.Services.AddService(typeof(Camera), this.camera);

            this.fps = new FPS(this);
            this.Components.Add(this.fps);

            this.kinect = new Kinect(this);
            this.Components.Add(this.kinect);
            //this.Services.AddService(typeof(Kinect), this.kinect);

            effect = new BasicEffect(GraphicsDevice);


            //for sphere
            //create new distortion object
            distortion = new Distortion(Vector3.Backward, 1, 1, distortionShift, 3, 1.33f);
            View = Matrix.CreateLookAt(new Vector3(0, 0, 2), Vector3.Zero, Vector3.Up);
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 16.0f / 9.0f, 1, 500);


            screenCenter = new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent() {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            width = graphics.PreferredBackBufferWidth;
            height = graphics.PreferredBackBufferHeight;
            int heads = 200;
            int slits = 16;
            scene = new Scene(this, width, height, heads, slits);
            scene.Renderer.addAnimalTexture(animal.AnimalType.Fish, Content.Load<Model>("venus_brown"));
            scene.Renderer.addAnimalTexture(animal.AnimalType.Whale, Content.Load<Model>("whale"));
            
            monster = Content.Load<Model>("venus_brown");

            LoadColorModel();

            LoadDistortion();
            LoadCalibration();
        }

        public void LoadColorModel()
        {
            colorPool.Add(Content.Load<Model>("venus_brown"));
            colorPool.Add(Content.Load<Model>("venus_yellow"));
            colorPool.Add(Content.Load<Model>("venus_blue"));
            colorPool.Add(Content.Load<Model>("venus_red"));
        }

        public void LoadCalibration()
        {
            calibrationTexture = Content.Load<Texture2D>("calibration");
        }

        public void LoadDistortion()
        {
            PresentationParameters pp = GraphicsDevice.PresentationParameters;
            //make sure the render target has a depth buffer
            viewport = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight, false, SurfaceFormat.Color, DepthFormat.Depth16);


            distortionEffect = new BasicEffect(graphics.GraphicsDevice);

            distortionEffect.World = Matrix.Identity;
            distortionEffect.View = View;
            distortionEffect.Projection = Projection;
            distortionEffect.TextureEnabled = true;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent() {
            // TODO: Unload any non ContentManager content here
        }

        bool leftButtonPressed = false;
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime) {
            UpdateColor(gameTime);
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            switch (Mouse.GetState().LeftButton) {
                case ButtonState.Pressed:
                    leftButtonPressed = true;
                    break;
                case ButtonState.Released:
                    if (leftButtonPressed == false)
                        break;
                    scene.reset();
                    leftButtonPressed = false;
                    break;
            }

            //Keyboard
            KeyboardState keyboard = Keyboard.GetState();

            if(keyboard.IsKeyDown(Keys.Escape))
                Exit();
            if (keyboard.IsKeyDown(Keys.E))
            {
                if (Game1.bInEmbrace == false)
                {
                    Game1.bInEmbrace = true;
                }
                else
                {
                    Game1.bInEmbrace = false;
                    scene.reset();
                }

            }
            // TODO: Add your update logic here
            scene.Updater.update(gameTime);

            base.Update(gameTime);
        }

        public void UpdateColor(GameTime gameTime)
        {
            intervalTime += gameTime.ElapsedGameTime.Milliseconds;
            if (intervalTime > timeThreshold)
            {
                colorIndex++;
                if (colorIndex >= 4)
                    colorIndex = 0;

                scene.Renderer.replaceAnimalTexture(animal.AnimalType.Fish, colorPool[colorIndex]);
                intervalTime = 0;
            }
        }
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            //this.GraphicsDevice.SetRenderTarget(viewport);
            //GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            

            //Matrix[] transforms = new Matrix[monster.Bones.Count];
            //monster.CopyAbsoluteBoneTransformsTo(transforms);

            //Matrix world, scale, translation;
            //scale = Matrix.CreateScale(0.3f, 0.3f, 0.3f);
            //Vector3 position = new Vector3(0, 0, 0);
            //translation = Matrix.CreateTranslation(position);
            //world = scale * translation;

            //foreach (ModelMesh mesh in monster.Meshes)
            //{
            //    foreach (BasicEffect effectTemp in mesh.Effects)
            //    {
            //        //effectTemp.EnableDefaultLighting();
            //        effectTemp.World = transforms[mesh.ParentBone.Index] * world;
            //        effectTemp.View = camera.ViewMatrix;
            //        effectTemp.Projection = camera.ProjectMatrix;
            //        mesh.Draw();
            //    }
            //}

            ////effect.EnableDefaultLighting();
            //effect.World = Matrix.CreateTranslation(1000, 500, 0);
            //effect.View = camera.ViewMatrix;
            //effect.Projection = camera.ProjectMatrix;
            ////spriteBatch.Begin();
            //spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, effect);
            scene.Renderer.draw(spriteBatch);
            //spriteBatch.End();


            //DrawForCylinder();

            //DrawCalibrationPattern();

            base.Draw(gameTime);
        }


        private void DrawForCylinder()
        {
            //draw distortion
            this.GraphicsDevice.SetRenderTarget(null);
            graphics.GraphicsDevice.Clear(Color.Black);
            //this.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
            distortionEffect.Texture = viewport;

            Matrix gameWorldRotationViewport3 = Matrix.CreateRotationZ(MathHelper.ToRadians(0));
            foreach (EffectPass pass in distortionEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                distortionEffect.World = gameWorldRotationViewport3;
                GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleStrip, distortion.Vertices, 0, 3999);
            }

            //gameWorldRotationViewport3 = Matrix.CreateRotationZ(MathHelper.ToRadians(120));
            //foreach (EffectPass pass in distortionEffect.CurrentTechnique.Passes)
            //{
            //    pass.Apply();
            //    distortionEffect.World = gameWorldRotationViewport3;
            //    GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleStrip, distortion.Vertices, 0, 3999);
            //}

            //gameWorldRotationViewport3 = Matrix.CreateRotationZ(MathHelper.ToRadians(240));
            //foreach (EffectPass pass in distortionEffect.CurrentTechnique.Passes)
            //{
            //    pass.Apply();
            //    distortionEffect.World = gameWorldRotationViewport3;
            //    GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleStrip, distortion.Vertices, 0, 3999);
            //}

        }


        private void DrawCalibrationPattern()
        {
            this.GraphicsDevice.SetRenderTarget(null);
            this.GraphicsDevice.Clear(Color.Black);
            this.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
            distortionEffect.Texture = calibrationTexture;

            Matrix gameWorldRotation2 = Matrix.CreateRotationZ(MathHelper.ToRadians(0));
            foreach (EffectPass pass in distortionEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                distortionEffect.World = gameWorldRotation2;
                GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleStrip, distortion.Vertices, 0, 3999);
            }

            //gameWorldRotation2 = Matrix.CreateRotationZ(MathHelper.ToRadians(120));
            //foreach (EffectPass pass in distortionEffect.CurrentTechnique.Passes)
            //{
            //    pass.Apply();
            //    distortionEffect.World = gameWorldRotation2;
            //    distortionEffect.DiffuseColor = Color.Red.ToVector3();
            //    GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleStrip, distortion.Vertices, 0, 3999);
            //}

            //gameWorldRotation2 = Matrix.CreateRotationZ(MathHelper.ToRadians(240));
            //foreach (EffectPass pass in distortionEffect.CurrentTechnique.Passes)
            //{
            //    pass.Apply();
            //    distortionEffect.World = gameWorldRotation2;
            //    distortionEffect.DiffuseColor = Color.Blue.ToVector3();
            //    GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleStrip, distortion.Vertices, 0, 3999);
            //}

            //gameWorldRotation2 = Matrix.CreateRotationZ(MathHelper.ToRadians(360));
            //foreach (EffectPass pass in distortionEffect.CurrentTechnique.Passes)
            //{
            //    pass.Apply();
            //    distortionEffect.World = gameWorldRotation2;
            //    distortionEffect.DiffuseColor = Color.Green.ToVector3();
            //    GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleStrip, distortion.Vertices, 0, 3999);
            //}
        }
    }
}
