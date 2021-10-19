using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CardKit.Utils
{
    public class ObjectPool<T> : AObjectPool<T>
    {
        private readonly Func<T> createNewObject;
        private readonly Action<T> destroyObject;
        private readonly Func<T, int> getCodeForObject;

        #region Initialization

        public ObjectPool(Func<T> createNewObject, Action<T> destroyObject, Func<T, int> getCodeForObject) : base(null, 0, -1)
        {
            this.createNewObject = createNewObject;
            this.destroyObject = destroyObject;
            this.getCodeForObject = getCodeForObject;
        }

        #endregion

        #region Internal
        protected override T CreateNewObject() => createNewObject.Invoke();
        protected override void DestroyObject(T gameObject) => destroyObject.Invoke(gameObject);
        protected override int GetCodeForObject(T gameObject) => getCodeForObject.Invoke(gameObject);     

        #endregion
    }
}