namespace ExtEditor.UberMaterialPropertyDrawer
{
	internal struct CurveTextureOptions
	{
		internal readonly GeneratedTextureOptions CommonOptions;
		internal readonly bool Accumulate;

		public CurveTextureOptions(GeneratedTextureOptions commonOptions = new GeneratedTextureOptions(), bool accumulate = false)
		{
			CommonOptions = commonOptions;
			Accumulate = accumulate;
		}
	}
}