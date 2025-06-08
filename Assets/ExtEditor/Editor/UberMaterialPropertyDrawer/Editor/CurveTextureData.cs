using UnityEngine;

namespace ExtEditor.UberMaterialPropertyDrawer
{
    public class CurveTextureData : ScriptableObject
    {
        public AnimationCurve curveR = AnimationCurve.Linear(0, 0, 1, 1);
        public AnimationCurve curveG = AnimationCurve.Linear(0, 0, 1, 1);
        public AnimationCurve curveB = AnimationCurve.Linear(0, 0, 1, 1);
        public AnimationCurve curveA = AnimationCurve.Linear(0, 0, 1, 1);
        [HideInInspector] public Texture2D texture;
    }
}
