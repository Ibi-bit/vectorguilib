using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using VectorGraphics;

namespace VectorGui;

public class Gui
{
    public abstract class GuiComponent
    {
        public Vector2 Position { get; set; }
        public Color Color { get; set; }
        public bool IsVisible { get; set; } = true;
        public bool wasPressed { get; set; } = false;
        public abstract void Draw(SpriteBatch spriteBatch, PrimitiveBatch primitiveBatch);

        /// <summary>
        /// Checks if the button is pressed on said gui component but not if button is pressed first
        /// So you must check if button is pressed first for peformance reasons
        /// <summary>
        public abstract bool IsPressed(MouseState mouseState);

        /// <summary>
        /// Checks if the button is pressed on said gui component but not if button is pressed first
        /// So you must check if button is pressed first for peformance reasons
        /// <summary>
        public abstract bool IsReleased(MouseState mouseState);

        /// <summary>
        /// Checks if the button is pressed on said gui component but not if button is pressed first
        /// So you must check if button is pressed first for peformance reasons
        /// <summary>
        public abstract bool IsHovered(MouseState mouseState);
    }

    public class GuiRectangle : GuiComponent
    {
        public Vector2 Size { get; set; }

        public override void Draw(SpriteBatch spriteBatch, PrimitiveBatch primitiveBatch)
        {
            if (IsVisible)
            {
                PrimitiveBatch.Rectangle primitiveRectangle = new PrimitiveBatch.Rectangle(
                    Position,
                    Size,
                    Color
                );
                primitiveRectangle.Draw(spriteBatch, primitiveBatch);
            }
        }

        public override bool IsPressed(MouseState mouseState)
        {
            wasPressed = mouseState.LeftButton == ButtonState.Pressed && IsHovered(mouseState);

            return wasPressed;
        }

        public override bool IsReleased(MouseState mouseState)
        {
            if (wasPressed && mouseState.LeftButton == ButtonState.Released)
            {
                wasPressed = false;
                return true;
            }
            return false;
        }

        public override bool IsHovered(MouseState mouseState)
        {
            return mouseState.Position.X >= Position.X
                && mouseState.Position.X <= Position.X + Size.X
                && mouseState.Position.Y >= Position.Y
                && mouseState.Position.Y <= Position.Y + Size.Y;
        }
    }

    public class GuiCircle : GuiComponent
    {
        public float Radius { get; set; }

        public override void Draw(SpriteBatch spriteBatch, PrimitiveBatch primitiveBatch)
        {
            if (IsVisible)
            {
                PrimitiveBatch.Circle primitiveCircle = new PrimitiveBatch.Circle(
                    Position,
                    Radius,
                    Color
                );
                primitiveCircle.Draw(spriteBatch, primitiveBatch);
            }
        }

        public override bool IsHovered(MouseState mouseState)
        {
            return Vector2.DistanceSquared(mouseState.Position.ToVector2(), Position)
                <= Radius * Radius;
        }

        public override bool IsPressed(MouseState mouseState)
        {
            wasPressed = mouseState.LeftButton == ButtonState.Pressed && IsHovered(mouseState);
            return wasPressed;
        }

        public override bool IsReleased(MouseState mouseState)
        {
            if (wasPressed && mouseState.LeftButton == ButtonState.Released)
            {
                wasPressed = false;
                return true;
            }
            return false;
        }
    }
}
