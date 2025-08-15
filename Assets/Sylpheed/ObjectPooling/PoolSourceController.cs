using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using System.Linq;

/// <summary>
/// PoolSourceController serves as an abstraction to a collection of PoolSource objects.
/// By any means, you can still pool an object without the presence of this script.
/// </summary>
public class PoolSourceController : MonoBehaviour
{
    [System.Serializable]
    public class PoolSourceData
    {
        public Poolable Prefab;
        public int Buffer = 0;
    }

    /// <summary>
    /// List of PoolData objects which contains the Prefab and Buffer.
    /// This can only be set in inspector. Use RegisterPoolData during runtime
    /// Do not add duplicate objects
    /// </summary>
    public List<PoolSourceData> PreconfiguredPoolList = new List<PoolSourceData>();
    /// <summary>
    /// Reparent object to this gameobject when pooled
    /// </summary>
    public bool ReparentOnPool;

    public PoolSourceCollection SourceCollection { get; private set; }

    void Awake()
    {
        Transform parent = null;
        if (ReparentOnPool) parent = transform;

        SourceCollection = new PoolSourceCollection(parent);

        // Initialize pool data
        foreach (PoolSourceData data in PreconfiguredPoolList)
        {
            SourceCollection.RegisterSource(data.Prefab, data.Buffer);
        }
    }
}