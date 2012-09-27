using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using flocking.animal;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace flocking {
    public class AnimalRenderer {
        public Formation Formation { get; set; }
        private Camera camera { get; set; }
        private Game1 game { get; set; }
        private List<Model> animalLooks;
        private List<Vector2> textureTranslations;

        public AnimalRenderer(Game1 game) {
            this.game = game;
            camera = game.camera;

            this.animalLooks = new List<Model>();
            this.textureTranslations = new List<Vector2>();
        }

        public void addAnimalTexture(AnimalType type, Model model) {
            int index = (int)type;
            for (int i = animalLooks.Count; i <= index; i++) {
                animalLooks.Add(null);
                textureTranslations.Add(Vector2.Zero);
            }
            animalLooks[index] = model;
            //textureTranslations[index] = new Vector2(tex.Width / 2, tex.Height / 2);
        }

        public void draw(SpriteBatch spr) {
            foreach (Animal anm in Formation.AnimalList) {
                int index = (int) anm.AnimalType;
                Vector2 trns = textureTranslations[index];
                float rot = (float) Math.Atan2(anm.Direction.Y, anm.Direction.X);
                //spr.Draw(animalLooks[index], anm.Position, null, anm.Color,
                //    rot, trns, 1.0f, SpriteEffects.None, 0.0f);

                Matrix[] transforms = new Matrix[animalLooks[index].Bones.Count];
                animalLooks[index].CopyAbsoluteBoneTransformsTo(transforms);

                Matrix world, scale, rotation, translation;
                scale = Matrix.CreateScale(0.008f, 0.008f, 0.008f);
                Vector3 position = new Vector3(anm.Position.X, anm.Position.Y, 0);
                rotation = Matrix.CreateRotationY(rot);
                translation = Matrix.CreateTranslation(position);
                world = scale * translation;

                foreach (ModelMesh mesh in animalLooks[index].Meshes)
                {
                    foreach (BasicEffect effectTemp in mesh.Effects)
                    {
                        effectTemp.EnableDefaultLighting();
                        effectTemp.LightingEnabled = true;
                        effectTemp.DiffuseColor = Color.White.ToVector3();
                        effectTemp.AmbientLightColor = Color.White.ToVector3();
                        effectTemp.World = transforms[mesh.ParentBone.Index] * world;
                        effectTemp.View = camera.ViewMatrix;
                        effectTemp.Projection = camera.ProjectMatrix;
                        mesh.Draw();
                    }
                }

            }
        }
    }
}
