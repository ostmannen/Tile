using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CameraMove : MonoBehaviour, IPointerDownHandler
{
    private Vector3 _orgin;
    private Vector3 _difference;
    private Vector3 _resetCamera;

    private bool _drag = false;
    [Header("Click Info")]
    [SerializeField] private float _maxMoveClick = 0.5f;
    [SerializeField] private LayerMask _onClickLayerMask;
    void Awake()
    {
        _resetCamera = transform.position;
    }
    private void LateUpdate()
    {
        if (!_drag) return;
        _difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        transform.position = _orgin - _difference;
        transform.position = new Vector3(transform.position.x, _resetCamera.y, transform.position.z);
    }
    public void OnMove(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            _orgin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        _drag = ctx.started || ctx.performed;
        if (ctx.canceled)
        {
            if (Vector3.Distance(_orgin, Camera.main.ScreenToWorldPoint(Input.mousePosition)) < _maxMoveClick)
            {

                //OnClick();
            }
        }
    }
    private void OnClick()
    {
        Vector3 _start = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Debug.Log(_start);
        Debug.DrawRay(_start, new Vector3(0, -20, 0), Color.black, 5f);
        RaycastHit hit;
        if (Physics.Raycast(_start, new Vector3(0, -1, 0), out hit, Mathf.Infinity, _onClickLayerMask, QueryTriggerInteraction.Ignore))
        {
            Debug.Log(hit.transform.gameObject.name);
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("fle");
        Debug.Log(eventData.selectedObject.name);
    }
}
