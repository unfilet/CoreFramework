using System;
using System.Reflection;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MonoBehaviourExt : MonoBehaviour
{
    public Animator animator
    {
        get
        {
            if(cachedAnimator == null)
                cachedAnimator = this.GetComponent<Animator>();
            return cachedAnimator;
        }
    }
    private Animator cachedAnimator = null;

	public new Animation animation
	{
		get
		{
			if(cachedAnimation == null)
				cachedAnimation = this.GetComponent<Animation>();
			return cachedAnimation;
		}
	}
	private Animation cachedAnimation = null;

	/// <summary>
	/// Cached transform
	/// </summary>
	public new Transform transform
	{
		get
		{
			if(cachedTransform == null)
				cachedTransform = base.transform;
			return cachedTransform;
		}
	}
    private Transform cachedTransform = null;

    public new Rigidbody rigidbody
    {
        get
        {
            if(cachedRigidbody == null)
                cachedRigidbody = this.GetComponent<Rigidbody>();
            return cachedRigidbody;
        }
    }
    protected Rigidbody cachedRigidbody = null;

    public new Renderer renderer
    {
        get
        {
            if(cachedRenderer == null)
				cachedRenderer = this.GetComponent<Renderer>();
            return cachedRenderer;
        }
    }
    private Renderer cachedRenderer = null;

    public new AudioSource audio
    {
        get
        {
            if(cachedAudio == null)
                cachedAudio = this.GetComponent<AudioSource>();
            return cachedAudio;
        }
    }
    protected AudioSource cachedAudio = null;

	public new Collider collider
	{
		get
		{
			if(cachedCollider == null)
				cachedCollider = this.GetComponent<Collider>();
			return cachedCollider;
		}
	}
	private Collider cachedCollider = null;

    public new Rigidbody2D rigidbody2D
    {
        get
        {
            if(cachedRigidbody2D == null)
                cachedRigidbody2D = this.GetComponent<Rigidbody2D>();
            return cachedRigidbody2D;
        }
    }
    protected Rigidbody2D cachedRigidbody2D = null;

    public new Collider2D collider2D
    {
        get
        {
            if(cachedCollider2D == null)
                cachedCollider2D = this.GetComponent<Collider2D>();
            return cachedCollider2D;
        }
    }
    private Collider2D cachedCollider2D = null;

    protected virtual void Awake()
    {
        cachedTransform = gameObject.transform;
    }

    protected virtual void Start () {}
    protected virtual void OnEnable () {}
    protected virtual void OnDisable () {}

//	protected virtual void OnDestroy() {}
}

static public class MethodExtensionForMonoBehaviourTransform
{
	public static T CopyComponentValue<T>(this Component comp, T other) where T : Component
	{
		Type type = comp.GetType();
		if (type != other.GetType()) return null; // type mis-match
		BindingFlags flags = BindingFlags.Public | 
				BindingFlags.NonPublic | 
				BindingFlags.Instance | 
				BindingFlags.Default | 
				BindingFlags.DeclaredOnly;
		PropertyInfo[] pinfos = type.GetProperties(flags);
		foreach (var pinfo in pinfos) {
			if (pinfo.CanWrite) {
				try {
					pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
				}
				catch { } // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
			}
		}
		FieldInfo[] finfos = type.GetFields(flags);
		foreach (var finfo in finfos) {
			finfo.SetValue(comp, finfo.GetValue(other));
		}
		return comp as T;
	}

	public static T AddComponent<T>(this GameObject go, T toAdd) where T : Component
	{
		return go.AddComponent<T>().CopyComponentValue(toAdd) as T;
	}

    /// <summary>
    /// Gets or add a component. Usage example:
    /// BoxCollider boxCollider = transform.GetOrAddComponent<BoxCollider>();
    /// </summary>
    static public T GetOrAddComponent<T> (this Component child) where T: Component {
        T result = child.GetComponent<T>();
        if (result == null) {
            result = child.gameObject.AddComponent<T>();
        }
        return result;
    }

    static public T GetOrAddComponent<T> (this GameObject child) where T: Component {
        T result = child.GetComponent<T>();
        if (result == null) {
            result = child.AddComponent<T>();
        }
        return result;
    }

	static public Vector3 GlobalScale(this Transform transform)
	{
		Transform parentTransform = transform;
		Vector3 retVal = transform.localScale;

		while (parentTransform = parentTransform.parent)
			retVal = retVal.Divide(parentTransform.localScale);
		return retVal;
	}
}

static public class MethodExtension
{

	static public Vector3 Divide(this Vector3 a, Vector3 d)
	{
		return new Vector3(a.x / d.x, a.y / d.y, a.z / d.z);
	}

	public static string UppercaseFirst(this string s)
	{
		// Check for empty string.
		if (string.IsNullOrEmpty(s))
		{
			return string.Empty;
		}
		// Return char and concat substring.
		return char.ToUpper(s[0]) + s.Substring(1);
	}
	public static bool IsEmpty<T>(this IEnumerable<T> list)
	{
		if (list is ICollection<T>) return ((ICollection<T>)list).Count == 0;
		return !list.Any();
	}
}