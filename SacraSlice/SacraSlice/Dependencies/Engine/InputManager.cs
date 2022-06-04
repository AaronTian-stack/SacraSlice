using ImGuiNET;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SacraSlice.Dependencies.Engine
{
    public class InputManager : ImGuiElement // Manages and maps input for Keyboard. Put this into a stack in your screen! Might add controller support later
    {

        // Dual array to map Actions to Keys
        List<string> actions;
        List<Keys> actionKeys;

        // A dictionary to actions to true/false values (pressed or not)
        Dictionary<string, bool> actionOnOff = new Dictionary<string, bool>();

        public InputManager() { }
        public InputManager(List<string> actions, List<Keys> actionKeys) // provide a list of strings as actions
        {
            Initialize(actions, actionKeys);
        }

        public void Initialize(List<string> actions, List<Keys> actionKeys)
        {
            if (actions.Count != actionKeys.Count)
                throw new NotSupportedException("InputManager number of actions and actionKeys must match");
            this.actions = actions;
            this.actionKeys = actionKeys;
            foreach (string s in actions)
            {
                actionOnOff.Add(s, false);
            }
        }

        /// <summary>
        /// Sets all active action keys to true if they are pressed
        /// </summary>
        public void Poll()
        {

            foreach (var key in actionOnOff.Keys.ToList())
            {
                actionOnOff[key] = false;
            }

            for (int i = 0; i < actionKeys.Count; i++)
            {
                if (KeyboardExtended.GetState().IsKeyDown(actionKeys[i]))
                {
                    actionOnOff[actions[i]] = true;
                }
            }

        }
        /// <summary>
        /// Returns true if key name is active, false if not
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public bool IsActive(string action)
        {
            bool a;
            if (actionOnOff.TryGetValue(action, out a))
                return a;
            throw new ArgumentOutOfRangeException($"There is no action in InputManager named '{action}'");
        }
        /// <summary>
        /// Returns true is all listed keys are active, false if not
        /// </summary>
        /// <param name="actions"></param>
        /// <returns></returns>
        public bool IsActiveAll(params string[] actions)
        {
            foreach (string s in actions)
            {
                if (!IsActive(s))
                    return false;
            }
            return true;
        }
        /// <summary>
        /// Returns true if any listed keys are active, false if not
        /// </summary>
        /// <param name="actions"></param>
        /// <returns></returns>
        public bool IsActiveOr(params string[] actions)
        {
            foreach (string s in actions)
            {
                if (IsActive(s))
                    return true;
            }
            return false;
        }

        public override void CustomRender()
        {
            if (ImGui.CollapsingHeader("Input Manager"))
            {
                if (ImGui.BeginTable("Input Manager Table", 2))
                {
                    ImGui.TableSetupColumn("Actions");
                    ImGui.TableSetupColumn("Key");
                    ImGui.TableHeadersRow();

                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();


                    for (int i = 0; i < actions.Count; i++)
                    {
                        if (actionOnOff[actions[i]])
                            ImGui.TextColored(new System.Numerics.Vector4(0, 255, 0, 255), actions[i]);
                        else
                            ImGui.Text(actions[i]);
                        ImGui.TableNextColumn();

                        string a = actionKeys[i].ToString();
                        ImGui.InputText(actions[i], ref a, 1, ImGuiInputTextFlags.CharsUppercase);
                        if (!a.Equals("")) actionKeys[i] = (Keys)Enum.Parse(typeof(Keys), a);

                        ImGui.TableNextRow();
                        ImGui.TableNextColumn();
                    }

                    ImGui.EndTable();
                }
            }
        }
    }
}
