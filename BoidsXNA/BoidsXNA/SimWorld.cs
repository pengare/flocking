using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

//A Singleton object that contains all the information about the game world 
//so we can easily get this object anywhere to get the elements of the simulation.
namespace BoidsXNA
{
    class SimWorld
    {
        private List<Boid> mBoidList;
        private List<Plane> mCollisionList;

        static SimWorld mInstance = null;

        SimWorld()
        {
            mBoidList = new List<Boid>();
            mCollisionList = new List<Plane>();
        }

        public static SimWorld GetInstance()
        {
            if (mInstance == null)
            {
                mInstance = new SimWorld();
            }

            return mInstance;
        }

        public void AddBoid(Boid newBoid) { mBoidList.Add(newBoid); }
        public List<Boid> GetBoidList() { return mBoidList; }

        public void AddCollisionPlane(Plane newPlane) { mCollisionList.Add(newPlane); }
        public List<Plane> GetCollisionList() { return mCollisionList; }
    }
}
