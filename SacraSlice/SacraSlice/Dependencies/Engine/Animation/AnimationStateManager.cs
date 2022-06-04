using ImGuiNET;
using SacraSlice.Dependencies.Engine.States;
using System;
using System.Collections.Generic;

namespace SacraSlice.Dependencies.Engine.Animation
{
    public class AnimationStateManager : ImGuiElement // Holds a bunch of animations and switches them based off abstract "State"
    {

        public Animation<TextureRegion> currAnim;
        List<Animation<TextureRegion>> animations;
        List<State> states;

        public float scaleX, scaleY;

        Dictionary<State, Animation<TextureRegion>> lookup = new Dictionary<State, Animation<TextureRegion>>();
        public AnimationStateManager()
        {

        }
        public AnimationStateManager(List<State> states, List<Animation<TextureRegion>> animations)
        {
            Initialize(states, animations);
        }

        public void Initialize(List<State> states, List<Animation<TextureRegion>> animations)
        {
            // Lists must be in corresponding order
            this.states = states;
            this.animations = animations;
            if (states.Count != animations.Count)
                throw new NotSupportedException("AnimationStateManager number of states and animations must match");
            for (int i = 0; i < states.Count; i++)
            {
                lookup.Add(states[i], animations[i]);
            }
        }

        public void AddState(State s, Animation<TextureRegion> a)
        {
            lookup.Add(s, a);
        }

        public void SetAnimation(State s)
        {
            if (!lookup.TryGetValue(s, out currAnim))
                throw new KeyNotFoundException($"Could not find animation {s} in AnimationStateManager");
        }

        public override void CustomRender()
        {
            if (ImGui.CollapsingHeader("Animation State Manager"))
            {
                if (ImGui.BeginTable("Animation State Manager Table", 2))
                {
                    ImGui.TableSetupColumn("State (String)");
                    ImGui.TableSetupColumn("Animation String Name");
                    ImGui.TableHeadersRow();

                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();


                    foreach (State s in lookup.Keys)
                    {
                        ImGui.Text(s.ToString());
                        ImGui.TableNextColumn();

                        ImGui.Text(lookup[s].ToString());

                        ImGui.TableNextRow();
                        ImGui.TableNextColumn();
                    }

                    ImGui.EndTable();
                }
            }
        }


    }
}
