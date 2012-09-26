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
        private List<Texture2D> animalLooks;
        private List<Vector2> textureTranslations;

        public AnimalRenderer() {
            this.animalLooks = new List<Texture2D>();
            this.textureTranslations = new List<Vector2>();
        }

        public void addAnimalTexture(AnimalType type, Texture2D tex) {
            int index = (int)type;
            for (int i = animalLooks.Count; i <= index; i++) {
                animalLooks.Add(null);
                textureTranslations.Add(Vector2.Zero);
            }
            animalLooks[index] = tex;
            textureTranslations[index] = new Vector2(tex.Width / 2, tex.Height / 2);
        }

        public void draw(SpriteBatch spr) {
            foreach (Animal anm in Formation.AnimalList) {
                int index = (int) anm.AnimalType;
                Vector2 trns = textureTranslations[index];
                float rot = (float) Math.Atan2(anm.Direction.Y, anm.Direction.X);
                spr.Draw(animalLooks[index], anm.Position, null, anm.Color,
                    rot, trns, 1.0f, SpriteEffects.None, 0.0f);
            }
        }
    }
}
