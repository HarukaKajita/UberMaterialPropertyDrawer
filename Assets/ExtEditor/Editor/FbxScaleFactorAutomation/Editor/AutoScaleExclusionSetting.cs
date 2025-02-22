using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace FBXScaleAutomationExtension
{
    [CreateAssetMenu(fileName = "AutoScaleExclusion", menuName = "ScriptableObjects/AutoScaleExclusion")]
    public class AutoScaleExclusionSetting : ScriptableObject
    {
        public List<Object> exclusiveModels = new ();
        [SerializeField, HideInInspector] private List<string> exclusiveModelsPath = new();
        [SerializeField, HideInInspector] private List<string> removed = new();
        [SerializeField, HideInInspector] private List<string> additional = new();

        public List<string> GetExclusiveModelPath()
        {
            return exclusiveModelsPath;
        }
        private void OnValidate()
        {
            var newPathList = FetchExclusiveModelPath().ToList();
            //リストから除かれたパスを記録
            removed.AddRange(exclusiveModelsPath.Except(newPathList).ToList());
            removed = removed.Distinct().ToList();
            //リストに追加されたパスを記録
            additional.AddRange(newPathList.Except(exclusiveModelsPath).ToList());
            additional = additional.Distinct().ToList();
            
            exclusiveModelsPath = newPathList;
        }

        public List<string> FetchExclusiveModelPath()
        {
            var result = exclusiveModels
                .Where(x => x != null)
                .Select(AssetDatabase.GetAssetPath)
                .ToList();
            return result;
        }

        public void Reimport()
        {
            //reimportして反映
            Debug.Log("Reimport Removed : \n"+string.Join("\n", removed));
            foreach (var path in removed)
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);

            Debug.Log("Reimport Additional : \n" + string.Join("\n", additional));
            foreach (var path in additional)
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
            
            removed.Clear();
            additional.Clear();
        }
        
        public void Revert()
        {
            var additionalModels = additional.Select(AssetDatabase.LoadAssetAtPath<Object>);
            var removedModels = removed.Select(AssetDatabase.LoadAssetAtPath<Object>);
            exclusiveModels = exclusiveModels.Except(additionalModels).ToList();
            exclusiveModels.AddRange(removedModels);
            additional.Clear();
            removed.Clear();
            exclusiveModelsPath = FetchExclusiveModelPath();
        }
    }
}
