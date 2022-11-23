using UnityEngine;

namespace FM.Pool
{
    public class ObjectPoolSystem
    {
        private sealed class ObjectPoolData
        {
            public GameObject goInstance;
            public float lifeTime;
        
            /// <summary>
            /// Constructor that takes in a GameObject instance and a lifetime float as parameters
            /// </summary>
            /// <param name="goInstance"></param>
            /// <param name="lifeTime"></param>
            public ObjectPoolData(GameObject goInstance, float lifeTime)
            {
                this.goInstance = goInstance;
                this.lifeTime = lifeTime;
            }
        }
        
        private int _instanceCount, _maxInstances;
        private bool _randomLife;
        private float _minLifeTime, _maxLifeTime;
        private ObjectPoolData[] _instances;
        
        /// <summary>
        /// Constructor that takes in a prefab and the number of instances to create with an optional fixed life time
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="minInstances"></param>
        /// <param name="maxInstances"></param>
        /// <param name="lifeTime"></param>
        public ObjectPoolSystem(GameObject prefab, int minInstances, int maxInstances, float lifeTime = 0f)
        {
            CreatePoolSystem(prefab, minInstances, maxInstances, lifeTime, lifeTime, false);
        }
        
        /// <summary>
        /// Constructor that takes in a GameObject (scene instance or prefab) and the number of instances to create with an optional random life time
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="minInstances"></param>
        /// <param name="maxInstances"></param>
        /// <param name="minLifeTime"></param>
        /// <param name="maxLifeTime"></param>
        public ObjectPoolSystem(GameObject prefab, int minInstances, int maxInstances, float minLifeTime = 0f, float maxLifeTime = 0f)
        {
            CreatePoolSystem(prefab, minInstances, maxInstances, minLifeTime, maxLifeTime, true);
        }
        
        private void CreatePoolSystem(GameObject prefab, int minInstances, int maxInstances, float minLifeTime = 0f, float maxLifeTime = 0f, bool randomLife = false)
        {
            _randomLife = randomLife;
            _instanceCount = minInstances;
            _maxInstances = maxInstances;
            _instances = new ObjectPoolData[maxInstances];
            _minLifeTime = minLifeTime;
            _maxLifeTime = maxLifeTime;
            
            for (var i = 0; i < _instanceCount; i++)
            {
                var obj = GameObject.Instantiate(prefab);
                obj.name = $"{prefab.name} PoolInstance";
                obj.SetActive(false);
                _instances[i] = new ObjectPoolData(obj, _randomLife ? Random.Range(_minLifeTime, _maxLifeTime) : _maxLifeTime);
            }
        }
        
        /// <summary>
        /// Returns an available instance
        /// </summary>
        /// <returns></returns>
        public GameObject GetInstance()
        {
            for (var i = 0; i < _instanceCount; i++)
            {
                if (_instances[i].goInstance.activeInHierarchy) continue;
                
                //returns the first one that is not taken
                _instances[i].lifeTime = _randomLife ? Random.Range(_minLifeTime, _maxLifeTime) : _maxLifeTime;//reset the life time
                _instances[i].goInstance.SetActive(true);
                return _instances[i].goInstance;
            }
            
            //If max instances reached get the oldest one and return it as the new one
            if (_instanceCount >= _maxInstances)
            {
                var newest = _instances[0];
                newest.lifeTime = _randomLife ? Random.Range(_minLifeTime, _maxLifeTime) : _maxLifeTime;//reset the life time
                newest.goInstance.SetActive(false);//this will trigger OnDisable in the attached components
                newest.goInstance.SetActive(true);//this will trigger OnEnable in the attached components
                return newest.goInstance;
            }
            
            //If we are allowed to resize, create a new instance and return it
            _instanceCount++;
            //System.Array.Resize(ref _instances, _instanceCount);
            var newIns = GameObject.Instantiate(_instances[0].goInstance);
            newIns.name = _instances[0].goInstance.name;
            _instances[Mathf.Max(0,_instanceCount - 1)] = new ObjectPoolData(newIns, _randomLife ? Random.Range(_minLifeTime, _maxLifeTime) : _maxLifeTime);//add it to the end of the array
            newIns.SetActive(false);//this will trigger OnDisable in the attached components
            newIns.SetActive(true);//this will trigger OnEnable in the attached components
            return newIns;
        }
        
        /// <summary>
        /// Returns an instance to the pool and sets it to inactive
        /// </summary>
        /// <param name="instance"></param>
        public void ReturnInstance(GameObject instance)
        {
            for (var i = 0; i < _instanceCount; i++)
            {
                if (_instances[i].goInstance != instance) continue;
                _instances[i].lifeTime = 0f;//reset the life time
                _instances[i].goInstance.SetActive(false);
            }
        }
        
        /// <summary>
        /// Returns an instance to the pool and sets it to inactive using an index
        /// </summary>
        /// <param name="instanceIndex"></param>
        public void ReturnInstance(int instanceIndex)
        {
            if (instanceIndex < 0 || instanceIndex >= _instanceCount) return;
            _instances[instanceIndex].lifeTime = 0f;//reset the life time
            _instances[instanceIndex].goInstance.SetActive(false);
        }
        
        /// <summary>
        /// Call this every frame to update the life time of all instances (if any instance lifetime reaches zero, it will deactivate automatically)
        /// </summary>
        public void Tick()
        {
            if(_maxLifeTime <= 0f) return;
            
            for (var i = 0; i < _instanceCount; i++)
            {
                _instances[i].lifeTime -= Time.deltaTime;
                if (_instances[i].lifeTime <= 0f)
                    _instances[i].goInstance.SetActive(false);
            }
        }
        
        // Destructor
        //~ObjectPoolSystem()
        /// <summary>
        /// Call this to destroy all the instances in the pool
        /// </summary>
        public void Dispose()
        {
            for (var i = 0; i < _instanceCount; i++)
            {
                if (_instances[i].goInstance)
                    GameObject.Destroy(_instances[i].goInstance);
            }
        }
    }
}