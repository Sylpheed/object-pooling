using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System.Linq;

public class Poolable : MonoBehaviour {

    public PoolSource Source { get; private set; }
    public bool Bound { get { return Source != null; } }

    /// <summary>
    /// Checks if the object is currently pooled
    /// DO NOT MODIFY. This is used by PoolSource internally.
    /// </summary>
    public bool Pooled { get; set; }

    /// <summary>
    /// Binds this object to the PoolableSource.
    /// Object can only be bound once. If you need to rebind it to a different source, you must create a new instance.
    /// DO NOT INVOKE. This method is invoked by PoolSource internally.
    /// </summary>
    /// <param name="source"></param>
    public void BindSource(PoolSource source)
    {
        Assert.IsFalse(Bound, "Poolable can only be bound once. You need to instantiate a new object and rebind it.");
        Source = source;
    }

    /// <summary>
    /// Wrapper for pooling.
    /// </summary>
    public void Pool()
    {
        Assert.IsTrue(Bound, "Poolable object not bound to a PoolSource");
        Source.Pool(this);
    }

#if UNITY_EDITOR
    [ContextMenu("Log Source")]
    private void LogSource()
    {
        Debug.Log("GUID: " + Source.Guid + "\tObjects: " + Source.PooledObjects.Count(), Source.Container);
    }
#endif
}

public static class PoolableExtension
{
    /// <summary>
    /// Pools the GameObject
    /// </summary>
    /// <param name="obj"></param>
    /// <returns>True on successful pool</returns>
    public static bool Pool(this GameObject obj)
    {
        Poolable poolable = obj.GetComponent<Poolable>();
        if (poolable && poolable.Bound)
        {
            poolable.Pool();
            return true;
        }

        return false;
    }

    public static bool Pool(this Component comp)
    {
        return Pool(comp.gameObject);
    }

    /// <summary>
    /// Pools the object if it can be pooled. Else, destroy it.
    /// </summary>
    /// <param name="obj"></param>
    public static void PoolOrDestroy(this GameObject obj)
    {
        Poolable poolable = obj.GetComponent<Poolable>();
        if (poolable && poolable.Bound) poolable.Pool();
        else Object.Destroy(obj);
    }

    public static void PoolOrDestroy(this Component comp)
    {
        PoolOrDestroy(comp.gameObject);
    }

    public static bool IsPoolable(this GameObject obj)
    {
        Poolable poolable = obj.GetComponent<Poolable>();
        if (!poolable) return false;

        return poolable.Bound;
    }
}