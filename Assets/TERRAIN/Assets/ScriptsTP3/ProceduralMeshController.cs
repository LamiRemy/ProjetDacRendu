using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class ProceduralMeshController : MonoBehaviour {

    private static List<Vector3> _vertices;

    private static List<int> indices;
    private static Dictionary<uint, int> newVectices;

    protected static Mesh p_mesh;

    static Vector3[] vertices;
    static Vector3[] normales;
    static Vector2[] uvs;
    static int[] triangles;

    //Créer le mesh et appele la fonction InitNeighbour du model pour créer la liste de liste des voisins en fonction du mesh créer
    public static void CreationMesh()
    {
        p_mesh = GameObject.FindGameObjectWithTag("IHM").GetComponent<MeshFilter>().mesh;
        vertices = p_mesh.vertices;
        triangles = p_mesh.triangles;
        normales = p_mesh.normals;
        uvs = p_mesh.uv;

        //float radius = 1f;
        //// Longitude |||
        //int nbLong = 24;
        //// Latitude ---
        //int nbLat = 16;

        //#region Vertices
        //vertices = new Vector3[(nbLong + 1) * nbLat + 2];
        //float _pi = Mathf.PI;
        //float _2pi = _pi * 2f;

        //vertices[0] = Vector3.up * radius;
        //for (int lat = 0; lat < nbLat; lat++)
        //{
        //    float a1 = _pi * (float)(lat + 1) / (nbLat + 1);
        //    float sin1 = Mathf.Sin(a1);
        //    float cos1 = Mathf.Cos(a1);

        //    for (int lon = 0; lon <= nbLong; lon++)
        //    {
        //        float a2 = _2pi * (float)(lon == nbLong ? 0 : lon) / nbLong;
        //        float sin2 = Mathf.Sin(a2);
        //        float cos2 = Mathf.Cos(a2);

        //        vertices[lon + lat * (nbLong + 1) + 1] = new Vector3(sin1 * cos2, cos1, sin1 * sin2) * radius;
        //    }
        //}
        //vertices[vertices.Length - 1] = Vector3.up * -radius;
        //#endregion

        //#region Normales		
        //normales = new Vector3[vertices.Length];
        //for (int n = 0; n < vertices.Length; n++)
        //    normales[n] = vertices[n].normalized;
        //#endregion

        //#region UVs
        //uvs = new Vector2[vertices.Length];
        //uvs[0] = Vector2.up;
        //uvs[uvs.Length - 1] = Vector2.zero;
        //for (int lat = 0; lat < nbLat; lat++)
        //    for (int lon = 0; lon <= nbLong; lon++)
        //        uvs[lon + lat * (nbLong + 1) + 1] = new Vector2((float)lon / nbLong, 1f - (float)(lat + 1) / (nbLat + 1));
        //#endregion

        //#region Triangles
        //int nbFaces = vertices.Length;
        //int nbTriangles = nbFaces * 2;
        //int nbIndexes = nbTriangles * 3;
        //triangles = new int[nbIndexes];

        ////Top Cap
        //int i = 0;
        //for (int lon = 0; lon < nbLong; lon++)
        //{
        //    triangles[i++] = lon + 2;
        //    triangles[i++] = lon + 1;
        //    triangles[i++] = 0;
        //}

        ////Middle
        //for (int lat = 0; lat < nbLat - 1; lat++)
        //{
        //    for (int lon = 0; lon < nbLong; lon++)
        //    {
        //        int current = lon + lat * (nbLong + 1) + 1;
        //        int next = current + nbLong + 1;

        //        triangles[i++] = current;
        //        triangles[i++] = current + 1;
        //        triangles[i++] = next + 1;

        //        triangles[i++] = current;
        //        triangles[i++] = next + 1;
        //        triangles[i++] = next;
        //    }
        //}

        ////Bottom Cap
        //for (int lon = 0; lon < nbLong; lon++)
        //{
        //    triangles[i++] = vertices.Length - 1;
        //    triangles[i++] = vertices.Length - (lon + 2) - 1;
        //    triangles[i++] = vertices.Length - (lon + 1) - 1;
        //}
        //#endregion

        //p_mesh.vertices = vertices;
        //p_mesh.normals = normales;
        //p_mesh.uv = uvs;
        //p_mesh.triangles = triangles;

        //p_mesh.name = "ProceduralMesh";
        //p_mesh.RecalculateBounds();

        //GameObject.FindGameObjectWithTag("IHM").GetComponent<MeshFilter>().mesh = p_mesh;
        //GameObject.FindGameObjectWithTag("IHM").AddComponent<MeshCollider>();

        ProceduralMeshModel.InitNeighbour(p_mesh);
    }

    //Modifie l'objet à sculpter en fonction des paramètre de l'outil
    public static void ToolForModification(MeshCollider meshCollider, RaycastHit hit, float rayonBrush, float forceBrush, string choice)
    {
        Vector3 dir = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));

        List<int> l_indice = new List<int>();

        l_indice.Add(triangles[hit.triangleIndex * 3 + 0]);
        l_indice.Add(triangles[hit.triangleIndex * 3 + 1]);
        l_indice.Add(triangles[hit.triangleIndex * 3 + 2]);

        bool notFinish = true;
        
        while (notFinish)
        {
            notFinish = false;
            for (int i = 0; i < l_indice.Count; i++)
            {
                for (int j = 0; j < ProceduralMeshModel.l_neighbour[l_indice[i]].Count; j++)
                {
                    if (Search(ProceduralMeshModel.l_neighbour[l_indice[i]][j], l_indice) && Mathf.Abs(Vector3.Distance(hit.point, p_mesh.vertices[ProceduralMeshModel.l_neighbour[l_indice[i]][j]])) < rayonBrush)
                    {
                        notFinish = true;
                        l_indice.Add(ProceduralMeshModel.l_neighbour[l_indice[i]][j]);
                    }
                }
            }
        }

        if (choice == "Depression")
        {
            for (int i = 0; i < l_indice.Count; i++)
                vertices[l_indice[i]] -= (dir * Time.deltaTime * (forceBrush/2) * (1 - (Mathf.Abs(Vector3.Distance(hit.point, p_mesh.vertices[l_indice[i]])) / rayonBrush)));
        }
        if (choice == "Elevation")
        {
            for (int i = 0; i < l_indice.Count; i++)
                vertices[l_indice[i]] += (dir * Time.deltaTime * (forceBrush/2) * (1 - (Mathf.Abs(Vector3.Distance(hit.point, p_mesh.vertices[l_indice[i]])) / rayonBrush)));
        }

        p_mesh.vertices = vertices;
        p_mesh.RecalculateNormals();
        GameObject.FindGameObjectWithTag("IHM").GetComponent<MeshFilter>().mesh = p_mesh;
        Destroy(GameObject.FindGameObjectWithTag("IHM").GetComponent<MeshCollider>());
        GameObject.FindGameObjectWithTag("IHM").AddComponent<MeshCollider>();
    }

    private static bool Search(int i, List<int> tab)
    {
        for(int j = 0; j<tab.Count; j++)
        {
            if(i == tab[j])
            {
                return false;
            }
        }
        return true;
    }
}
