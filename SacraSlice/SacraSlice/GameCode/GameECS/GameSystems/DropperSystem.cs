using Microsoft.Xna.Framework;
using MonoGame.Extended.Collections;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using SacraSlice.Dependencies.Engine;
using SacraSlice.Dependencies.Engine.ECS.Component;
using System;
using System.Collections.Generic;
using System.Text;

namespace SacraSlice.GameCode.GameECS.GameSystems
{
    public class DropperSystem : EntityUpdateSystem
    {
        private ComponentMapper<string> stringMapper;
        private ComponentMapper<Wrapper<bool>> boolMapper;
        private ComponentMapper<Position> positionMapper;
        Wrapper<int> score;
        public DropperSystem(Wrapper<int> score) : base(Aspect.All(typeof(string), typeof(Wrapper<bool>),
            typeof(Position)))
        {
            this.score = score;
        }
        public override void Initialize(IComponentMapperService mapperService)
        {
            stringMapper = mapperService.GetMapper<string>();
            boolMapper = mapperService.GetMapper<Wrapper<bool>>();
            positionMapper = mapperService.GetMapper<Position>();
        }

        
        public override void Update(GameTime gameTime)
        {
            foreach (var entity in ActiveEntities)
            {


                var s = stringMapper.Get(entity);
                var b = boolMapper.Get(entity);
                var p = positionMapper.Get(entity);

                if(s.Equals("Dropping Item") && b)
                {
                    // reset
                    b.Value = false;
                    
                }
                


            }
        }
    }
}
