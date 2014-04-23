using UnityEngine;
using System.Collections;
using System;
namespace Extensions
{
	public static class UITextureExtensions 
	{
		/*
		public static void SetTextureImageWithName(this UITexture texture, Texture2DAtlas atlas, string imageName)
		{
			if(imageName==null) return;
			if(atlas== null) return;
			if (atlas.AtlasMaterial != texture.material)
			{
				texture.mainTexture = null;
				texture.material = atlas.AtlasMaterial;
			}
			texture.uvRect = atlas.GetRect(imageName);
		}

		public static void SetMaterialWithUVRect(this UITexture texture, Material material, Rect uvrect)
		{
			if(material==null) return;
			if (material != texture.material)
			{
				texture.mainTexture = null;
				texture.material = material;
			}
			texture.uvRect = uvrect;
		}
		*/
	}
}