# Indie Dev Tool : AI Particles

## AI Particles
This is a package utilizes the Sprite Exploder and AI Simulator packages. It provides a new set of
components and demos that show how to bridge these vastly different packages to create some fun effects.

## Demo Scenes

### Explosion Spawning
A fun demo that shows a Sprite Exploder explode and spawn new instances.

### Crab Battle
This demo expounds on the previous AI Simulator demo and shows four crab factions battling each other for
dominance. When a crab runs out of health, the crab explodes into ever smaller crabs. When small crabs are
eaten, the crab that ate them grows bigger. Which crab faction will reign supreme?

## Core components and classes

### Explosion Spawner
This component takes a Sprite Exploder and spawns instances of a prefab as the particle lifetimes end.

### Footprint
This class creates a footprint so that large map elements can occupy more than once map cell at a time.

### Abstract Sub-Agent
This abstract class is used by the footprint to create a series of map elements that link to the main
map element.

### Trigger Animator
A helper component that makes triggering animation states triggers easier.