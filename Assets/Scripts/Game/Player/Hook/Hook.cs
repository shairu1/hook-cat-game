using UnityEngine;


public class Hook : MonoBehaviour
{
    [SerializeField] private HookRaycaster _hookRaycaster;
    [SerializeField] private DistanceJoint2D _distanceJoint;
    [SerializeField] private HookRenderer _hookRenderer;

    public float MaxRaycastDistance;
    public float MinHookLength;
    public float SpeedOfChangeHookLength;   // Stage HookFlight
    public float SpeedChangeVelocity;       // Stage PlayerFlight. ????, ??????? ????? ???????? ???????? 
    public float SpeedPlayerInFlight;       // Stage PlayerFlight. ????????, ? ??????? ????? ?????????? ?????

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
            Vector2 positionHook = Vector2.zero;

            switch (_stage)
            {
                case HookStage.HookFlight:
                    HookFlight(out positionHook);
                    break;

                case HookStage.PlayerFlight:
                    positionHook = _hit.point;
                    break;

                case HookStage.Hitch:
                    Hitch(out positionHook);
                    break;
            }

            _hookRenderer.Render(transform.position, positionHook);
        }  
    }

    private void FixedUpdate()
    {
        if (_stage == HookStage.PlayerFlight)
        {
            PlayerFlight();
        }
    }

    private void HookFlight(out Vector2 positionHook)
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

    private void PlayerFlight()
    {
        _collider.isTrigger = true;

        // расстояние от игрока до точки попадание крюка > длина крюка
        if (Vector2.Distance(transform.position, _hit.point) > MinHookLength)
        {
            Vector2 direction = ((Vector2)((Vector3)_hit.point - transform.position)).normalized * SpeedPlayerInFlight;
            _rigidbody.velocity = Vector2.Lerp(_rigidbody.velocity, direction, Time.fixedDeltaTime * SpeedChangeVelocity);
        }
        else
        {
            _stage = HookStage.Hitch;
            SetActiveDistanceJoint(true);
        }
    }

    private void Hitch(out Vector2 positionHook)
    {
        positionHook = _hit.point;
        _collider.isTrigger = false;
    }

    /// <summary>
    /// Создаёт крюк
    /// </summary>
    public void CreateHook()
    {
        Vector2 target = _camera.ScreenToWorldPoint(Input.mousePosition);

        bool pendant = false;
        float minDistance = float.MaxValue;
        RaycastHit2D[] hits = _hookRaycaster.Raycast(transform.position, target, MaxRaycastDistance);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider != null)
            {
                if (pendant)
                {
                    float distance = Vector2.Distance(hits[i].point, target);

                    if (hits[i].collider.tag != "Pendant")
                        continue;

                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        _hit = hits[i];
                    }
                }
                else
                {
                    if (hits[i].collider.tag == "Pendant")
                    {
                        pendant = true;
                        minDistance = float.MaxValue;
                        i -= 1;
                        continue;
                    }

                    float distance = Vector2.Distance(transform.position, hits[i].point);

                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        _hit = hits[i];
                    }
                }
            } 
        }
        
        _stage = HookStage.HookFlight;
        _positionHookInFlight = transform.position;
        enabled = true;
    }

    private void SetActiveDistanceJoint(bool value)
    {
        _distanceJoint.enabled = value;

        if (value) 
            _distanceJoint.connectedAnchor = _hit.collider == null ? Vector2.zero : _hit.point;
    }
    
    public void DestroyHook()
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
