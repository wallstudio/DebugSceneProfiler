using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;

[ScriptedImporter(1, "cube")]
public class PrimitiveImporter : ScriptedImporter
{
    [Serializable]
    class MeshData
    {
        public List<Vector3> vertices = new List<Vector3>()
        {
            Vector3.left, Vector3.right, Vector3.up,
        };
        public List<int> trianles = new List<int>()
        {
            0, 1, 2,
        };
    }

    public float m_Scale = 1;

    public override void OnImportAsset(AssetImportContext ctx)
    {
        var j = JsonUtility.ToJson(new MeshData(), true);
        File.WriteAllText(@"C:\Users\huser\Desktop\j.json", j);

        Debug.Log("Reimpo");
        var cube = new GameObject();
        var mData = JsonUtility.FromJson<MeshData>(File.ReadAllText(ctx.assetPath));
        var m = new Mesh();
        m.SetVertices(mData.vertices);
        m.SetTriangles(mData.trianles, 0);
        var mf = cube.AddComponent<MeshFilter>();
        mf.mesh = m;
        var material = new Material(Shader.Find("Standard"));
        material.color = Color.green;
        var mr = cube.AddComponent<MeshRenderer>();
        UnityEngine.Object map;
        GetExternalObjectMap().TryGetValue(new SourceAssetIdentifier(typeof(Material), "Standard"), out map);
        mr.material =  map as Material ?? material;


        // 'cube' は ゲームオブジェクトで、自動的にプレハブに転換されます
        // ( 'Main Asset' だけがプレハブになります)
        ctx.AddObjectToAsset("main obj", cube);;
        ctx.AddObjectToAsset("my Material", material);
        ctx.AddObjectToAsset("mesh", m);
        ctx.SetMainObject(cube);

        // インポート出力としてコンテキストに渡されないアセットは破棄する必要があります
        var tempMesh = new Mesh();
        DestroyImmediate(tempMesh);
    }
}