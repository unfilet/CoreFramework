using UnityEngine;
using Debug = UnityEngine.Debug;

[CreateAssetMenu(menuName = "Settings/BundleVersionHolder")]
public class BundleVersionSettings : ScriptableObject
{
    public Version version;
    public string LastBuildTime;

}
