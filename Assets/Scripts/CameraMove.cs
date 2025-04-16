using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    private Vector3 _orgin;
    private Vector3 _difference;
    private Vector3 _resetCamera;

    private bool drag = false;
    // Start is called before the first frame update
    void Awake()
    {
        _resetCamera = transform.position;
    }

    private void LateUpdate()
    {

        if (Input.GetMouseButton(0))
        {
            _difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - Camera.main.transform.position;
            if (drag == false)
            {
                drag = true;
                _orgin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Debug.Log("orgin:" + (new Vector3(_orgin.x, _resetCamera.y, _orgin.y) - new Vector3(_difference.x, 0, _difference.y)) + " position:" + transform.position);
                Debug.Log("Dif" + _difference);
            }
        }
        else
        {
            drag = false;
        }

        if (drag)
        {
            transform.position = new Vector3(_orgin.x, _resetCamera.y, _orgin.y) - new Vector3(_difference.x, 0, _difference.y);
        }
        if (Input.GetMouseButton(1))
        {
            transform.position = _resetCamera;
        }
    }
}
