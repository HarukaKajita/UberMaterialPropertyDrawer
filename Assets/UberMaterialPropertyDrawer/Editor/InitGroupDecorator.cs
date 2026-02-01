using UnityEditor;
using UnityEngine;

namespace ExtEditor.UberMaterialPropertyDrawer
{
    public sealed class InitGroupDecorator : MaterialPropertyDrawer
    {
        public InitGroupDecorator()
        {
        }

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            UberDrawerLogger.Log($"GetPropertyHeight: {GetType().Name}");
            var data = GroupDataCache.GetOrCreate(UberDrawerBase.GetTargetMaterial(editor));
            BeginPassIfLayout(data);
            UberGroupState.ResetNest(data);
            return 0f;
        }

        public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
        {
            var data = GroupDataCache.GetOrCreate(UberDrawerBase.GetTargetMaterial(editor));
            BeginPass(data);
            UberGroupState.ResetNest(data);
        }

        private static void BeginPassIfLayout(GroupData data)
        {
            var current = Event.current;
            if (current == null)
            {
                UberGroupState.BeginPass(data);
                return;
            }
            if (current.type != EventType.Layout) return;
            UberGroupState.BeginPass(data);
        }

        private static void BeginPass(GroupData data)
        {
            UberGroupState.BeginPass(data);
        }
    }
}
