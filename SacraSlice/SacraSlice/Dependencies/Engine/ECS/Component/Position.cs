using ImGuiNET;
using Microsoft.Xna.Framework;

namespace SacraSlice.Dependencies.Engine.ECS.Component
{
    public class Position : ImGuiElement
    {

        public Position()
        {
            //this.collideClamp = collideClamp;
        }

        public Vector2 velocity = new Vector2();
        public Vector2 carryVelocity = new Vector2();

        public Vector2 currPosition = new Vector2();
        public Vector2 prevPosition = new Vector2();
        public Vector2 renderPosition = new Vector2();

        public float start; // save a 1D starting velocity
        public float saved; // save a 1D velocity

        public bool orientation, ground, gravity;// collideClamp; // true for left false for right

        System.Numerics.Vector2 vec2 = new System.Numerics.Vector2();

        public float depth;

        /// <summary>
        /// Set all positions to given vector
        /// </summary>
        public void SetAllPosition(Vector2 pos, float depth = 0)
        {
            
            this.depth = depth;
                
            currPosition = pos;
            prevPosition = pos;
            renderPosition = pos;
        }

        public override void CustomRender()
        {
            if (ImGui.CollapsingHeader("Position"))
            {
                vec2 = new System.Numerics.Vector2(currPosition.X, currPosition.Y);

                ImGui.DragFloat2("Current Position", ref vec2);

                currPosition = new Vector2(vec2.X, vec2.Y);

                vec2 = new System.Numerics.Vector2(prevPosition.X, prevPosition.Y);

                ImGui.DragFloat2("Previous Position", ref vec2);

                prevPosition = new Vector2(vec2.X, vec2.Y);

                vec2 = new System.Numerics.Vector2(renderPosition.X, renderPosition.Y);

                ImGui.DragFloat2("Render Position", ref vec2);

                renderPosition = new Vector2(vec2.X, vec2.Y);

                ImGui.Separator();

                vec2 = new System.Numerics.Vector2(velocity.X, velocity.Y);

                ImGui.DragFloat2("Velocity", ref vec2);

                velocity = new Vector2(vec2.X, vec2.Y);

                vec2 = new System.Numerics.Vector2(carryVelocity.X, carryVelocity.Y);

                ImGui.DragFloat2("Carry Velocity", ref vec2);

                carryVelocity = new Vector2(vec2.X, vec2.Y);

                ImGui.Separator();

                ImGui.DragFloat("Start Number", ref start);
                ImGui.DragFloat("Saved Number", ref saved);

                ImGui.Separator();

                ImGui.DragFloat("Depth (cur pos Z)", ref depth, 0.01f, 0, 1, null, ImGuiSliderFlags.AlwaysClamp);

                ImGui.Separator();

                if (ground)
                    ImGui.TextColored(new System.Numerics.Vector4(0, 255, 0, 255), "Ground");
                else
                    ImGui.TextColored(new System.Numerics.Vector4(255, 0, 0, 255), "Ground");
            }

        }
    }
}
