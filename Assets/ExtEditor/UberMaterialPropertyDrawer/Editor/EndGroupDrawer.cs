using ExtEditor.UberMaterialPropertyDrawer.Editor;
using UnityEditor;
using UnityEngine;

public class EndGroupDrawer : MaterialPropertyDrawer
{
    private readonly string _groupName = "";
    private readonly int _indentNum = 0;
    private readonly string _memo;
    public EndGroupDrawer(string groupName, string memo = "")
    {
        this._groupName = groupName;
        this._memo = memo;
        this._indentNum = Mathf.Max(0,UberDrawer.GetGroupIntentLevel()-1);
        var popGroup = UberDrawer.PopGroup();
        if (groupName != popGroup) Debug.LogError("Not Corresponded Group Begin-End : " + popGroup + " - " + groupName);
    }

    public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
    {
        return 0;
    }

    public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
    { 
        EndPanel();
    }

    private void EndPanel()
    {
        EditorGUI.indentLevel -= 1;
    }
}
