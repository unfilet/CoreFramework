using System;
using System.Collections;
using UnityEngine;
using System.Threading;
using UniRx.Triggers;

namespace UniRx.Tween
{
    public static class UniRxTween
    {
        //https://github.com/fumobox/TweenRx/blob/master/Assets/Plugins/TweenRx/Tween.cs
        public static IObservable<Vector2> Play(Vector2 start, Vector2 end, float duration = 1, Easing.Type easeType = Easing.Type.Linear, float delayBefore = 0, float delayAfter = 0)
            => Execute(0, 1, duration, easeType, delayBefore, delayAfter).Select(t => Vector2.LerpUnclamped(start, end, t));

        public static IObservable<Vector3> Play(Vector3 start, Vector3 end, float duration = 1, Easing.Type easeType = Easing.Type.Linear, float delayBefore = 0, float delayAfter = 0)
            => Execute(0, 1, duration, easeType, delayBefore, delayAfter).Select(t => Vector3.LerpUnclamped(start, end, t));

        public static IObservable<Vector4> Play(Vector4 start, Vector4 end, float duration = 1, Easing.Type easeType = Easing.Type.Linear, float delayBefore = 0, float delayAfter = 0)
            => Execute(0, 1, duration, easeType, delayBefore, delayAfter).Select(t => Vector4.LerpUnclamped(start, end, t));

        public static IObservable<float> Play(float start, float end, float duration = 1, Easing.Type easeType = Easing.Type.Linear, float delayBefore = 0, float delayAfter = 0)
            => Execute(start, end, duration, easeType, delayBefore, delayAfter);

        public static IObservable<Color> Play(Color start, Color end, float duration = 1, Easing.Type easeType = Easing.Type.Linear, float delayBefore = 0, float delayAfter = 0)
            => Execute(0, 1, duration, easeType, delayBefore, delayAfter).Select(t => Color.LerpUnclamped(start, end, t));

        public static IObservable<Quaternion> Play(Quaternion start, Quaternion end, float duration = 1, Easing.Type easeType = Easing.Type.Linear, float delayBefore = 0, float delayAfter = 0)
            => Execute(0, 1, duration, easeType, delayBefore, delayAfter).Select(t => Quaternion.LerpUnclamped(start, end, t));

        public static IObservable<Vector2> TweenTo(this Vector2 start, Vector2 end, float duration = 1, Easing.Type easeType = Easing.Type.Linear, float delayBefore = 0, float delayAfter = 0)
            => TweenTo(start, end, duration, easeType, delayBefore, delayAfter);

        public static IObservable<Vector3> TweenTo(this Vector3 start, Vector3 end, float duration = 1, Easing.Type easeType = Easing.Type.Linear, float delayBefore = 0, float delayAfter = 0)
            => TweenTo(start, end, duration, easeType, delayBefore, delayAfter);

        public static IObservable<Vector4> TweenTo(this Vector4 start, Vector4 end, float duration = 1, Easing.Type easeType = Easing.Type.Linear, float delayBefore = 0, float delayAfter = 0)
            => TweenTo(start, end, duration, easeType, delayBefore, delayAfter);

        public static IObservable<float> TweenTo(this float start, float end, float duration = 1, Easing.Type easeType = Easing.Type.Linear, float delayBefore = 0, float delayAfter = 0)
            => TweenTo(start, end, duration, easeType, delayBefore, delayAfter);

        public static IObservable<Color> TweenTo(this Color start, Color end, float duration = 1, Easing.Type easeType = Easing.Type.Linear, float delayBefore = 0, float delayAfter = 0)
            => TweenTo(start, end, duration, easeType, delayBefore, delayAfter);

        public static IObservable<Quaternion> TweenTo(this Quaternion start, Quaternion end, float duration = 1, Easing.Type easeType = Easing.Type.Linear, float delayBefore = 0, float delayAfter = 0)
            => TweenTo(start, end, duration, easeType, delayBefore, delayAfter);

        private static IObservable<float> Execute(float start, float end, float duration, Easing.Type easeType, float delayBefore, float delayAfter)
        {
            var easingMethod = typeof(Easing).GetMethod(easeType.ToString());
            object[] methodParams = { 0, start, end - start, duration };

            IEnumerator TweenEnumerator(IObserver<float> observer, CancellationToken ct)
            {
                if (delayBefore > 0)
                    yield return new WaitForSeconds(delayBefore);

                if (ct.IsCancellationRequested)
                {
                    observer.OnCompleted();
                    yield break;
                }

                if (ct.IsCancellationRequested)
                {
                    observer.OnCompleted();
                    yield break;
                }

                float elapsedTime = 0;
                float p = 0;

                while (elapsedTime < duration)
                {
                    if (ct.IsCancellationRequested)
                    {
                        observer.OnCompleted();
                        yield break;
                    }

                    elapsedTime += Time.deltaTime;
                    methodParams[0] = Math.Min(elapsedTime, duration);
                    p = Convert.ToSingle(easingMethod?.Invoke(null, methodParams)); ;

                    observer.OnNext(p);

                    yield return null;
                }

                if (delayAfter > 0)
                    yield return new WaitForSeconds(delayAfter);

                observer.OnCompleted();
            }

            return Observable.FromCoroutine<float>(TweenEnumerator);
        }


        private static IObservable<float> Execute(
           GameObject gameObject, float start, float end, float duration, Easing.Type easeType, float delayBefore = 0, float delayAfter = 0)
        {
            var easingMethod = typeof(Easing).GetMethod(easeType.ToString());
            object[] methodParams = { 0, start, end - start, duration };

            return
                //Observable.Empty<double>()
                //.StartWith(() => Time.time)
                //.SelectMany(startTime => observable.Select(_ => Time.time - startTime))

                gameObject.UpdateAsObservable()
                .Select(_ => Time.deltaTime)
                .Scan(0f, (a, b) => a + b)
                .TakeWhile(elapsedTime => elapsedTime <= duration)
                .Last()
                .Select(currentTime =>
                {
                    methodParams[0] = Math.Min(currentTime, duration);
                    return Convert.ToSingle(easingMethod?.Invoke(null, methodParams));
                });
        }

        public static IObservable<float> Ease(this GameObject gameObject, float startPoint, float endPoint, float duration, Easing.Type easeType = Easing.Type.Linear)
            => Execute(gameObject, startPoint, endPoint, duration, easeType);

        public static IObservable<Vector3> MoveTo(this GameObject gameObject, Vector3 dest, float duration, Easing.Type easeType)
        {
            var start = gameObject.transform.position;
            return Execute(gameObject, 0, 1, duration, easeType)
                .Select(t => Vector3.LerpUnclamped(start, dest, t))
                .Do(x => gameObject.transform.position = x);
        }
    }



    /**
    public struct Number : IEquatable<Number> //, IComparable<Number>
    {
        public object Value { get; private set; }

        public Number(object value)
            => Value = value;

        //public int CompareTo(Number other)
        //    => Value.CompareTo(other.Value);

        public bool Equals(Number other)
            => Value.Equals(other.Value);

        public override int GetHashCode()
            => Value.GetHashCode();

        public override string ToString()
            => $"{base.ToString()} - {Value.GetType()} - {Value}";

        #region Conversions

        public static implicit operator Number(double d)
            => new Number((float)d);
        public static implicit operator Number(float d)
            => new Number(d);
        public static implicit operator Number(Vector3 d)
            => new Number(d);
        public static implicit operator Number(Vector2 d)
            => new Number(d);
        public static implicit operator Vector3(Number d)
            => (Vector3)d.Value;
        public static implicit operator Vector2(Number d)
            => (Vector2)d.Value;
        public static implicit operator double(Number d)
            => Convert.ToDouble(d.Value);
        public static implicit operator float(Number d)
            => (float)d.Value;

        #endregion

        #region OPERATORS

        public static Number operator +(Number c1, Number c2)
            => new Number(Expression
            .Lambda(
                Expression.Add(Expression.Constant(c1.Value), Expression.Constant(c2.Value)))
            .Compile()
            .DynamicInvoke());

        public static Number operator -(Number c1, Number c2)
             => new Number(Expression
            .Lambda(
                Expression.Subtract(Expression.Constant(c1.Value), Expression.Constant(c2.Value)))
            .Compile()
            .DynamicInvoke());

        public static Number operator *(Number c1, Number c2)
             => new Number(Expression
            .Lambda(
                Expression.Multiply(Expression.Constant(c1.Value), Expression.Constant(c2.Value)))
            .Compile()
            .DynamicInvoke());

        public static Number operator /(Number c1, Number c2)
             => new Number(Expression
            .Lambda(
                Expression.Divide(Expression.Constant(c1.Value), Expression.Constant(c2.Value)))
            .Compile()
            .DynamicInvoke());

        #endregion

    }
    /**/
}