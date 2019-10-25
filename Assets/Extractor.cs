using UnityEngine;
using UnityEditor;

public class Extractor
{
    [MenuItem("Assets/ExtractFromAsset")]
    public static void ExtractFromAsset()
    {
        @ExtractFromAsset(Selection.activeObject,
            AssetDatabase.GetAssetPath(Selection.activeObject) + "_SUaB.mat");
    }

    public static void @ExtractFromAsset(Object subAsset, string destinationPath)
    {   
        var id = new AssetImporter.SourceAssetIdentifier(typeof(Material), "Standard");
        var main = AssetImporter.GetAtPath("Assets/maki.cube");
        main.AddRemap(id, subAsset);


        // var clone = Object.Instantiate(subAsset);
        // AssetDatabase.CreateAsset(clone, destinationPath);

        var ai = AssetImporter.GetAtPath("Assets/maki.cube");
        // var assetImporter = AssetImporter.GetAtPath(assetPath);
        // assetImporter.AddRemap(new AssetImporter.SourceAssetIdentifier(subAsset), clone);

        // AssetDatabase.WriteImportSettingsIfDirty(assetPath);
        // AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);

        foreach (var item in ai.GetExternalObjectMap())
        {
            var k = item.Key;
            var v = item.Value;
            var i = v.GetInstanceID();
            var p = AssetDatabase.GetAssetPath(i);
            var g = AssetDatabase.AssetPathToGUID(p);
            Debug.Log(g);
        }
    }
}