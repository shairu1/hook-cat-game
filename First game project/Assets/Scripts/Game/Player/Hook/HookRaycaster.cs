using UnityEngine;


public class HookRaycaster : MonoBehaviour
{
    [SerializeField] LayerMask _layerMask;

    public RaycastHit2D[] Raycast(Vector2 origin, Vector2 target, float distance)
    {
        return Physics2D.RaycastAll(origin, target - origin, distance, _layerMask);
    }
}
