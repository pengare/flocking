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
        private Camera camera;
        private FPS fps;
        #endregion members
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Scene scene;

        BasicEffect effect;

        Model monster = null;

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

            this.camera = new Camera(this, new Vector3(0, 0, 1500),
                                    new Vector3(1, 0, 0),
                                    new Vector3(0, 1, 0),
                                    new Vector3(0, 0, -500));
            this.Components.Add(this.camera);
            this.Services.AddService(typeof(Camera), this.camera);

            this.fps = new FPS(this);
            this.Components.Add(this.fps);

            effect = new BasicEffect(GraphicsDevice);
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

            int width = graphics.PreferredBackBufferWidth;
            int height = graphics.PreferredBackBufferHeight;
            int heads = 200;
            int slits = 32;
            scene = new Scene(width, height, heads, slits);
            scene.Renderer.addAnimalTexture(animal.AnimalType.Fish, Content.Load<Texture2D>("v2"));
            scene.Renderer.addAnimalTexture(animal.AnimalType.Whale, Content.Load<Texture2D>("v3"));
            
            monster = Content.Load<Model>("venus");
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
            // TODO: Add your update logic here
            scene.Updater.update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            //GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            

            Matrix[] transforms = new Matrix[monster.Bones.Count];
            monster.CopyAbsoluteBoneTransformsTo(transforms);

            Matrix world, scale, translation;
            scale = Matrix.CreateScale(0.3f, 0.3f, 0.3f);
            Vector3 position = new Vector3(0, 0, 0);
            translation = Matrix.CreateTranslation(position);
            world = scale * translation;

            foreach (ModelMesh mesh in monster.Meshes)
            {
                foreach (BasicEffect effectTemp in mesh.Effects)
                {
                    //effectTemp.EnableDefaultLighting();
                    effectTemp.World = transforms[mesh.ParentBone.Index] * world;
                    effectTemp.View = camera.ViewMatrix;
                    effectTemp.Projection = camera.ProjectMatrix;
                    mesh.Draw();
                }
            }

            ////effect.EnableDefaultLighting();
            //effect.World = Matrix.CreateTranslation(1000, 500, 0);
            //effect.View = camera.ViewMatrix;
            //effect.Projection = camera.ProjectMatrix;
            ////spriteBatch.Begin();
            //spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, effect);
            //scene.Renderer.draw(spriteBatch);
            //spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
