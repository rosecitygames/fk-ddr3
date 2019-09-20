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

        int sortedParticleCount = 0;

        ParticleSystem.Particle[] particles = null;
        Vector2 particleScale = Vector2.one;

        List<Vector4> customDatas = new List<Vector4>();

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
            sortedParticleCount = spriteExploder.GetMaxParticleCount();
            particles = new ParticleSystem.Particle[sortedParticleCount];
            QuickSortParticles();
            particleScale = spriteExploder.GetParticleScale();
        }

        void LateUpdate()
        {
            if (sortedParticleCount <= 0) return;

            int particleCount = spriteExploder.GetParticleCount();

            int destroyedCount = sortedParticleCount - particleCount;

            if (destroyedCount > 0)
            {
                for (int i = 0; i < destroyedCount; i++)
                {
                    ParticleSystem.Particle destroyedParticle = particles[i];
                    int destroyedTileIndex = (int)customDatas[i].x;
                    GameObject instance = Instantiate(prefab, transform.parent);
                    instance.name = prefab.name + destroyedTileIndex;
                    instance.transform.localPosition = destroyedParticle.position;
                    instance.transform.localScale = particleScale;
                    instance.transform.localEulerAngles = new Vector3(0, 0, destroyedParticle.rotation);
                }
            }

            QuickSortParticles();
        }
        
        void QuickSortParticles()
        {
            sortedParticleCount = spriteExploder.GetParticles(particles);

            if (sortedParticleCount > 0)
            {
                spriteExploder.GetCustomParticleData(customDatas);
                QuickSortParticles_Sub(0, sortedParticleCount - 1);
            }

        }
        
        void QuickSortParticles_Sub(int left, int right)
        {
            IComparer<float> comparer = Comparer<float>.Default;
            int i = left, j = right;
            ParticleSystem.Particle pivot = particles[(left + right) / 2];
            while (i <= j)
            {
                while (comparer.Compare(particles[i].remainingLifetime, pivot.remainingLifetime) < 0)
                {
                    i++;
                }
                while (comparer.Compare(particles[j].remainingLifetime, pivot.remainingLifetime) > 0)
                {
                    j--;
                }
                if (i <= j)
                {
                    ParticleSystem.Particle tempParticle = particles[i];
                    particles[i] = particles[j];
                    particles[j] = tempParticle;

                    Vector4 tempCustomData = customDatas[i];
                    customDatas[i] = customDatas[j];
                    customDatas[j] = tempCustomData;

                    i++;
                    j--;
                }
            }
            if (left < j)
            {
                QuickSortParticles_Sub(left, j);
            }
            if (i < right)
            {
                QuickSortParticles_Sub(i, right);
            }
        }
    }
}

