using Microsoft.Xna.Framework;
using MonoGame.Extended.Collections;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using SacraSlice.Dependencies.Engine;
using System;
using System.Collections.Generic;
using System.Text;

namespace SacraSlice.GameCode.GameECS.GameSystems
{
    public class DropperSystem : EntityUpdateSystem
    {
        private ComponentMapper<string> stringMapper;
        private ComponentMapper<Wrapper<bool>> boolMapper;
        //Bag<Entity> bag;
        Wrapper<int> score;
        public DropperSystem(Wrapper<int> score) : base(Aspect.All(typeof(string), typeof(Wrapper<bool>)))
        {
            //this.bag = bag;
            this.score = score;
        }
        public override void Initialize(IComponentMapperService mapperService)
        {
            stringMapper = mapperService.GetMapper<string>();
            boolMapper = mapperService.GetMapper<Wrapper<bool>>();
        }

        
        public override void Update(GameTime gameTime)
        {
            foreach (var entity in ActiveEntities)
            {


                var s = stringMapper.Get(entity);
                var b = boolMapper.Get(entity);


                if(s.Equals("Dropping Item") && b)
                {
                    // remove the entity
                    score.Value++;                    
                    DestroyEntity(entity);
                    
                }
                


            }
        }
    }
}
