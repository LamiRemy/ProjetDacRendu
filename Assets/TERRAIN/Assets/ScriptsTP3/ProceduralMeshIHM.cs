using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class ProceduralMeshIHM : MonoBehaviour
{
    public float minForceBrush;
    public float maxForceBrush;
    public float minRayonBrush;
    public float maxRayonBrush;
    public float forceBrush = 0.3f;
    public float rayonBrush = 0.5f;

    public Canvas canvas;

    protected Mesh p_mesh;

    private GameObject sphere;

    protected string choice;

    //Démarre le projet en appelant la fonction CreationMesh dans le controller, initialise l'outil et l'interface
    void Start()
    {
        ProceduralMeshController.CreationMesh();
        choice = "Depression";
        canvas.GetComponent<TextManager2>().outils = choice;
        canvas.GetComponent<TextManager2>().force = forceBrush;
        canvas.GetComponent<TextManager2>().rayon = rayonBrush;
    }

    //Récupère à chaque frame si on appuie sur un touche, agit en conséquence et vérifie si on touche la l'objet à modifier
    protected virtual void Update()
    {
        //Modifie l'interface en fonction des paramètre de l'outil
        canvas.GetComponent<TextManager2>().outils = choice;
        canvas.GetComponent<TextManager2>().force = forceBrush;
        canvas.GetComponent<TextManager2>().rayon = rayonBrush;
        
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            if (rayonBrush < maxRayonBrush)
                rayonBrush += 0.1f;
        }
        else if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            if (rayonBrush > minRayonBrush)
                rayonBrush -= 0.1f;
        }

        //Vérifie si le raycast qui part de la caméra touche l'object à sculpter ou non
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        RaycastHit hit;
        if (!Physics.Raycast(ray, out hit))
        {
            Destroy(sphere);
            return;
        }
        MeshCollider meshCollider = hit.collider as MeshCollider;
        if (meshCollider == null || meshCollider.sharedMesh == null)
        {
            Destroy(sphere);
            return;
        }

        if (!sphere)
        {
            sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Destroy(sphere.GetComponent<SphereCollider>());
        }
        if (sphere)
        {
            sphere.transform.position = hit.point;
            sphere.transform.localScale = new Vector3(rayonBrush, rayonBrush, rayonBrush);
        }
        
        if (Input.GetMouseButton(0))
        {
            choice = "Elevation";
            ProceduralMeshController.ToolForModification(meshCollider, hit, rayonBrush, forceBrush, choice);
        }

        if (Input.GetMouseButton(1))
        {
            choice = "Depression";
            ProceduralMeshController.ToolForModification(meshCollider, hit, rayonBrush, forceBrush, choice);
        }
    }
}