using UnityEngine;

/// <summary>
/// Be aware this will not prevent a non singleton constructor
///   such as `T myT = new T();`
/// To prevent that, add `protected T () {}` to your singleton class.
/// 
/// As a note, this is made as MonoBehaviour because we need Coroutines.
/// </summary>
public class Singleton<T> : MonoBehaviourExt where T : MonoBehaviourExt
{
	private static T _instance;

	private static object _lock = new object();

	public static T Instance
	{
		get
		{
			if (applicationIsQuitting) {
				Debug.LogWarning("[Singleton] Instance '"+ typeof(T) +
					"' already destroyed on application quit." +
					" Won't create again - returning null.");
				return null;
			}

			lock(_lock)
			{
				if (_instance == null)
				{
					_instance = (T) FindObjectOfType(typeof(T));
                    if (_instance != null)
                        DontDestroyOnLoad(_instance);

                    if ( FindObjectsOfType(typeof(T)).Length > 1 )
                    {
                        Debug.LogError("[Singleton] Something went really wrong " +
                            " - there should never be more than 1 singleton!" +
                            " Reopenning the scene might fix it.");
                        return _instance;
					}

					if (_instance == null)
					{
						GameObject singleton = new GameObject(typeof(T).Name, typeof(T));
						singleton.name = "(singleton) "+ typeof(T).ToString();
						_instance = _instance ?? singleton.GetComponent<T>();
						DontDestroyOnLoad(singleton);

						Debug.Log("[Singleton] An instance of " + typeof(T) + 
							" is needed in the scene, so '" + singleton +
							"' was created with DontDestroyOnLoad.");
					} else {
						Debug.Log("[Singleton] Using instance already created: " +
							_instance.gameObject.name);
					}
				}

				return _instance;
			}
		}

	}

	public static bool HasInstance {
		get {
			return !IsDestroyed;
		}
	}

	public static bool IsDestroyed {
		get {
			if(_instance == null) {
				return true;
			} else {
				return false;
			}
		}
	}

	protected override void Awake() {
		base.Awake ();
		if (_instance == null) {
			_instance = this as T;
			DontDestroyOnLoad(this.gameObject);
		}
		else if (_instance != this){
			Destroy(this.gameObject);
		}
	}

	private static bool applicationIsQuitting = false;
	/// <summary>
	/// When Unity quits, it destroys objects in a random order.
	/// In principle, a Singleton is only destroyed when application quits.
	/// If any script calls Instance after it have been destroyed, 
	///   it will create a buggy ghost object that will stay on the Editor scene
	///   even after stopping playing the Application. Really bad!
	/// So, this was made to be sure we're not creating that buggy ghost object.
	/// </summary>
	protected virtual void OnDestroy () {
		if (_instance != this) return;
		_instance = null;
		applicationIsQuitting = true;
	}
	/*
	protected virtual void OnApplicationQuit () {
		if (_instance != this) return;
		_instance = null;
		applicationIsQuitting = true;
	}
	*/
}