using ImGuiNET;
using SacraSlice.Dependencies.Engine.ECS.Component;
using System;
using System.Collections.Generic;

namespace SacraSlice.Dependencies.Engine.States
{
    public class StateManager : ImGuiElement // Holds of bunch of abstract type "State" classes and handles logic to switch between them
    {
        Dictionary<string, State> lookup = new Dictionary<string, State>();

        public State currentState;

        public State resetState, defaultState;

        //List<State> states;
        public StateManager() { }
        public StateManager(List<State> states)
        {
            Initialize(states);
        }

        public void Initialize(List<State> states)
        {
            //this.states = states;

            for (int i = 0; i < states.Count; i++)
            {
                lookup.Add(states[i].ToString(), states[i]);
            }
            currentState = states[0];
        }
        public void Update()
        {
            if (currentState != null)
                currentState.Act();
        }
        public void AddState(string s, State st)
        {
            lookup.Add(s, st);
        }
        public bool SetState(string n)
        {
            State s;
            if (lookup.TryGetValue(n, out s))
            {
                if (currentState != s)
                {
                    s.OnEnter();
                    currentState.OnLeave();
                } 
                currentState = s;
            } 
            else
            {
                //throw new KeyNotFoundException("State '"+n+"' could not be found");
                DebugLog.Print("StateManager", MessageType.error, "State '" + n + "' could not be found");
                return false;
            }
            return true;
        }

        [Obsolete("Does not reset the timer. Add timer parameter to reset State Time.")]
        public bool SetStateUpdate(string n)
        {
            if (SetState(n))
            {
                Update();
                return true;
            }
            return false;
        }

        public bool SetStateUpdate(string n, Timer t)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            if (SetStateUpdate(n))
            {
                t.GetTimer("State Time").Value = 0;
                return true;
            }
            return false;
#pragma warning restore CS0618 // Type or member is obsolete
        }
        public override void CustomRender()
        {
            if (ImGui.CollapsingHeader("State Manager"))
            {
                if (ImGui.BeginTable("State Manager Table", 1))
                {

                    ImGui.TableSetupColumn("State");
                    ImGui.TableHeadersRow();

                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();


                    foreach (State s in lookup.Values)
                    {
                        if (s == currentState)
                            ImGui.TextColored(new System.Numerics.Vector4(255, 0, 0, 255), s.ToString());
                        else
                            ImGui.Text(s.ToString());

                        ImGui.TableNextRow();
                        ImGui.TableNextColumn();
                    }

                    ImGui.EndTable();
                }

            }
        }

    }
}
