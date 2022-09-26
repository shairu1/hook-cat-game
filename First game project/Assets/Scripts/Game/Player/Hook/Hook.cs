using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : MonoBehaviour
{
    [SerializeField] private HookRaycaster _hookRaycaster;
    [SerializeField] private DistanceJoint2D _distanceJoint;
    [SerializeField] private HookRenderer _hookRenderer;

    public float maxRaycastDistance { get; set; }
    public float minHookLength { get; set; }
    public float speedOfChangeHookLength { get; set; }


    private RaycastHit2D _hit;
    private Camera _camera;


    private void Start()
    {
        _camera = Camera.main;
        _hookRaycaster = GetComponent<HookRaycaster>();
        _distanceJoint.enabled = false;
        enabled = false;

        maxRaycastDistance = 100;
        minHookLength = 2;
        speedOfChangeHookLength = 10;
    }

    private void Update()
    {
        if (_hit.collider != null)
        {
            if (_distanceJoint.distance > minHookLength)
            {
                _distanceJoint.distance -= speedOfChangeHookLength * Time.deltaTime;
            }

            _hookRenderer.Render(transform.position, _hit.point);
        }  
    }

    public void CreateHook()
    {
        Vector2 target = _camera.ScreenToWorldPoint(Input.mousePosition);
        _hit = _hookRaycaster.Raycast(transform.position, target, maxRaycastDistance);

        if (_hit.collider != null)
        {
            enabled = true;
            _distanceJoint.enabled = true;
            _distanceJoint.connectedAnchor = _hit.point;
            _hookRenderer.Render(transform.position, _hit.point);
        }
    }

    public void DisableHook()
    {
        _distanceJoint.enabled = false;
        _hookRenderer.Disable();
        enabled = false;
    }
}
