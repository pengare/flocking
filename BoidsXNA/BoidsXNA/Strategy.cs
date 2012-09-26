using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

//Using a strategy pattern to hold each of the different AI movement algorithms.
//We can continue to override the base class and build new algorithms and then
//put logic in the Boid DetermineStrategy() function to decide what to do next.

namespace BoidsXNA
{
    class Strategy
    {
        protected Strategy() { }
        public virtual Vector2 UpdateAI(GameTime gameTime, Boid me)
        {
            return Vector2.Zero;
        }

        protected Vector2 CollisionTest(Boid me)
        {
            List<Plane> collisionList = SimWorld.GetInstance().GetCollisionList();

            Vector4 pos = new Vector4( me.GetPosition(), 0.0f, 1.0f );
            Vector3 avoidCollisionVel = Vector3.Zero;

            foreach( Plane p in collisionList )
            {
                float distance = p.Dot(pos) / p.Normal.Length();

                if (distance <= 5.0f)
                {
                    avoidCollisionVel += p.Normal * (me.Radius - distance);
                }
            }

            return new Vector2(avoidCollisionVel.X, avoidCollisionVel.Y) * 0.001f;
        }
    };

    class IdleStrategy : Strategy
    {
        public IdleStrategy() { }

        //don't do anything.
        public override Vector2 UpdateAI(GameTime gameTime, Boid me)
        {
            me.Stamina = me.Stamina + (float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.01f;
            return Vector2.Zero;
            //return base.UpdateAI(gameTime, me, boidList);
        }
    }

    class FlockStrategy : Strategy
    {
        public FlockStrategy() { }
        public override Vector2 UpdateAI(GameTime gameTime, Boid me)
        {
            return Flock(me);
            //return base.UpdateAI(gameTime, boidList);
        }

        //find the center of the screen and go there.
        private Vector2 FollowMouse(Boid me)
        {
            MouseState mouse = Mouse.GetState();

            Vector2 mousePos = new Vector2(mouse.X, mouse.Y);
            
            return( ( mousePos - me.GetPosition() ) * 0.05f);
            //return Vector2.Zero;
        }

        private Vector2 FleeFromPredator( Boid me )
        {
            if (me.ToString() == "BoidSimulation.Predator")
            {
                return Vector2.Zero;
            }
            else
            {
                Vector2 FleeVel = Vector2.Zero;
                int PredCnt = 0;
                float percent = 0.01f;

                List<Boid> boidList = SimWorld.GetInstance().GetBoidList();
                foreach (Boid b in boidList)
                {
                    if (b.ToString() == "BoidSimulation.Predator")
                    {
                        Vector2 diff = b.GetPosition() - me.GetPosition();
                        if(diff.Length() <= me.StayAwayRadius )
                        {
                            FleeVel -= diff;
                            PredCnt++;
                        }
                    }
                }

                if (PredCnt > 0)
                {
                    FleeVel /= PredCnt;
                    return FleeVel * percent;
                }

                return Vector2.Zero;
            }
        }

        private Vector2 Flock(Boid me)
        {
            Vector2 CenterOfMassVel = Vector2.Zero;
            Vector2 KeepApartVel = Vector2.Zero;
            Vector2 MatchBoidVel = Vector2.Zero;

            List<Boid> boidList = SimWorld.GetInstance().GetBoidList();
            foreach (Boid b in boidList)
            {
                if (me == b)
                {
                    continue;
                }
                else
                {
                    //sum up the positions
                    CenterOfMassVel += b.GetPosition();

                    //sum up the velocities.
                    MatchBoidVel += b.GetVelocity();

                    //test to see if we are smaller than a certain range (circle test)
                    Vector2 diff = b.GetPosition() - me.GetPosition();
                    if (diff.Length() <= me.Radius)
                    {
                        KeepApartVel -= diff;
                    }
                }
            }

            CenterOfMassVel /= boidList.Count - 1;
            CenterOfMassVel *= 0.01f;

            MatchBoidVel /= boidList.Count - 1;
            MatchBoidVel *= me.Strength;

            MatchBoidVel *= 0.01f;

            Vector2 totalVel = (CenterOfMassVel + MatchBoidVel + KeepApartVel + FollowMouse( me )) * 0.001f;
            totalVel += FleeFromPredator(me) * 0.05f;
            totalVel += CollisionTest(me);

            return totalVel;
        }
    };

    class WanderStrategy : Strategy
    {
        public WanderStrategy()
        {
        }

        public override Vector2 UpdateAI(GameTime gameTime, Boid me)
        {
            //get distance between me and the wander point.
            Vector2 wanderPoint = GetNewWanderPoint(me);
            
            Vector2 diff = wanderPoint - me.GetPosition();
            diff += KeepApart(me);
            diff += CollisionTest(me);

            return diff * 0.01f;
             //return base.UpdateAI(gameTime, boidList);
        }

        //can we merge this with a general collision avoidance algorithm?
        private Vector2 KeepApart(Boid me)
        {
            Vector2 KeepApartVel = Vector2.Zero;

            List<Boid> boidList = SimWorld.GetInstance().GetBoidList();
            foreach (Boid b in boidList)
            {
                if (me == b)
                {
                    continue;
                }
                else
                {
                    //test to see if we are smaller than a certain range (circle test)
                    Vector2 diff = b.GetPosition() - me.GetPosition();
                    if (diff.Length() <= me.Radius)
                    {
                        KeepApartVel -= diff;
                    }
                }
            }

            return KeepApartVel;
        }

        private Vector2 GetNewWanderPoint( Boid me )
        {
            Vector2 mForward = me.GetVelocity();
            if (mForward.Length() <= 0.0f)
            {
                return Vector2.Zero;
            }

            mForward.Normalize();

            Vector2 circleCenter = me.GetPosition() + mForward * 10.0f;

            Random rand = new Random();
            float angle = rand.Next(360);
            float radius = 10.0f;

            Vector2 circlePoint = new Vector2((float)Math.Cos(MathHelper.ToRadians(angle) * radius),
                                              (float)Math.Sin(MathHelper.ToRadians(angle) * radius));

            return circleCenter + circlePoint;
        }
    }
}
