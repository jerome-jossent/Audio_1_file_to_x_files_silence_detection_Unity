using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class Polygon2D : MonoBehaviour
{
    MeshRenderer meshRenderer;
    public Color RGBA;
    public Vector2[] points;

    void OnValidate()
    {
        DrawPolygon();
    }
    void Start()
    {
        DrawPolygon();
    }

    public void DrawPolygon()
    {
        if (points == null) return;

        Vector3[] vertices3D = ConvertVector2ToVector3Array(points);

        // calcul les indices avec Triangulator
        int[] indices = new Triangulator(points).Triangulate();

        // créé le mesh
        Mesh mesh = new Mesh
        {
            vertices = vertices3D,
            triangles = indices,
        };

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        // update le mesh
        MeshFilter filter = gameObject.GetComponent<MeshFilter>();
        if (filter == null) filter = gameObject.AddComponent<MeshFilter>();
        filter.mesh = mesh;

        //update le material
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        if (meshRenderer == null) meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = new Material(Shader.Find("Unlit/Transparent Colored"));
        SetColor(RGBA);
    }

    public void InsertPoint(int index)
    {
        InsertPoint(index, points[index]);
    }

    public void InsertPoint(int index, Vector2 newPoint)
    {
        List<Vector2> l = points.ToList();
        l.Insert(index, newPoint);
        points = l.ToArray();
        DrawPolygon();
    }

    public void DeletePoint(int index)
    {
        List<Vector2> l = points.ToList();
        l.RemoveAt(index);
        points = l.ToArray();
        DrawPolygon();
    }

    void SetColor(Color color)
    {
        meshRenderer?.sharedMaterial.SetColor("_Color", color);
    }

    Vector3[] ConvertVector2ToVector3Array(Vector2[] v2)
    {
        Vector3[] v3 = new Vector3[v2.Length];
        for (int i = 0; i < v2.Length; i++)
            v3[i] = new Vector3(v2[i].x, v2[i].y);
        return v3;
    }
}
