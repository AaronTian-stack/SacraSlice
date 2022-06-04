using ImGuiNET;
using SacraSlice.Dependencies.Engine.States;
using System.Collections.Generic;

namespace SacraSlice.Dependencies.Engine.Animation
{
    public class SquashManager : ImGuiElement // Follows factory pattern
    {

        public Dictionary<State, SquashValue> lookup = new Dictionary<State, SquashValue>();

        public SquashManager(params SquashValue[] sq)
        {
            foreach (SquashValue s in sq)
            {
                lookup.Add(s.state, s);
            }
        }

        public override void CustomRender()
        {
            if (ImGui.CollapsingHeader("Squash Manager"))
            {
                foreach (KeyValuePair<State, SquashValue> entry in lookup)
                {
                    ImGui.Separator();
                    entry.Value.CustomRender();
                }
            }
        }
    }
}
