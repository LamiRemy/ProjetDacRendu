using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextManager : MonoBehaviour {

    public Dropdown typeSubdivision;
    public Dropdown objet;
    public GameObject[] objets;
    public InputField nombreSubdivision;
    public InputField seuil;
    GameObject gameobj;
    
    void Start()
    {
        nombreSubdivision.enabled = false;
        seuil.enabled = false;
        gameobj = Instantiate(objets[0]);
        objet.onValueChanged.AddListener(delegate { objetDropdownValueChanged(); });
        typeSubdivision.onValueChanged.AddListener(delegate { typeSubdivisionDropdownValueChanged(); });
        nombreSubdivision.onValueChanged.AddListener(delegate { nombreSubdivisionValueChanged(); });
        seuil.onValueChanged.AddListener(delegate { seuilValueChanged(); });
    }

    void Update()
    {
        if(typeSubdivision.value == 2) nombreSubdivision.enabled = true;
        else nombreSubdivision.enabled = false;
        
        if (typeSubdivision.value == 3) seuil.enabled = true;
        else seuil.enabled = false;
    }

    void objetDropdownValueChanged()
    {
        GameObject[] pickingObjs = GameObject.FindGameObjectsWithTag("PickingObject");
        foreach (GameObject pickingObj in pickingObjs) Destroy(pickingObj);
        Destroy(gameobj);
        gameobj = Instantiate(objets[objet.value]);
    }

    void typeSubdivisionDropdownValueChanged()
    {
        if (typeSubdivision.value == 0) gameobj.GetComponent<ProceduralMeshCreation>().typeSubdivision = ProceduralMeshCreation.TypeSubdivision.Elementaire;
        if (typeSubdivision.value == 1) gameobj.GetComponent<ProceduralMeshCreation>().typeSubdivision = ProceduralMeshCreation.TypeSubdivision.Uniforme;
        if (typeSubdivision.value == 2) gameobj.GetComponent<ProceduralMeshCreation>().typeSubdivision = ProceduralMeshCreation.TypeSubdivision.NbSubdivision;
        if (typeSubdivision.value == 3) gameobj.GetComponent<ProceduralMeshCreation>().typeSubdivision = ProceduralMeshCreation.TypeSubdivision.ResolutionUniformeAvecSeuil;
    }

    void nombreSubdivisionValueChanged()
    {
        gameobj.GetComponent<ProceduralMeshCreation>().nombreSubdivision = int.Parse(nombreSubdivision.text);
    }

    void seuilValueChanged()
    {
        gameobj.GetComponent<ProceduralMeshCreation>().seuil = float.Parse(seuil.text);
    }
}
