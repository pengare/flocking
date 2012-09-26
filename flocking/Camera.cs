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


namespace flocking
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Camera : Microsoft.Xna.Framework.GameComponent, ICameraService
    {
        private GraphicsDevice graphicDevice;

        public Vector3 Position;
        public Vector3 cameraRight;
        public Vector3 cameraUp;
        public Vector3 cameraLook;
        private Matrix view;
        private Matrix project;

        public Vector3 cameraLookOriginal;
        public float currYRotation = 0.0f;
        public float currXRotation = 0.0f;

        private KeyboardState previousState;
        public MouseState previousMouseState;


        private static float nearPlane = 0.1f;
        private static float farPlane = 10000.0f;
        public static float cameraSpeed = 0.05f;

        public Camera(Game game, Vector3 cameraPosition, Vector3 cameraRight, Vector3 cameraUp, Vector3 cameraLook)
            : base(game)
        {
            this.Position = cameraPosition;
            this.cameraRight = cameraRight;
            this.cameraUp = cameraUp;
            this.cameraLook = cameraLook;
            this.cameraLookOriginal = this.cameraLook;
            this.graphicDevice = game.GraphicsDevice;
            this.setupViewProjection();
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            this.previousState = Keyboard.GetState();
            this.previousMouseState = Mouse.GetState();
            base.Initialize();
        }

        public void UpdateCameraLook(float XRotation, float YRotation, float ZRotation)
        {
            currYRotation += YRotation;
            Matrix yaw = Matrix.CreateRotationY(currYRotation);

            currXRotation += XRotation;
            if (currXRotation > 0.5f * MathHelper.Pi - 0.01f)
                currXRotation = 0.5f * MathHelper.Pi - 0.01f;
            else if (currXRotation < -0.5f * MathHelper.Pi + 0.01f)
                currXRotation = -0.5f * MathHelper.Pi + 0.01f;

            Matrix pitch = Matrix.CreateRotationX(currXRotation);

            this.cameraLook = Vector3.Transform(cameraLookOriginal, pitch);
            this.cameraLook = Vector3.Transform(cameraLook, yaw);
        }
        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            //Keyboard State
            KeyboardState currentState = Keyboard.GetState();
            if (currentState.IsKeyDown(Keys.Right))
            {
                UpdateCameraLook(0.0f, -Camera.cameraSpeed, 0.0f);
            }
            if (currentState.IsKeyDown(Keys.Left))
            {
                UpdateCameraLook(0.0f, Camera.cameraSpeed, 0.0f);
            }
            if (currentState.IsKeyDown(Keys.Up))
            {
                UpdateCameraLook(Camera.cameraSpeed, 0.0f, 0.0f);
            }
            if (currentState.IsKeyDown(Keys.Down))
            {
                UpdateCameraLook(-Camera.cameraSpeed, 0.0f, 0.0f);
            }

            this.previousState = currentState;

            //Mouse State
            MouseState currentMouseState = Mouse.GetState();
            //if (currentMouseState.ScrollWheelValue < previousMouseState.ScrollWheelValue)
            //{
            //    this.Position.Z += Camera.cameraSpeed * 4;
            //}
            //if (currentMouseState.ScrollWheelValue > previousMouseState.ScrollWheelValue)
            //{
            //    this.Position.Z -= Camera.cameraSpeed * 4;
            //}
            //if (currentMouseState.MiddleButton == ButtonState.Pressed)
            //{
            //    if (currentMouseState.X > previousMouseState.X)
            //    {
            //        this.Position.X -= Camera.cameraSpeed;
            //        this.cameraLook.X -= Camera.cameraSpeed;
            //    }
            //    if (currentMouseState.X < previousMouseState.X)
            //    {
            //        this.Position.X += Camera.cameraSpeed;
            //        this.cameraLook.X += Camera.cameraSpeed;
            //    }
            //    if (currentMouseState.Y < previousMouseState.Y)
            //    {
            //        this.Position.Y -= Camera.cameraSpeed;
            //        this.cameraLook.Y -= Camera.cameraSpeed;
            //    }
            //    if (currentMouseState.Y > previousMouseState.Y)
            //    {
            //        this.Position.Y += Camera.cameraSpeed;
            //        this.cameraLook.Y += Camera.cameraSpeed;
            //    }
            //}
            this.previousMouseState = currentMouseState;

            this.setupViewProjection();

            base.Update(gameTime);
        }

        public Matrix ViewMatrix
        {
            get { return view; }
        }

        public Matrix ProjectMatrix
        {
            get { return project; }
        }

        private void setupViewProjection()
        {
            this.view = Matrix.CreateLookAt(this.Position, this.cameraLook, this.cameraUp);
            this.project = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                                                               this.graphicDevice.Viewport.AspectRatio,
                                                               Camera.nearPlane,
                                                               Camera.farPlane);
        }
    }
}
