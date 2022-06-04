using ImGuiNET;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using SacraSlice.Dependencies.Engine.ECS.Component;
using System.Collections.Generic;

namespace SacraSlice.Dependencies.Engine
{
    public class GameCamera : ImGuiElement // Wrapper with functions for managing actions and cutscenes
    {

        public Stack<Position> targets = new Stack<Position>();

        public Stack<float> targetZooms = new Stack<float>();

        /// <summary>
        /// Position speed
        /// </summary>
        public Stack<float> speedP = new Stack<float>();

        /// <summary>
        /// Zoom speed
        /// </summary>
        public Stack<float> speedZ = new Stack<float>();

        // TODO: camera interpolations, lag speed 

        public Stack<Interpolation> interpolationP = new Stack<Interpolation>();

        public Stack<Interpolation> interpolationZ = new Stack<Interpolation>();



        /// <summary>
        /// The actual orthographic camera
        /// </summary>
        public OrthographicCamera orthoCamera;

        BoxingViewportAdapter _viewportAdapter;
        public GameCamera(BoxingViewportAdapter viewportAdapter)
        {
            this._viewportAdapter = viewportAdapter;
            orthoCamera = new OrthographicCamera(viewportAdapter);
        }
        public Vector2 Position { get => orthoCamera.Center; set => orthoCamera.LookAt(value); }
        public float Zoom { get => orthoCamera.Zoom; set => orthoCamera.Zoom = value; }
        public float Rotation { get => orthoCamera.Rotation; set => orthoCamera.Rotation = value; }
        public Matrix ViewMatrix { get => orthoCamera.GetViewMatrix(); }

        public bool manual;
        public void Move(Vector2 move)
        {
            orthoCamera.Move(move);
        }

        public void LookAt(float x, float y)
        {
            LookAt(new Vector2(x, y));
        }

        public void LookAt(Vector2 look)
        {
            orthoCamera.LookAt(look);
        }

        System.Numerics.Vector2 vec2 = new System.Numerics.Vector2();

        public override void CustomRender()
        {
            if (ImGui.CollapsingHeader("Game Camera"))
            {
                vec2 = new System.Numerics.Vector2(Position.X, Position.Y);
                ImGui.DragFloat2("Position", ref vec2);
                Position = new Vector2(vec2.X, vec2.Y);

                float zoom = orthoCamera.Zoom;
                ImGui.DragFloat("Zoom", ref zoom, 0.01f, orthoCamera.MinimumZoom, orthoCamera.MaximumZoom, null, ImGuiSliderFlags.AlwaysClamp);
                orthoCamera.Zoom = zoom;

                float rot = Rotation;
                ImGui.DragFloat("Rotation", ref rot, 0.1f);
                Rotation = rot;

                ImGui.Separator();

                ImGui.TextColored(new System.Numerics.Vector4(255, 0, 0, 255), "Stacks");
                ImGui.SetNextItemOpen(true);
                if (ImGui.TreeNode("Target Positions"))
                {
                    foreach (Position p in targets)
                    {
                        ImGui.BulletText(p.renderPosition.ToString());
                    }
                    ImGui.TreePop();
                }
                FloatStack(speedP, "Position Speeds");
                InterpolationStack(interpolationP, "Position Interpolations");

                FloatStack(targetZooms, "Target Zooms");
                FloatStack(speedZ, "Zoom Speeds");
                InterpolationStack(interpolationZ, "Zoom Interpolations");

            }
        }

        public void FloatStack(Stack<float> stack, string s)
        {
            ImGui.SetNextItemOpen(true);
            if (ImGui.TreeNode(s))
            {
                foreach (float p in stack)
                {
                    ImGui.BulletText(p.ToString());
                }
                ImGui.TreePop();
            }
        }

        public void InterpolationStack(Stack<Interpolation> stack, string s)
        {
            ImGui.SetNextItemOpen(true);
            if (ImGui.TreeNode(s))
            {
                foreach (Interpolation p in stack)
                {
                    ImGui.BulletText(p.ToString());
                }
                ImGui.TreePop();
            }
        }
    }
}
