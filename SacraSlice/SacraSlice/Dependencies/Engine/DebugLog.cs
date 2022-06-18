using ImGuiNET;
using System;
using System.Collections.Generic;

namespace SacraSlice.Dependencies.Engine
{
    public enum MessageType
    {
        info, warn, error
    }

    public class DebugLog : ImGuiElement
    {
        public DebugLog()
        {
            Print("Debug ID", MessageType.info, "This is the debug log");
        }

        System.Numerics.Vector4 col;

        static List<(string, string, MessageType, string)> messages = new List<(string, string, MessageType, string)>();
        public override void CustomRender()
        {

            if (showWindow)
            {
                if (!ImGui.Begin("Debug Log", ref showWindow))
                {
                    ImGui.End();
                }
                else
                {
                    if (ImGui.Button("Clear"))
                        messages.Clear();

                    ImGui.Separator();

                    for (int i = messages.Count - 1; i >= 0; i--)
                    {
                        (string, string, MessageType, string) t = messages[i];
                        switch (t.Item3)
                        {
                            case MessageType.error:
                                col = new System.Numerics.Vector4(255, 0, 0, 255);
                                break;
                            case MessageType.warn:
                                col = new System.Numerics.Vector4(255, 255, 0, 255);
                                break;
                            case MessageType.info:
                                col = new System.Numerics.Vector4(0, 255, 0, 255);
                                break;
                        }

                        ImGui.TextColored(col, "[" + t.Item3.ToString() + "]");
                        ImGui.SameLine();
                        ImGui.TextWrapped("[" + t.Item1 + "]");
                        ImGui.SameLine();
                        ImGui.TextWrapped("[" + t.Item2 + "]");
                        ImGui.SameLine();
                        ImGui.TextWrapped("[" + t.Item4 + "]");
                    }


                    ImGui.End();
                }
            }


        }
        public static void Print(object classO, object message)
        {
            Print(classO.GetType().ToString(), MessageType.info, message);
        }
        public static void Print(string classname, object message)
        {
            Print(classname, MessageType.info, message);
        }
        public static void Print(string id, MessageType type, object message)
        {
            messages.Add((DateTime.Now.ToString("yy.MM.dd:HH.mm.ss"), id, type, message.ToString()));
            if (messages.Count > 500)
                messages.Clear();
        }
    }

}
