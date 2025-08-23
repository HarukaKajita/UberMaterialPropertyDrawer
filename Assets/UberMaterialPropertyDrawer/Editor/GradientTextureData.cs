using UnityEngine;

namespace ExtEditor.UberMaterialPropertyDrawer
{
    public class GradientTextureData : ScriptableObject
    {
        public Gradient gradient = new Gradient();
        [HideInInspector] public Texture2D texture;
    }
}
