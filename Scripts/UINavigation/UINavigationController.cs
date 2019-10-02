using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace MVC.UISystem
{
    public interface INavigationController
    {
        IViewController VisibleViewController { get; }
        void PushViewController(IViewController n, bool animated = false);
        IViewController PopViewController(bool animated = false);
    }

    public sealed class UINavigationController : System.Object, INavigationController
    {
    	public enum NavigationType
    	{
    		Pop,
    		Push
    	}

    	private static UINavigationController _instance = null;
    	
    	public static UINavigationController Instance {
    		get { return _instance ?? (_instance = new UINavigationController ()); }
    	}

    	private Stack<IViewController> viewControllers = new Stack<IViewController> ();

    	private UINavigationController ()
    	{}

    	public IViewController VisibleViewController
    	{
    		get { 
    			if (viewControllers.Count > 0)
    				return viewControllers.Peek ();
    			else
    				return null;
    		}
    	}
    			
    	public void PushViewController (IViewController n, bool animated = false)
    	{
            IViewController p = VisibleViewController;
    		if (n.Equals(p)) return;

    		viewControllers.Push (n);
    		StartAnim (p, n, NavigationType.Push, animated, false);
    	}

    	public IViewController PopViewController (bool animated = false)
    	{
    		if (viewControllers.Count < 1)
    			return null;

    		var retVal = viewControllers.Pop ();
    		StartAnim(retVal, VisibleViewController, NavigationType.Pop, animated, false);
    		return retVal;
    	}

    	private void StartAnim (IViewController current, IViewController next, NavigationType type, bool animated, bool released = true)
    	{
    		Time.timeScale = 1f;

    		if  (current is UIViewControllerAnimation)
    		{
    			UIViewControllerAnimation anim = current as UIViewControllerAnimation;
    			anim.PlayNavigationAnimation(NavigationType.Pop, animated);
    		}
    		else if (current != null)
    			current.gameObject.SetActive(false);

    		if  (next is UIViewControllerAnimation)
    		{
    			UIViewControllerAnimation anim = next as UIViewControllerAnimation;
    			anim.PlayNavigationAnimation(NavigationType.Push, animated);
    		}
    		else if (next != null)
    			next.gameObject.SetActive(true);
    	}
    }
}