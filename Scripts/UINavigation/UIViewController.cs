using UnityEngine;
using System.Collections;

namespace MVC.UISystem
{
    public interface IViewController
    {
        GameObject gameObject { get; }
    }

    public class UIViewController : MonoBehaviourExt, IViewController {
    	// Called when the view is about to made visible. Default does nothing
    	public virtual void viewWillAppear(bool animated) {}
    	// Called when the view has been fully transitioned onto the screen. Default does nothing
    	public virtual void viewDidAppear(bool animated) {}
    	// Called when the view is dismissed, covered or otherwise hidden. Default does nothing
    	public virtual void viewWillDisappear(bool animated) {}
    	// Called after the view was dismissed, covered or otherwise hidden. Default does nothing
    	public virtual void viewDidDisappear(bool animated) {}
    }

    public interface UIViewControllerAnimation
    {
    	void PlayNavigationAnimation(UINavigationController.NavigationType type, bool animated);
    }
}