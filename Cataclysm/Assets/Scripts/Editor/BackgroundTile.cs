using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BackgroundTile : Tile 
{
	[SerializeField]
	private string baseName="Tiles_";
	[SerializeField]
	private Sprite[] backgroundSprites;

//	[SerializeField]
//	private Sprite preview;

	public override void RefreshTile(Vector3Int position, ITilemap Tilemap)
	{
		for (int y = -1; y <= 1; y++)
		{
			for (int x = -1; x <= 1; x++)
			{
				Vector3Int nPos = new Vector3Int (position.x + x, position.y + y, position.z);

				if (HasBackground (Tilemap, nPos))
				{
					Tilemap.RefreshTile (nPos);
				}
			}
		}
	}

	public override void GetTileData (Vector3Int position, ITilemap tilemap, ref TileData tileData)
	{
		string composition = string.Empty;

		for (int x = -1; x <= 1; x++)
		{
			for (int y = -1; y <= 1; y++)
			{
				if (!(x == 0 && y == 0))
				{
					composition += HasBackground (tilemap, new Vector3Int (position.x + x, position.y + y, position.z)) ? 'B' : 'E';
				}
			}
		}

		tileData.sprite = GetCorrectSprite (composition);
	}

	private bool HasBackground(ITilemap tilemap, Vector3Int position)
	{
		return tilemap.GetTile (position) == this;
	}

	private bool WildCompare(string a, string b)
	{
		int baseLength = baseName.Length;
		for(int c=0;c<b.Length;c++)
		{
			if (a[c+baseLength]!='.')
			{
				if (a [c+baseLength] != b [c])
				{
					return false;
				}
			}
		}
		return true;
	}

	private Sprite GetCorrectSprite(string composition)
	{
		foreach (var s in backgroundSprites)
		{
			if (WildCompare (s.name, composition))
			{
				return s;
			}
		}
		return backgroundSprites [19];
	}

#if UNITY_EDITOR
	[MenuItem("Assets/Create/Tiles/BackgroundTile")]
	public static void CreateBackgroundTile()
	{
		string path = EditorUtility.SaveFilePanelInProject ("Save Background Tile", "NewBackgroundTile", "asset", "Save Background Tile", "Assets/Tilesets/AutoPalette");
		if (path == "")
		{
			return;
		}
		AssetDatabase.CreateAsset (ScriptableObject.CreateInstance<BackgroundTile> (), path);
	}
#endif
}
