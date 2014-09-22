/*
 * Represents a GlitchPlayer.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace GameOne
{
    class GlitchPlayer : UserControlledSprite
    {
        // state pattern
        const int NUM_STATES = 4;
        enum GlitchPlayerState
        {
            Walking,
            Climbing,
            Jumping,
            Sleeping
        }
        GlitchPlayerState currentState;
        AbstractState[] states;

        // constants for this particular sprite
        static Point glitchNumberOfFrames = new Point(40, 3);
        static CollisionOffset glitchCollisionOffset = new CollisionOffset(35, 0, 80, 80);
        static Vector2 glitchSpeed = new Vector2(64, 32);
        static Vector2 glitchFriction = new Vector2(0.8f, 1f);
        static Point glitchFrameSize = new Point(102, 122);

        // constructor
        public GlitchPlayer(Texture2D image)
             : base(new SpriteSheet(image, glitchNumberOfFrames, .5f), 
            Vector2.Zero, glitchCollisionOffset, glitchSpeed, glitchFriction)
            
        {
            // set the segments and frame size
            spriteSheet.addSegment(glitchFrameSize, new Point(0, 0), new Point(11,0), 20);
            spriteSheet.addSegment(glitchFrameSize, new Point(0, 1), new Point(19, 1), 20);
            spriteSheet.addSegment(glitchFrameSize, new Point(0, 2), new Point(31, 2), 42);
            spriteSheet.addSegment(glitchFrameSize, new Point(0, 3), new Point(40, 3), 20);

            // define the states
            states = new AbstractState[NUM_STATES];
            states[(Int32)GlitchPlayerState.Walking] = new WalkingState(this);
            states[(Int32)GlitchPlayerState.Sleeping] = new SleepingState(this);
            states[(Int32)GlitchPlayerState.Jumping] = new JumpingState(this);
            states[(Int32)GlitchPlayerState.Climbing] = new ClimbingState(this);

            // start in Walking state
            switchState(GlitchPlayerState.Walking);
        }

        /*
         * Update the sprite.
         */
        public override void Update(GameTime gameTime, Rectangle clientBounds)
        {
            // call Update for the current state
            states[(Int32)currentState].Update(gameTime, clientBounds);

            // Update the sprite (base class)
            base.Update(gameTime, clientBounds);
        }

        /*
         * Called when this sprite has collided with something else.
         */
        public override void Collision(Sprite otherSprite)
        {
            if (currentState == GlitchPlayerState.Sleeping)
                switchState(GlitchPlayerState.Walking);
        }

        /*
         * Implement the State Pattern!
         */
        private void switchState(GlitchPlayerState newState)
        {
            pauseAnimation = false; // just in case

            // switch the state to the given state
            currentState = newState;
            spriteSheet.setCurrentSegment((Int32)newState);
            currentFrame = spriteSheet.currentSegment.startFrame;
        }


        /** STATES **/
        private abstract class AbstractState
        {
            protected readonly GlitchPlayer player;

            protected AbstractState(GlitchPlayer player)
            {
                this.player = player;
            }

            public virtual void Update(GameTime gameTime, Rectangle clientBounds)
            {
            }
        }
        
        /* Walking State */
        private class WalkingState : AbstractState
        {
            Point stillFrame;
            int timeSinceLastMove = 0;
            const int timeForSleep = 3000;

            public WalkingState(GlitchPlayer player)
                : base(player)
            {
                stillFrame = new Point(14, 0);
            }

            public override void Update(GameTime gameTime, Rectangle clientBounds)
            {
                // pause animation if the sprite is not moving
                if (player.direction.X == 0)
                {
                    player.pauseAnimation = true;
                    player.currentFrame = stillFrame; // standing frame
                }
                else
                {
                    timeSinceLastMove = 0;
                    player.pauseAnimation = false;
                }

                // transition to jumping state
                if (Keyboard.GetState().IsKeyDown(Keys.Space) || GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed)
                {
                   
                    timeSinceLastMove = 0;
                    player.switchState(GlitchPlayerState.Jumping);
                }

                //transition to climbing state
                if (Keyboard.GetState().IsKeyDown(Keys.W))
                {
                    timeSinceLastMove = 0;
                    player.switchState(GlitchPlayerState.Climbing);
                }

                // transition to sleep state?
                timeSinceLastMove += gameTime.ElapsedGameTime.Milliseconds;
                if (timeSinceLastMove > timeForSleep)
                {
                    timeSinceLastMove = 0;
                    player.switchState(GlitchPlayerState.Sleeping);
                }
            }
        }

        /* Sleeping State */
        private class SleepingState : AbstractState
        {
            Vector2 sleepingPosition;
            Boolean fallingToSleep = true;

            public SleepingState(GlitchPlayer player)
                : base(player)
            {
            }

            public override void Update(GameTime gameTime, Rectangle clientBounds)
            {
                if (fallingToSleep)
                {
                    sleepingPosition = player.position;
                    fallingToSleep = false;
                }

                if (player.currentFrame == player.spriteSheet.currentSegment.endFrame)
                {
                    player.pauseAnimation = true;
                }

                if (sleepingPosition != player.position)
                {
                    fallingToSleep = true;
                    player.switchState(GlitchPlayerState.Walking);
                }
            }
        }


        /* Jumping State */
        private class JumpingState : AbstractState
        {
            private int direction;

            public JumpingState(GlitchPlayer player)
                : base(player)
            {

            }

            public override void Update(GameTime gameTime, Rectangle clientBounds)
            {
                player.pauseAnimation = false;

                if (player.effects == SpriteEffects.None)
                {
                    direction = 1;
                }
                else
                {
                    direction = -1;
                }

                if (player.currentFrame.Y == 2)
                {
                    player.position.Y--;
                    player.position.X = player.position.X + direction;
                }
                else
                {
                    player.position.Y++;
                    player.position.X = player.position.X + direction;
                }


                //transition back to walking state
                if (player.currentFrame == player.spriteSheet.currentSegment.endFrame)
                {
                    player.switchState(GlitchPlayerState.Walking);
                    player.currentFrame = new Point(14, 0);  // start standing still
                }
            }
        
        }

        /* Climbing State */
        private class ClimbingState : AbstractState
        {

            Point stillFrame;
            int timeSinceLastMove = 0;
            const int timeForWalk = 500;

            public ClimbingState(GlitchPlayer player)
                : base(player)
            {
                stillFrame = new Point(4, 1);
            }

            public override void Update(GameTime gameTime, Rectangle clientBounds)
            {
                player.pauseAnimation = false;

                if (player.direction.Y == 0)
                {
                    player.pauseAnimation = true;
                    player.currentFrame = stillFrame; // standing frame
                }
                else
                {
                    timeSinceLastMove = 0;
                    player.pauseAnimation = false;
                }

                //transition back to walking state
                timeSinceLastMove += gameTime.ElapsedGameTime.Milliseconds;
                if (timeSinceLastMove > timeForWalk)
                {
                    timeSinceLastMove = 0;
                    player.switchState(GlitchPlayerState.Walking);
                }
            }
        }
    }
}
