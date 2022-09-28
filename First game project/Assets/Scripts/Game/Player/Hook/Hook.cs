using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Hook : MonoBehaviour
{
    [SerializeField] private HookRaycaster _hookRaycaster;
    [SerializeField] private DistanceJoint2D _distanceJoint;
    [SerializeField] private HookRenderer _hookRenderer;

    public float MaxRaycastDistance;
    public float MinHookLength;
    public float SpeedOfChangeHookLength;
    public float HookForce;

    private Collider2D _collider;
    private Rigidbody2D _rigidbody;
    private Camera _camera;
    private RaycastHit2D _hit;
    private HookStage _stage;
    private Vector2 _positionHookInFlight;


    private void Start()
    {
        _camera = Camera.main;
        _hookRaycaster = GetComponent<HookRaycaster>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();

        _positionHookInFlight = Vector2.zero;
        SetActiveDistanceJoint(false);
        enabled = false;
    }

    private void Update()
    {
        if (_hit.collider != null && _stage != HookStage.None)
        {
            Vector2 positionHook= Vector2.zero;

            if (_stage == HookStage.HookFlight)
            {
                if (Vector2.Distance(_positionHookInFlight, _hit.point) > 0.1f)
                {
                    positionHook = Vector2.MoveTowards(_positionHookInFlight,
                        _hit.point, SpeedOfChangeHookLength * Time.deltaTime);

                    _positionHookInFlight = positionHook;
                }
                else
                {
                    _stage = HookStage.PlayerFlight;
                    positionHook = _hit.point;
                }
            }
            else if (_stage == HookStage.PlayerFlight)
            {
                if (Vector2.Distance(transform.position, _hit.point) > MinHookLength)
                {
                    Vector3 direction = (Vector3)_hit.point - transform.position;
                    _rigidbody.velocity = direction.normalized * HookForce;
                }
                else
                {
                    _stage = HookStage.Hitch;
                    SetActiveDistanceJoint(true);
                }
                
                positionHook = _hit.point;
            }
            else if (_stage == HookStage.Hitch)
            {
                positionHook = _hit.point;
                _collider.isTrigger = false;
            }

            _hookRenderer.Render(transform.position, positionHook);
        }  
    }

    public void CreateHook()
    {
        Vector2 target = _camera.ScreenToWorldPoint(Input.mousePosition);

        float minDistance = float.MaxValue;
        RaycastHit2D[] hits = _hookRaycaster.Raycast(transform.position, target, MaxRaycastDistance);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider != null)
            {
                float distance = Vector2.Distance(hits[i].point, target);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    _hit = hits[i];
                }
            } 
        }
        
        _stage = HookStage.HookFlight;
        _positionHookInFlight = transform.position;
        _collider.isTrigger = true;
        enabled = true;
    }

    private void SetActiveDistanceJoint(bool value)
    {
        _distanceJoint.enabled = value;

        if (value) 
            _distanceJoint.connectedAnchor = _hit.collider == null ? Vector2.zero : _hit.point;
    }

    public void DisableHook()
    {
        SetActiveDistanceJoint(false);
        _hookRenderer.Disable();
        _collider.isTrigger = false;
        _stage = HookStage.None;
        enabled = false;
    }

    private enum HookStage
    {
        None,
        HookFlight,
        PlayerFlight,
        Hitch
    }
}
