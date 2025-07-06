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

        public GuiComponent(Vector2 position, Color color, bool isVisible = true)
        {
            Position = position;
            Color = color;
            IsVisible = isVisible;
        }

        public GuiComponent()
            : this(Vector2.Zero, Color.White) { }

        public abstract void Draw(SpriteBatch spriteBatch, PrimitiveBatch primitiveBatch);

        public abstract bool IsPressed(MouseState mouseState);

        public abstract bool IsReleased(MouseState mouseState);

        public abstract bool IsHovered(MouseState mouseState);
    }

    public class GuiRectangle : GuiComponent
    {
        public Vector2 Size { get; set; }

        public GuiRectangle(Vector2 position, Vector2 size, Color color, bool isVisible = true)
            : base(position, color, isVisible)
        {
            Size = size;
        }

        public GuiRectangle()
            : this(Vector2.Zero, Vector2.One * 100, Color.White) { }

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
            // Only return true on the frame the button transitions from released to pressed
            if (mouseState.LeftButton == ButtonState.Pressed && IsHovered(mouseState))
            {
                if (!wasPressed)
                {
                    wasPressed = true;
                    return true;
                }
                // If already pressed, do not trigger again
                return false;
            }
            else
            {
                wasPressed = false;
                return false;
            }
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

    public class GuiRoundedRectangle : GuiRectangle
    {
        public float CornerRadius { get; set; }

        public GuiRoundedRectangle(
            Vector2 position,
            Vector2 size,
            float cornerRadius,
            Color color,
            bool isVisible = true
        )
            : base(position, size, color, isVisible)
        {
            CornerRadius = cornerRadius;
        }

        public override void Draw(SpriteBatch spriteBatch, PrimitiveBatch primitiveBatch)
        {
            if (IsVisible)
            {
                PrimitiveBatch.RoundedRectangle primitiveRoundedRectangle =
                    new PrimitiveBatch.RoundedRectangle(Position, Size, CornerRadius, Color);
                primitiveRoundedRectangle.Draw(spriteBatch, primitiveBatch);
            }
        }
    }

    public class GuiCircle : GuiComponent
    {
        public float Radius { get; set; }

        public GuiCircle(Vector2 position, float radius, Color color, bool isVisible = true)
            : base(position, color, isVisible)
        {
            Radius = radius;
        }

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
            return wasPressed =
                mouseState.LeftButton == ButtonState.Pressed && IsHovered(mouseState);
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
