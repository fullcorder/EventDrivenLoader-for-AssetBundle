using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TestEventDrivenBaseLoader : EventDrivenBaseLoader
{
	[SerializeField]
	private Image testSprite;

	private List<KeyValuePair<string, Action>> mHandlerList = new List<KeyValuePair<string, Action>> ();


	void Start ()
	{

		Initialize (OnInitialize);

		//GUI Button for test.
		AddHandler ("TestLoadCard1", TestLoadCard1);
		AddHandler ("TestLoadCard2", TestLoadCard2);
	}

	void OnGUI ()
	{
		DrawButton ();
	}

	void OnInitialize (bool isSuccess, AssetBundleManifest result)
	{
		Debug.Log ("GameController#OnInitialize " + "isSuccess: " + isSuccess + " result: " + result);
	}

	void TestLoadCard1 ()
	{
		GetAssetAsync<Texture2D> (

			"card.unity3d",
			"f001",
			(success, asset) => {

				Debug.Log ("GetAssetAsync");
				Debug.Log ("Success: " + success);
				Debug.Log ("Asset: " + asset);

				if (!success)
					return;

				Sprite sprite= Sprite.Create (asset, new Rect (0, 0, asset.width, asset.height), Vector2.zero, 1.0f);
				testSprite.sprite = sprite;
			

			}
		);
	}

	void TestLoadCard2()
	{
		GetAssetAsync<Texture2D> (

			"card.unity3d",
			"f002",
			(success, asset) => {

				Debug.Log ("GetAssetAsync");
				Debug.Log ("Success: " + success);
				Debug.Log ("Asset: " + asset);

				if (!success)
					return;

				Sprite sprite = Sprite.Create (asset, new Rect (0, 0, asset.width, asset.height), Vector2.zero, 1.0f);
				testSprite.sprite = sprite;
			

			}
		);
	}

	//Show GUI Button For Test.
	void DrawButton ()
	{
		GUILayoutOption[] options = {};

		GUILayout.BeginVertical ();

		mHandlerList.ForEach ((pair) => {

			if (GUILayout.Button (pair.Key, options)) {
				pair.Value ();
			}
		});

		GUILayout.EndHorizontal ();
	}

	//Add Button and CallBack.
	void AddHandler (string label, Action handler)
	{
		mHandlerList.Add (new KeyValuePair<string, Action> (label, handler));	
	}
	
}

