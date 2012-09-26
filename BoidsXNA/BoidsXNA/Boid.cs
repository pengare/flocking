using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BoidsXNA
{
    class Boid
    {
        protected Vector2 mPosition;
        protected Vector2 mVelocity;
        protected float mRadius;
        protected float mStayAwayRadius;
        protected float mStrength;
        protected float mCurrStamina;
        protected float mMaxStamina;

        protected Texture2D mSprite;
        protected Strategy mAIStrategy;
        protected Strategy mPrevAIStrategy;
        protected Random mRandGen;
        protected BoundingBox mBoundingBox;

        public Boid(float x, float y, Strategy strategy)
        {
            Init( x, y, strategy );
        }

        public void Init( float x, float y, Strategy strategy )
        {
            mPosition.X = x;
            mPosition.Y = y;
            mRadius = 20.0f;
            mStayAwayRadius = 200.0f;

            Random rand = new Random( 130 );
            mStrength = rand.Next(100) * 0.01f;
            mMaxStamina = 20 + rand.Next(80);
            mCurrStamina = mMaxStamina;
            mVelocity = Vector2.Zero;

            mAIStrategy = strategy;
            mPrevAIStrategy = strategy;
            mRandGen = new Random(43);

        }

        public virtual void LoadGraphicAsset(ContentManager content)
        {
            //can I put this in a flyweight class later?
            mSprite = content.Load<Texture2D>("Images/albino");

            mBoundingBox = new BoundingBox(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(mSprite.Width, mSprite.Height, 0.0f));
        }

        public virtual void UpdateBoid(GameTime gameTime )
        {
            DetermineStrategy();

            mVelocity = mAIStrategy.UpdateAI(gameTime, this);

            mPosition = mPosition + (mVelocity * (float)gameTime.ElapsedGameTime.TotalMilliseconds);
            mCurrStamina -= mVelocity.Length() * (float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.05f;
        }

        public virtual bool DetectNearbyPredators()
        {
            if( this.ToString() == "BoidSimulation.Predator" )
            {
                return false;
            }

            List<Boid> boidList = SimWorld.GetInstance().GetBoidList();
            foreach (Boid b in boidList)
            {
                if (b.ToString() == "BoidSimulation.Predator")
                {
                    Vector2 diff = b.GetPosition() - this.GetPosition();
                    if (diff.Length() <= mStayAwayRadius)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void ChangeStrategy(Strategy newStrategy)
        {
            mPrevAIStrategy = mAIStrategy;
            mAIStrategy = newStrategy;
        }

        public virtual void DetermineStrategy()
        {
            string str = mAIStrategy.ToString();

            if (str == "BoidSimulation.IdleStrategy")
            {
                if (DetectNearbyPredators())
                {
                    ChangeStrategy(new FlockStrategy());
                }
                else if (mCurrStamina > mMaxStamina * 0.95f)
                {
                    ChangeStrategy(new FlockStrategy());
                }
            }
            else if (str == "BoidSimulation.FlockStrategy" || str == "BoidSimulation.WanderStrategy" )
            {
                if (mCurrStamina < mMaxStamina * 0.025f)
                {
                    ChangeStrategy(new IdleStrategy());
                }
            }
        }

        public Texture2D GetSprite()
        {
            return mSprite;
        }

        public Vector2 GetPosition()
        {
            return mPosition;
        }

        public Vector2 GetVelocity()
        {
            return mVelocity;
        }

        public BoundingBox GetBoundingBox()
        {
            return mBoundingBox;
        }

        public float Radius
        {
            get
            {
                return mRadius;
            }
        }

        public float Strength
        {
            get
            {
                return mStrength;
            }
        }

        public float StayAwayRadius
        {
            get
            {
                return mStayAwayRadius;
            }
        }

        public float Stamina
        {
            get
            {
                return mCurrStamina;
            }
            set
            {
                if (value < mMaxStamina)
                {
                    mCurrStamina = value;
                }
                else
                {
                    mCurrStamina = mMaxStamina;
                }
            }
        }

        public void DrawBoid( SpriteBatch spriteBatch, GameTime gameTime)
        {
            string str = mAIStrategy.ToString();
            if (str == "BoidSimulation.IdleStrategy" )
            {
                Vector2 spriteCenter = new Vector2(mSprite.Width * 0.5f, mSprite.Height * 0.5f);
                spriteBatch.Draw(mSprite, mPosition-spriteCenter, Color.Red);
            }
            else if (str == "BoidSimulation.WanderStrategy")
            {
                spriteBatch.Draw(mSprite, mPosition, Color.Blue);
            }
            else
            {
                spriteBatch.Draw(mSprite, mPosition, Color.White);
            }
        }
    };

    class Predator: Boid
    {
        public Predator( float x, float y, Strategy strategy ) :
            base( x, y, strategy )
        {
            mStrength = 200;
        }

        public override void LoadGraphicAsset(ContentManager content)
        {
            mSprite = content.Load<Texture2D>("Images/whorse");
            mBoundingBox = new BoundingBox(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(mSprite.Width, mSprite.Height, 0.0f));
        }

        public override void DetermineStrategy()
        {
            string str = mAIStrategy.ToString();

            if (str == "BoidSimulation.IdleStrategy")
            {
                if (mCurrStamina > mMaxStamina * 0.95f)
                {
                    ChangeStrategy(mPrevAIStrategy);
                }
            }
            else if (str == "BoidSimulation.FlockStrategy" || str == "BoidSimulation.WanderStrategy")
            {
                if (mCurrStamina < mMaxStamina * 0.025f)
                {
                    ChangeStrategy(new IdleStrategy());
                }
            }
        }
    };
}


