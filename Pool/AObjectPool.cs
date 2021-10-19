using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CardKit.Utils
{
    public abstract class AObjectPool<T> : IPool
    {
        protected string name = string.Empty;
        private int initialSize;
        private int maxSize = -1;

        private bool HasMaxSize => maxSize > 0;
        private bool HasPooledObjects => pooledObjects.Count > 0;
        public bool Initialized { get; protected set; }

        #region Caches
        private readonly List<T> pooledObjects = new List<T>();
        private readonly Dictionary<int, T> aliveObjects = new Dictionary<int, T>();
        #endregion

        #region Initialization

        public AObjectPool(string name = null, int size = 0, int maxSize = -1) : base()
        {
            this.name = name;
            this.initialSize = size;
            this.maxSize = maxSize;
        }

        public void Initialize(bool forceReinitialization = false)
        {
            if (!Initialized || forceReinitialization)
            {
                Initialized = true;
                Populate(initialSize);
            }
        }

        #endregion

        #region Internal
        protected abstract T CreateNewObject();
        protected abstract void DestroyObject(T gameObject);
        protected abstract int GetCodeForObject(T gameObject);

        protected virtual void OnObjectSpawn(T gameObject) { }
        protected virtual void OnObjectDespawn(T gameObject) { }

        #endregion

        #region GetObject/Component
        /// <summary>
        /// Gets an object from the pool.
        /// </summary>
        /// <returns>The retrieved object.</returns>

        object IPool.GetObject() => GetObject();

        public T GetObject() 
        {
            T obj;
            if (HasPooledObjects)
            {
                obj = pooledObjects[pooledObjects.Count - 1];
                pooledObjects.RemoveAt(pooledObjects.Count - 1);
            }
            else
            {
                obj = CreateNewObject();
            }

            if (obj == null)
            {
                Debug.LogWarning(string.Format("Object in pool '{0}' was null or destroyed; it may have been destroyed externally. Attempting to retrieve a new object", name));
                return GetObject();
            }

            aliveObjects.Add(GetCodeForObject(obj), obj);

            OnObjectSpawn(obj);
            return obj;
        }
        #endregion

        #region Release/Destroy

        void IPool.Release(object gameObject) => Release((T)gameObject);
        /// <summary>
        /// Releases an object and returns it back to the pool, effectively 'destroying' it from the scene.
        /// Pool equivalent of Destroy.
        /// </summary>
        /// <param name="obj">The object to release.</param>
        public void Release(T obj)
        {
            if (obj == null)
                return;

            if (!aliveObjects.Remove(GetCodeForObject(obj)))
            {
                Debug.LogWarning(string.Format("Object '{0}' could not be found in pool '{1}'; it may have already been released.", obj, name));
                return;
            }

            if (obj != null)
            {
                OnObjectDespawn(obj);

                if (HasMaxSize && pooledObjects.Count >= maxSize)
                    Destroy(obj);
                else
                    pooledObjects.Add(obj);
            }
        }

        /// <summary>
        /// Releases a collection of objects and returns them back to the pool, effectively 'destroying' them from the scene.
        /// </summary>
        /// <param name="objs">the objects to release.</param>
        public void Release(IEnumerable<T> objs)
        {
            foreach (T obj in objs)
                Release(obj);
        }

        /// <summary>
        /// Releases every active object in this pool.
        /// </summary>
        public void ReleaseAll()
        {
            Release(aliveObjects.Values);
        }

        /// <summary>
        /// Forcibly destroys the object and does not return it to the pool.
        /// </summary>
        /// <param name="obj">The object to destroy.</param>
        public virtual void Destroy(T obj)
        {
            aliveObjects.Remove(GetCodeForObject(obj));
            DestroyObject(obj);

        }

        /// <summary>
        /// Forcibly destroys a collection of objects and does not return them to the pool.
        /// </summary>
        /// <param name="objs">The objects to destroy.</param>
        public void Destroy(IEnumerable<T> objs)
        {
            foreach (T obj in objs)
                Destroy(obj);
        }
        #endregion

        #region Miscellaneous
        public void Populate(int quantity)
        {
            quantity = Mathf.Max(quantity, 0);
            if (HasMaxSize)
                quantity = Mathf.Min(quantity, maxSize - pooledObjects.Count);

            for (int i = 0; i < quantity; i++)
            {
                T newObj = CreateNewObject();
                pooledObjects.Add(newObj);
            }
        }

        /// <summary>
        /// Destroys every object in the pool, both alive and pooled.
        /// </summary>
        public virtual void Purge()
        {
            foreach (T obj in pooledObjects) { DestroyObject(obj); }
            foreach (T obj in aliveObjects.Values) { DestroyObject(obj); }
            pooledObjects.Clear();
            aliveObjects.Clear();
        }

        public IEnumerable<T> GetAllActiveObjects()
        {
            return aliveObjects.Values
                .Where(x => x != null);
        }

        #endregion
    }
}