using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{

    private float minY = -45.0f;
    private float maxY = 45.0f;

    private float sensX = 100.0f;
    private float sensY = 100.0f;

    private float rotationY = 0.0f;
    private float rotationX = 0.0f;
    public float speedTranslate = 0.05f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;     
    }

    void Update()
    {
        Cursor.visible = false;

        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(new Vector3(1, 0, 0) * speedTranslate);
        }
        if (Input.GetKey(KeyCode.Q))
        {
            transform.Translate(new Vector3(-1, 0, 0) * speedTranslate);
        }
        if (Input.GetKey(KeyCode.Z))
        {
            transform.Translate(new Vector3(0, 0, 1) * speedTranslate);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(new Vector3(0, 0, -1) * speedTranslate);
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            transform.Translate(new Vector3(0, 1, 0) * speedTranslate);
        }
        if (Input.GetKey(KeyCode.LeftControl))
        {
            transform.Translate(new Vector3(0, -1, 0) * speedTranslate);
        }
        rotationX += Input.GetAxis("Mouse X") * sensX * Time.deltaTime;
        rotationY += Input.GetAxis("Mouse Y") * sensY * Time.deltaTime;
        rotationY = Mathf.Clamp(rotationY, minY, maxY);
        transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
    }
}
