using IndieDevTools.Exploders;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace IndieDevTools.Spawners
{
    /// <summary>
    /// A component that spawns instances of a prefab as exploded tile particles
    /// complete their lifetime.
    /// </summary>
    public class ExplosionSpawner : MonoBehaviour, ISpawnable
    {
        /// <summary>
        /// The Sprite Exploder component used to integrate spawning.
        /// </summary>
        public SpriteExploder SpriteExploder = null;

        /// <summary>
        /// The prefab game object used to spawn instances.
        /// </summary>
        public GameObject Prefab = null;

        /// <summary>
        /// The parent object that spawned instances become children of.
        /// </summary>
        public Transform InstanceParent = null;

        /// <summary>
        /// An action event that gets invoked each time an instance is spawned.
        /// </summary>
        public Action<GameObject> OnInstanceCreated;

        /// <summary>
        /// An action event that gets invoked when all instances have been spawned.
        /// </summary>
        public Action OnCompleted;

        /// <summary>
        /// The number of instances that will be spawned.
        /// </summary>
        public int MaxInstanceCount => SpriteExploder.GetMaxParticleCount();

        /// <summary>
        /// The current number of spawned instances.
        /// </summary>
        public int InstanceCount => MaxInstanceCount - sortedParticleCount;

        /// <summary>
        /// The current number of alive sorted particles.
        /// </summary>
        int sortedParticleCount = 0;

        /// <summary>
        /// A reference to the sprite exploder component's particle system.
        /// </summary>
        ParticleSystem spriteExploderParticleSystem = null;

        /// <summary>
        /// An array of particles that are alive in the particle system.
        /// </summary>
        ParticleSystem.Particle[] particles = null;

        /// <summary>
        /// A reference to the tile particle scale.
        /// </summary>
        Vector2 particleScale = Vector2.one;

        /// <summary>
        /// The stream of custom per-particle data.
        /// </summary>
        List<Vector4> customDatas = new List<Vector4>();

        /// <summary>
        /// A reference to the pixel size of the sprite exploder tiles.
        /// </summary>
        int pixelSize = 0;

        /// <summary>
        /// A reference to the pixel offset value to reach the center of a sprite
        /// exploder tile.
        /// </summary>
        int centerOffset = 0;

        /// <summary>
        /// The number of columns in tile grid of the sprite exploder.
        /// </summary>
        int tileCountX = 0;

        /// <summary>
        /// The number of rows in the tile grid of the sprite exploder.
        /// </summary>
        int tileCountY = 0;

        void Awake() => Init();
        /// <summary>
        /// Initializes the component by setting the particle system reference property
        /// and adding event handlers.
        /// </summary>
        void Init()
        {
            spriteExploderParticleSystem = SpriteExploder.GetParticleSystem();
            AddEventHandlers();
        }

        /// <summary>
        /// Adds event handlers for the sprite exploder explosion.
        /// </summary>
        void AddEventHandlers()
        {
            // Prevent double handling by removing any potential handlers that have
            // already been added.
            RemoveEventHandlers();

            /// Add the OnExploded event handler.
            SpriteExploder.OnExploded += SpriteExploder_OnExploded;
        }

        /// <summary>
        /// Remove event handlers for the sprite exploder explosion.
        /// </summary>
        void RemoveEventHandlers()
        {
            SpriteExploder.OnExploded -= SpriteExploder_OnExploded;
        }

        /// <summary>
        /// Handler for the Sprite Exploder component's OnExploded event. This method
        /// sets various particle system references that will be used in later spawning.
        /// </summary>
        void SpriteExploder_OnExploded()
        {
            RemoveEventHandlers();

            sortedParticleCount = SpriteExploder.GetMaxParticleCount();
            if (sortedParticleCount <= 1) // If there are no particles, then call the complete event.
            {
                sortedParticleCount = 0;
                OnCompleted?.Invoke();
                return;
            }

            particleScale = SpriteExploder.GetParticleScale();

            particles = new ParticleSystem.Particle[sortedParticleCount];  
            UpdateCustomParticleData();

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

        void LateUpdate() => SpawnInstancesFromParticles();
        /// <summary>
        /// Spawns prefab instances from particles that have died since the last update.
        /// </summary>
        void SpawnInstancesFromParticles()
        {
            // If there are no alive sorted particles, then skip doing anything else.
            if (sortedParticleCount <= 0) return;

            // Determine the number of newly destroyed particles.
            int particleCount = SpriteExploder.GetParticleCount();
            int destroyedCount = sortedParticleCount - particleCount;

            // If there are newly destroyed particles, then we'll potentially spawn
            // instances of the prefab in their place.
            if (destroyedCount > 0)
            {
                Texture2D texture = SpriteExploder.GetTexture();
                Vector2 flipVector = SpriteExploder.GetFlipVector2();

                // Enumerate through the destroyed count and spawn instances where
                // the particle tile's texture color isn't clear in the center.
                for (int i = 0; i < destroyedCount; i++)
                {
                    // The Sprite Exploder works by sending index values in the custom
                    // data stream to the shader. The index value is stored in the x property.
                    int tileIndex = (int)customDatas[i].x;

                    // With the particle tile index, we can get the tile X and y values
                    // from the Sprite Exploder.
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

                    // With tile x and y values, we can determing the texture x and y
                    // values of particle tile.
                    int tileCenterPixelX = tileX * pixelSize + centerOffset;
                    int tileCenterPixelY = tileY * pixelSize + centerOffset;

                    Color color = texture.GetPixel(tileCenterPixelX, tileCenterPixelY);

                    // If the center particle is totally clear then skip making an
                    // instance since the tile will likely look empty or too small.
                    if (color.a <= 0.0f) continue;

                    // Get a reference to the particle and its current color.
                    ParticleSystem.Particle particle = particles[i];
                    Color32 spawnColor = particle.GetCurrentColor(spriteExploderParticleSystem);

                    // Spawn an instance of the prefab with the particle's transform data and color
                    GameObject instance = Spawn(particle.position, particleScale, particle.rotation, particle.velocity, particle.angularVelocity, spawnColor);
                    // Invoke the OnInstanceCreated event for any outside class that are handling that.
                    OnInstanceCreated?.Invoke(instance); 
                }
            }

            // Update the custom particle data
            UpdateCustomParticleData();

            // If there are no more particles, then invoke the completed event.
            if (sortedParticleCount <= 0)
            {
                OnCompleted?.Invoke();
            }
        }

        /// <summary>
        /// Spawns an instance of the prefab.
        /// This public method is used outside the context of particle system handling.
        /// So, the sprite renderer color is used for determing the instance color.
        /// </summary>
        /// <param name="localPosition">The local position of the spawned instance</param>
        /// <param name="localScale">The local scale of the spawned instance.</param>
        /// <param name="rotation">The local z euler rotation of the spawned instance.</param>
        /// <returns>The spawned instance</returns>
        public GameObject Spawn(Vector3 localPosition, Vector3 localScale, float rotation = 0.0f)
        {
            Color32 instanceColor = GetComponent<SpriteRenderer>().color;
            return Spawn(localPosition, localScale, rotation, Vector2.zero, 0.0f, instanceColor);
        }

        /// <summary>
        /// Spawns an instance of the prefab.
        /// </summary>
        /// <param name="localPosition">The local position of the spawned instance</param>
        /// <param name="localScale">The local scale of the spawned instance.</param>
        /// <param name="rotation">The local z euler rotation of the spawned instance.</param>
        /// <param name="velocity">The velocity of the spawned instance.</param>
        /// <param name="angularVelocity">The angylar velocity of the spawned instance.</param>
        /// <param name="color">The sprite renderer color of the spawned instance.</param>
        /// <returns>The spawned instance</returns>
        public GameObject Spawn(Vector3 localPosition, Vector3 localScale, float rotation, Vector2 velocity, float angularVelocity, Color color)
        {
            GameObject instance = Instantiate(Prefab, InstanceParent);
            instance.name = Prefab.name;
            instance.transform.localPosition = localPosition;
            instance.transform.localScale = localScale;
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
        
        /// <summary>
        /// Updates the particle array and custom datas. When taken from the particle system,
        /// the particle order is different on each update. So, the values will need to be sorted
        /// so that spawning can be properly handled.
        /// </summary>
        void UpdateCustomParticleData()
        {
            sortedParticleCount = SpriteExploder.GetParticles(particles);

            if (sortedParticleCount > 0)
            {
                SpriteExploder.GetCustomParticleData(customDatas);
                QuickSortParticles(0, sortedParticleCount - 1);
            }
        }
        
        /// <summary>
        /// A method that sorts particles by remaining lifetime via quick sort recursion.
        /// </summary>
        void QuickSortParticles(int left, int right)
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
                QuickSortParticles(left, j);
            }
            if (i < right)
            {
                QuickSortParticles(i, right);
            }
        }
    }
}

