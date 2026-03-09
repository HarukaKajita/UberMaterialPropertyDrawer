using UnityEditor;
using UnityEngine;

namespace ExtEditor.UberMaterialPropertyDrawer
{
    public class BeginGroupDrawer : UberDrawerBase
    {
        private string _memo;

        public BeginGroupDrawer(string groupName) : base(groupName)
        {
        }
        
        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            var data = GetGroupData(editor);
            var beginGroup = TryBeginGroup(editor, prop);
            if (beginGroup) BeginGroupScope(editor);
            var parentPath         = UberGroupState.GetParentPath(editor);
            var parentVisible = UberGroupState.IsParentScopeVisible(data, editor);
            var groupPath = UberGroupState.GetCurrentPath(editor);
            
            UberDrawerLogger.Log($"{GetType().Name}({GroupName}).GetPropertyHeight()");
            UberDrawerLogger.Log($"\t{nameof(beginGroup)}:{beginGroup}");
            UberDrawerLogger.Log($"\t{nameof(parentPath)}:{parentPath}");
            UberDrawerLogger.Log($"\t{nameof(groupPath)}:{groupPath}");
            UberDrawerLogger.Log($"\t{nameof(parentVisible)}:{parentVisible}");
            UberDrawerLogger.Log(GroupName + "のParent" + parentPath + "は" + (parentVisible ? "可視":"不可視"));
            return GroupVisibility.CanShowHeader(data, editor) ? GUIHelper.GroupHeaderHeight : GUIHelper.ClosedHeight;
        }

        public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
        {
            var data = GetGroupData(editor);
            var beginGroup = TryBeginGroup(editor, prop);
            if (beginGroup) BeginGroupScope(editor);
            
            var parentPath         = UberGroupState.GetParentPath(editor);
            var parentVisible = UberGroupState.IsParentScopeVisible(data, editor);
            var groupPath = UberGroupState.GetCurrentPath(editor);
            
            UberDrawerLogger.Log($"{GetType().Name}({GroupName}).OnGUI()");
            UberDrawerLogger.Log($"\t{nameof(beginGroup)}:{beginGroup}");
            UberDrawerLogger.Log($"\t{nameof(parentPath)}:{parentPath}");
            UberDrawerLogger.Log($"\t{nameof(groupPath)}:{groupPath}");
            UberDrawerLogger.Log($"\t{nameof(parentVisible)}:{parentVisible}");
            UberDrawerLogger.Log(GroupName + "のParent" + parentPath + "は" + (parentVisible ? "可視" : "不可視"));
            _memo = $" parent:{parentPath}";
            var state = UberGroupState.GetExpanded(data, groupPath);
            if (GroupVisibility.CanShowHeader(data, editor))
            {
                var newState = BeginPanel(position, data, state);
                if (state != newState)
                    UberGroupState.SetExpanded(data, groupPath, newState);
                EditorGUI.indentLevel++;
            }
        }

        private bool BeginPanel(Rect position, GroupData data, bool expanded)
        {
            UberDrawerLogger.Log("BeginPanel " + GroupName);
            var style = new GUIStyle("ShurikenModuleTitle");
            style.border = new RectOffset(7, 7, 4, 4); // Background edge tweaks.
            style.fixedHeight = GUIHelper.GroupHeaderHeight; // Background height.
            position.y += GUIHelper.GroupHeaderTopPadding;
            var bgRect = EditorGUI.IndentedRect(position);
            GUI.Box(bgRect, "", style); // Background.
            var interactiveRect = new Rect(bgRect.x, bgRect.y, bgRect.width, bgRect.height);
            var e = Event.current;
            if (e.type == EventType.Repaint)
            {
                EditorStyles.foldout.Draw(new Rect(position.x + 2 + GUIHelper.GetIndentWidth(), position.y, 18, 18), false, false,
                    expanded, false);
            }

            if (e.type == EventType.MouseDown && interactiveRect.Contains(e.mousePosition))
            {
                expanded = !expanded;
                e.Use();
            }

            EditorGUI.LabelField(
                new Rect(position.x + 18, position.y, 300, 18),
                GroupName + ":" + _memo,
                EditorStyles.boldLabel);

            return expanded;
        }

        private static bool TryBeginGroup(MaterialEditor editor, MaterialProperty prop)
        {
            return UberGroupState.TryRecordPush(editor, prop?.name);
        }
    }
}
