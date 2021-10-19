using System;
using Firebase.Database;

namespace UniRx.Triggers
{
    public static class ObsevableDatabaseReference
    {
        #region Query

        public static IObservable<ValueChangedEventArgs> OnValueChangedAsObservable(this Query query)
        {
            return Observable.FromEvent<EventHandler<ValueChangedEventArgs>, ValueChangedEventArgs>(
                h => (_, e) => h.Invoke(e),
                h => query.ValueChanged += h,
                h => query.ValueChanged -= h
            );
        }

        public static IObservable<ChildChangedEventArgs> OnChildAddedAsObservable(this Query query)
        {
            return Observable.FromEvent<EventHandler<ChildChangedEventArgs>, ChildChangedEventArgs>(
                h => (_, e) => h.Invoke(e),
                h => query.ChildAdded += h,
                h => query.ChildAdded -= h
            );
        }

        public static IObservable<ChildChangedEventArgs> OnChildRemovedAsObservable(this Query query)
        {
            return Observable.FromEvent<EventHandler<ChildChangedEventArgs>, ChildChangedEventArgs>(
                h => (_, e) => h.Invoke(e),
                h => query.ChildRemoved += h,
                h => query.ChildRemoved -= h
            );
        }
        
        public static IObservable<ChildChangedEventArgs> OnChildChangedAsObservable(this Query query)
        {
            return Observable.FromEvent<EventHandler<ChildChangedEventArgs>, ChildChangedEventArgs>(
                h => (_, e) => h.Invoke(e),
                h => query.ChildChanged += h,
                h => query.ChildChanged -= h
            );
        }
        
        public static IObservable<ChildChangedEventArgs> OnChildMovedAsObservable(this Query query)
        {
            return Observable.FromEvent<EventHandler<ChildChangedEventArgs>, ChildChangedEventArgs>(
                h => (_, e) => h.Invoke(e),
                h => query.ChildMoved += h,
                h => query.ChildMoved -= h
            );
        }
        #endregion
    }
}