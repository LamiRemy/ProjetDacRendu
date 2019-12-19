using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class CreateCube : MonoBehaviour
{
    private Mesh p_mesh;
    private Vector3[] p_vertices;
    private Vector3[] p_normals;
    private int[] p_triangles;

    public enum TypeCube { Cube24, Cube8 }
    public TypeCube typeCube;
    public float width;


    Vector3 p0;
    Vector3 p1;
    Vector3 p2;
    Vector3 p3;
    Vector3 p4;
    Vector3 p5;
    Vector3 p6;
    Vector3 p7;

    void Awake()
    {
        float w = -width / 2.0f;
        float W = width / 2.0f;
        p0 = new Vector3(w, w, w);
        p1 = new Vector3(w, W, w);
        p2 = new Vector3(W, W, w);
        p3 = new Vector3(W, w, w);
        p4 = new Vector3(w, w, W);
        p5 = new Vector3(w, W, W);
        p6 = new Vector3(W, W, W);
        p7 = new Vector3(W, w, W);

        switch (typeCube)
        {
            case TypeCube.Cube8:
                CreerCube8(); break;
            case TypeCube.Cube24:
                CreerCube24(); break;
        }
    }

    private void CreerCube24()
    {
        p_mesh = new Mesh();
        p_mesh.name = "MyProceduralCube";
        p_vertices = new Vector3[]{
            p0,p1,p2,p3, // devant
            p4,p5,p1,p0, // gauche
            p3,p2,p6,p7, // Droite
            p7,p6,p5,p4, // Derrière
            p1,p5,p6,p2, // Dessus
            p4,p0,p3,p7 // dessous
        };
        p_triangles = new int[12 * 3];
        int index = 0;
        for (int i = 0; i < 6; i++) // 6 faces à 2 triangles
        { // triangle 1
            p_triangles[index++] = i * 4;
            p_triangles[index++] = i * 4 + 1;
            p_triangles[index++] = i * 4 + 3;
            // triangle 2
            p_triangles[index++] = i * 4 + 1;
            p_triangles[index++] = i * 4 + 2;
            p_triangles[index++] = i * 4 + 3;
        }
        index = 0;
        int index2 = 0;
        p_normals = new Vector3[24];
        for (int ind = 0; ind < 6; ind++)
        {
            int i = index;
            int j = index + 1;
            int k = index + 3;
            Vector3 v1 = p_vertices[j] - p_vertices[i];
            Vector3 v2 = p_vertices[k] - p_vertices[i];
            Vector3 pv = Vector3.Cross(v1, v2);
            Vector3 n = pv / pv.magnitude;

            p_normals[index2++] = n;
            p_normals[index2++] = n;
            i = index + 1;
            j = index + 2;
            k = index + 3;
            v1 = p_vertices[j] - p_vertices[i];
            v2 = p_vertices[k] - p_vertices[i];
            pv = Vector3.Cross(v1, v2);
            n = pv / pv.magnitude;

            p_normals[index2++] = n;
            p_normals[index2++] = n;
            index += 4;
        }

        p_mesh.Clear();
        p_mesh.vertices = p_vertices;
        p_mesh.triangles = p_triangles;
        p_mesh.normals = p_normals;
        GetComponent<MeshFilter>().mesh = p_mesh;
        gameObject.AddComponent<MeshCollider>();
    }
    private void CreerCube8()
    {
        p_mesh = new Mesh();
        p_mesh.name = "MyProceduralCube";
        p_vertices = new Vector3[]{
            p0,p1,p2,p3,
            p4,p5,p6,p7
        };
        p_triangles = new int[12 * 3];
        int index = 0;
        //face 1
        p_triangles[index++] = 0;
        p_triangles[index++] = 1;
        p_triangles[index++] = 3;
        p_triangles[index++] = 1;
        p_triangles[index++] = 2;
        p_triangles[index++] = 3;
        //face 2
        p_triangles[index++] = 4;
        p_triangles[index++] = 5;
        p_triangles[index++] = 0;
        p_triangles[index++] = 5;
        p_triangles[index++] = 1;
        p_triangles[index++] = 0;
        //face 3
        p_triangles[index++] = 3;
        p_triangles[index++] = 2;
        p_triangles[index++] = 7;
        p_triangles[index++] = 2;
        p_triangles[index++] = 6;
        p_triangles[index++] = 7;
        //face 4
        p_triangles[index++] = 7;
        p_triangles[index++] = 6;
        p_triangles[index++] = 4;
        p_triangles[index++] = 6;
        p_triangles[index++] = 5;
        p_triangles[index++] = 4;
        //face 5
        p_triangles[index++] = 1;
        p_triangles[index++] = 5;
        p_triangles[index++] = 2;
        p_triangles[index++] = 5;
        p_triangles[index++] = 6;
        p_triangles[index++] = 2;
        //face 6
        p_triangles[index++] = 4;
        p_triangles[index++] = 0;
        p_triangles[index++] = 7;
        p_triangles[index++] = 0;
        p_triangles[index++] = 3;
        p_triangles[index++] = 7;

        index = 0;
        int index2 = 0;
        p_normals = new Vector3[8];
        Vector3[] p_normalsTriangle = new Vector3[12];
        for (int ind = 0; ind < 12; ind++)
        {
            int i = p_triangles[index++];
            int j = p_triangles[index++];
            int k = p_triangles[index++];
            Vector3 v1 = p_vertices[j] - p_vertices[i];
            Vector3 v2 = p_vertices[k] - p_vertices[i];
            Vector3 pv = Vector3.Cross(v1, v2);
            Vector3 n = pv / pv.magnitude;

            p_normalsTriangle[index2++] = n;
        }

        for (int i = 0; i < 8; i++)
        {
            int[] triangles = new int[5];
            int idTriangle = 0;
            for (int it = 0; it < 36; it++)
            {
                if (p_triangles[it] == i)
                {
                    triangles[idTriangle++] = it / 3;
                }
            }

            p_normals[i] = p_normalsTriangle[triangles[0]] + p_normalsTriangle[triangles[1]] + p_normalsTriangle[triangles[2]] + p_normalsTriangle[triangles[3]] + p_normalsTriangle[triangles[4]];
            p_normals[i].Normalize();
        }

        p_mesh.Clear();
        p_mesh.vertices = p_vertices;
        p_mesh.triangles = p_triangles;
        p_mesh.normals = p_normals;
        GetComponent<MeshFilter>().mesh = p_mesh;
        gameObject.AddComponent<MeshCollider>();
    }

    
}
