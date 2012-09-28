using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using flocking.spec;
using flocking.behavior;

namespace flocking.animal {
    public enum AnimalType { Fish, Whale }

    public abstract class Animal {
        public Vector2 Position { get; set; }
        public Vector2 Direction { get; set; }
        public float ZPosition = 101;
        public int ZDirection = 1;
        public Random randZ = new Random();
        public Boolean bActive = true;
        public int iLeftRightHand = 0;  //left whale -1, right hand whale 1, fish 0
        public Color Color { get; set; }
        public AnimalType AnimalType { get; set; }
        public Spec AnimalSpec { get; set; }
        public List<Behavior> Behaviors { get; set; }

        //for repulsion behavior
        public float RadialVelocity;


        public abstract void updateColor(Animal nearest, float dist);
        public abstract void aimAt(Animal someone, float sdist, ref Animal target, ref float tdist);

        public void update(float dt, Formation frm) {
            IEnumerable<Animal> others = frm.findNeighbors(this);
            updateDirection(dt, frm, others);
            updatePosition(dt, frm);
        }

        public virtual void updateDirection(float dt, Formation frm, IEnumerable<Animal> others) {
            if (!Game1.bInEmbrace)
            {
                Vector2 newDir = Vector2.Zero;
                Animal target = null;
                float tdist = float.MaxValue;

                foreach (Animal you in others)
                {
                    reactTo(you, ref target, ref tdist, ref newDir);
                }
                updateColor(target, tdist);
                if (newDir.Length() < 0.01f)
                    return;
                newDir.Normalize();

                float rotLimit = AnimalSpec.RotationLimitation * dt;
                Direction = restrictRotationSpeed(Direction, newDir, rotLimit);

                //update Z position
                ZPosition += ZDirection;//*0.2f;
                if (ZPosition >= Game1.thickness || ZPosition <= 0.0f)
                    ZDirection = -ZDirection;
            }
            else //in embrace gesture
            {
                Vector2 newDir = Vector2.Zero;
                Animal target = null;
                float tdist = float.MaxValue;

                foreach (Animal you in others)
                {
                    reactTo(you, ref target, ref tdist, ref newDir);
                }
                updateColor(target, tdist);
                if (newDir.Length() < 0.01f)
                    return;
                newDir.Normalize();

                float rotLimit = AnimalSpec.RotationLimitation * dt;
                Direction = restrictRotationSpeed(Direction, newDir, rotLimit);

                Vector2 offset = new Vector2((float)randZ.NextDouble(), (float)randZ.NextDouble());
                newDir = Game1.screenCenter - this.Position;
                newDir.Normalize();
                Direction = Direction + newDir + offset;
            }

        }

        public virtual void reactTo(Animal you, ref Animal target, ref float tdist, ref Vector2 newDir) {
            if (this == you)
                return;
            Vector2 dir = you.Position - Position;
            float dist = dir.Length();
            if (!(0.01f < dist && dist < AnimalSpec.DetectionDistance))
                return;
            aimAt(you, dist, ref target, ref tdist);
            dir.Normalize();
            foreach (Behavior bhv in Behaviors) {
                if (bhv.isReactive(this, you, ref dir, dist))
                    newDir += bhv.react(this, you, ref dir, dist);
            }
        }

        public virtual void updatePosition(float dt, Formation frm) {
            if (this.AnimalType == AnimalType.Fish)
            {
                if (!Game1.bInEmbrace)
                {
                    Vector2 newPos = Position + this.RadialVelocity * Direction * dt;
                    frm.normalize(ref newPos, out newPos);
                    frm.move(this, newPos);

                    if (this.RadialVelocity > FishSpec.RadialVelocityOriginal)
                    {
                        this.RadialVelocity *= 0.999f;
                    }
                }
                else //in embrace, gather quickly
                {
                    Vector2 newPos = Position + AnimalSpec.RadialVelocity * Direction * dt * 20.0f;
                    frm.normalize(ref newPos, out newPos);
                    frm.move(this, newPos);
                }

            }
            else if (this.AnimalType == AnimalType.Whale && this.iLeftRightHand == -1)  //left hand
            {
                if (Kinect.bLeftDataValid)
                {
                    this.bActive = true;

                    Vector2 leftHandPos = Kinect.leftHandWhale;
                    leftHandPos.X += 70;
                    leftHandPos.Y += 40;
                    if (leftHandPos.X < 0)
                        leftHandPos.X = 0;
                    else if (leftHandPos.X > 100)
                        leftHandPos.X = 100;

                    if (leftHandPos.Y < 0)
                        leftHandPos.Y = 0;
                    else if (leftHandPos.Y > 100)
                        leftHandPos.Y = 100;

                    Vector2 newPos = new Vector2(leftHandPos.X / 100.0f * Game1.width, leftHandPos.Y / 100.0f * Game1.height);

                    frm.normalize(ref newPos, out newPos);
                    frm.move(this, newPos);
                }
                else
                {
                    this.bActive = false;
                }
            }
            else if (this.AnimalType == AnimalType.Whale && this.iLeftRightHand == 1)  //right hand
            {
                if (Kinect.bRightDataValid)
                {
                    this.bActive = true;

                    Vector2 rightHandPos = Kinect.rightHandWhale;
                    rightHandPos.X += 30;
                    rightHandPos.Y += 40;
                    if (rightHandPos.X < 0)
                        rightHandPos.X = 0;
                    else if (rightHandPos.X > 100)
                        rightHandPos.X = 100;

                    if (rightHandPos.Y < 0)
                        rightHandPos.Y = 0;
                    else if (rightHandPos.Y > 100)
                        rightHandPos.Y = 100;

                    Vector2 newPos = new Vector2(rightHandPos.X / 100.0f * Game1.width, rightHandPos.Y / 100.0f * Game1.height);
                    frm.normalize(ref newPos, out newPos);
                    frm.move(this, newPos);
                }
                else
                {
                    this.bActive = false;
                }

            }
        }

        public Vector2 restrictRotationSpeed(Vector2 oldDir, Vector2 newDir, float limit) {
            float oldAngle = (float)Math.Atan2(oldDir.Y, oldDir.X);
            float newAngle = (float)Math.Atan2(newDir.Y, newDir.X);
            float d = sharpenAngle(newAngle - oldAngle);
            newAngle = oldAngle + MathHelper.Clamp(d, -limit, limit);
            return new Vector2((float)Math.Cos(newAngle), (float)Math.Sin(newAngle));
        }

        public float sharpenAngle(float angle) {
            if (angle < -MathHelper.Pi)
                angle += MathHelper.TwoPi;
            else if (angle > MathHelper.Pi)
                angle -= MathHelper.TwoPi;
            return angle;
        }
    }
}