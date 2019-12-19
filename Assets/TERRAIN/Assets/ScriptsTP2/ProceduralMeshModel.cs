using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralMeshModel : MonoBehaviour {
    
    public static List<List<int>> l_neighbour;

    public static void InitNeighbour(Mesh mesh)
    {
        List<List<int>> returnList = new List<List<int>>();
        int k = 0;
        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            returnList.Add(new List<int>());
            returnList[i].Add(i);
            for (int j = 0; j < mesh.triangles.Length; j++)
            {
                if (mesh.triangles[j] == i)
                {
                    if (k == 0)
                    {
                        returnList[i].Add(mesh.triangles[j + 1]);
                        returnList[i].Add(mesh.triangles[j + 2]);
                    }
                    if (k == 1)
                    {
                        returnList[i].Add(mesh.triangles[j - 1]);
                        returnList[i].Add(mesh.triangles[j + 1]);
                    }
                    if (k == 2)
                    {
                        returnList[i].Add(mesh.triangles[j - 2]);
                        returnList[i].Add(mesh.triangles[j - 1]);
                    }
                }

                k++;
                if (k == 3)
                {
                    k = 0;
                }
            }
            for (int l = 0; l < mesh.vertices.Length; l++)
            {
                if (mesh.vertices[i] == mesh.vertices[l])
                {
                    returnList[i].Add(l);
                }
            }
        }
        l_neighbour = returnList;
    }
}
