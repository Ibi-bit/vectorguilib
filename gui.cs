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
            if (mouseState.LeftButton == ButtonState.Pressed && IsHovered(mouseState))
            {
                if (!wasPressed)
                {
                    wasPressed = true;
                    return true;
                }
                
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
    public class Slider 
    {
        public Vector2 Position { get; set; }
        public float Min { get; set; }
        public float Max { get; set; }
        public float Value { get; set; }
        public float Width { get; set; } = 200f;
        public float Height { get; set; } = 20f;

        public Color BackgroundColor { get; set; } = Color.Gray;
        public Color FillColor { get; set; } = Color.Blue;
        public Color HandleColor { get; set; } = Color.White;

        public bool IsDragging { get; set; } = false;

        public VectorGraphics.PrimitiveBatch.Rectangle BackgroundRectangle { get; set; }
        public VectorGraphics.PrimitiveBatch.Rectangle FillRectangle { get; set; }
        public GuiRectangle Handle { get; set; }


        public Slider(Vector2 position, float min, float max, float initialValue, Color backgroundColor, Color fillColor, Color handleColor)
        {
            Position = position;
            Min = min;
            Max = max;
            Value = initialValue;
        }
        public void Initialize()
        {
            BackgroundRectangle = new PrimitiveBatch.Rectangle(Position, new Vector2(Width, Height), BackgroundColor);
            FillRectangle = new PrimitiveBatch.Rectangle(Position, new Vector2((Value - Min) / (Max - Min) * Width, Height), FillColor);
            Handle = new GuiRectangle(new Vector2(Position.X + FillRectangle.size.X - Height / 2, Position.Y - (Height / 2)), new Vector2(Height, Height), HandleColor);
        }

        public void Update(MouseState mouseState)
        {
            if (IsDragging)
            {
                float mouseX = mouseState.X;
                float clampedX = MathHelper.Clamp(mouseX, Position.X, Position.X + Width);
                Value = Min + (clampedX - Position.X) / Width * (Max - Min);
                FillRectangle.size = new Vector2((Value - Min) / (Max - Min) * Width, Height);
                Handle.Position = new Vector2(Position.X + FillRectangle.size.X - Height / 2, Position.Y - (Height / 2));

                if (mouseState.LeftButton == ButtonState.Released)
                {
                    IsDragging = false;
                }
            }
            else if (Handle.IsPressed(mouseState))
            {
                IsDragging = true;
            }
        }
        public bool IsHovered(MouseState mouseState)
        {
            return Handle.IsHovered(mouseState);
        }

        public void Draw(SpriteBatch spriteBatch, PrimitiveBatch primitiveBatch)
        {
            BackgroundRectangle.Draw(spriteBatch, primitiveBatch);
            FillRectangle.Draw(spriteBatch, primitiveBatch);
            Handle.Draw(spriteBatch, primitiveBatch);}
    }
}
