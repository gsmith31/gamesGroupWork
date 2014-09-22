using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameOne
{
    class Platform : StationarySprite
    {
        public Platform(Texture2D texture)
            : base(new SpriteSheet(texture, new Point(0, 0), 1.0f), new Vector2(0, 50), new CollisionOffset(50, 0, 50, 50))
        {
            Point frameSize = new Point(148,110);
            spriteSheet.addSegment(frameSize, new Point(0, 0), new Point(0, 0), 0);
            spriteSheet.setCurrentSegment(0);
        }

        public override void Update(GameTime gameTime, Rectangle clientBounds)
        {
            base.Update(gameTime, clientBounds);
        }

        
    }
}
