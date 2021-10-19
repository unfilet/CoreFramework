using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using Zenject;
using Object = UnityEngine.Object;

namespace CardKit.Utils
{
    public class GameObjectPool : AObjectPool<GameObject>
    {
        private readonly DiContainer container;
        private GameObject prefab;

        #region Caches
        private readonly Dictionary<Tuple<int, Type>, object> componentCache = new Dictionary<Tuple<int, Type>, object>();
        #endregion

        #region Initialization

        public GameObjectPool(DiContainer container, GameObject prefab) : base(null, 5, -1)
        {
            this.container = container;
            this.prefab = prefab;
            if (prefab && string.IsNullOrEmpty(name))
                this.name = prefab.name;

            Initialize();
        }

        #endregion

        #region GetObject/Component

        /// <summary>
        /// Gets an object from the pool, and then retrieves the specified component using a cache to improve performance.
        /// </summary>
        /// <typeparam name="T">The component type to get.</typeparam>
        /// <returns>The retrieved component.</returns>
        public T GetObject<T>() where T : class
        {
            GameObject obj = GetObject();
            return GetObjectComponent<T>(obj);
        }

        /// <summary>
        /// Retrieves the specified component from an object using a cache to improve performance.
        /// </summary>
        /// <typeparam name="T">The component type to get.</typeparam>
        /// <param name="obj">The object to get the component from.</param>
        /// <returns>The retrieved component.</returns>
        private T GetObjectComponent<T>(GameObject obj) where T : class
        {
            Tuple<int, Type> key = new Tuple<int, Type>(obj.GetInstanceID(), typeof(T));

            if (componentCache.TryGetValue(key, out object cmp))
            {
                if (cmp is T comp)
                    return comp;
                else
                    componentCache.Remove(key);
            }

            if (obj.TryGetComponent<T>(out T component))
                componentCache[key] = component;

            return component;
        }
        #endregion

        #region Release/Destroy

        public override void Destroy(GameObject obj)
        {
            var pooledObject = GetObjectComponent<IPoolable>(obj);
            Assert.IsTrue(pooledObject.Pool == this);
            base.Destroy(obj);
        }

        #endregion

        #region Internal
        protected override GameObject CreateNewObject()
        {
            GameObject obj = container.InstantiatePrefab(prefab);
            IPoolable pooledObject = obj.GetOrAddComponent<PooledObject>();
            pooledObject.InitializeTemplate(this);
            obj.SetActive(false);
            return obj;
        }

        protected override void DestroyObject(GameObject gameObject)
        {
            var keys = componentCache.Keys
               .Where(tpl => tpl.Item1 == gameObject.GetInstanceID())
               .ToArray();
            foreach (var key in keys)
                componentCache.Remove(key);

            Object.Destroy(gameObject);
        }

        protected override int GetCodeForObject(GameObject gameObject)
        {
            return gameObject.GetInstanceID();
        }

        protected override void OnObjectSpawn(GameObject gameObject)
        {
            base.OnObjectSpawn(gameObject);
            gameObject?.SetActive(true);
        }

        protected override void OnObjectDespawn(GameObject gameObject)
        {
            base.OnObjectDespawn(gameObject);
            gameObject.SetActive(false);
        }

        #endregion

        #region Miscellaneous

        public override void Purge()
        {
            base.Purge();
            componentCache.Clear();
        }

        #endregion
    }

    public class PooledObject : MonoBehaviour, IPoolable
    {
        public IPool Pool { get; private set; }

        void IPoolable.InitializeTemplate(IPool pool)
        {
            this.Pool = pool;
        }

        public void Release()
        {
            Pool?.Release(gameObject);
        }
    }
}