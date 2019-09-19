using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MVC
{
    public interface IView<T>
    {
        T Model { get; }
        void Init(T model);
    }

    public abstract class View<T> : View, IView<T> where T : class//, IModel
    {
        [SerializeField] private T _model = null;
        public T Model
        {
            get { return _model; }
            private set { _model = value; }
        }

        public void Init(T model)
        {
            this.Model = model;
            this.Init();
        }
    }

    public abstract class View : MonoBehaviour
    {
        protected virtual void Awake()
        {
        }

        protected virtual void Start()
        {
        }

        protected virtual void OnEnable()
        {
        }

        protected virtual void OnDisable()
        {
        }

        public virtual void UpdateIfNeeded()
        {
            this.Init();
        }

        protected abstract void Init();
    }
}
