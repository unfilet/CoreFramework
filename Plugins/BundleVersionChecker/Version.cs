using System;
using UnityEngine;

[Serializable]
public class Version
{
	public string version;
	public string build;

	public override string ToString()
		=> $"Version: {version}\nBuild: {build}";
}
