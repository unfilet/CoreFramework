using UnityEngine;
using System.Collections;

public static class ComponentExtensions
{
    public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
    {
        if (!gameObject.TryGetComponent(out T component))
            component = gameObject.AddComponent<T>();
        return component;
    }

    public static T GetOrAddComponent<T>(this Component component) where T : Component
        => component.gameObject.GetOrAddComponent<T>();

}
