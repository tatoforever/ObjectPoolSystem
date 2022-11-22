# ObjectPoolSystem
Simple but powerful GameObject pool system that can optionally handle instances lifecycles.

## Instructions and API usage

- To initialize the a pool of instances call
```ObjectPoolSystem(GameObject prefab, int minInstances, int maxInstances, float lifeTime = 0f)```

Example:

```var poolSystemOfBloodParticles = new ObjectPoolSystem(BloodParticlePrefab, minInstances, maxInstances, lifeTime);```
###### Note: This is done only once per scene/prefab type.

The pool is instantiated with a minimum set of instances, if you need more instances than the minimum set, it will resize the pool untill it reach max instances. Once the pool instance reaches max instances it will swap the oldest instance with the new instance when you request a new one. But if you never reach max instances, no new instance is created.

- To follow the example above. To get a new instance call ```ObjectPoolSystem.GetInstance();``` like this:

```var newInstance = poolSystemOfBloodParticles.GetInstance();```
This call replaces ```GameObject.Instantiate();``` in your code.

- To return manually an instance to the pool call ```ObjectPoolSystem.ReturnInstance(GameObject instance);``` like this:

```poolSystemOfBloodParticles.ReturnInstance(newInstance);```
This call replaces GameObject.Destroy/DestroyImmediate(); in your code.
###### Note: You only need to manually return an instance to the pool if you din't supplied a lifeTime value when creating the pool. If you supplied a lifeTime, instances will return automatically to the pool after lifeTime has reached zero. If you are requesting more instances than max instances the system will return the oldest one.

- To handle automatic lifecycle of instances, call every frame ```ObjectPoolSystem.Tick();``` like this:
```
void Update()
{
  poolSystemOfBloodParticles.Tick();
}
```
For example you want to instantiate a bunch of objects but you don't want to manually handle its deactivation. This call takes care of their deactivation time using the lifeTime you provided when created the pool.

Once the object managing the pool instance is destroyed, call ```ObjectPoolSystem.Dispose();``` like this:

```poolSystemOfBloodParticles.Dispose();```
That will de-allocate and destroy all instances created by the pool.

## Tips:
With this sytem what i basically do is to create the pool of instances OnEnable (only once), and OnDisable i dispose it (if previously created).
You can use this system with anything that is a GameObject that needs tons of instantiations/destroy or if you want to avoid the activation hit (spike) after a GameObject is instantiated at runtime. Reseting instances properties is 100x faster than instantiating a new object, plus zero garbage collection. Make sure that any component attached to those instances handles their properies initialization/reset with OnEnable/OnDisable so when their instance get activated/deactivated it will automatically reset its state.
