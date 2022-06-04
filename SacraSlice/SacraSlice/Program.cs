using System;

namespace SacraSlice
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new GameContainer())
                game.Run();
        }
    }
}
