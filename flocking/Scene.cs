using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using flocking.animal;
using flocking.behavior;

namespace flocking {
    public class Scene {
        public AnimalUpdater Updater { get; private set; }
        public AnimalRenderer Renderer { get; private set; }
        private Formation formation;
        private Rectangle screen;
        private int heads, slits;
        private Random rand;

        private Game1 game { get; set; }

        public Scene(Game1 game, int width, int height, int heads, int slits) {
            rand = new Random();
            this.screen = new Rectangle(0, 0, width, height);
            this.heads = heads;
            this.slits = slits;
            this.game = game;
            Updater = new AnimalUpdater();
            Renderer = new AnimalRenderer(game);
            reset();
        }
        public void reset() {
            this.formation = new Formation(screen, slits);
            List<Behavior> fishbhs = new List<Behavior>();
            fishbhs.Add(new DraftBehavior());
            fishbhs.Add(new InertiaBehavior());
            fishbhs.Add(new RandomBehavior());
            fishbhs.Add(new AlignmentBehavior());
            fishbhs.Add(new SeparationBehavior());
            fishbhs.Add(new CohesionBehavior());
            fishbhs.Add(new RepulsionBehavior());
            
            List<Behavior> whalebhs = new List<Behavior>();
            whalebhs.Add(new InertiaBehavior());
            whalebhs.Add(new RandomBehavior());
            whalebhs.Add(new AlignmentBehavior());
            whalebhs.Add(new SeparationBehavior());
            whalebhs.Add(new HuntingBehavior());

            float whaleRatio = 0.02f;

            Vector2 pos = Vector2.Zero;
            Vector2 dir = Vector2.Zero;
            for (int i = 0; i < heads; i++) {
                pos.X = (float)(screen.X + (rand.NextDouble() * screen.Width) % screen.Width);
                pos.Y = (float)(screen.Y + (rand.NextDouble() * screen.Height) % screen.Height);

                double randAngle = rand.NextDouble() * 2 * Math.PI;
                dir.X = (float)Math.Cos(randAngle);
                dir.Y = (float)Math.Sin(randAngle);

                Animal anm;
                if (rand.NextDouble() < whaleRatio) {
                    anm = new Whale(pos, dir);
                    anm.Behaviors = whalebhs;
                } else {
                    anm = new Fish(pos, dir);
                    anm.Behaviors = fishbhs;
                }
                formation.add(anm);
            }
            Updater.Formation = formation;
            Renderer.Formation = formation;
        }
    }
}
