using UnityEngine;

namespace ExtEditor.UberMaterialPropertyDrawer
{
	internal readonly struct GeneratedTextureOptions
	{
		public readonly int Resolution;
		private readonly int _channelNum;
		public readonly bool UseHalfTexture;

		public GeneratedTextureOptions(int resolution=256, int channelNum=1, bool useHalfTexture=false)
		{
			Resolution = resolution;
			_channelNum = channelNum;
			UseHalfTexture = useHalfTexture;
		}

		public TextureFormat ResolveTextureFormat()
		{
			if (UseHalfTexture)
			{
				if (_channelNum == 1) return TextureFormat.RHalf;
				if (_channelNum == 2) return TextureFormat.RGHalf;
				return TextureFormat.RGBAHalf;
			}

			if (_channelNum == 1) return TextureFormat.R8;
			if (_channelNum == 2) return TextureFormat.RG16;
			if (_channelNum == 3) return TextureFormat.RGB24;
			return TextureFormat.RGBA32;
		}
	}
}