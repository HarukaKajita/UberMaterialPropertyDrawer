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
            // BeginPassIfLayout(editor);
            UberGroupState.BeginPass(editor);
            UberGroupState.ResetNest(editor);
            return 0f;
        }

        public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
        {
            UberGroupState.BeginPass(editor);
            UberGroupState.ResetNest(editor);
        }

        // private static void BeginPassIfLayout(MaterialEditor editor)
        // {
        //     var current = Event.current;
        //     if (current == null)
        //     {
        //         UberGroupState.BeginPass(editor);
        //         return;
        //     }
        //     if (current.type != EventType.Layout) return;
        //     UberGroupState.BeginPass(editor);
        // }
    }
}
