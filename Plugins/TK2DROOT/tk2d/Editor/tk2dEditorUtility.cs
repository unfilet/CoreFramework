using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

[InitializeOnLoad]
public static class tk2dEditorUtility
{
	public static double version = 2.5;
	public static int releaseId = 8; // < -10001 = alpha 1, other negative = beta release, 0 = final, positive = final hotfix
	public static int buildNo = 9;


	public static string ReleaseStringIdentifier(double _version, int _releaseId, int _buildNo)
	{
		string id = _version.ToString("0.0");
		if (_releaseId == 0) id += ".0";
		else if (_releaseId > 0) id += "." + _releaseId.ToString();
		else if (_releaseId < -10000) id += " alpha " + (-_releaseId - 10000).ToString();
		else if (_releaseId < 0) id += " beta " + (-_releaseId).ToString();

		if (_buildNo > 0) id += "." + _buildNo.ToString();

		return id;
	}
	
	/// <summary>
	/// Release filename for the current version
	/// </summary>
	public static string CurrentReleaseFileName(string product, double _version, int _releaseId, int _buildNo)
	{
		string id = product + _version.ToString("0.0");
		if (_releaseId == 0) id += ".0";
		else if (_releaseId > 0) id += "." + _releaseId.ToString();
		else if (_releaseId < -10000) id += "alpha" + (-_releaseId - 10000).ToString();
		else if (_releaseId < 0) id += "beta" + (-_releaseId).ToString();

		if (_buildNo > 0) id += "." + _buildNo.ToString();

		return id;
	}
	

	public static string CreateNewPrefab(string name) // name is the filename of the prefab EXCLUDING .prefab
	{
		Object obj = Selection.activeObject;
		string assetPath = AssetDatabase.GetAssetPath(obj);
		if (assetPath.Length == 0)
		{
			assetPath = tk2dGuiUtility.SaveFileInProject("Create...", "Assets/", name, "prefab");
		}
		else
		{
			// is a directory
			string path = System.IO.Directory.Exists(assetPath) ? assetPath : System.IO.Path.GetDirectoryName(assetPath);
			assetPath = AssetDatabase.GenerateUniqueAssetPath(path + "/" + name + ".prefab");
		}
		
		return assetPath;
	}
	
	
	[System.ObsoleteAttribute]
	static T[] FindPrefabsInProjectWithComponent<T>() where T : Component
	// returns null if nothing is found
	{
		List<T> allGens = new List<T>();
		
		Stack<string> paths = new Stack<string>();
		paths.Push(Application.dataPath);
		while (paths.Count != 0)
		{
			string path = paths.Pop();
			string[] files = Directory.GetFiles(path, "*.prefab");
			foreach (var file in files)
			{
				GameObject go = AssetDatabase.LoadAssetAtPath( file.Substring(Application.dataPath.Length - 6), typeof(GameObject) ) as GameObject;
				if (!go) continue;
				
				T gen = go.GetComponent<T>();
				if (gen)
				{
					allGens.Add(gen);
				}
			}
			
			foreach (string subdirs in Directory.GetDirectories(path)) 
				paths.Push(subdirs);
		}
		
		if (allGens.Count == 0) return null;
		
		T[] allGensArray = new T[allGens.Count];
		for (int i = 0; i < allGens.Count; ++i)
			allGensArray[i] = allGens[i];
		return allGensArray;
	}
	
	public static GameObject CreateGameObjectInScene(string name)
	{
		string realName = name;
		int counter = 0;
		while (GameObject.Find(realName) != null)
		{
			realName = name + counter++;
		}
		
        GameObject go = new GameObject(realName);
		if (Selection.activeGameObject != null)
		{
			string assetPath = AssetDatabase.GetAssetPath(Selection.activeGameObject);
			if (assetPath.Length == 0) {
				go.transform.parent = Selection.activeGameObject.transform;
				go.layer = Selection.activeGameObject.layer;
			}
		}
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;	
        return go;
	}
	
	public static void DrawMeshBounds(Mesh mesh, Transform transform, Color c)
	{
		var e = mesh.bounds.extents;
		Vector3[] boundPoints = new Vector3[] {
			mesh.bounds.center + new Vector3(-e.x, e.y, 0.0f),
			mesh.bounds.center + new Vector3( e.x, e.y, 0.0f),
			mesh.bounds.center + new Vector3( e.x,-e.y, 0.0f),
			mesh.bounds.center + new Vector3(-e.x,-e.y, 0.0f),
			mesh.bounds.center + new Vector3(-e.x, e.y, 0.0f) };
		
		for (int i = 0; i < boundPoints.Length; ++i)
			boundPoints[i] = transform.TransformPoint(boundPoints[i]);
		
		Handles.color = c;
		Handles.DrawPolyLine(boundPoints);
	}
	
	public static void UnloadUnusedAssets()
	{
		Object[] previousSelectedObjects = Selection.objects;
		Selection.objects = new Object[0];
		
		System.GC.Collect();
#if (UNITY_3_5 || UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9)
		EditorUtility.UnloadUnusedAssets();
#else
		EditorUtility.UnloadUnusedAssetsImmediate();
#endif
		
		Selection.objects = previousSelectedObjects;
	}	

	public static void CollectAndUnloadUnusedAssets()
	{
		System.GC.Collect();
		System.GC.WaitForPendingFinalizers();
#if (UNITY_3_5 || UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9)
		EditorUtility.UnloadUnusedAssets();
#else
		EditorUtility.UnloadUnusedAssetsImmediate();
#endif
	}

	public static void DeleteAsset(UnityEngine.Object obj)
	{
		if (obj == null) return;
		UnityEditor.AssetDatabase.DeleteAsset(UnityEditor.AssetDatabase.GetAssetPath(obj));
	}

	public static bool IsPrefab(Object obj)
	{
		return (PrefabUtility.GetPrefabAssetType(obj) != PrefabAssetType.NotAPrefab);
	}

	public static bool IsEditable(UnityEngine.Object obj) {
    	MonoBehaviour mb = obj as MonoBehaviour;
    	return (mb && (mb.gameObject.hideFlags & HideFlags.NotEditable) == 0);
	}

	public static void SetGameObjectActive(GameObject go, bool active)
	{
#if UNITY_3_5
		go.SetActiveRecursively(active);
#else
		go.SetActive(active);
#endif		
	}

	public static bool IsGameObjectActive(GameObject go)
	{
#if UNITY_3_5
		return go.active;
#else
		return go.activeSelf;
#endif		
	}

#if !(UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2)
	private static System.Reflection.PropertyInfo sortingLayerNamesPropInfo = null;
	private static bool sortingLayerNamesChecked = false;

	private static string[] GetSortingLayerNames() {
		if (sortingLayerNamesPropInfo == null && !sortingLayerNamesChecked) {
			sortingLayerNamesChecked = true;
			try {
				System.Type IEU = System.Type.GetType("UnityEditorInternal.InternalEditorUtility,UnityEditor");
				if (IEU != null) {
					sortingLayerNamesPropInfo = IEU.GetProperty("sortingLayerNames", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
				}
			}
			catch { }
			if (sortingLayerNamesPropInfo == null) {
				Debug.Log("tk2dEditorUtility - Unable to get sorting layer names.");
			}
		}

		if (sortingLayerNamesPropInfo != null) { 
			return sortingLayerNamesPropInfo.GetValue(null, null) as string[];
		}
		else {
			return new string[0];
		}
	}

	public static string SortingLayerNamePopup( string label, string value ) {
		if (value == "") {
			value = "Default";
		}
		string[] names = GetSortingLayerNames();
		if (names.Length == 0) {
			return EditorGUILayout.TextField(label, value);			
		}
		else {
			int sel = 0;
			for (int i = 0; i < names.Length; ++i) {
				if (names[i] == value) {
					sel = i;
					break;
				}
			}
			sel = EditorGUILayout.Popup(label, sel, names);
			return names[sel];
		}
	}
#endif

    [MenuItem(tk2dMenu.createBase + "Empty GameObject", false, 55000)]
    static void DoCreateEmptyGameObject()
    {
		GameObject go = tk2dEditorUtility.CreateGameObjectInScene("GameObject");
		Selection.activeGameObject = go;
		Undo.RegisterCreatedObjectUndo(go, "Create Empty GameObject");
    }
}
