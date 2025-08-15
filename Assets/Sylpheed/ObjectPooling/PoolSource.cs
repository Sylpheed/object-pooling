using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using System.Linq;

namespace Sylpheed.ObjectPooling
{
    public class PoolSource
    {
        public Poolable Prefab { get; private set; }
        public Transform Container { get; private set; }

        public string Guid { get; private set; }

        public IEnumerable<Poolable> PooledObjects => _pooledObjects;

        private readonly HashSet<Poolable> _pooledObjects = new HashSet<Poolable>();

        /// <summary>
        /// Creates a PoolSource for the Poolable prefab specified.
        /// </summary>
        /// <param name="prefab">Must be a prefab or it may not work properly</param>
        /// <param name="buffer">Number of pre-instantiated objects. This is useful if you want to preload during loading screen.</param>
        /// <param name="container"></param>
        public PoolSource(Poolable prefab, int buffer, Transform container = null)
        {
            Assert.IsNotNull(prefab);

            Prefab = prefab;
            Container = container;
            Guid = System.Guid.NewGuid().ToString();

            // Buffer
            for (var i = 0; i < buffer; i++)
            {
                var obj = Object.Instantiate<Poolable>(Prefab);
                obj.BindSource(this);

                // Add to pool
                obj.Pooled = true;
                _pooledObjects.Add(obj);
                obj.SendMessage("OnPoolSleep", SendMessageOptions.DontRequireReceiver);
                obj.gameObject.SetActive(false);
                if (Container) obj.transform.SetParent(Container, false);
                obj.gameObject.name += " (Pooled)";
            }
        }

        ~PoolSource()
        {
            Clear();
        }

        /// <summary>
        /// Gets object from the pool. Creates new object if pool is empty and InstantiateOnEmptyBuffer is true.
        /// </summary>
        /// <returns></returns>
        public Poolable GetObject()
        {
            var pooled = _pooledObjects.FirstOrDefault();

            // Create new object if pool is empty
            if (pooled == null)
            {
                pooled = Object.Instantiate<Poolable>(Prefab);
                pooled.BindSource(this);
            }
            // Remove object from pool if found
            else _pooledObjects.Remove(pooled);

            pooled.gameObject.name = Prefab.gameObject.name;
            pooled.gameObject.SetActive(true);
            pooled.Pooled = false;
            pooled.transform.SetParent(null);

            pooled.SendMessage("OnPoolAwake", SendMessageOptions.DontRequireReceiver);

            return pooled;
        }

        /// <summary>
        /// Gets object from the pool. Creates new object if pool is empty and InstantiateOnEmptyBuffer is true.
        /// Sends OnPoolAwake message to the gameobject
        /// </summary>
        /// <typeparam name="T">Casts the object to the specified type</typeparam>
        /// <returns></returns>
        public T GetObject<T>() where T : Component
        {
            var pooled = GetObject();

            var comp = pooled.GetComponent<T>();
            Assert.IsNotNull(comp, "Trying to get missing component from pooled object");

            return comp;
        }

        /// <summary>
        /// Pools and disables the object
        /// Sends OnPoolSleep message to the gameobject.
        /// </summary>
        /// <param name="obj"></param>
        public void Pool(Poolable obj)
        {
            Assert.IsTrue(obj.Source == this, "Cannot pool object that has different PoolSource");
            Assert.IsFalse(_pooledObjects.Contains(obj), "Object is already pooled!");

            // Add to pool
            obj.Pooled = true;
            _pooledObjects.Add(obj);
            obj.SendMessage("OnPoolSleep", SendMessageOptions.DontRequireReceiver);
            obj.gameObject.SetActive(false);
            if (Container) obj.transform.SetParent(Container, false);
            obj.gameObject.name += " (Pooled)";
        }

        /// <summary>
        /// Destroys all pooled objects. Must be explicitly called from Unity's API 
        /// </summary>
        public void Clear()
        {
            foreach (var pool in _pooledObjects)
            {
                Object.Destroy(pool.gameObject);
            }

            _pooledObjects.Clear();
        }
    }
}