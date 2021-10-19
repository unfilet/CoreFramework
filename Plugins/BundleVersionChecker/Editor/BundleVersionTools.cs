using System;
using System.Linq;
using UnityEditor;
using Debug = UnityEngine.Debug;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.IO;

[InitializeOnLoad]
public class BundleVersionTools : Editor, IPreprocessBuildWithReport
{
    public int callbackOrder => 12;

    public void OnPreprocessBuild(BuildReport report)
    {
        TryUpdateInfo();
    }

    /// <summary>
	/// Class attribute [InitializeOnLoad] triggers calling the static constructor on every refresh.
	/// </summary>
	static BundleVersionTools()
    {
        /**
        var instances = GetAllInstances<BundleVersionSettings>();
        if (instances.Length == 0)
        {
            var targetDir = EditorUtility.SaveFolderPanel("Select Target Folder", Application.dataPath, "Generated");
            CreateAsset<BundleVersionSettings>(targetDir);
            TryUpdateInfo();
        }
        /**/
    }

    private static T[] GetAllInstances<T>() where T : ScriptableObject
    {
        string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);
        return guids
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<T>)
            .ToArray();
    }

    public static void CreateAsset<T>(string path) where T : ScriptableObject
    {
        T asset = ScriptableObject.CreateInstance<T>();

        path = Path.Combine(path, $"{typeof(T)}.asset");
        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path);

        AssetDatabase.CreateAsset(asset, assetPathAndName);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh(ImportAssetOptions.Default);
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }

    [MenuItem("Tools/UpdateBundleInfo")]
    private static bool TryUpdateInfo()
    {
        var instances = GetAllInstances<BundleVersionSettings>();

        if (instances.Length == 0)
        {
            // TODO: AUTO-CREATE ONE!
            return false;
        }

        if (instances.Length > 1)
            Debug.LogErrorFormat("Found more than 1 BundleVersionSettings: {0}. Using first one!",
                instances.Length);

        var buildSettings = instances.First();
        buildSettings.LastBuildTime = DateTime.Now.ToString("yyyy/MM/dd-HH:mm:ss"); // case sensitive
        buildSettings.version = new Version
        {
            version = PlayerSettings.bundleVersion,
#if UNITY_IOS
            build = PlayerSettings.iOS.buildNumber
#elif UNITY_ANDROID
            build = $"{PlayerSettings.Android.bundleVersionCode}"
#endif
        };

        EditorUtility.SetDirty(buildSettings);
        AssetDatabase.SaveAssets();

        Selection.activeObject = buildSettings;

        return true;
    }


}