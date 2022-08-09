using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Input;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;
using MonoGame.ImGui;
using MonoGameSaveManager;
using SacraSlice.Dependencies.Engine;
using SacraSlice.Dependencies.Engine.InterfaceLayout;
using SacraSlice.Dependencies.Engine.Scene;
using SacraSlice.GameCode.Screens;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace SacraSlice
{
    public class GameContainer : Game
    {
        private GraphicsDeviceManager graphics;
        public static SpriteBatch _spriteBatch;
        private Texture2D atlasTexture;

        public static TextureAtlas atlas;
        public static Dictionary<string, SoundEffect> sounds;
        public static Dictionary<string, Song> songs;

        public readonly ScreenManager _screenManager;

        public static ImGUIRenderer GuiRenderer; //This is the ImGuiRenderer

        public PlayScreen play;
        public SplashScreen splash;
        public TitleScreen title;
        public BlankScreen blank;

        public GameContainer()
        {
            graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _screenManager = new ScreenManager();
            Components.Add(_screenManager);
            graphics.HardwareModeSwitch = false;
        }

        protected override void Initialize()
        {

            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
            graphics.ApplyChanges();

            base.Initialize();

            play = new PlayScreen(this);
            splash = new SplashScreen(this);
            title = new TitleScreen(this);
            blank = new BlankScreen(this);

            LoadScreen(splash, 1f);

            //GuiRenderer = new ImGUIRenderer(this).Initialize().RebuildFontAtlas();

            this.Window.AllowUserResizing = true;

            IsFixedTimeStep = false;

            SaveManager mySave = new IsolatedStorageSaveManager("SacraSlice", "mysave.dat");
            mySave.Load();
            if (mySave.Data.fullscreen)
                FullScreen();

        }

        public void LoadScreen(GameScreen screen, float duration)
        {
            _screenManager.LoadScreen(screen, new FadeTransitionFixed(GraphicsDevice, Color.Black, duration));
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            sounds = ContentLoader.LoadListContent<SoundEffect>(this.Content, "Sounds");
            songs = ContentLoader.LoadListContent<Song>(this.Content, "Music");

            var macos = "";

            if (Environment.OSVersion.Platform == PlatformID.Unix) // MAC OS
                macos = System.AppDomain.CurrentDomain.BaseDirectory;


            atlasTexture = Content.Load<Texture2D>(macos + "Sprites/Sprites");

            atlas = new TextureAtlas(atlasTexture, macos + "Sprites/Sprites.atlas");

            TextDrawer.AddFont("Main Font", Content.Load<BitmapFont>(macos + "Fonts/JollyGoodSans"));
            TextDrawer.AddFont("Title Font", Content.Load<BitmapFont>(macos + "Fonts/FFFFoward"));
            TextDrawer.sb = _spriteBatch;
            TextDrawer.gd = GraphicsDevice;
            SpriteAligner.sb = _spriteBatch;
            SpriteAligner.gd = GraphicsDevice;
            ShapeAligner.gd = GraphicsDevice;
        }

        public static float alpha;

        public static GameTime gameTime;
        protected override void Update(GameTime gameTime)
        {
           
            GameContainer.gameTime = gameTime;

            base.Update(gameTime);


        }
        int oldX, oldY;
        protected override void Draw(GameTime gameTime)
        {
            if(play != null) GameContainer.alpha = play.alpha;
            base.Draw(gameTime);
           
            if (KeyboardExtended.GetState().WasKeyJustDown(Keys.F) && !_screenManager.Visible)
            {
                FullScreen(); 
            }
        }
        public void FullScreen()
        {
            graphics.IsFullScreen = !graphics.IsFullScreen;

            SaveManager mySave = new IsolatedStorageSaveManager("SacraSlice", "mysave.dat");
            mySave.Load();
            mySave.Data.fullscreen = graphics.IsFullScreen;
            mySave.Save();

            if (graphics.IsFullScreen)
            {
                oldX = Window.ClientBounds.Width;
                oldY = Window.ClientBounds.Height;
                graphics.PreferredBackBufferWidth =
               GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                graphics.PreferredBackBufferHeight =
                    GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            }
            else
            {
                graphics.PreferredBackBufferWidth = oldX;
                graphics.PreferredBackBufferHeight = oldY;
            }

            graphics.ApplyChanges();
        }
    }
}
