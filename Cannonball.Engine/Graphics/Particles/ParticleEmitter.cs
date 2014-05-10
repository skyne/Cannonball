using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cannonball.Engine.Graphics.Particles
{
    public class ParticleEmitter
    {
        public Vector3 Position
        {
            get { return actPosition; }
            set
            {
                previousPosition = actPosition;
                actPosition = value;
            }
        }

        public float ParticlesPerSecond
        {
            get { return (1.0f / timeBetweenParticles); }
            set { timeBetweenParticles = 1.0f / value; }
        }

        Vector3 actPosition;
        Vector3 previousPosition;
        float timeBetweenParticles;
        float timeLeftOver;
        ParticleSystem system;

        public ParticleEmitter(ParticleSystem system)
        {
            this.system = system;
            system.AddEmitter(this);
        }

        public void Update(GameTime gameTime)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (elapsedTime > 0)
            {
                Vector3 velocity = (Position - previousPosition) / elapsedTime;

                float timeToSpend = timeLeftOver + elapsedTime;

                float currentTime = -timeLeftOver;

                while (timeToSpend > timeBetweenParticles)
                {
                    currentTime += timeBetweenParticles;
                    timeToSpend -= timeBetweenParticles;

                    float mu = currentTime / elapsedTime;

                    Vector3 position = Vector3.Lerp(previousPosition, Position, mu);

                    system.AddParticle(position, velocity);
                }

                timeLeftOver = timeToSpend;
            }

            previousPosition = Position;
        }
    }
}