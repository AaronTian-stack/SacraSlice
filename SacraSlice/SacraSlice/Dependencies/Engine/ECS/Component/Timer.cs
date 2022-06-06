using ImGuiNET;
using System;
using System.Collections.Generic;

namespace SacraSlice.Dependencies.Engine.ECS.Component
{
    public class Timer : ImGuiElement // TODO: maybe make different kinds of timers with different times (for player etc)
    {

        Dictionary<string, Wrapper<float>> lookup = new Dictionary<string, Wrapper<float>>();
        Dictionary<string, Wrapper<bool>> lookupSwitch = new Dictionary<string, Wrapper<bool>>(); // on and off switches
        Wrapper<float> r;
        Wrapper<bool> b;
        /// <summary>
        /// Adds a timer by format "Name Timer". If not present a new one is created
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Wrapper<float> GetTimer(String name)
        {

            if (lookup.TryGetValue(name, out r))
                return r;
            else
                lookup.Add(name, new Wrapper<float>(0));

            return GetTimer(name);

        }
        public Wrapper<bool> GetSwitch(String name)
        {
            if (lookupSwitch.TryGetValue(name, out b))
                return b;
            else
                lookupSwitch.Add(name, new Wrapper<bool>(false));

            return GetSwitch(name);
        }

        public override void CustomRender()
        {
            if (ImGui.CollapsingHeader("Timer"))
            {
                foreach (var item in lookup)
                {
                    float f = item.Value.Value;
                    ImGui.DragFloat(item.Key, ref f);
                    item.Value.Value = f;
                    if (item.Value.Value < 0)
                        item.Value.Value = 0;
                }
                foreach (var item in lookupSwitch)
                {
                    ImGui.Text(item.Key);
                    ImGui.SameLine();
                    if (item.Value)
                        ImGui.TextColored(new System.Numerics.Vector4(0, 255, 0, 255), item.Value.ToString());
                    else
                        ImGui.TextColored(new System.Numerics.Vector4(255, 0, 0, 255), item.Value.ToString());
                }
            }

        }
    }
}
