using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace FBXScaleAutomationExtension
{
    public class AdditionalMenu
    {
        enum UnitType
        {
            Meter,
            Cm,
            Inch
        }
        [MenuItem("FBX/Convert To Meter")]
        static void ConvertToMeter()
        {
            ConvertFBXScale(UnitType.Meter);
        }
        [MenuItem("FBX/Convert To Cm")]
        static void ConvertToCentiMeter()
        {
            ConvertFBXScale(UnitType.Cm);
        }
        [MenuItem("FBX/Convert To Inch")]
        static void ConvertToInch()
        {
            ConvertFBXScale(UnitType.Inch);
        }
        
        static void ConvertFBXScale(UnitType unitType)
        {
            var inputs = new List<string>();
            var outputs = new List<string>();
            if(Selection.assetGUIDs != null && Selection.assetGUIDs.Length > 0)
            {
                inputs = Selection.assetGUIDs.Select(guid =>  Path.GetFullPath(AssetDatabase.GUIDToAssetPath(guid))).ToList();
                outputs = inputs.Select(path =>
                {
                    var split = path.Split(".");
                    return split[0] + "_"+ unitType +"." + split[1];
                }).ToList();
                inputs.ForEach(Debug.Log);
                outputs.ForEach(Debug.Log);
            }

            float scale = 1;
            if (unitType == UnitType.Meter) scale = 100;
            if (unitType == UnitType.Inch) scale = 2.54f;
            for(var i = 0; i < inputs.Count; i++)
                FbxGlobalScaleChanger.ChangeFBXScale(inputs[i], outputs[i], scale);
            // FbxScaleFactorAutomation.ExportScene(scene, "D:\\UnityProjects\\FbxScaleFactor\\Assets\\out_test.fbx");
        }
    }
}
