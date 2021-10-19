using System;
using System.Threading;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

public static class UniRxExtensions
{
    /**/
    public static IObservable<T> AsObservable<T>(this Action<T> action)
    {
        return Observable.FromEvent<T> (
            h => action += h,
            h => action -= h);
    }

    public static IObservable<T> AsObservable<T>(this Action<T, T> action)
    {
        return Observable.FromEvent<Action<T,T>, T>(
            h => (prev, next) => h.Invoke(next),
            h => action += h,
            h => action -= h);
    }
    
    public static IObservable<T> AsObservable<T>(this EventHandler<T> handler) where  T : EventArgs
    {
        return Observable.FromEvent<EventHandler<T>, T>(
            h => (_, e) => h.Invoke(e),
            h => handler += h,
            h => handler -= h);
    }
    /**/

    public static IDisposable BindToFloatReactiveProperty(this Slider slider, IReactiveProperty<float> floatReactiveProperty)
    {
        slider.value = floatReactiveProperty.Value;
        return slider.OnValueChangedAsObservable()
            .SubscribeWithState(floatReactiveProperty, (v, p) => p.Value = v);
    }
    
    
    public static AsyncSubject<TSource> GetAwaiter<TSource, TException>(this IObservable<TSource> source, IObservable<TException> error,
        CancellationToken cancellationToken) where TException: Exception
    {
        if (source == null) throw new ArgumentNullException("source");

        var s = new AsyncSubject<TSource>();

        if (cancellationToken.IsCancellationRequested)
        {
            s.OnError(new OperationCanceledException(cancellationToken));
            return s;
        }
        
        var d1 = source.Subscribe(unit =>
        {
            s.OnNext(unit);
            s.OnCompleted();
        }, s.OnError, s.OnCompleted);
        var d2 = error.Subscribe(s.OnError);
        var d = StableCompositeDisposable.Create(d1, d2);
        
        if (cancellationToken.CanBeCanceled)
        {
            var ctr = cancellationToken.Register(() =>
            {
                d.Dispose();
                s.OnError(new OperationCanceledException(cancellationToken));
            });
          
            s.Subscribe(_ => { }, _ => ctr.Dispose(), ctr.Dispose);
        }

        return s;
    }
}
