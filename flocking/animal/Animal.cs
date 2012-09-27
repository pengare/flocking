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
        public Color Color { get; set; }
        public AnimalType AnimalType { get; set; }
        public Spec AnimalSpec { get; set; }
        public List<Behavior> Behaviors { get; set; }

        public abstract void updateColor(Animal nearest, float dist);
        public abstract void aimAt(Animal someone, float sdist, ref Animal target, ref float tdist);

        public void update(float dt, Formation frm) {
            IEnumerable<Animal> others = frm.findNeighbors(this);
            updateDirection(dt, frm, others);
            updatePosition(dt, frm);
        }

        public virtual void updateDirection(float dt, Formation frm, IEnumerable<Animal> others) {
            Vector2 newDir = Vector2.Zero;
            Animal target = null;
            float tdist = float.MaxValue;

            foreach (Animal you in others) {
                reactTo(you, ref target, ref tdist, ref newDir);
            }
            updateColor(target, tdist);
            if (newDir.Length() < 0.01f)
                return;
            newDir.Normalize();

            float rotLimit = AnimalSpec.RotationLimitation * dt;
            Direction = restrictRotationSpeed(Direction, newDir, rotLimit);

            //update Z position
            ZPosition += ZDirection * 0.2f;
            if (ZPosition >= Game1.thickness || ZPosition <= 100.0f)
                ZDirection = -ZDirection;

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
            Vector2 newPos = Position + AnimalSpec.RadialVelocity * Direction * dt;
            frm.normalize(ref newPos, out newPos);
            frm.move(this, newPos);
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