using ImGuiNET;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using SacraSlice.Dependencies.Engine.Animation;
using SacraSlice.Dependencies.Engine.ECS.Component;
using SacraSlice.Dependencies.Engine.States;
using System;

namespace SacraSlice.Dependencies.Engine.ECS.Systems
{
    public class ImGuiEntityDraw : EntityDrawSystem // Draws all the ImGui entities
    {

        private ComponentMapper<String> nameMapper;
        private ComponentMapper<Position> positionMapper;
        private ComponentMapper<Sprite> spriteMapper;
        private ComponentMapper<HitBox> hitboxMapper;
        private ComponentMapper<GameCamera> cameraMapper;
        private ComponentMapper<InputManager> inputMapper;
        private ComponentMapper<Timer> timerMapper;
        private ComponentMapper<StateManager> stateMapper;
        private ComponentMapper<AnimationStateManager> animationMapper;
        private ComponentMapper<SquashManager> squashMapper;
        private ComponentMapper<VerletRope> ropeMapper;
        private ComponentMapper<RopeClamper> ropeclamperMapper;
        private bool floatMapper;

        public ImGuiEntityDraw() : base(Aspect.One())
        {

        }

        string name;

        static int selected = -1;

        bool showWindow;
        public override void Draw(GameTime gameTime)
        {
            // loop through all entities
            if (ImGui.CollapsingHeader("Entities"))
            {
                int count = 0;
                ImGui.TextColored(new System.Numerics.Vector4(255, 0, 0, 1), ActiveEntities.Count + " TOTAL ENTITES");
                foreach (var entity in ActiveEntities)
                {

                    if (nameMapper.Get(entity) == null)
                        name = "NO NAME!";
                    else
                        name = nameMapper.Get(entity);

                    if (ImGui.Selectable(name, selected == count))
                    {
                        selected = count;
                        showWindow = true;
                    }


                    if (selected == count) // Open a new window
                    {

                        if (showWindow)
                        {
                            if (!ImGui.Begin(name, ref showWindow))
                            {
                                ImGui.End();
                            }
                            else
                            {
                                //var e = GetEntity(entity);

                                if (positionMapper.Get(entity) != null)
                                    positionMapper.Get(entity).CustomRender();

                                if (spriteMapper.Get(entity) != null)
                                    spriteMapper.Get(entity).CustomRender();

                                if (hitboxMapper.Get(entity) != null)
                                    hitboxMapper.Get(entity).CustomRender();

                                if (cameraMapper.Get(entity) != null)
                                    cameraMapper.Get(entity).CustomRender();

                                if (inputMapper.Get(entity) != null)
                                    inputMapper.Get(entity).CustomRender();

                                if (timerMapper.Get(entity) != null)
                                    timerMapper.Get(entity).CustomRender();

                                if (stateMapper.Get(entity) != null)
                                    stateMapper.Get(entity).CustomRender();

                                if (animationMapper.Get(entity) != null)
                                    animationMapper.Get(entity).CustomRender();

                                if (squashMapper.Get(entity) != null)
                                    squashMapper.Get(entity).CustomRender();

                                if (ropeMapper.Get(entity) != null)
                                    ropeMapper.Get(entity).CustomRender();

                                if (ropeclamperMapper.Get(entity) != null)
                                    ropeclamperMapper.Get(entity).CustomRender();

                                // TODO: Put all other ImGuiElements in here


                                ImGui.End();
                            }
                        }

                    }

                    count++;

                }

            }
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            nameMapper = mapperService.GetMapper<string>();
            positionMapper = mapperService.GetMapper<Position>();
            spriteMapper = mapperService.GetMapper<Sprite>();
            hitboxMapper = mapperService.GetMapper<HitBox>();
            cameraMapper = mapperService.GetMapper<GameCamera>();
            inputMapper = mapperService.GetMapper<InputManager>();
            timerMapper = mapperService.GetMapper<Timer>();
            stateMapper = mapperService.GetMapper<StateManager>();
            animationMapper = mapperService.GetMapper<AnimationStateManager>();
            squashMapper = mapperService.GetMapper<SquashManager>();
            ropeMapper = mapperService.GetMapper<VerletRope>();
            ropeclamperMapper = mapperService.GetMapper<RopeClamper>();
        }
    }
}
