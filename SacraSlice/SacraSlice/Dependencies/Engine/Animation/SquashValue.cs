using ImGuiNET;
using Microsoft.Xna.Framework;
using SacraSlice.Dependencies.Engine.States;

namespace SacraSlice.Dependencies.Engine.Animation
{
    public class SquashValue : ImGuiElement // contains some numbers and interpolations for squashing during animation in a single state
    {

        public State state;

        public Interpolation interpolationX, interpolationY;

        public Vector2 startValues = new Vector2();
        public Vector2 endValues = new Vector2();

        public (Wrapper<float>, float) timeX;
        public (Wrapper<float>, float) timeY;

        public SquashValue(State state, Interpolation ix, Interpolation iy, Vector2 st, Vector2 en, (Wrapper<float>, float) tx, (Wrapper<float>, float) ty)
        {
            this.state = state;
            interpolationX = ix;
            interpolationY = iy;
            startValues = st;
            endValues = en;
            timeX = tx;
            timeY = ty;
        }

        public override void CustomRender()
        {

            ImGui.TextColored(new System.Numerics.Vector4(255, 0, 0, 255), "State:");
            ImGui.SameLine();
            ImGui.Text(state.ToString());
            ImGui.NewLine();


            ImGui.TextColored(new System.Numerics.Vector4(255, 0, 0, 255), "InterpolationX:");
            ImGui.SameLine();
            ImGui.Text(interpolationX.ToString());
            ImGui.NewLine();

            ImGui.TextColored(new System.Numerics.Vector4(255, 0, 0, 255), "InterpolationY:");
            ImGui.SameLine();
            ImGui.Text(interpolationY.ToString());
            ImGui.NewLine();

            ImGui.TextColored(new System.Numerics.Vector4(255, 0, 0, 255), "Start Values:");
            ImGui.SameLine();
            ImGui.Text(startValues.ToString());
            ImGui.NewLine();

            ImGui.TextColored(new System.Numerics.Vector4(255, 0, 0, 255), "End Values:");
            ImGui.SameLine();
            ImGui.Text(endValues.ToString());
            ImGui.NewLine();

            ImGui.TextColored(new System.Numerics.Vector4(255, 0, 0, 255), "TimeX:");
            ImGui.SameLine();
            ImGui.Text(timeX.ToString());
            ImGui.NewLine();

            ImGui.TextColored(new System.Numerics.Vector4(255, 0, 0, 255), "TimeY:");
            ImGui.SameLine();
            ImGui.Text(timeY.ToString());
            ImGui.NewLine();
        }
    }
}
