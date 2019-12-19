using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class ProceduralMeshCreation : MonoBehaviour {

    private Mesh p_mesh;
    private Vector3[] p_vertices;
    private Vector3[] p_normals;
    private int[] p_triangles;
    public GameObject PickingObject;
    private bool Dragging_PickingObjects = false;
    private Camera cam;
    private GameObject PickedObject;
    private GameObject[] pos;
    private Dictionary<string, List<int>> les_po = new Dictionary<string, List<int>>();    public Material SharedMat;    public Material NonSharedMat;    public Material SelectedMat;    private bool[] shared;    public enum TypeCalculNormales { Calculciblé, CalculAutomatique }
    public TypeCalculNormales typeCalculNormales;    public enum TypeSubdivision { Elementaire, Uniforme, NbSubdivision, ResolutionUniformeAvecSeuil }
    public TypeSubdivision typeSubdivision;
    public int nombreSubdivision;
    public float seuil;
    public float rayon;
    void Start()
    {
        GenererPickingObjects();
        cam = Camera.main;
        ProceduralMeshModel.InitNeighbour(p_mesh);
    }

    private void GenererPickingObjects()
    {
        p_mesh = GetComponent<MeshFilter>().mesh;
        p_vertices = p_mesh.vertices;
        p_triangles = p_mesh.triangles;
        p_normals = p_mesh.normals;

        const float SEUIL_DISTANCE_VERTICES_SIMILAIRES = 0.01f;
        bool[] bool_vert = new bool[p_vertices.Length];
        shared = new bool[p_vertices.Length];
        // indique si un vertex a été traité / faux par defaut
        for (int i = 0; i < bool_vert.Length; i++) bool_vert[i] = false;
        // l'ensemble des PickingObjects , chacun aura un nom unique
        // à chaque PickingObjetc est associé une liste de vertices "similairesé
        // un vertex est jugé similaire d'un autre s'il se trouve à une
        // distance négligeable, i.e. inférieure à un seuil
        int index_vert = 0;
        int nb_pickingObjects = 0;
        while (index_vert < p_vertices.Length) // traiter tous les vertices
        {
            // à faire !!
            if (!bool_vert[index_vert])
            {
                GameObject obj = Instantiate(PickingObject, p_vertices[index_vert], Quaternion.identity);
                obj.name = nb_pickingObjects.ToString();
                int i = index_vert + 1;
                List<int> listVert = new List<int>();
                listVert.Add(index_vert);
                bool_vert[index_vert] = true;
                while (i < p_vertices.Length)
                {
                    if (Vector3.Distance(p_vertices[index_vert], p_vertices[i]) < SEUIL_DISTANCE_VERTICES_SIMILAIRES && bool_vert[i] == false)
                    {
                        listVert.Add(i);
                        bool_vert[i] = true;
                    }
                    i++;
                }
                les_po.Add(nb_pickingObjects.ToString(), listVert);
                if (listVert.Count < 2)
                {
                    obj.GetComponent<MeshRenderer>().material = NonSharedMat;
                    shared[nb_pickingObjects] = false;
                }
                else
                {
                    obj.GetComponent<MeshRenderer>().material = SharedMat;
                    shared[nb_pickingObjects] = true;
                }
                nb_pickingObjects++;
            }
            index_vert++;
        }
    }
    private Vector3 mouseOffset;
    Vector3 mousePosi;
    public float timeBetweenUI = 0.5f;
    private float timestamp;
    void Update()
    {
        if ((Time.time >= timestamp) && Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (!Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit))
                return;
            if (hit.transform.gameObject == transform.gameObject)
            {
                if (typeSubdivision == TypeSubdivision.Elementaire) SubdiviserTriangle(hit.triangleIndex);
                else if (typeSubdivision == TypeSubdivision.Uniforme)
                {
                    int triangleLength = p_triangles.Length;
                    for (int i = 0; i < triangleLength / 3; i++)
                    {
                        SubdiviserTriangle(i);
                    }
                }
                else if (typeSubdivision == TypeSubdivision.NbSubdivision)
                {
                    int nbSub = 0;
                    while (nombreSubdivision != nbSub)
                    {
                        int triangleLength = p_triangles.Length;
                        for (int i = 0; i < triangleLength / 3; i++)
                        {
                            SubdiviserTriangle(i);
                        }
                        nbSub++;
                    }
                }
                else if (typeSubdivision == TypeSubdivision.ResolutionUniformeAvecSeuil)
                {
                    bool stillToSub = true;
                    while (stillToSub)
                    {
                        stillToSub = false;
                        int triangleLength = p_triangles.Length;
                        for (int i = 0; i < triangleLength / 3; i++)
                        {
                            if ((Vector3.Cross(p_vertices[p_triangles[i + 1]] - p_vertices[p_triangles[i]], p_vertices[p_triangles[i + 2]] - p_vertices[p_triangles[i]]).magnitude / 2) > seuil)
                            {
                                SubdiviserTriangle(i);
                            }
                        }                    }
                }
            }
            timestamp = Time.time + timeBetweenUI;
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (!Dragging_PickingObjects)
            {
                LayerMask maskPickingObjects = LayerMask.GetMask("L_PickingObject");
                RaycastHit hit;
                if (!Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, maskPickingObjects)) { return; }
                PickedObject = hit.transform.gameObject;
                print("debut dragging " + PickedObject.name);

                mouseOffset = PickedObject.transform.position - getMouseWorldPosi(PickedObject);
                Dragging_PickingObjects = true;
            }

        }
        if (Dragging_PickingObjects)
        {
            PickedObject.transform.position = getMouseWorldPosi(PickedObject) + mouseOffset;
            PickedObject.GetComponent<MeshRenderer>().material = SelectedMat;
            //List<int> l_indice = new List<int>();
            //List<int> listVert;
            //les_po.TryGetValue(PickedObject.name, out listVert);
            //foreach (int vert in listVert)
            //{
            //    l_indice.Add(vert);
            //}
            //foreach (var po in les_po.Keys)
            //{
            //            if (Mathf.Abs(Vector3.Distance(p_vertices[listVert[0]], po)) < rayon)
            //            {
            //            }
            //}
            modifyMesh();
        }
        if (Input.GetMouseButtonUp(0))
        {
            if(Dragging_PickingObjects)
                {
                if (shared[int.Parse(PickedObject.name)] == true) PickedObject.GetComponent<MeshRenderer>().material = SharedMat;
                else PickedObject.GetComponent<MeshRenderer>().material = NonSharedMat;
            }
            Dragging_PickingObjects = false;
        }
    }

    private Vector3 getMouseWorldPosi(GameObject obj)
    {
        mousePosi = Input.mousePosition;
        mousePosi.z = cam.WorldToScreenPoint(obj.transform.position).z;
        return cam.ScreenToWorldPoint(mousePosi);
    }

    private void modifyMesh()
    {
        List<int> listVert;
        les_po.TryGetValue(PickedObject.name, out listVert);
        foreach (int vert in listVert)
        {
            p_vertices[vert] = PickedObject.transform.position;
            if (typeCalculNormales == TypeCalculNormales.Calculciblé) p_normals[vert] = NormalAtVertex(vert);
        }
        p_mesh.vertices = p_vertices;
        p_mesh.normals = p_normals;
        if (typeCalculNormales == TypeCalculNormales.CalculAutomatique) p_mesh.RecalculateNormals();
        GetComponent<MeshFilter>().mesh = p_mesh;
        DestroyImmediate(GetComponent<MeshCollider>());
        gameObject.AddComponent<MeshCollider>();
    }

    private static bool Search(int i, List<int> tab)
    {
        for (int j = 0; j < tab.Count; j++)
        {
            if (i == tab[j])
            {
                return false;
            }
        }
        return true;
    }

    private Vector3 NormalAtVertex(int indexVertex)
    {
        int nb = 0;
        Vector3[] trianglesNormales = new Vector3[p_triangles.Length/3];
        int index = 0;
        for (int it = 0; it < p_triangles.Length/3; it++)
        {
            int i = p_triangles[index++];
            int j = p_triangles[index++];
            int k = p_triangles[index++];
            Vector3 v1 = p_vertices[j] - p_vertices[i];
            Vector3 v2 = p_vertices[k] - p_vertices[i];
            Vector3 pv = Vector3.Cross(v1, v2);
            trianglesNormales[it] = pv / pv.magnitude;
        }
        Vector3 normal, sommeNormal = Vector3.zero;
        for (int it = 0; it < p_triangles.Length; it++)
        {
            if (p_triangles[it] == indexVertex)
            {

                sommeNormal += trianglesNormales[it/3];
                nb++;
            }
        }
        normal = sommeNormal / nb;
        return normal.normalized;
    }

    void SubdiviserTriangle(int _index)
    {
        int[] triangles_temp = new int[p_triangles.Length + 6];
        p_triangles.CopyTo(triangles_temp, 0);
        p_triangles = new int[triangles_temp.Length];
        triangles_temp.CopyTo(p_triangles, 0);

        Vector3[] vertices_temp = new Vector3[p_vertices.Length + 1];
        p_vertices.CopyTo(vertices_temp, 0);
        p_vertices = new Vector3[vertices_temp.Length];
        vertices_temp.CopyTo(p_vertices, 0);

        Vector3[] normals_temp = new Vector3[p_normals.Length + 1];
        p_normals.CopyTo(normals_temp, 0);
        p_normals = new Vector3[normals_temp.Length];
        normals_temp.CopyTo(p_normals, 0);


        int i1 = p_triangles[_index * 3];
        int i2 = p_triangles[_index * 3 + 1];
        int i3 = p_triangles[_index * 3 + 2];

        int newI = p_vertices.Length-1;

        Vector3 p1 = p_vertices[p_triangles[_index * 3]];
        Vector3 p2 = p_vertices[p_triangles[_index * 3 + 1]];
        Vector3 p3 = p_vertices[p_triangles[_index * 3 + 2]];

        Vector3 newP;

        newP.x = (p1.x + p2.x + p3.x) / 3;
        newP.y = (p1.y + p2.y + p3.y) / 3;
        newP.z = (p1.z + p2.z + p3.z) / 3;

        p_vertices[newI] = newP;

        p_triangles[_index * 3] = i1;
        p_triangles[_index * 3 + 1] = i2;
        p_triangles[_index * 3 + 2] = newI;

        p_triangles[p_triangles.Length - 6] = i2;
        p_triangles[p_triangles.Length - 5] = i3;
        p_triangles[p_triangles.Length - 4] = newI;

        p_triangles[p_triangles.Length - 3] = i3;
        p_triangles[p_triangles.Length - 2] = i1;
        p_triangles[p_triangles.Length - 1] = newI;

        CalculNormalesTriangle(_index);        CalculNormalesTriangle(p_triangles.Length / 3 -2);        CalculNormalesTriangle(p_triangles.Length / 3 -1);

        p_mesh.Clear();
        p_mesh.vertices = p_vertices;
        p_mesh.triangles = p_triangles;
        p_mesh.normals = p_normals;
        p_mesh.name = "Mesh_&_Subdiv_Locale";

        GetComponent<MeshFilter>().mesh = p_mesh;
        DestroyImmediate(GetComponent<MeshCollider>());
        gameObject.AddComponent<MeshCollider>();        GenererPickingObjectAtVert(newI);
    }

    private void CalculNormalesTriangle(int i) // sans vertice partagé
    {
        Vector3 v1, v2, pv;
        v1 = p_vertices[p_triangles[i * 3 + 1]] - p_vertices[p_triangles[i * 3]];
        v2 = p_vertices[p_triangles[i * 3 + 2]] - p_vertices[p_triangles[i * 3]];
        pv = Vector3.Cross(v1, v2);
        pv = pv / pv.magnitude;
        p_normals[p_triangles[i * 3 + 0]] = pv;
        p_normals[p_triangles[i * 3 + 1]] = pv;
        p_normals[p_triangles[i * 3 + 2]] = pv;
    }    private void GenererPickingObjectAtVert(int index)
    {
        p_mesh = GetComponent<MeshFilter>().mesh;
        p_vertices = p_mesh.vertices;
        p_triangles = p_mesh.triangles;
        p_normals = p_mesh.normals;

        const float SEUIL_DISTANCE_VERTICES_SIMILAIRES = 0.01f;
        bool[] bool_vert = new bool[p_vertices.Length];
        bool[] shared_temp = new bool[shared.Length + 1];
        shared.CopyTo(shared_temp, 0);
        shared = new bool[shared_temp.Length];
        shared_temp.CopyTo(shared, 0);
        // indique si un vertex a été traité / faux par defaut
        for (int i = 0; i < bool_vert.Length; i++) bool_vert[i] = false;
        int nb_pickingObjects = les_po.Count;
        if (!bool_vert[index])
        {
            GameObject obj = Instantiate(PickingObject, p_vertices[index], Quaternion.identity);
            obj.name = nb_pickingObjects.ToString();
            int i = index + 1;
            List<int> listVert = new List<int>();
            listVert.Add(index);
            bool_vert[index] = true;
            while (i < p_vertices.Length)
            {
                if (Vector3.Distance(p_vertices[index], p_vertices[i]) < SEUIL_DISTANCE_VERTICES_SIMILAIRES && bool_vert[i] == false)
                {
                    listVert.Add(i);
                    bool_vert[i] = true;
                }
                i++;
            }
            les_po.Add(nb_pickingObjects.ToString(), listVert);
            if (listVert.Count < 2)
            {
                obj.GetComponent<MeshRenderer>().material = NonSharedMat;

                shared[shared.Length-1] = false;
            }
            else
            {
                obj.GetComponent<MeshRenderer>().material = SharedMat;

                shared[shared.Length-1] = true;
            }
            nb_pickingObjects++;
        }
    }    //void SubdiviserToutLesTriangles()
    //{
    //    int tempLengthTr = p_triangles.Length;
    //    int[] triangles_temp = new int[p_triangles.Length*3];
    //    p_triangles.CopyTo(triangles_temp, 0);
    //    p_triangles = new int[triangles_temp.Length];
    //    triangles_temp.CopyTo(p_triangles, 0);

    //    int tempLengthvert = p_vertices.Length;
    //    Vector3[] vertices_temp = new Vector3[p_vertices.Length + p_vertices.Length];
    //    p_vertices.CopyTo(vertices_temp, 0);
    //    p_vertices = new Vector3[vertices_temp.Length];
    //    vertices_temp.CopyTo(p_vertices, 0);

    //    Vector3[] normals_temp = new Vector3[p_normals.Length + p_normals.Length];
    //    p_normals.CopyTo(normals_temp, 0);
    //    p_normals = new Vector3[normals_temp.Length];
    //    normals_temp.CopyTo(p_normals, 0);

    //    for (int _index = 0; _index < tempLengthTr/3; _index++)
    //    {
    //        int i1 = p_triangles[_index * 3];
    //        int i2 = p_triangles[_index * 3 + 1];
    //        int i3 = p_triangles[_index * 3 + 2];

    //        int newI = tempLengthTr  + _index;

    //        Vector3 p1 = p_vertices[p_triangles[_index * 3]];
    //        Vector3 p2 = p_vertices[p_triangles[_index * 3 + 1]];
    //        Vector3 p3 = p_vertices[p_triangles[_index * 3 + 2]];

    //        Vector3 newP;

    //        newP.x = (p1.x + p2.x + p3.x) / 3;
    //        newP.y = (p1.y + p2.y + p3.y) / 3;
    //        newP.z = (p1.z + p2.z + p3.z) / 3;
    //        p_vertices[newI] = newP;

    //        p_triangles[_index * 3] = i1;
    //        p_triangles[_index * 3 + 1] = i2;
    //        p_triangles[_index * 3 + 2] = newI;

    //        p_triangles[tempLengthTr + (_index * 6)] = i2;
    //        p_triangles[tempLengthTr + (_index * 6) + 1] = i3;
    //        p_triangles[tempLengthTr + (_index * 6) + 2] = newI;

    //        p_triangles[tempLengthTr + (_index * 6) + 3] = i3;
    //        p_triangles[tempLengthTr + (_index * 6) + 4] = i1;
    //        p_triangles[tempLengthTr + (_index * 6) + 5] = newI;

    //        CalculNormalesTriangle(_index);
    //        CalculNormalesTriangle((tempLengthTr + _index) / 3 + 1);    
    //        CalculNormalesTriangle((tempLengthTr + _index) / 3 + 2);

    //        p_mesh.Clear();
    //        p_mesh.vertices = p_vertices;
    //        p_mesh.triangles = p_triangles;
    //        p_mesh.normals = p_normals;
    //        p_mesh.name = "Mesh_&_Subdiv_Locale";
    //        GetComponent<MeshFilter>().mesh = p_mesh;
    //        DestroyImmediate(GetComponent<MeshCollider>());
    //        gameObject.AddComponent<MeshCollider>();

    //        GenererPickingObjectAtVert(newI);
    //    }
    //}
}
