using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace SacraSlice.Dependencies.Engine
{
    public class TextureAtlas
    {

        Texture2D texture;

        Dictionary<string, TextureRegion> table = new Dictionary<string, TextureRegion>();

        Dictionary<string, List<TextureRegion>> animations = new Dictionary<string, List<TextureRegion>>();

        List<string> names = new List<string>();

        public TextureAtlas(Texture2D tex, string path) // path goes from Content folder
        {
            Initialize(tex, path);
        }

        TextureRegion t;
        public TextureRegion FindRegion(string name)
        {
            if (table.TryGetValue(name, out t))
                return t;
            else
                throw new KeyNotFoundException($"Could not find texture region '{name}' in atlas");
        }

        List<TextureRegion> g;
        public List<TextureRegion> FindRegions(string name)
        {
            if (animations.TryGetValue(name, out g))
                return g;
            else
                throw new KeyNotFoundException($"Could not find animation '{name}' in atlas");
        }

        // This uses like 60 mb of memory but whatever
        public void Initialize(Texture2D tex, string path)
        {
            texture = tex;

            // parse .ATLAS file and create table

            StreamReader read = new StreamReader(@"Content/" + path);
            char[] delimiterChars = { ' ', ',', '\t' };

            string animationName = null;
            string lastName = null;

            List<TextureRegion> add = new List<TextureRegion>();

            for (int i = 0; i < 6; i++) read.ReadLine(); // skip 6 lines

            while (!read.EndOfStream)
            {
                string name = read.ReadLine(); // name

                read.ReadLine(); // rotation

                string[] coor = read.ReadLine().Split(delimiterChars); // xy

                string[] size = read.ReadLine().Split(delimiterChars); // size

                read.ReadLine(); // origin
                read.ReadLine(); // offset

                string[] ind = read.ReadLine().Split(delimiterChars); // index

                int leftX = int.Parse(coor[3]);

                int leftY = int.Parse(coor[5]);

                int width = int.Parse(size[3]);

                int height = int.Parse(size[5]);

                int index = int.Parse(ind[3]);

                if (index != -1)
                {

                    if (!(animationName is null) && !lastName.Equals(name))
                    {
                        // this is a new animation
                        AddAnimation(add, animationName);
                    }
                    animationName = name;
                    // add the animation
                    TextureRegion t = new TextureRegion(tex, new Rectangle(leftX, leftY, width, height));
                    t.index = index;
                    table.Add(name + "_" + index, t);
                    add.Add(t);
                }
                else
                {
                    // if it is not an animation
                    TextureRegion t = new TextureRegion(tex, new Rectangle(leftX, leftY, width, height));
                    table.Add(name, t);
                    t.index = index;
                    if (add.Count > 0) // this has to be here??
                        AddAnimation(add, animationName);
                }

                lastName = name;

            }

            read.Close();

            read.Dispose();

        }

        private void AddAnimation(List<TextureRegion> add, string animationName)
        {

            if (animations.ContainsKey(animationName)) return;

            //Debug.WriteLine("Adding animation " + animationName);aaa

            add.Sort((x, y) => x.index.CompareTo(y.index));
            animations.Add(animationName, new List<TextureRegion>(add));
            add.Clear();

        }

        [Obsolete("This is deprecated, please use GDX format instead.")]
        public void OldInitialize(Texture2D tex, string path)
        {
            texture = tex;

            // parse .ATLAS file and create table

            StreamReader read = new StreamReader(@"Content/" + path);

            char[] delimiterChars = { ',', '\t' };

            while (!read.EndOfStream)
            {
                string name = read.ReadLine();

                names.Add(name);

                if (name.Equals("")) // start animations
                    break;

                string[] coor = read.ReadLine().Split(delimiterChars);

                int leftX = int.Parse(coor[1]);

                int leftY = int.Parse(coor[2]);

                int width = int.Parse(coor[3]);

                int height = int.Parse(coor[4]);

                table.Add(name, new TextureRegion(tex, new Rectangle(leftX, leftY, width, height)));

                read.ReadLine();

            }

            while (!read.EndOfStream)
            {
                string name = read.ReadLine(); // animation name

                if (name.Equals(""))
                    break;

                read.ReadLine();

                List<TextureRegion> add = new List<TextureRegion>();

                string[] coor = read.ReadLine().Split(delimiterChars);

                for (int i = 1; i < coor.Length; i++)
                {

                    if (table.TryGetValue(names[int.Parse(coor[i])], out t))
                        add.Add(t);
                    else
                        throw new KeyNotFoundException($"Could not find animation '{name}' in atlas");

                }

                animations.Add(name, add);

            }

            read.Close();

            read.Dispose();
        }

    }
}
