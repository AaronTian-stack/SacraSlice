using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;
using SacraSlice.Dependencies.Engine;
using System;
using System.Collections.Generic;
using System.Text;

namespace SacraSlice.Dependencies.Engine.ECS.Component
{
    public class VerletRope : ImGuiElement
    {
        public float segmentLength;
        public List<RopeSegment> ropeSegments = new List<RopeSegment>();
        public int segments;
        public Vector2 gravityForce;
        public Vector2 target;

        public Color color;
        public Color outlineColor;

        public float depth, depthOffset;

        public bool clamp;

        public string name;

        public int constraint;

        public float width, outlineWidth;


        public VerletRope(string name, Vector2 target, Vector2 gravityForce, Color color, Color outlineColor, bool clamp,
            float depthOffset, float segmentLength = 0.25f, int segments = 35, float width = 1, float outlineWidth = 0, int constraint = 50
            )
        {
            this.name = name;
            this.gravityForce = gravityForce;
            this.target = target;
            this.color = color;
            this.outlineColor = outlineColor;
            this.clamp = clamp;

            this.segmentLength = segmentLength;
            this.segments = segments;

            this.width = width;
            this.outlineWidth = outlineWidth;
            this.constraint = constraint;

            this.depthOffset = depthOffset;

            if (segments < 3)
                this.segments = 3;

            Vector2 start = target;

            for (int i = 0; i < segments; i++)
            {
                ropeSegments.Add(new RopeSegment(start));
            }
        }

        public void Update()
        {
            // simulate
            for (int i = 0; i < segments; i++)
            {
                RopeSegment firstSegment = ropeSegments[i];
                Vector2 velocity = firstSegment.posNow - firstSegment.posOld;
                firstSegment.posOld = firstSegment.posNow;
                firstSegment.posNow += velocity;
                firstSegment.posNow += gravityForce;
                ropeSegments[i] = firstSegment;
            }

            // constraint
            for (int i = 0; i < constraint; i++)
            {
                ApplyConstraint();
            }
        }

        public void ApplyConstraint()
        {
            var firstSegment = ropeSegments[0];
            firstSegment.posNow = target; // target

            ropeSegments[0] = firstSegment;

            for (int i = 0; i < segments - 1; i++)
            {
                var firstSeg = ropeSegments[i];
                var secondSeg = ropeSegments[i + 1];

                float dist = (firstSeg.posNow - secondSeg.posNow).Length();
                float error = MathF.Abs(dist - segmentLength);
                Vector2 changeDir = Vector2.Zero;

                if (dist > segmentLength)
                {
                    changeDir = firstSeg.posNow - secondSeg.posNow;
                    changeDir.Normalize();
                }
                else if (dist < segmentLength)
                {
                    changeDir = secondSeg.posNow - firstSeg.posNow;
                    changeDir.Normalize();
                }

                Vector2 changeAmount = changeDir * error;

                if (i != 0)
                {
                    firstSeg.posNow -= changeAmount * 0.5f;
                    ropeSegments[i] = firstSeg;
                    secondSeg.posNow += changeAmount * 0.5f;
                    ropeSegments[i + 1] = secondSeg;
                }
                else
                {
                    secondSeg.posNow += changeAmount;
                    ropeSegments[i + 1] = secondSeg;
                }


            }

        }

        public override void CustomRender()
        {
            if (ImGui.CollapsingHeader("Verlet Rope"))
            {
                //ImGui.Text(target.ToString());
                //ImGui.Text(ropeSegments[1].posNow.ToString());
            }

        }
    }

    public struct RopeSegment
    {
        public Vector2 posNow;
        public Vector2 posOld;

        public RopeSegment(Vector2 pos)
        {
            posNow = pos;
            posOld = pos;
        }
    }

}
