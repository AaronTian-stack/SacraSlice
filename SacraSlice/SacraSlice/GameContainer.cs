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
using SacraSlice.Dependencies.Engine;
using SacraSlice.Dependencies.Engine.InterfaceLayout;
using SacraSlice.GameCode.Screens;
using System;
using System.Collections.Generic;

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

        private readonly ScreenManager _screenManager;

        public static ImGUIRenderer GuiRenderer; //This is the ImGuiRenderer

        public PlayScreen play;
        public SplashScreen splash;
        public TitleScreen title;

        public GameContainer()
        {
            graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _screenManager = new ScreenManager();
            Components.Add(_screenManager);

        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            graphics.PreferredBackBufferWidth = 1600;
            graphics.PreferredBackBufferHeight = 900;
            graphics.ApplyChanges();

            base.Initialize();

            //play = new PlayScreen(this);
            splash = new SplashScreen(this);
            title = new TitleScreen(this);

            LoadScreen(title, 1f);

            GuiRenderer = new ImGUIRenderer(this).Initialize().RebuildFontAtlas();

            this.Window.AllowUserResizing = true;

            IsFixedTimeStep = false;

        }

        public void LoadScreen(GameScreen screen, float duration)
        { 
            _screenManager.LoadScreen(screen, new FadeTransition(GraphicsDevice, Color.Black, duration));
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            sounds = ContentLoader.LoadListContent<SoundEffect>(this.Content, "Sounds");
            songs = ContentLoader.LoadListContent<Song>(this.Content, "Music");

            atlasTexture = Content.Load<Texture2D>("Sprites/Sprites");

            atlas = new TextureAtlas(atlasTexture, "Sprites/Sprites.atlas");

            TextDrawer.AddFont("Main Font", Content.Load<BitmapFont>("Fonts/JollyGoodSans"));
            TextDrawer.sb = _spriteBatch;
            TextDrawer.gd = GraphicsDevice;
            SpriteAligner.sb = _spriteBatch;
            SpriteAligner.gd = GraphicsDevice;
        }

        public static float alpha;

        public static GameTime gameTime;
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            GameContainer.gameTime = gameTime;

            base.Update(gameTime);

        }
        int oldX, oldY;
        protected override void Draw(GameTime gameTime)
        {
            if(play != null) GameContainer.alpha = play.alpha;
            base.Draw(gameTime);

            if (KeyboardExtended.GetState().WasKeyJustDown(Keys.F))
            {
                FullScreen(); 
            }
        }
        public void FullScreen()
        {
            graphics.IsFullScreen = !graphics.IsFullScreen;

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
