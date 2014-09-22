using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameOne
{
    class StationarySprite : Sprite
    {
        protected Vector2 speed;
        public StationarySprite(SpriteSheet spriteSheet, Vector2 position, CollisionOffset collOffset)
            : base(spriteSheet, position, collOffset)
        {
            this.speed = new Vector2(0, 0);
        }


        public override Vector2 direction
        {
            get
            {
                return speed;
            }
        }

        public override void Update(GameTime gameTime, Rectangle clientBounds)
        {

            base.Update(gameTime, clientBounds);
        }
    }
}
