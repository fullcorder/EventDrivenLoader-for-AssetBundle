##Project Info

### Env
| name | value |
|:-----------|:------|:------|
| Unity version |5.0.2f1||

### Assets
| name | value |
|:------|:------|
|AssetBunlde Demo|http://files.unity3d.com/vincent/assetbundle-demo/users_assetbundle-demo.zip|
|JewelSaviorFREE|http://www.jewel-s.jp/|


Load AssetBunlde Like this.

```csharp
GetAssetAsync<Texture2D> (

	"card.unity3d",
	"f001",
	(success, asset) => { //Callback

		Debug.Log ("GetAssetAsync");
		Debug.Log ("Success: " + success);
		Debug.Log ("Asset: " + asset);

		if (!success)
			return;

		Sprite sprite= Sprite.Create (asset, new Rect (0, 0, asset.width, asset.height), Vector2.zero, 1.0f);
		testSprite.sprite = sprite;

	}
);
```

