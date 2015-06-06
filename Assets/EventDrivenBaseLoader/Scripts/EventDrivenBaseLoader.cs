using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

public delegate void AssetBundleManagerEvent<T_0, T_1>(T_0 result0, T_1 result1);

public class EventDrivenBaseLoader : MonoBehaviour
{
    public string AssetBundleRootUrl;

    const string kAssetBundlesPath = "/AssetBundles/";

    private MonoBehaviour mCoroutineDriver;
    private GameObject mCoroutineDriverParent;


    #if UNITY_EDITOR
	private static readonly Dictionary<BuildTarget, string> sPlatformFolderMap =
		new Dictionary<BuildTarget, string>
	{
		{BuildTarget.Android,"Android"},
		{BuildTarget.iOS,"iOS"},
		{BuildTarget.WebPlayer, "WebPlayer"},
		{BuildTarget.StandaloneWindows, "Windows"},
		{BuildTarget.StandaloneWindows64, "Windows"},
		{BuildTarget.StandaloneOSXIntel, "OSX"},
		{BuildTarget.StandaloneOSXIntel64, "OSX"},
		{BuildTarget.StandaloneOSXUniversal, "OSX"},
	};

#else
    private static readonly Dictionary<RuntimePlatform, string> sRuntimePlatformFolderMap =
        new Dictionary<RuntimePlatform, string> {
            { RuntimePlatform.Android, "Android"},
            { RuntimePlatform.IPhonePlayer, "iOS"},
            { RuntimePlatform.WindowsWebPlayer, "WebPlayer"},
            { RuntimePlatform.OSXWebPlayer, "WebPlayer"},
            { RuntimePlatform.WindowsPlayer, "Windows"},
            { RuntimePlatform.OSXPlayer, "OSX"},
        };
    #endif

    public void Initialize (AssetBundleManagerEvent<bool, AssetBundleManifest> onFinish = null)
    {
        StartCoroutine (InitializeCroutine (onFinish));
    }

    protected IEnumerator InitializeCroutine (AssetBundleManagerEvent<bool, AssetBundleManifest> onFinish = null)
    {
        DontDestroyOnLoad (gameObject);

#if UNITY_EDITOR

		Debug.Log ("EventDrivenBaseLoader Mode:" + (AssetBundleManager.SimulateAssetBundleInEditor ?
			"Editor Simulation" : "Normal") );

#endif

        AssetBundleManager.BaseDownloadingURL = new StringBuilder ()
			.Append (GetRelativePath ())
			.Append (kAssetBundlesPath)
			.Append (PlatformfolderForAssetBundles)
			.Append ("/")
			.ToString ();

        var request = AssetBundleManager.Initialize (PlatformfolderForAssetBundles);

        if (request != null)
            yield return GetDriver ().StartCoroutine (request);

        if (onFinish != null) 
		{
			var manifest = request.GetAsset<AssetBundleManifest> ();
			onFinish (manifest != null, manifest);
        }
    }


    public void GetAssetAsync<T> (string bundleName, string assetName,
                              AssetBundleManagerEvent<bool, T> callBack) where T : UnityEngine.Object
    {
//		Debug.Log("EventDrivenBaseLoader#GetAssetAsync bundleName", bundleName, "AssetName", assetName);

        StartCoroutine (GetAssetInternal<T> (bundleName, assetName, callBack));
    }

    IEnumerator GetAssetInternal<T> (string bundleName, string assetName,
                                 AssetBundleManagerEvent<bool, T> callBack) where T : UnityEngine.Object
    {

        AssetBundleLoadAssetOperation operation = AssetBundleManager.LoadAssetAsync (bundleName,
                                                assetName, typeof(T));

        if (operation == null) {
            callBack (false, null);
            yield break;
        }

        yield return StartCoroutine (operation);

        T asset = operation.GetAsset<T> ();

        var isSuccess = asset != null;

        AssetBundleManager.UnloadAssetBundle (bundleName);
        callBack (isSuccess, asset);

    }


    public void GetAssetsAsync<T> (string bundleName, string[] assetNames,
                               AssetBundleManagerEvent<bool, T[]>callBack) where T :UnityEngine.Object
    {
//		Debug.Log("EventDrivenBaseLoader#GetAssetsAsync bundleName", bundleName, "AssetName", assetNames);

        StartCoroutine (GetAssetsAsyncInternal (bundleName, assetNames, callBack));
    }

    IEnumerator GetAssetsAsyncInternal<T> (string bundleName, string[] assetNames,
                                       AssetBundleManagerEvent<bool, T[]>callBack) where T :UnityEngine.Object
    {

        var operations = new List<AssetBundleLoadAssetOperation> ();

        foreach (var assetName in assetNames) {
            var request = AssetBundleManager.LoadAssetAsync (bundleName, assetName, typeof(T));
            operations.Add (request);
            yield return StartCoroutine (request);
        }

        var isNullCount = operations.Count (operation => operation.GetAsset<T> () == null);
        var assets = operations.Select (operation => operation.GetAsset<T> ())
			.ToArray ();

        AssetBundleManager.UnloadAssetBundle (bundleName);
        callBack (isNullCount == 0, assets);

    }

    public string GetRelativePath ()
    {
        if (!string.IsNullOrEmpty (AssetBundleRootUrl))
            return AssetBundleRootUrl;
        else if (Application.isEditor)
            return "file://" + System.Environment.CurrentDirectory.Replace ("\\", "/");
        else if (Application.isWebPlayer)
            return System.IO.Path.GetDirectoryName (Application.absoluteURL).Replace ("\\", "/") + "/StreamingAssets";
        else if (Application.isMobilePlatform || Application.isConsolePlatform)
            return Application.streamingAssetsPath;
        else
            return "file://" + Application.streamingAssetsPath;
    }

    public string PlatformfolderForAssetBundles {
        get {

#if UNITY_EDITOR
			return sPlatformFolderMap[EditorUserBuildSettings.activeBuildTarget];
#else
            return sRuntimePlatformFolderMap [Application.platform];
#endif

        }
    }


    public void StopSelfCoroutines ()
    {
        Destroy (mCoroutineDriver);
    }

    private MonoBehaviour GetDriver ()
    {

        if (mCoroutineDriverParent == null) {
            mCoroutineDriverParent = new GameObject ("EventDrivenBaseLoaderCoroutine");
            DontDestroyOnLoad (mCoroutineDriverParent);
            mCoroutineDriver = mCoroutineDriverParent.AddComponent<MonoBehaviour> ();
        }

        return mCoroutineDriver;

    }

    public void Clear ()
    {
        mCoroutineDriver = null;
        Destroy (mCoroutineDriverParent);
    }


}
