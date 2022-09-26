using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookRenderer : MonoBehaviour
{
    [SerializeField] private LineRenderer _lineRenderer;

    public void Render(Vector2 start, Vector2 end)
    {
        _lineRenderer.enabled = true;
        _lineRenderer.SetPosition(0, start);
        _lineRenderer.SetPosition(1, end);
    }

    public void Disable()
    {
        _lineRenderer.enabled = false;
    }
}
