using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(MeshFilter))]
public class Tp3 : MonoBehaviour
{
    [Header("Plane")]
    public float _Dimension = 100;
    public int _Resolution = 2;
    [Space(10)]
    public Texture2D _Tex;
    public bool usetexture;

    [Header("Brush")]
    public Texture2D[] brush;
    public int selectedBrush;
    public float size = 30.0f;
    [Space(10)]
    public AnimationCurve power;
    public GameObject circleSprite;

    [HideInInspector]
    public float[,] noiseMap;


    [Header("Test")]
    public bool test = false;


    private Mesh _Mesh;
    private void Start()
    {
        _Mesh = GetComponent<MeshFilter>().mesh;

        createCleanPlane();
        deformMeshWithABrush(new Vector3(250, 0, 250), 10f);
    }

    private void Update()
    {
        //need change
        //circleSprite.transform.localScale = Vector3.one * size;

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Input.GetMouseButton(0) && Physics.Raycast(ray, out hit, 1000) && test)
        {
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                deformMeshWithABrush(hit.point, -10);
            else
                deformMeshWithABrush(hit.point, 10);
        }

        
        size += (Input.mouseScrollDelta.y / 2);
        
        if (size < 1f)
            size = 1f;
    }

    //need edit
    public void deformMesh(Vector3 point, float force)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        _Mesh.GetVertices(vertices);
        _Mesh.GetNormals(normals);
        for (int i = 0; i < _Mesh.vertices.Length; i++)
        {
            if (Vector3.Distance(_Mesh.vertices[i], point) <= size)
            {
                float dis = Vector3.Distance(_Mesh.vertices[i], point);
                vertices[i] += (power.Evaluate(dis / size) * Vector3.up) / 10 * force;
                normals[i] = vertices[i] + Vector3.up;
            }
        }

        _Mesh.SetVertices(vertices);
        _Mesh.SetNormals(normals);
        GetComponent<MeshFilter>().mesh = _Mesh;
        Destroy(GetComponent<MeshCollider>());
        gameObject.AddComponent<MeshCollider>();
    }

    public void deformMeshWithABrush(Vector3 point, float force)
    {
        //initialisation
        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        _Mesh.GetVertices(vertices);
        _Mesh.GetNormals(normals);

        Vector2 tempX = new Vector2(point.x - size / 2, point.x + size / 2);
        Vector2 tempZ = new Vector2(point.z - size / 2, point.z + size / 2);

        int temp = (int)((size / 2) / (_Dimension / _Resolution));
        int centerPoint = (int)(point.x / (_Dimension / _Resolution) + (point.z / (_Dimension / _Resolution)) * (_Resolution + 1));
        int left = Mathf.FloorToInt(centerPoint - temp + 1);
        int right = Mathf.CeilToInt(centerPoint + temp - 1);
        int upLeft = Mathf.FloorToInt(left - (_Resolution + 1) * (temp - 1));
        int downright = Mathf.CeilToInt(right + (_Resolution + 1) * (temp - 1));


        Debug.Log("///");
        Debug.Log(centerPoint);
        Debug.Log(left);
        Debug.Log(right);
        Debug.Log(upLeft);
        Debug.Log(downright);
        Debug.Log("///");

        if(upLeft < 0)
        {
            upLeft = 0;
        }

        
        for (int i = 0; i < (temp * 2)-1; i++)
            for (int y = 0; y < (temp * 2) - 1; y++) {
                int index = upLeft + y + i * (_Resolution + 1);
                Debug.Log(index);
                if (vertices[index].x > tempX.x && vertices[index].x < tempX.y && vertices[index].z > tempZ.x && vertices[index].z < tempZ.y)
                {
                    Vector2 normalisePos = new Vector2((vertices[i].x - tempX.x) / size, (vertices[i].z - tempZ.x) / size);
                    vertices[index] +=  Vector3.up * (force * brush[selectedBrush].GetPixelBilinear(normalisePos.x, normalisePos.y).a);
                    normals[index] = vertices[index] + Vector3.up;
                }
        } 

        _Mesh.SetVertices(vertices);
        _Mesh.SetNormals(normals);
        GetComponent<MeshFilter>().mesh = _Mesh;
    }

    public void subdivise()
    {
        Debug.Log("oui");
        int oldRes = _Resolution;

        _Resolution = oldRes * 2;

        int resMid = _Resolution + 1;
        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> vertices2 = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        _Mesh.GetVertices(vertices);
        int[] triangles = new int[(int)Mathf.Pow(_Resolution, 2) * 6];

        Vector3 v1, v2, vfinal;

        for (int y = 0; y < oldRes; y++)
        {
            for (int x = 0; x < oldRes; x++)
            {
                vertices2.Add(vertices[y * (oldRes + 1) + x]);
                v1 = vertices[y * (oldRes + 1) + x + 1] - vertices[y * (oldRes + 1) + x];
                vfinal = vertices[y * (oldRes + 1) + x + 1] - (v1 / 2.0f);

                vertices2.Add(vfinal);
            }

            vertices2.Add(vertices[y * (oldRes + 1) + oldRes]);

            for (int x = 0; x < oldRes; x++)
            {
                v1 = vertices[y * (oldRes + 1) + oldRes + 1 + x] - vertices[y * (oldRes + 1) + x];
                vfinal = vertices[y * (oldRes + 1) + x] + (v1 / 2.0f);

                vertices2.Add(vfinal);


                v2 = vertices[y * (oldRes + 1) + x + 1] - vertices[y * (oldRes + 1) + oldRes + 1 + x];
                vfinal = vertices[y * (oldRes + 1) + x + 1] - (v2 / 2.0f);

                vertices2.Add(vfinal);
            }

            v1 = vertices[y * (oldRes + 1) + oldRes * 2 + 1] - vertices[y * (oldRes + 1) + oldRes];
            vfinal = vertices[y * (oldRes + 1) + oldRes] + (v1 / 2.0f);


            vertices2.Add(vfinal);
        }

        for (int x = 0; x < oldRes; x++)
        {
            vertices2.Add(vertices[oldRes * (oldRes + 1) + x]);
            v1 = vertices[oldRes * (oldRes + 1) + x + 1] - vertices[oldRes * (oldRes + 1) + x];
            vfinal = vertices[oldRes * (oldRes + 1) + x] + (v1 / 2.0f);

            vertices2.Add(vfinal);
        }

        vertices2.Add(vertices[vertices.Count - 1]);

        int a = 0;

        for (int i = 0; i < _Resolution; i++)
            for (int y = 0; y < _Resolution; y++)
            {
                triangles[(i * 6 * _Resolution) + (y * 6) + 0] = y + (i * resMid);
                triangles[(i * 6 * _Resolution) + (y * 6) + 1] = y + 1 + (i * resMid);
                triangles[(i * 6 * _Resolution) + (y * 6) + 2] = y + 1 + resMid + (i * resMid);

                triangles[(i * 6 * _Resolution) + (y * 6) + 3] = y + (i * resMid);
                triangles[(i * 6 * _Resolution) + (y * 6) + 4] = y + 1 + resMid + (i * resMid);
                triangles[(i * 6 * _Resolution) + (y * 6) + 5] = y + resMid + (i * resMid);
            }

        foreach (Vector3 v in vertices2)
            normals.Add(v + Vector3.up);


        _Mesh.SetVertices(vertices2);
        _Mesh.triangles = triangles;
        _Mesh.SetNormals(normals);
        GetComponent<MeshFilter>().mesh = _Mesh;
        Destroy(GetComponent<MeshCollider>());
        gameObject.AddComponent<MeshCollider>();
    }

    public void createCleanPlane()
    {

        //Initialisation
        if (_Resolution % 2 == 1)
            _Resolution++;

        int resMid = _Resolution + 1;
        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        int[] triangles = new int[(int)Mathf.Pow(_Resolution, 2) * 6];

        //If we want to use a height map to create the map
        if (_Tex != null && usetexture)
        {
            float xRatio = _Tex.width / _Resolution;
            float yRatio = _Tex.height / _Resolution;
            noiseMap = new float[resMid, resMid];
            for (float x = 0; x < resMid; x++)
                for (float z = 0; z < resMid; z++)
                {
                    Color height = _Tex.GetPixel((int)(xRatio * x), (int)(yRatio * z));
                    noiseMap[(int)x, (int)z] = height.r * 100;
                    vertices.Add(new Vector3(x / _Resolution * _Dimension, height.r * 100, z / _Resolution * _Dimension));
                }
            this.GetComponent<MapGenerator>().GenerateMap();
        }
        else //Else, we create a plane
            for (float x = 0; x < resMid; x++)
                for (float z = 0; z < resMid; z++)
                    vertices.Add(new Vector3(x / _Resolution * _Dimension, 0, z / _Resolution * _Dimension));
        
        //Creation of triangles
        for (int i = 0; i < _Resolution; i++)
            for (int y = 0; y < _Resolution; y++)
            {
                triangles[(i * 6 * _Resolution) + (y * 6) + 0] = y + (i * resMid);
                triangles[(i * 6 * _Resolution) + (y * 6) + 1] = y + 1 + (i * resMid);
                triangles[(i * 6 * _Resolution) + (y * 6) + 2] = y + 1 + resMid + (i * resMid);

                triangles[(i * 6 * _Resolution) + (y * 6) + 3] = y + (i * resMid);
                triangles[(i * 6 * _Resolution) + (y * 6) + 4] = y + 1 + resMid + (i * resMid);
                triangles[(i * 6 * _Resolution) + (y * 6) + 5] = y + resMid + (i * resMid);
            }
        //Creation of normal
        for (int x = 0; x < resMid; x++)
            for (int z = 0; z < resMid; z++)
                normals.Add(new Vector3(vertices[x * resMid + z].x + 0, 1, vertices[x * resMid + z].z + 0));

        //update of value
        _Mesh.SetVertices(vertices);
        _Mesh.triangles = triangles;
        _Mesh.SetNormals(normals);
        GetComponent<MeshFilter>().mesh = _Mesh;
        Destroy(GetComponent<MeshCollider>());
        gameObject.AddComponent<MeshCollider>();
    }
    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.DrawSphere(GetComponent<MeshFilter>().mesh.vertices[48], 10.4f);
    //    Gizmos.DrawSphere(GetComponent<MeshFilter>().mesh.vertices[49], 10.4f);
    //    Gizmos.DrawSphere(GetComponent<MeshFilter>().mesh.vertices[50], 10.4f);
    //    Gizmos.DrawSphere(GetComponent<MeshFilter>().mesh.vertices[59], 10.4f);
    //    Gizmos.DrawSphere(GetComponent<MeshFilter>().mesh.vertices[60], 10.4f);
    //    Gizmos.DrawSphere(GetComponent<MeshFilter>().mesh.vertices[61], 10.4f);
    //    Gizmos.DrawSphere(GetComponent<MeshFilter>().mesh.vertices[70], 10.4f);
    //    Gizmos.DrawSphere(GetComponent<MeshFilter>().mesh.vertices[71], 10.4f);
    //    Gizmos.DrawSphere(GetComponent<MeshFilter>().mesh.vertices[72], 10.4f);

    //}
    /*
    private void OnDrawGizmosSelected()
    {
        if (normal)
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < GetComponent<MeshFilter>().mesh.normals.Length; i++)
                Gizmos.DrawLine(GetComponent<MeshFilter>().mesh.vertices[i], GetComponent<MeshFilter>().mesh.normals[i]);
        }
        float c = 0;
        if (triangle)
        {
            Gizmos.color = Color.blue;
            for (int i = 0; i < GetComponent<MeshFilter>().mesh.triangles.Length - 1; i++)
                Gizmos.DrawLine(GetComponent<MeshFilter>().mesh.vertices[GetComponent<MeshFilter>().mesh.triangles[i]], GetComponent<MeshFilter>().mesh.vertices[GetComponent<MeshFilter>().mesh.triangles[i + 1]]);

            for (int i = 0; i < GetComponent<MeshFilter>().mesh.vertices.Length; i++)
            {
                Gizmos.color = new Color(0, c, 0);
                c += 0.025f;
                Gizmos.DrawSphere(GetComponent<MeshFilter>().mesh.vertices[i], 0.4f);
            }
        }
    }*/



}
