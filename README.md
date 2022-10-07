# ObjectPoolSystem
Simple but powerful GameObject pool system that can optionally handle instances lifecycles.

## Instructions and API usage

- To initialize the a pool of instances call ```ObjectPoolSystem(GameObject prefab, int minInstances, int maxInstances, float lifeTime = 0f)```
Example:

```var poolSystemOfBloodParticles = new ObjectPoolSystem(BloodParticlePrefab, minInstances, maxInstances, lifeTime);```
###### Note: This is done only once per scene/prefab type.

The pool is instantiated with a minimum set of instances, if you need more instances than the minimum set, it will resize the pool untill it reach max instances. Once the pool instance reaches max instances it will swap the oldest instance with the new instance when you request a new one. But if you never reach max instances, no new instance is created.

- To follow the example above. To get a new instance call ObjectPoolSystem.Tick();, like this:

```var newInstance = poolSystemOfBloodParticles.GetInstance();```
This call replaces GameObject.Instantiate(); in your code.

- To return manually an instance to the pool call ObjectPoolSystem.ReturnInstance(GameObject instance); like this:

```poolSystemOfBloodParticles.ReturnInstance(newInstance);```
This call replaces GameObject.Destroy/DestroyImmediate(); in your code.
###### Note: You only need to manually return an instance to the pool if you din't supplied a lifeTime when creating the pool. If you supplied a lifeTime, instances will return automatically to the pool after lifeTime has reached zero.

- To handle automatic lifecycle of instances, call every frame ObjectPoolSystem.Tick(); like this:
```
void Update()
{
  poolSystemOfBloodParticles.Tick();
}
```
Once the object managing the pool instance is destroyed, call ObjectPoolSystem.Dispose(); like this:

```poolSystemOfBloodParticles.Dispose();```
That will de-allocate and destroy all instances created by the pool.

## Tips:
With this sytem what i basically do is to create the pool of instances OnEnable (if not already done), and OnDisable i dispose it (if previously created).
You can use this with anything that is a GameObject that needs tons of spawns. Make sure that any component attached to those instances handles their properies initialization/reset with OnEnable/OnDisable, like enemies that are de-activated will reset their states/enery/attributes/etc
