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

namespace BoidsXNA
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        const int NUMBOIDS = 100;

        GraphicsDeviceManager graphics;
        ContentManager content;
        SpriteBatch mSpriteBatch;

        SimWorld mSimWorldInstance;
        Texture2D mMouseCursor;
        Viewport mViewport;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            content = new ContentManager(Services);
            Window.AllowUserResizing = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }


        /// <summary>
        /// Load your graphics content.  If loadAllContent is true, you should
        /// load content from both ResourceManagementMode pools.  Otherwise, just
        /// load ResourceManagementMode.Manual content.
        /// </summary>
        /// <param name="loadAllContent">Which type of content to load.</param>
        protected override void LoadContent()
        {
            //if (loadAllContent)
            {
                mSimWorldInstance = SimWorld.GetInstance();
                mViewport = graphics.GraphicsDevice.Viewport;

                //set up the collision detection planes
                mSimWorldInstance.AddCollisionPlane(new Plane(new Vector3(1.0f, 0.0f, 0.0f), 0.0f));
                mSimWorldInstance.AddCollisionPlane(new Plane(new Vector3(-1.0f, 0.0f, 0.0f), mViewport.Width));
                mSimWorldInstance.AddCollisionPlane(new Plane(new Vector3(0.0f, 1.0f, 0.0f), 0.0f));
                mSimWorldInstance.AddCollisionPlane(new Plane(new Vector3(0.0f, 1.0f, 0.0f), mViewport.Height));

                //mouse cursor sprite.
                mMouseCursor = content.Load<Texture2D>("Images/mouse");

                //set up the boid sprites.
                mSpriteBatch = new SpriteBatch(graphics.GraphicsDevice);

                //create boids.
                Random rand = new Random();
                for (int i = 0; i < NUMBOIDS; ++i)
                {
                    Boid newBoid = new Boid(rand.Next(mViewport.Width), rand.Next(mViewport.Height), new FlockStrategy());
                    newBoid.LoadGraphicAsset(content);
                    mSimWorldInstance.AddBoid(newBoid);
                }

                for (int j = 0; j < 10; ++j)
                {
                    Predator newPredator = new Predator(rand.Next(mViewport.Width), rand.Next(mViewport.Height), new WanderStrategy());
                    newPredator.LoadGraphicAsset(content);
                    mSimWorldInstance.AddBoid(newPredator);
                }
            }
        }


        /// <summary>
        /// Unload your graphics content.  If unloadAllContent is true, you should
        /// unload content from both ResourceManagementMode pools.  Otherwise, just
        /// unload ResourceManagementMode.Manual content.  Manual content will get
        /// Disposed by the GraphicsDevice during a Reset.
        /// </summary>
        /// <param name="unloadAllContent">Which type of content to unload.</param>
        protected override void UnloadContent()
        {
            //if (unloadAllContent)
            {
                // TODO: Unload any ResourceManagementMode.Automatic content
                content.Unload();
            }

            // TODO: Unload any ResourceManagementMode.Manual content
        }


        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            List<Boid> boidList = mSimWorldInstance.GetBoidList();
            foreach (Boid b in boidList)
            {
                b.UpdateBoid(gameTime);
            }

            base.Update(gameTime);
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Green);

            // TODO: Add your drawing code here
            mSpriteBatch.Begin();

            List<Boid> boidList = mSimWorldInstance.GetBoidList();
            foreach (Boid b in boidList)
            {
                b.DrawBoid(mSpriteBatch, gameTime);
            }

            MouseState mouseState = Mouse.GetState();
            Vector2 mousePos = new Vector2(mouseState.X - mMouseCursor.Width / 2, mouseState.Y - mMouseCursor.Height / 2);
            mSpriteBatch.Draw(mMouseCursor, mousePos, Color.White);

            mSpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
