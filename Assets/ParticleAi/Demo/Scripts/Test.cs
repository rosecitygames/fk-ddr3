using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
namespace IndieDevTools.AiParticles
{
    public class Test : MonoBehaviour
    {
        [SerializeField]
        SpriteExploder.SpriteExploder spriteExploder = null;

        [SerializeField]
        GameObject prefab = null;

        int lastParticleCount = 0;

        List<ParticleSystem.Particle> lastParticles = null;
        ParticleSystem.Particle[] particles = null;
        Vector2 particleScale = Vector2.one;

        void Awake()
        {
            AddEventHandlers();
        }

        void AddEventHandlers()
        {
            RemoveEventHandlers();

            spriteExploder.OnExploded += SpriteExploder_OnExploded;
        }

        void RemoveEventHandlers()
        {
            spriteExploder.OnExploded -= SpriteExploder_OnExploded;
        }

        void SpriteExploder_OnExploded()
        {
            RemoveEventHandlers();
            lastParticleCount = spriteExploder.GetMaxParticleCount();
            particles = new ParticleSystem.Particle[lastParticleCount];
            lastParticles = GetSortedParticles();
            particleScale = spriteExploder.GetParticleScale();
        }

        void LateUpdate()
        {
            if (lastParticleCount <= 0) return;

            int particleCount = spriteExploder.GetParticles(particles);

            int destroyCount = lastParticleCount - particleCount;

            if (destroyCount > 0)
            {
                
                for (int i = 0; i < destroyCount; i++)
                {
                    ParticleSystem.Particle destroyedParticle = lastParticles[i];
                    GameObject instance = Instantiate(prefab, transform.parent);
                    instance.transform.position = destroyedParticle.position;
                    instance.transform.localScale = particleScale;
                }
            }
   
            lastParticleCount = particleCount;
            lastParticles = GetSortedParticles();
        }

        List<ParticleSystem.Particle> GetSortedParticles()
        {
            int particleCount = spriteExploder.GetParticles(particles);
            if (particleCount <= 0) new List<ParticleSystem.Particle>();

            float leastRemainingLifetime = 0.0f;

            List<ParticleSystem.Particle> sortedParticles = new List<ParticleSystem.Particle>();
            for (int i = 0; i < particleCount; i++)
            {
                ParticleSystem.Particle particle = particles[i];
                float remainingLifetime = particle.remainingLifetime;           
                if (i == 0 || remainingLifetime < leastRemainingLifetime)
                {
                    leastRemainingLifetime = remainingLifetime;
                    sortedParticles.Insert(0, particle);
                }
                else
                {
                    sortedParticles.Add(particle);
                }
            }

            return sortedParticles;
        }
    }
}

