using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using System.Linq;

namespace Sylpheed.ObjectPooling
{
    /// <summary>
    /// PoolSourceController serves as an abstraction to a collection of PoolSource objects.
    /// By any means, you can still pool an object without the presence of this script.
    /// </summary>
    public class PoolSourceController : MonoBehaviour
    {
        [SerializeField] private List<PoolSourceData> _preconfiguredPoolList = new();
        [SerializeField] private bool _reparentOnPool;

        /// <summary>
        /// List of PoolData objects which contains the Prefab and Buffer.
        /// This can only be set in inspector. Use RegisterPoolData during runtime
        /// Do not add duplicate objects
        /// </summary>
        public IReadOnlyCollection<PoolSourceData> PreconfiguredPoolList => _preconfiguredPoolList;
        /// <summary>
        /// Reparent object to this gameobject when pooled
        /// </summary>
        public bool ReparentOnPool => _reparentOnPool;
        public PoolSourceCollection SourceCollection { get; private set; }

        void Awake()
        {
            var parent = ReparentOnPool ? transform : null;
            SourceCollection = new PoolSourceCollection(parent);

            // Initialize pool data
            foreach (var data in PreconfiguredPoolList)
            {
                SourceCollection.RegisterSource(data.Prefab, data.Buffer);
            }
        }
        
        [Serializable]
        public class PoolSourceData
        {
            [SerializeField] private Poolable _prefab;
            [SerializeField] private int _buffer;
            
            public Poolable Prefab => _prefab;
            public int Buffer => _buffer;
        }
    }
}