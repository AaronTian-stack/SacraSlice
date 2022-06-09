using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace SacraSlice.Dependencies.Engine.Animation
{
    public enum PlayMode { NORMAL, REVERSED, LOOP, LOOP_REVERSED, LOOP_RANDOM }
    public class Animation<T>
    {

        private List<T> keyFrames; // internal array
        private float frameDuration;
        private float animationDuration;
        private int lastFrameNumber;
        private float lastStateTime;

        private PlayMode playMode = PlayMode.NORMAL;
        public string name;

        public Animation(string name, float frameDuration, List<T> keyframes, PlayMode playMode)
        {
            this.name = name;

            this.frameDuration = frameDuration;
            this.playMode = playMode;

            this.keyFrames = keyframes;

            if(keyFrames != null)
                this.animationDuration = keyFrames.Count * frameDuration;

        }

        Random rnd = new Random();
        public int getKeyFrameIndex(float stateTime)
        {
            if (keyFrames.Count == 1) return 0;

            int frameNumber = (int)(stateTime / frameDuration);
            switch (playMode)
            {
                case PlayMode.NORMAL:
                    frameNumber = MathHelper.Min(keyFrames.Count - 1, frameNumber);
                    break;
                case PlayMode.LOOP:
                    frameNumber = frameNumber % keyFrames.Count;
                    break;
                case PlayMode.REVERSED:
                    frameNumber = MathHelper.Max(keyFrames.Count - frameNumber - 1, 0);
                    break;
                case PlayMode.LOOP_REVERSED:
                    frameNumber = frameNumber % keyFrames.Count;
                    frameNumber = keyFrames.Count - frameNumber - 1;
                    break;
                case PlayMode.LOOP_RANDOM:
                    int lastFrameNumber = (int)((lastStateTime) / frameDuration);
                    if (lastFrameNumber != frameNumber)
                    {
                        frameNumber = rnd.Next(keyFrames.Count - 1);
                    }
                    else
                    {
                        frameNumber = this.lastFrameNumber;
                    }
                    break;
            }

            lastFrameNumber = frameNumber;
            lastStateTime = stateTime;

            return frameNumber;
        }

        public T getKeyFrame(float stateTime)
        {
            int frameNumber = getKeyFrameIndex(stateTime);
            return keyFrames[frameNumber];
        }

        public bool isAnimationFinished(float stateTime)
        {
            int frameNumber = (int)(stateTime / frameDuration);
            return keyFrames.Count - 1 < frameNumber;
        }

        public void setFrameDuration(float frameDuration)
        {
            this.frameDuration = frameDuration;
            this.animationDuration = keyFrames.Count * frameDuration;
        }

        public PlayMode PlayMode1 { get => playMode; set => playMode = value; }
        public float AnimationDuration { get => animationDuration; set => animationDuration = value; }
        public int LastFrameNumber { get => lastFrameNumber; set => lastFrameNumber = value; }
        public float LastStateTime { get => lastStateTime; set => lastStateTime = value; }
        public List<T> KeyFrames 
        { 
            get => keyFrames; 
            set 
            { 
                keyFrames = value; 
                this.animationDuration = keyFrames.Count * frameDuration; 
            } 
        }

        public override string ToString()
        {
            return name;
        }

    }
}
