using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Texture2DAtlas
{
	protected Texture2D texture;
	protected Material material;
	public Material AtlasMaterial
	{
		get {return material;}
	}

	static public Vector2 defaultScale = new Vector2(1.0f, 1.0f);
	protected Dictionary<string, Rect> rectsDict;
	public Dictionary<string, Rect> RectsDict
	{
		get { return rectsDict; }
	}
	public Rect GetRect(string key)
	{
		Rect result = new Rect(0f,0f,0f,0f);
		if (rectsDict != null && rectsDict.Keys.Count > 0)
		{
			if (rectsDict.ContainsKey(key))
			{
				result = rectsDict[key];
			}
			else
			{
				Debug.LogWarning("WARNING: Texture2DAtlas: rect not found for key: " + key);
				result = rectsDict[rectsDict.Keys.ElementAt(0)];
			}
		}
		else
		{
			Debug.LogWarning("WARNING: Texture2DAtlas: rectsDict is empty");
		}

		return result;
	}

	public Texture2DAtlas(Dictionary <string, Texture2D> texturesDict, Vector2 scale, int padding = 1, int maximumAtlasSize = 4096,  bool enableAnisotropy = true, bool compressResult = false)
	{
		texture = new Texture2D(maximumAtlasSize, maximumAtlasSize, TextureFormat.RGBA4444, false);
		texture.wrapMode = TextureWrapMode.Clamp;
		if (enableAnisotropy)
		{
			texture.filterMode = FilterMode.Trilinear;
			texture.anisoLevel = 4;
		}
		else
			texture.filterMode = FilterMode.Point;

		List<Texture2D> textures = new List<Texture2D>();
		List<string> keys = new List<string>();

		foreach (string key in texturesDict.Keys)
		{
			Texture2D tex = texturesDict[key];
			if (tex != null)
			{
				textures.Add(tex);
				keys.Add(key);
			}
		}

		Rect[] rects = texture.PackTextures(textures.ToArray(), padding, maximumAtlasSize);

		if (scale.x != 1.0f && scale.y != 1.0f)
			TextureScale.Point(texture, (int)(texture.width * scale.x), (int)(texture.height * scale.y));

		rectsDict = new Dictionary<string, Rect>();
		for (int i = 0; i<rects.Length; i++)
		{
			string key = keys[i];
			rectsDict[key] = rects[i];
		}

		if (compressResult)
			texture.Compress(true);

		material = new Material(Shader.Find("Unlit/Transparent Colored"));
		material.mainTexture = texture;
	}

	//Do not forget to call this method before RECREATION of texture atlas
	public void Unload()
	{
		if (texture != null)
			Texture2D.Destroy (texture);

		if (material != null)
			Material.Destroy (material);

		texture = null;
		material = null;
		rectsDict = null;
	}

}