using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextManager2 : MonoBehaviour {

    public Text textOutils;
    public Text rayonBrush;
    public Text forceBrush;

    public string outils;
    public float force;
    public float rayon;
    
	void Update () {
        textOutils.text = ("Outils Courant : " + outils);
        rayonBrush.text = ("Rayon du brush : " + rayon);
        forceBrush.text = ("Force du brush : " + force);
    }
}
