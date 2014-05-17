using Cannonball.Engine.Graphics.Camera;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cannonball.Engine.Graphics.Particles
{
    public class ParticleSystem
    {
        ParticleSettings settings;
        Effect particleEffect;

        ICamera camera;
        List<ParticleEmitter> emitters = new List<ParticleEmitter>();

        EffectParameter effectViewParameter;
        EffectParameter effectProjectionParameter;
        EffectParameter effectViewportScaleParameter;
        EffectParameter effectTimeParameter;

        ParticleVertex[] particles;

        DynamicVertexBuffer vertexBuffer;
        IndexBuffer indexBuffer;

        GraphicsDevice device;

        int firstActiveParticle;
        int firstNewParticle;
        int firstFreeParticle;
        int firstRetiredParticle;
        float currentTime;

        int drawCounter;

        Random rand;

        public ParticleSystem(GraphicsDevice device, ParticleSettings settings, Texture2D texture, Effect effect, ICamera camera)
        {
            this.settings = settings;
            this.device = device;
            this.particleEffect = effect;
            this.camera = camera;

            rand = new Random(settings.Seed);

            EffectParameterCollection parameters = particleEffect.Parameters;

            effectViewParameter = parameters["View"];
            effectProjectionParameter = parameters["Projection"];
            effectViewportScaleParameter = parameters["ViewportScale"];
            effectTimeParameter = parameters["CurrentTime"];

            parameters["Duration"].SetValue((float)settings.Duration.TotalSeconds);
            parameters["DurationRandomness"].SetValue(settings.DurationRandomness);
            parameters["Gravity"].SetValue(settings.Gravity);
            parameters["EndVelocity"].SetValue(settings.EndVelocity);
            parameters["MinColor"].SetValue(settings.MinColor.ToVector4());
            parameters["MaxColor"].SetValue(settings.MaxColor.ToVector4());

            parameters["RotateSpeed"].SetValue(new Vector2(settings.MinRotateSpeed, settings.MaxRotateSpeed));
            parameters["StartSize"].SetValue(new Vector2(settings.MinStartSize, settings.MaxStartSize));
            parameters["EndSize"].SetValue(new Vector2(settings.MinEndSize, settings.MaxEndSize));

            parameters["Texture"].SetValue(texture);

            particles = new ParticleVertex[settings.MaxParticles * 4];

            for (int i = 0; i < settings.MaxParticles; i++)
            {
                particles[i * 4 + 0].Corner = new Vector2(-1, -1);
                particles[i * 4 + 1].Corner = new Vector2(1, -1);
                particles[i * 4 + 2].Corner = new Vector2(1, 1);
                particles[i * 4 + 3].Corner = new Vector2(-1, 1);
            }

            // Create a dynamic vertex buffer.
            vertexBuffer = new DynamicVertexBuffer(device, ParticleVertex.VertexDeclaration,
                                                   settings.MaxParticles * 4, BufferUsage.WriteOnly);

            ushort[] indices = new ushort[settings.MaxParticles * 6];

            for (int i = 0; i < settings.MaxParticles; i++)
            {
                indices[i * 6 + 0] = (ushort)(i * 4 + 0);
                indices[i * 6 + 1] = (ushort)(i * 4 + 1);
                indices[i * 6 + 2] = (ushort)(i * 4 + 2);

                indices[i * 6 + 3] = (ushort)(i * 4 + 0);
                indices[i * 6 + 4] = (ushort)(i * 4 + 2);
                indices[i * 6 + 5] = (ushort)(i * 4 + 3);
            }

            indexBuffer = new IndexBuffer(device, typeof(ushort), indices.Length, BufferUsage.WriteOnly);

            indexBuffer.SetData(indices);
        }

        public void Update(GameTime gameTime)
        {
            foreach (var emitter in emitters)
            {
                emitter.Update(gameTime);
            }

            currentTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

            RetireActiveParticles();
            FreeRetiredParticles();

            if (firstActiveParticle == firstFreeParticle)
                currentTime = 0;

            if (firstRetiredParticle == firstActiveParticle)
                drawCounter = 0;
        }

        void RetireActiveParticles()
        {
            float particleDuration = (float)settings.Duration.TotalSeconds;

            while (firstActiveParticle != firstNewParticle)
            {
                float particleAge = currentTime - particles[firstActiveParticle * 4].Time;

                if (particleAge < particleDuration)
                    break;

                // Remember the time at which we retired this particle.
                particles[firstActiveParticle * 4].Time = drawCounter;

                firstActiveParticle++;

                if (firstActiveParticle >= settings.MaxParticles)
                    firstActiveParticle = 0;
            }
        }

        void FreeRetiredParticles()
        {
            while (firstRetiredParticle != firstActiveParticle)
            {
                int age = drawCounter - (int)particles[firstRetiredParticle * 4].Time;

                if (age < 3)
                    break;

                firstRetiredParticle++;

                if (firstRetiredParticle >= settings.MaxParticles)
                    firstRetiredParticle = 0;
            }
        }

        public void Draw(GameTime gameTime)
        {
            if (vertexBuffer.IsContentLost)
            {
                vertexBuffer.SetData(particles);
            }

            if (firstNewParticle != firstFreeParticle)
            {
                AddNewParticlesToVertexBuffer();
            }

            if (firstActiveParticle != firstFreeParticle)
            {
                device.BlendState = settings.BlendState;
                device.DepthStencilState = DepthStencilState.DepthRead;

                effectViewportScaleParameter.SetValue(new Vector2(0.5f / device.Viewport.AspectRatio, -0.5f));

                effectTimeParameter.SetValue(currentTime);
                effectViewParameter.SetValue(camera.ViewMatrix);
                effectProjectionParameter.SetValue(camera.ProjectionMatrix);

                device.SetVertexBuffer(vertexBuffer);
                device.Indices = indexBuffer;

                foreach (EffectPass pass in particleEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    if (firstActiveParticle < firstFreeParticle)
                    {
                        device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0,
                                                     firstActiveParticle * 4, (firstFreeParticle - firstActiveParticle) * 4,
                                                     firstActiveParticle * 6, (firstFreeParticle - firstActiveParticle) * 2);
                    }
                    else
                    {
                        device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0,
                                                     firstActiveParticle * 4, (settings.MaxParticles - firstActiveParticle) * 4,
                                                     firstActiveParticle * 6, (settings.MaxParticles - firstActiveParticle) * 2);

                        if (firstFreeParticle > 0)
                        {
                            device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0,
                                                         0, firstFreeParticle * 4,
                                                         0, firstFreeParticle * 2);
                        }
                    }
                }

                device.DepthStencilState = DepthStencilState.Default;
            }

            drawCounter++;
        }

        void AddNewParticlesToVertexBuffer()
        {
            int stride = ParticleVertex.SizeInBytes;

            if (firstNewParticle < firstFreeParticle)
            {
                vertexBuffer.SetData(firstNewParticle * stride * 4, particles,
                                     firstNewParticle * 4,
                                     (firstFreeParticle - firstNewParticle) * 4,
                                     stride, SetDataOptions.NoOverwrite);
            }
            else
            {
                vertexBuffer.SetData(firstNewParticle * stride * 4, particles,
                                     firstNewParticle * 4,
                                     (settings.MaxParticles - firstNewParticle) * 4,
                                     stride, SetDataOptions.NoOverwrite);

                if (firstFreeParticle > 0)
                {
                    vertexBuffer.SetData(0, particles,
                                         0, firstFreeParticle * 4,
                                         stride, SetDataOptions.NoOverwrite);
                }
            }

            firstNewParticle = firstFreeParticle;
        }

        public void AddParticle(Vector3 position, Vector3 velocity)
        {
            int nextFreeParticle = firstFreeParticle + 1;

            if (nextFreeParticle >= settings.MaxParticles)
                nextFreeParticle = 0;

            if (nextFreeParticle == firstRetiredParticle)
                return;

            velocity *= settings.EmitterVelocitySensitivity;

            velocity.X += MathHelper.Lerp(settings.MinXVelocity,
                                          settings.MaxXVelocity,
                                          (float)rand.NextDouble());
            velocity.Y += MathHelper.Lerp(settings.MinYVelocity,
                                          settings.MaxYVelocity,
                                          (float)rand.NextDouble());
            velocity.Z += MathHelper.Lerp(settings.MinZVelocity,
                                          settings.MaxZVelocity,
                                          (float)rand.NextDouble());

            Color randomValues = new Color((byte)rand.Next(255),
                                           (byte)rand.Next(255),
                                           (byte)rand.Next(255),
                                           (byte)rand.Next(255));

            for (int i = 0; i < 4; i++)
            {
                particles[firstFreeParticle * 4 + i].Position = position;
                particles[firstFreeParticle * 4 + i].Velocity = velocity;
                particles[firstFreeParticle * 4 + i].Random = randomValues;
                particles[firstFreeParticle * 4 + i].Time = currentTime;
            }

            firstFreeParticle = nextFreeParticle;
        }

        public void AddEmitter(ParticleEmitter emitter)
        {
            emitters.Add(emitter);
        }
    }
}