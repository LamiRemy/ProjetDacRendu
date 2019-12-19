using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class createTerrain : MonoBehaviour
{
    [FormerlySerializedAs("GridSize")]
    [SerializeField]
    private int gridSize = 10;

    [FormerlySerializedAs("Prefabs")]
    [SerializeField]
    private GameObject[] prefabs;

    private Mesh _mesh;
    private MeshFilter _meshFilter;
    private Vector3[] _vertices;
    private int[] _triangles;
    private Vector3[] _normals;

    [SerializeField]
    private AnimationCurve lenghtCurve;

    private List<Vector2> alreadyHaveObj = new List<Vector2>();

    private void Awake()
    {
        Generate();
    }

    private void Generate()
    {
        GetComponent<MeshFilter>().mesh = _mesh = new Mesh();
        _meshFilter = GetComponent<MeshFilter>();
        _mesh.name = "Procedural Grid";

        _vertices = new Vector3[(gridSize + 1) * (gridSize + 1)];
        for (int i = 0, y = 0; y <= gridSize; y++)
        {
            for (int x = 0; x <= gridSize; x++, i++)
            {
                _vertices[i] = new Vector3(x, 0f, y);
            }
        }


        _triangles = new int[gridSize * gridSize * 6];
        for (int ti = 0, vi = 0, y = 0; y < gridSize; y++, vi++)
        {
            for (int x = 0; x < gridSize; x++, ti += 6, vi++)
            {
                _triangles[ti] = vi;
                _triangles[ti + 3] = _triangles[ti + 2] = vi + 1;
                _triangles[ti + 4] = _triangles[ti + 1] = vi + gridSize + 1;
                _triangles[ti + 5] = vi + gridSize + 2;
            }
        }

        EndMesh();
    }

    public void ModifPosY(Vector3 position, float force, float size)
    {
        RecupMesh();

        for (int i = 0; i < _vertices.Length; i++)
        {
            float distance = Vector3.Distance(_vertices[i], position);
            if (distance < size)
            {
                int sizeCurve = lenghtCurve.length;
                float pourcent = distance * 100 / size;
                int index = (int)(sizeCurve * (pourcent / 100));

                _vertices[i].y += force * lenghtCurve[index].value;
            }
        }

        EndMesh();
    }

    private void RecupMesh()
    {
        _vertices = _mesh.vertices;
        _triangles = _mesh.triangles;
        _normals = _mesh.normals;
    }

    private void EndMesh()
    {
        _mesh.MarkDynamic();
        _mesh.vertices = _vertices;
        _mesh.triangles = _triangles;
        _mesh.RecalculateNormals();

        _meshFilter.mesh = _mesh;
        DestroyImmediate(GetComponent<MeshCollider>());
        gameObject.AddComponent<MeshCollider>();
    }

    private void CalculNormalesTriangle(int i)
    {
        Vector3 v1, v2, pv;
        v1 = _vertices[_triangles[i * 3 + 1]] - _vertices[_triangles[i * 3]];
        v2 = _vertices[_triangles[i * 3 + 2]] - _vertices[_triangles[i * 3]];
        pv = Vector3.Cross(v1, v2);
        pv = pv / pv.magnitude;
        _normals[_triangles[i * 3 + 0]] = pv;
        _normals[_triangles[i * 3 + 1]] = pv;
        _normals[_triangles[i * 3 + 2]] = pv;
    }

    public void UpdateTerrain()
    {
        foreach (Transform child in this.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        alreadyHaveObj = new List<Vector2>();
        foreach (Vector3 vert in _vertices)
        {
            if (vert.z >= 1 && vert.x >= 1 && vert.z <= gridSize-1 && vert.x <= gridSize-1)
            {
                if (vert.y >= 1.7f && vert.y <= 1.9f && !hasObjectAround((int)vert.x, (int)vert.z))
                {
                    AssetPlacement.SpawnAsset(vert, prefabs[Random.Range(0, 6)], GetComponent<MeshFilter>().transform);
                    alreadyHaveObj.Add(new Vector2((int)vert.x, (int)vert.z));
                }

                if (vert.y >= 2.1f && vert.y <= 15 && !hasObjectAround((int)vert.x, (int)vert.z))
                {
                    AssetPlacement.SpawnAsset(vert, prefabs[Random.Range(6, 10)], GetComponent<MeshFilter>().transform);
                    alreadyHaveObj.Add(new Vector2((int)vert.x, (int)vert.z));
                }


                if (vert.y >= 16 && vert.y <= 25 && !hasObjectAround((int)vert.x, (int)vert.z))
                {
                    AssetPlacement.SpawnAsset(vert, prefabs[Random.Range(9, 11)], GetComponent<MeshFilter>().transform);
                    alreadyHaveObj.Add(new Vector2((int)vert.x, (int)vert.z));
                }
            }
        }
    }

    bool hasObjectAround(int x, int z)
    {
        for(int x2 = x - Random.Range(2, 12); x2 <= x + Random.Range(2, 12); x2 = x2 + 1)
        {
            for (float z2 = z - Random.Range(2, 12); z2 <= z + Random.Range(2, 12); z2 = z2 + 1)
            {
                if (alreadyHaveObj.Contains(new Vector2(x2, z2))) return true;
            }
        }
        return false;
    }
}
