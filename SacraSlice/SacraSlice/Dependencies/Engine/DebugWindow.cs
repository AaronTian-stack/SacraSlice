using ImGuiNET;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using SacraSlice.Dependencies.Engine.Scene;
using System;
using System.Collections.Generic;
using System.Text;

namespace SacraSlice.Dependencies.Engine
{
    public class DebugWindow
    {
        DebugLog debug;
        Wrapper<int> ticks;
        Wrapper<float> dt;
        public DebugWindow(DebugLog debug, Wrapper<int> ticks, Wrapper<float> dt)
        {
            this.debug = debug;
            this.ticks = ticks;
            this.dt = dt;
            actor = new Actor(ticks, 0);
        }

        public void Draw(GameTime gameTime)
        {
            ImGui.Begin("Debug Window", ImGuiWindowFlags.MenuBar);

            if (ImGui.BeginMenuBar())
            {
                if (ImGui.BeginMenu("File"))
                {
                    if (ImGui.MenuItem("Open")) ; // TODO: fill these in
                    if (ImGui.MenuItem("Save")) ;
                    if (ImGui.MenuItem("Quit")) ;
                    ImGui.EndMenu();
                }
                if (ImGui.BeginMenu("Windows"))
                {
                    if (ImGui.MenuItem("Debug Log", null, debug.showWindow))
                    {
                        debug.showWindow = !debug.showWindow;

                    }
                    ImGui.EndMenu();
                }
                ImGui.EndMenuBar();
            }

            ImGui.Text("FPS: " + MathF.Round(ImGui.GetIO().Framerate));

            if (ImGui.Button("Reset Tickrate"))
            {
                ChangeTickRate(30, 0f, Interpolation.linear);
            }

            if (ImGui.Button("Slow Down Time"))
            {
                ChangeTickRate(5, 1f, Interpolation.fastSlow);
            }

            if (ImGui.Button("Reset Time To Normal"))
            {
                ChangeTickRate(30, 1f, Interpolation.slowFast);
            }

            if (ImGui.Button("Speed Up Time"))
            {
                ChangeTickRate(60, 1f, Interpolation.slowFast);
            }

            actor.Act(gameTime.GetElapsedSeconds());
            ticks.Value = (int)actor.x;
            var ti = ticks.Value;
            ImGui.DragInt("Ticks per second", ref ti, 1, 1, 30, null, ImGuiSliderFlags.AlwaysClamp);
            ticks.Value = ti;
            dt.Value = 1f / ticks;
        }

        static Actor actor;
        public static int justAdded;

        public void ChangeTickRate(int newTickRate, float time, Interpolation interpolation)
        // Adds an action to change the tick rate.
        {
            actor.x = ticks.Value;
            actor.AddAction(Actions.MoveTo(actor, newTickRate, 0, time, interpolation));
        }
    }
}
