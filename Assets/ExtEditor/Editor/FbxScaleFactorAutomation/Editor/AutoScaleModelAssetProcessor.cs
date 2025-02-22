using System;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace FBXScaleAutomationExtension
{
    public class AutoScaleModelAssetProcessor : AssetPostprocessor
    {
        //モデルファイルのインポート時にUnityのUnitLengthとモデルが書き出されたDCCツールのUnitLengthを一致させる変換を自動出かける
        void OnPreprocessModel()
        {
            // Debug.Log("OnPreProcessModel : " + assetImporter.assetPath);
            ModelImporter modelImporter = assetImporter as ModelImporter;
            
            bool useAutoScale = true;
            var exclusionDataPathList = AssetDatabase.FindAssets("t:" + nameof(AutoScaleExclusionSetting));

            //自動スケールの処理から除外されているかチェック
            foreach (var guid in exclusionDataPathList)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var data = AssetDatabase.LoadAssetAtPath<AutoScaleExclusionSetting>(path);
                var modelPathList = data.GetExclusiveModelPath();
                foreach (var modelPath in modelPathList)
                {
                    Debug.Log(modelPath);
                    if (modelPath == assetImporter.assetPath)
                    {
                        Debug.Log(modelPath +　"のUnit Lengthの自動変換はスキップされました。\n" +path + "でスキップ対象のモデルとして設定されています。");
                        useAutoScale = false;
                        break;
                    }
                }
                if(!useAutoScale) break;
            }
            
            if (useAutoScale)
            {
                modelImporter.useFileScale = false;
                // modelImporter.useFileUnits = true;
            }
        }

    }
}
