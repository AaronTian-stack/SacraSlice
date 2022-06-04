using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using SacraSlice.Dependencies.Engine.ECS.Component;
using System;
using System.Collections.Generic;
using System.Text;

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

                // TODO: Check for camera actions

                // Interpolate to the current position and zoom on top of the stack

                Position p;
                Interpolation i;
                float speed, zoom;

                if (camera.targets.TryPeek(out p) && camera.interpolationP.TryPeek(out i)
                    && camera.speedP.TryPeek(out speed))
                {
                    float x = i.apply(camera.Position.X, p.renderPosition.X, speed);
                    float y = i.apply(camera.Position.Y, p.renderPosition.Y, speed);

                    camera.Position = new Vector2(x, y);
                }


                if (camera.targetZooms.TryPeek(out zoom) && camera.interpolationZ.TryPeek(out i))
                {
                    camera.Zoom = i.apply(camera.Zoom, zoom, camera.speedZ.Peek());
                }

            }
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            cameraMapper = mapperService.GetMapper<GameCamera>();
        }

    }
}
