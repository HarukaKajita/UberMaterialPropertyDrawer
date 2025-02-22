using System.IO;
using Autodesk.Fbx;
using UnityEditor;
using UnityEngine;
using UnityEditor.Formats.Fbx.Exporter;

namespace FBXScaleAutomationExtension
{
    public class FbxGlobalScaleChanger
    {
        public static void ChangeFBXScale (string inputFileName, string outputFileName, float scale)
        {
            using(FbxManager fbxManager = FbxManager.Create ()){
                // configure IO settings.
                fbxManager.SetIOSettings (FbxIOSettings.Create (fbxManager, Globals.IOSROOT));

                // Import the scene to make sure file is valid
                using (FbxImporter importer = FbxImporter.Create (fbxManager, "myImporter")) {

                    // Initialize the importer.
                    bool status = importer.Initialize (inputFileName, -1, fbxManager.GetIOSettings ());

                    // Create a new scene so it can be populated by the imported file.
                    FbxScene scene = FbxScene.Create (fbxManager, "myScene");
                    
                    //Debug
                    Debug.Log("Sysytemunit : " + scene.GetGlobalSettings().GetSystemUnit());
                    
                    // Import the contents of the file into the scene.
                    importer.Import (scene);
                    
                    //Write Scale
                    var globalSettings = scene.GetGlobalSettings();
                    globalSettings.SetSystemUnit(new FbxSystemUnit(scale));
                    //Debug
                    Debug.Log("Sysytemunit : " + scene.GetGlobalSettings().GetSystemUnit());
                    
                    // Export the scene
                    using (FbxExporter exporter = FbxExporter.Create (fbxManager, "myExporter")) {

                        // Initialize the exporter.
                        bool exporterStatus = exporter.Initialize (outputFileName, -1, fbxManager.GetIOSettings ());

                        // Create a new scene to export
                        // FbxScene scene = FbxScene.Create (fbxManager, "myScene");

                        // Export the scene to the file.
                        exporter.Export (scene);
                        AssetDatabase.Refresh();
                        Debug.Log("Save Converted FBX");
                    }
                }
            }
        }
    }
   
}