using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Composite
{
    public abstract class Component<T>
    {
        public T Value { get; private set; }
        public Component(T value) => Value = value;

        public abstract T Calculate();
    }

    public abstract class Composite<T> : Component<T>
    {
        protected List<Component<T>> children = new List<Component<T>>();

        public Composite(T value) : base(value) {}

        public void Add(Component<T> component) => children.Add(component);
        public void Remove(Component<T> component) => children.Remove(component);
    }
}