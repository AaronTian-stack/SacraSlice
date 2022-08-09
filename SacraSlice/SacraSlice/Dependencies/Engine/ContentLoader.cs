using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace SacraSlice.Dependencies.Engine
{
    public static class ContentLoader
    {
        public static Dictionary<string, T> LoadListContent<T>(this ContentManager contentManager, string contentFolder)
        {
            DirectoryInfo dir;

            if (Environment.OSVersion.Platform == PlatformID.Win32NT) // WINDOWS
                dir = new DirectoryInfo(contentManager.RootDirectory + "/" + contentFolder);
            else // MAC OS or Linux
                dir = new DirectoryInfo(System.AppDomain.CurrentDomain.BaseDirectory
                    + contentManager.RootDirectory + "/" + contentFolder);

            Dictionary<string, T> result = new Dictionary<string, T>();

            FileInfo[] files = dir.GetFiles("*.*");
            foreach (FileInfo file in files)
            {
                string key = Path.GetFileNameWithoutExtension(file.Name);

                result[key] = contentManager.Load<T>(contentFolder + "/" + key);
            }
            return result;
        }
    }
}
