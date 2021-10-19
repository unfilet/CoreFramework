using System;

namespace UtilsCore.Scripts
{
    public static class CleanCodeExtensions
    {
        public static T With<T>(this T obj, Action<T> action)
        {
            action?.Invoke(obj);
            return obj;
        }
        
        public static T With<T>(this T obj, Action<T> action, Func<bool> when)
        {
            if (when())
                action?.Invoke(obj);
            return obj;
        }
        
        public static T With<T>(this T obj, Action<T> action, bool when)
        {
            if (when)
                action?.Invoke(obj);
            return obj;
        }
    }
}