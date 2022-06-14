using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using MonoGame.Extended;
using SacraSlice.Dependencies.Engine.ECS.Component;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace SacraSlice.Dependencies.Engine.ECS.Systems
{
    public class CameraTracker : EntityDrawSystem
    {
        private ComponentMapper<GameCamera> cameraMapper;

        public CameraTracker() : base(Aspect.All(typeof(GameCamera)))
        {

        }

        public override void Draw(GameTime gameTime)
        {
            foreach (var entity in ActiveEntities)
            {
                //var e = GetEntity(entity);

                var camera = cameraMapper.Get(entity);


                camera.Shake(gameTime.GetElapsedSeconds());

                if (Keyboard.GetState().IsKeyDown(Keys.H))
                {
                    camera.AddShake(1f, 0, 0.5f);
                }

                // TODO: Check for camera actions

                // Interpolate to the current position and zoom on top of the stack

                Position p;
                Interpolation i;
                float speed, zoom, rotation;

                if (camera.targets.TryPeek(out p) && camera.interpolationPosition.TryPeek(out i)
                    && camera.speedPosition.TryPeek(out speed))
                {
                    float x = i.Apply(camera.Position.X, p.renderPosition.X, speed);
                    float y = i.Apply(camera.Position.Y, p.renderPosition.Y, speed);

                    camera.Position = new Vector2(x, y);
                }

                if (camera.targetRotations.TryPeek(out rotation) && camera.interpolationRotation.TryPeek(out i)
                    && camera.speedRotation.TryPeek(out speed))
                {
                    camera.Rotation = i.Apply(camera.Rotation, rotation, speed);
                }


                if (camera.targetZooms.TryPeek(out zoom) && camera.interpolationZoom.TryPeek(out i))
                {
                    camera.Zoom = i.Apply(camera.Zoom, zoom, camera.speedZoom.Peek());
                }

            }
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            cameraMapper = mapperService.GetMapper<GameCamera>();
        }

    }
}
