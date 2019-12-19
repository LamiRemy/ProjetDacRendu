using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class pointer : MonoBehaviour
{
    [SerializeField]
    private LayerMask mask;
    [SerializeField]
    private Transform startPoint = null;
    [SerializeField]
    private float sizePointer = 2f;
    [SerializeField]
    private float forcePointer = 0.2f;

    [SerializeField]
    private Slider sizeSlide = null;
    [SerializeField]
    private Text sizeValue = null;

    [SerializeField]
    private Slider forceSlide = null;
    [SerializeField]
    private Text forceValue = null;


    [SerializeField]
    private GameObject prefab;
    private GameObject _prefabSphere = null;

    [SerializeField]
    private Dropdown outilsDrop = null;

    private Camera _camera;
    private createTerrain tempFile;

    public enum OutilsChoix { MONTER, DESCENDRE }
    private OutilsChoix _outils = OutilsChoix.MONTER;

    // Start is called before the first frame update
    void Start()
    {
        _camera = Camera.main;

        var position = startPoint.position;
        _prefabSphere = Instantiate(prefab, position, Quaternion.identity);
        _prefabSphere.SetActive(false);
        sizeSlide.onValueChanged.AddListener(delegate { SizeSlideValueChanged(); });
        forceSlide.onValueChanged.AddListener(delegate { ForceSlideValueChanged(); });
        outilsDrop.onValueChanged.AddListener(delegate { DropdownValueChanged(); });
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
        {
            if (OVRInput.Get(OVRInput.Button.One))
            {
                tempFile = hit.transform.GetComponent<createTerrain>();

                if (_outils == OutilsChoix.MONTER)
                    tempFile.ModifPosY(hit.point, forcePointer, sizePointer);
                else if (_outils == OutilsChoix.DESCENDRE)
                    tempFile.ModifPosY(hit.point, -forcePointer, sizePointer);
            }

            if (Input.GetMouseButtonUp(0))
            {
                tempFile = hit.transform.GetComponent<createTerrain>();
                tempFile.UpdateTerrain();
            }

            Cursor.visible = false;
            _prefabSphere.transform.position = hit.point;
            _prefabSphere.SetActive(true);
        }
        else
        {
            _prefabSphere.SetActive(false);
            Cursor.visible = true;
        }
    }

    void ForceSlideValueChanged()
    {
        forcePointer = forceSlide.value;
        forceValue.text = forcePointer.ToString(CultureInfo.CurrentCulture);
    }

    void SizeSlideValueChanged()
    {
        var value = sizeSlide.value;
        sizePointer = value;
        sizeValue.text = sizePointer.ToString(CultureInfo.CurrentCulture);
        _prefabSphere.transform.localScale = new Vector3(value, value, value);
    }

    void DropdownValueChanged()
    {
        if (outilsDrop.value == 0)
            _outils = OutilsChoix.MONTER;
        else if (outilsDrop.value == 1)
            _outils = OutilsChoix.DESCENDRE;
    }
}
