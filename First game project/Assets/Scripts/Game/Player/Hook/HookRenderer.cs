using UnityEngine;


public class HookRenderer : MonoBehaviour
{
    [SerializeField] private LineRenderer _lineRenderer;

    public void Render(Vector2 start, Vector2 end)
    {
        _lineRenderer.enabled = true;
        _lineRenderer.positionCount = 2;
        _lineRenderer.SetPosition(0, start);
        _lineRenderer.SetPosition(1, end);
    }

    public void Disable()
    {
        _lineRenderer.positionCount = 0;
        _lineRenderer.enabled = false;
    }
}
