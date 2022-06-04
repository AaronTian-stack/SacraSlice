using ImGuiNET;
using SacraSlice.Dependencies.Engine.Animation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;

namespace SacraSlice.Dependencies.Engine
{
    /// <summary>
    /// Clamps verlet to positions based off certain Animations
    /// </summary>
    public class RopeClamper : ImGuiElement
    {

        Dictionary<string, Dictionary<string, int[,]>> lookup = new Dictionary<string, Dictionary<string, int[,]>>();

        public RopeClamper(string path)
        {
            // rope clamper system will need timer (for animation frame), rope clamper for array lookup, sprite for positioning

            // take file path and parse

            Parse(path);

        }

        public int[,] GetArray(string ropeName, string animationName)
        {
            return lookup[ropeName][animationName];
        }

        /// <summary>
        /// super overkill XML parser
        /// </summary>
        /// <param name="path"></param>
        private void Parse(string path)
        {
            var reader = XmlReader.Create(@"Content/" + path);

            int[,] array;
            string currentRope = null;
            string currentAnimation = null;

            int count = 0;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:

                        if (reader.HasAttributes)
                        {

                            if (reader.GetAttribute("begin") != null)
                            {
                                //Debug.WriteLine("begin " + reader.Name);
                                currentRope = reader.Name;
                                lookup.Add(reader.Name, new Dictionary<string, int[,]>());
                            }

                            if (reader.GetAttribute("frames") != null)
                            {
                                //Debug.WriteLine("frames "+reader.Name);
                                //Debug.WriteLine("total frames " + reader.GetAttribute("frames"));
                                count = 0;
                                currentAnimation = reader.Name;
                                array = new int[int.Parse(reader.GetAttribute("frames")), 2];
                                lookup[currentRope].Add(currentAnimation, array);
                            }
                            if (reader.GetAttribute("X") != null && reader.GetAttribute("Y") != null)
                            {
                                //Debug.WriteLine("X " + reader.GetAttribute("X") + " Y " + reader.GetAttribute("Y"));
                                lookup[currentRope][currentAnimation][count, 0] = int.Parse(reader.GetAttribute("X"));
                                lookup[currentRope][currentAnimation][count, 1] = int.Parse(reader.GetAttribute("Y"));
                                count++;
                            }

                        }
                        break;
                }
            }

            reader.Close();
            reader.Dispose();

        }


        private void ParseOld(string path)
        {
            StreamReader read = new StreamReader(@"Content/" + path);

            char[] delimiterChars = { ' ' };

            while (!read.EndOfStream)
            {
                string[] head = read.ReadLine().Split(delimiterChars);

                for (int i = 0; i < int.Parse(head[1]); i++)
                {
                    string[] number = read.ReadLine().Split(delimiterChars);

                }
            }

            read.Close();
            read.Dispose();
        }

        public override void CustomRender()
        {
            if (ImGui.CollapsingHeader("Rope Clamper"))
            {

            }
        }
    }
}
