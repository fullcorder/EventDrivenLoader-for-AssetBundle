using UnityEngine;
using UnityEditor;

public class CleanCache 
{
	[MenuItem ("AssetBundles/Clean Cache")]
	public static void Clean()
	{
		Caching.CleanCache ();
		Debug.Log ("CleanCache.");
	}
}
