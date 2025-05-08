using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMove : MonoBehaviour
{
    [Header("Camera Settings")]
    public Camera cam;
    [SerializeField] private float groundY = 0f; // Y-nivå för dragning och klick

    [Header("Click Info")]
    [SerializeField] private float _maxMoveClick = 0.5f;
    [SerializeField] private LayerMask _onClickLayerMask;

    private Vector3 _origin;
    private Vector3 _resetCamera;
    private bool _drag = false;
    public Vector2 position;
    private Vector2 _downPosition;

    [Header("PlaceObject")]
    private bool _moving = false;
    private GameObject _selectedObject;
    public GameObject placeObejct;


    void Awake()
    {
        _resetCamera = transform.position;
    }
    void Update()
    {
        if (!_moving) return;

        _selectedObject.transform.position = GetWorldPositionOnGround(position);
    }
    private void LateUpdate()
    {
        if (!_drag) return;

        Vector3 current = GetWorldPositionOnGround(position);
        Vector3 difference = current - transform.position;
        transform.position = _origin - difference;

        transform.position = new Vector3(transform.position.x, _resetCamera.y, transform.position.z);
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            _origin = GetWorldPositionOnGround(position);
            _downPosition = position;
        }

        _drag = ctx.started || ctx.performed;

        if (ctx.canceled)
        {
            if (Vector3.Distance(_downPosition, position) < _maxMoveClick)
            {
                OnClick();
            }
        }
    }

    public void OnPosition(InputAction.CallbackContext ctx)
    {
        position = ctx.ReadValue<Vector2>();

        Ray ray = cam.ScreenPointToRay(position);
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.blue, 2f);
    }

    private void OnClick()
    {
        Ray ray = cam.ScreenPointToRay(position);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _onClickLayerMask, QueryTriggerInteraction.Ignore))
        {
            if (_selectedObject == null)
            {
                IClickable clickable = hit.collider.gameObject.GetComponent<IClickable>();
                if (clickable != null && clickable.OnClick())
                {
                    _moving = true;
                    _selectedObject = hit.collider.gameObject;
                }
                //Instantiate(placeObejct, hit.collider.transform.position + (hit.collider.transform.position.y / 2) * Vector3.up, Quaternion.identity);
            }
            else {
                _selectedObject.transform.position = hit.transform.position;
                _selectedObject = null;
                _moving = false;
            }
        }
    }

    private Vector3 GetWorldPositionOnGround(Vector2 screenPos)
    {
        Ray ray = cam.ScreenPointToRay(screenPos);
        Plane ground = new Plane(Vector3.up, new Vector3(0, groundY, 0));
        if (ground.Raycast(ray, out float enter))
        {
            return ray.GetPoint(enter);
        }

        return Vector3.zero;
    }
}
