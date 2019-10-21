using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace IndieDevTools.AiParticles
{
    public class ExplosionSpawner : MonoBehaviour, ISpawnable
    {
        public SpriteExploder.SpriteExploder SpriteExploder = null;

        public GameObject Prefab = null;

        public Transform InstanceParent = null;

        public Action<GameObject> OnInstanceCreated;

        public Action OnCompleted;

        public int MaxInstanceCount => SpriteExploder.GetMaxParticleCount();

        public int InstanceCount => sortedParticleCount;

        public bool IsSpawningParticleColor = true;

        ParticleSystem spriteExploderParticleSystem = null;

        Color32 instanceColor = new Color32();

        int sortedParticleCount = 0;

        ParticleSystem.Particle[] particles = null;
        Vector2 particleScale = Vector2.one;

        List<Vector4> customDatas = new List<Vector4>();

        int pixelSize = 0;
        int centerOffset = 0;

        int tileCountX = 0;
        int tileCountY = 0;

        void Awake()
        {
            spriteExploderParticleSystem = SpriteExploder.GetParticleSystem();
            AddEventHandlers();
        }

        void AddEventHandlers()
        {
            RemoveEventHandlers();

            SpriteExploder.OnExploded += SpriteExploder_OnExploded;
        }

        void RemoveEventHandlers()
        {
            SpriteExploder.OnExploded -= SpriteExploder_OnExploded;
        }

        void SpriteExploder_OnExploded()
        {
            RemoveEventHandlers();

            sortedParticleCount = SpriteExploder.GetMaxParticleCount();
            if (sortedParticleCount <= 1)
            {
                sortedParticleCount = 0;
                OnCompleted?.Invoke();
                return;
            }

            instanceColor = GetComponent<SpriteRenderer>().color;

            particleScale = SpriteExploder.GetParticleScale();

            particles = new ParticleSystem.Particle[sortedParticleCount];  
            QuickSortParticles();

            tileCountX = SpriteExploder.GetSubdivisionCountX();
            tileCountY = SpriteExploder.GetSubdivisionCountY();

            Texture2D texture = SpriteExploder.GetTexture();

            if (tileCountX < tileCountY)
            {
                pixelSize = texture.width / tileCountX;
            }
            else
            {
                pixelSize = texture.height / tileCountY;
            }

            centerOffset = pixelSize / 2;
        }

        void LateUpdate()
        {
            if (sortedParticleCount <= 0) return;

            int particleCount = SpriteExploder.GetParticleCount();
            int destroyedCount = sortedParticleCount - particleCount;

            if (destroyedCount > 0)
            {
                Vector2 flipVector = SpriteExploder.GetFlipVector2();

                Texture2D texture = SpriteExploder.GetTexture();

                for (int i = 0; i < destroyedCount; i++)
                {
                    int tileIndex = (int)customDatas[i].x;

                    int tileX = SpriteExploder.GetTileX(tileIndex, tileCountX);
                    if (flipVector.x < 0)
                    {
                        tileX = tileCountX - tileX;
                    }

                    int tileY = SpriteExploder.GetTileY(tileIndex, tileCountY);
                    if (flipVector.y < 0)
                    {
                        tileY = tileCountY - tileY;
                    }

                    int tileCenterPixelX = tileX * pixelSize + centerOffset;
                    int tileCenterPixelY = tileY * pixelSize + centerOffset;

                    Color color = texture.GetPixel(tileCenterPixelX, tileCenterPixelY);

                    if (color.a <= 0.0f) continue;

                    ParticleSystem.Particle particle = particles[i];

                    Color32 spawnColor;
                    if (IsSpawningParticleColor)
                    {
                        spawnColor = particle.GetCurrentColor(spriteExploderParticleSystem);
                    }
                    else
                    {
                        spawnColor = instanceColor;
                    }

                    GameObject instance = Spawn(particle.position, particleScale, particle.rotation, particle.velocity, particle.angularVelocity, spawnColor);
                    OnInstanceCreated?.Invoke(instance);
                }
            }

            QuickSortParticles();

            if (sortedParticleCount <= 0)
            {
                OnCompleted?.Invoke();
            }
        }

        public GameObject Spawn(Vector3 position, Vector3 scale, float rotation = 0.0f)
        {
            instanceColor = GetComponent<SpriteRenderer>().color;
            return Spawn(position, scale, rotation, Vector2.zero, 0.0f, instanceColor);
        }

        public GameObject Spawn(Vector3 position, Vector3 scale, float rotation, Vector2 velocity, float angularVelocity, Color color)
        {
            GameObject instance = Instantiate(Prefab, InstanceParent);
            instance.name = Prefab.name;
            instance.transform.localPosition = position;
            instance.transform.localScale = scale;
            instance.transform.localEulerAngles = new Vector3(0, 0, rotation);

            SpriteRenderer spriteRenderer = instance.GetComponentInChildren<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.color = color;
            }

            ParticleSystem particleSystem = instance.GetComponentInChildren<ParticleSystem>();
            if (particleSystem != null)
            {
                ParticleSystem.MainModule particleSystemMain = particleSystem.main;
                particleSystemMain.startColor = spriteExploderParticleSystem.main.startColor;
            }

            Rigidbody2D rigidbody2D = instance.GetComponentInChildren<Rigidbody2D>();
            if (rigidbody2D != null)
            {
                rigidbody2D.velocity = velocity;
                rigidbody2D.angularVelocity = angularVelocity;
            }

            return instance;
        }
        
        void QuickSortParticles()
        {
            sortedParticleCount = SpriteExploder.GetParticles(particles);

            if (sortedParticleCount > 0)
            {
                SpriteExploder.GetCustomParticleData(customDatas);
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

