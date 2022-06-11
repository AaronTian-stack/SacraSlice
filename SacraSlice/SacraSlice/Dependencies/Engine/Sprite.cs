using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;

namespace SacraSlice.Dependencies.Engine
{
    public class Sprite : ImGuiElement
    {
        public TextureRegion Textureregion
        {
            get { return textureregion; }
            set
            {
                textureregion = value;

                Origin = new Vector2(textureregion.sourceRectangle.Width / 2, textureregion.sourceRectangle.Height / 2); // rotation is around center by default

                boundingBox = new RectangleF(Position.X, Position.Y, textureregion.width * scale.X, textureregion.height * scale.Y);
            }
        }

        private TextureRegion textureregion;

        private Vector2 scale = new Vector2(1, 1);

        private RectangleF boundingBox;

        public Vector2 Position = new Vector2();

        /// <summary>
        /// Origin of rotation
        /// </summary>
        public Vector2 Origin;

        public Color Color = new Color(255, 255, 255);

        public Color BoundColor = new Color(255, 0, 0);

        public Boolean FlipX, FlipY;

        public bool debug;

        private float rotation;
        /// <summary>
        /// Rotation in degrees
        /// </summary>
        public float Rotation { get => MathHelper.ToDegrees(rotation); set => rotation = MathHelper.ToRadians(value); }
        public Sprite() { }

        public Sprite(TextureRegion region)
        {
            Textureregion = region;
        }

        public Vector2 Scale
        {
            get { return scale; }
            set
            {
                scale = value;
                boundingBox.Width = Textureregion.width * scale.X;
                boundingBox.Height = Textureregion.height * scale.Y;
            }
        }
        public RectangleF BoundingBox
        {
            get { return boundingBox; }
            set
            {
                boundingBox = value;
                scale.X = boundingBox.Width / Textureregion.width;
                scale.Y = boundingBox.Height / Textureregion.height;
            }
        }

        public void Draw(SpriteBatch sb, float layerDepth = 0)
        {

            if (Textureregion.texture != null)
            {

                if (FlipX && FlipY)
                {
                    sb.Draw(Textureregion.texture, Position, Textureregion.sourceRectangle, Color, rotation, Origin, Scale, SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically, layerDepth);
                    return;
                }


                if (FlipX)
                {
                    sb.Draw(Textureregion.texture, Position, Textureregion.sourceRectangle, Color, rotation, Origin, Scale, SpriteEffects.FlipHorizontally, layerDepth);
                    return;
                }


                if (FlipY)
                {
                    sb.Draw(Textureregion.texture, Position, Textureregion.sourceRectangle, Color, rotation, Origin, Scale, SpriteEffects.FlipVertically, layerDepth);
                    return;
                }


                sb.Draw(Textureregion.texture, Position, Textureregion.sourceRectangle, Color, rotation, Origin, Scale, SpriteEffects.None, layerDepth);
            }

        }

        public void SetFlip(bool x, bool y)
        {
            FlipX = x; FlipY = y;
        }

        System.Numerics.Vector2 vec2;
        System.Numerics.Vector4 vec4;
        public override void CustomRender() // TODO: Add other fields if needed
        {

            if (ImGui.CollapsingHeader("Sprite"))
            {

                vec2 = new System.Numerics.Vector2(scale.X, scale.Y);
                ImGui.DragFloat2("Scale", ref vec2);
                scale = new Vector2(vec2.X, vec2.Y);

                vec2 = new System.Numerics.Vector2(Position.X, Position.Y);
                ImGui.DragFloat2("Position", ref vec2);
                Position = new Vector2(vec2.X, vec2.Y);

                vec2 = new System.Numerics.Vector2(Origin.X, Origin.Y);
                ImGui.DragFloat2("Origin", ref vec2);
                Origin = new Vector2(vec2.X, vec2.Y);

                vec4 = new System.Numerics.Vector4(Color.R / 255f, Color.G / 255f, Color.B / 255f, Color.A / 255f);
                ImGui.ColorEdit4("Sprite Color", ref vec4);
                Color = new Color(vec4.X, vec4.Y, vec4.Z, vec4.W);

                float a = Rotation;
                ImGui.DragFloat("Rotation (degrees)", ref a, 1, 0, 360, null, ImGuiSliderFlags.AlwaysClamp);
                Rotation = a;

                vec4 = new System.Numerics.Vector4(BoundColor.R / 255f, BoundColor.G / 255f, BoundColor.B / 255f, BoundColor.A / 255f);
                ImGui.ColorEdit4("BoundingBox Color", ref vec4);
                BoundColor = new Color(vec4.X, vec4.Y, vec4.Z, vec4.W);


                vec4 = new System.Numerics.Vector4(boundingBox.X, boundingBox.Y, boundingBox.Width, boundingBox.Height);
                ImGui.DragFloat4("Bounding Box (not editable", ref vec4);


                ImGui.Checkbox("Show Sprite BoundingBox", ref debug);
            }


        }
    }
}
