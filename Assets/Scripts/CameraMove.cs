using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMove : MonoBehaviour
{
    private Vector3 _orgin;
    private Vector3 _difference;
    private Vector3 _resetCamera;

    private bool drag = false;
    void Awake()
    {
        _resetCamera = transform.position;
    }
    private void LateUpdate()
    {
        
        if (Input.GetMouseButton(0))
        {
            _difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            if (drag == false)
            {
                drag = true;
                _orgin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
        }
        else
        {
            drag = false;
        }
        if (drag)
        {
            transform.position = _orgin - _difference;
            transform.position = new Vector3(transform.position.x, _resetCamera.y, transform.position.z);
        }
        if (Input.GetMouseButton(1))
        {
            transform.position = _resetCamera;
        }
    }
    public void OnMove(InputAction.CallbackContext ctx){
        if (ctx.started){

        }
        Debug.Log("he");

    }
    public void Test(){

    }
}
