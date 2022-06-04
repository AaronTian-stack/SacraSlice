using ImGuiNET;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using SacraSlice.Dependencies.Engine.States;
using System.Collections.Generic;

namespace SacraSlice.Dependencies.Engine
{
    /// <summary>
    /// What side the sprite will touch when drawn if there is a hitbox
    /// </summary>
    public enum HitBoxFlag
    {
        Center, Right, Left, Bottom, Top
    }
    public class HitBox : ImGuiElement
    {
        public RectangleF rect;

        public List<RectangleF> stateRects;
        public List<State> states;

        private Color color = Color.LightGreen;

        public bool debug;

        public HitBoxFlag renderFlag = HitBoxFlag.Center;

        private Dictionary<State, RectangleF> lookup = new Dictionary<State, RectangleF>();
        public HitBox()
        {
           
        }

        public HitBox(List<State> states, List<RectangleF> stateRects)
        {
            this.states = states;
            this.stateRects = stateRects;
            for (int i = 0; i < states.Count; i++)
            {
                lookup.Add(states[i], stateRects[i]);
            }
        }

        public void AddState(State s, RectangleF rect)
        {
            lookup.Add(s, rect);
        }

        public void ChangeRect(State s, RectangleF rect)
        {
            lookup[s] = rect;
        }

        State oldState;

        /// <summary>
        /// Sets Rectangle based off State. If State not found nothing happens
        /// </summary>
        /// <param name="s"></param>
        public void SetRect(State s)
        {
            if (!lookup.TryGetValue(s, out rect))
            {
                if (oldState != s)
                {
                    DebugLog.Print("Hitbox", MessageType.error, "State [" + s + "] not found");
                    oldState = s;
                }
            }
        }

        System.Numerics.Vector2 vec2;

        public Color Color { get => color; set => color = value; }

        public override void CustomRender()
        {
            if (ImGui.CollapsingHeader("Hitbox"))
            {
                vec2 = new System.Numerics.Vector2(rect.X, rect.Y);
                ImGui.DragFloat2("Position", ref vec2);

                vec2 = new System.Numerics.Vector2(rect.Width, rect.Height);
                ImGui.DragFloat2("Dimensions", ref vec2);

                System.Numerics.Vector4 vec4 = new System.Numerics.Vector4(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
                ImGui.ColorEdit4("HitBox Color", ref vec4);
                color = new Color(vec4.X, vec4.Y, vec4.Z, vec4.W);

                if (ImGui.BeginTable("renderflag", 1))
                {
                    ImGui.TableSetupColumn("Render Flag");
                    ImGui.TableHeadersRow();

                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();

                    if (ImGui.Selectable("Center", renderFlag == HitBoxFlag.Center))
                        renderFlag = HitBoxFlag.Center;
                    if (ImGui.Selectable("Left", renderFlag == HitBoxFlag.Left))
                        renderFlag = HitBoxFlag.Left;
                    if (ImGui.Selectable("Right", renderFlag == HitBoxFlag.Right))
                        renderFlag = HitBoxFlag.Right;
                    if (ImGui.Selectable("Top", renderFlag == HitBoxFlag.Top))
                        renderFlag = HitBoxFlag.Top;
                    if (ImGui.Selectable("Bottom", renderFlag == HitBoxFlag.Bottom))
                        renderFlag = HitBoxFlag.Bottom;

                    ImGui.EndTable();
                }



                ImGui.Checkbox("Show HitBox", ref debug);

            }
        }
    }
}
