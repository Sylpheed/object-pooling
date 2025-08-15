using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using System.Linq;

public class PoolSourceCollection
{
    private HashSet<PoolSource> sources = new HashSet<PoolSource>();
    public IEnumerable<PoolSource> Sources { get { return sources; } }
    public Transform Container { get; private set; }

    public PoolSourceCollection(Transform container = null)
    {
        Container = container;
    }

    /// <summary>
    /// Register prefab for the pool
    /// </summary>
    /// <param name="prefab">Make sure that this is a prefab or it might cause problem if the object is destroyed.</param>
    /// <param name="instantiateOnEmptyPool">Instantiates new objects when pool is empty.</param>
    /// <param name="buffer">Number of pre-instantiated objects.</param>
    /// <returns>PoolData created</returns>
    public PoolSource RegisterSource(Poolable prefab, int buffer = 0)
    {
        Assert.IsNotNull(prefab, "Prefab cannot be null");

        // Return cached pool source if it already exists
        PoolSource source = sources.Where(p => p.Prefab == prefab).SingleOrDefault();
        if (source != null) return source;

        source = new PoolSource(prefab, buffer, Container);
        sources.Add(source);
        return source;
    }

    /// <summary>
    /// Register prefab for the pool
    /// </summary>
    /// <param name="prefab">Prefab must contain Poolable component</param>
    /// <param name="instantiateOnEmptyPool">Instantiates new objects when pool is empty.</param>
    /// <param name="buffer">Number of pre-instantiated objects.</param>
    /// <returns>PoolData created</returns>
    public PoolSource RegisterSource(GameObject prefab, int buffer = 0)
    {
        Assert.IsNotNull(prefab, "Prefab cannot be null");
        Assert.IsNotNull(prefab.GetComponent<Poolable>());

        return RegisterSource(prefab.GetComponent<Poolable>(), buffer);
    }

    /// <summary>
    /// Pools the object.
    /// </summary>
    /// <param name="obj">Source prefab must be registered in this controller.</param>
    public void Pool(Poolable obj)
    {
        if (!sources.Where(p => p == obj.Source).Any()) throw new UnityException("Cannot pool object which is not registered in this controller");
        obj.Pool();
    }

    /// <summary>
    /// Gets an object from the pool
    /// </summary>
    /// <param name="prefab">Prefab must be registered in this controller.</param>
    /// <returns></returns>
    public Poolable GetObject(Poolable prefab)
    {
        // Look for the pool source with the prefab
        PoolSource source = sources.Where(p => p.Prefab == prefab).SingleOrDefault();
        if (source == null) throw new UnityException("Prefab is not registered in this controller");
        return source.GetObject();
    }

    public bool IsRegistered(Poolable prefab)
    {
        return sources.Where(p => p.Prefab == prefab).Any();
    }

    /// <summary>
    /// Gets an object from the pool
    /// </summary>
    /// <typeparam name="T">Type of Component</typeparam>
    /// <param name="prefab">Prefab must be registered in this controller.</param>
    /// <returns></returns>
    public T GetObject<T>(Poolable prefab) where T : Component
    {
        Poolable pooled = GetObject(prefab);
        T comp = pooled.GetComponent<T>();
        Assert.IsNotNull(comp, "Trying to get missing component from pooled object");

        return comp;
    }

    /// <summary>
    /// Gets an object from the pool
    /// </summary>
    /// <typeparam name="T">Type of Component</typeparam>
    /// <param name="prefab">Prefab must be registered in this controller.</param>
    /// <returns></returns>
    public T GetObject<T>(T prefab) where T : Component
    {
        if (prefab.GetComponent<Poolable>() == null)
        {
            Debug.LogWarning(prefab);
        }
        Poolable poolable = prefab.GetComponent<Poolable>();
        if (!poolable) throw new UnityException("Prefab doesn't contain Poolable component");

        return GetObject<T>(poolable);
    }

    /// <summary>
    /// Gets an object from the pool
    /// </summary>
    /// <typeparam name="T">Type of Component</typeparam>
    /// <param name="prefab">Prefab must be registered in this controller.</param>
    /// <returns></returns>
    public T GetObject<T>(GameObject prefab) where T : Component
    {
        return GetObject<T>(prefab.GetComponent<T>());
    }

    public void Clear()
    {
        foreach (PoolSource source in sources)
        {
            source.Clear();
        }
    }
}
