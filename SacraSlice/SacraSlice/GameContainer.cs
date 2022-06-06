using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;
using MonoGame.ImGui;
using SacraSlice.Dependencies.Engine;
using SacraSlice.GameCode.Screens;
using System;

namespace SacraSlice
{
    public class GameContainer : Game
    {
        private GraphicsDeviceManager graphics;
        public static SpriteBatch _spriteBatch;
        private Texture2D atlasTexture;

        public static TextureAtlas atlas;

        private readonly ScreenManager _screenManager;

        public static ImGUIRenderer GuiRenderer; //This is the ImGuiRenderer

        public static int fps;

        PlayScreen play;

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



            play = new PlayScreen(this);

            _screenManager.LoadScreen(play, new FadeTransition(GraphicsDevice, Color.Black, 0f));

            GuiRenderer = new ImGUIRenderer(this).Initialize().RebuildFontAtlas();

            this.Window.AllowUserResizing = true;
            this.Window.ClientSizeChanged += new EventHandler<EventArgs>(Window_ClientSizeChanged);

            void Window_ClientSizeChanged(object sender, EventArgs e)
            {
                graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
                graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
                graphics.ApplyChanges();
            }

            IsFixedTimeStep = false;

            //TargetElapsedTime = TimeSpan.FromMilliseconds(1000.0f / 120);

            //graphics.SynchronizeWithVerticalRetrace = false;

            //Window.IsBorderless = true;
            //ShapeDrawer.LoadContent(graphics.GraphicsDevice);

        }
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            atlasTexture = Content.Load<Texture2D>("Sprites/Sprites");

            atlas = new TextureAtlas(atlasTexture, "Sprites/Sprites.atlas");

            TextDrawer.AddSpriteFont("LanaPixel72", Content.Load<SpriteFont>("Fonts/Font"));
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

        protected override void Draw(GameTime gameTime)
        {
            GameContainer.alpha = play.alpha;
            base.Draw(gameTime);
        }
    }
}
