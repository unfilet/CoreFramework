using System;
using System.Collections.Generic;
using UnityEngine;

namespace UniRx.Triggers
{
    public class ObservableAnimatorEventTrigger : ObservableTriggerBase
    {
        Subject<string> subject;

        /// <summary>Callback for processing animation movements for modifying root motion.</summary>
        public IObservable<string> OnAnimatorEventAsObservable()
            => subject ?? (subject = new Subject<string>());


        public void RaiseEvent(string name)
        {
            subject?.OnNext(name);
        }

        protected override void RaiseOnCompletedOnDestroy()
        {
        }
    }

    public static partial class ObservableTriggerExtensions
    {
        /// <summary>Callback for processing animation movements for modifying root motion.</summary>
        public static IObservable<string> OnAnimatorEventAsObservable(this Animator component)
        {
            if (component == null || component.gameObject == null) return Observable.Empty<string>();
            return component.GetOrAddComponent<ObservableAnimatorEventTrigger>().OnAnimatorEventAsObservable();
        }

    }
}