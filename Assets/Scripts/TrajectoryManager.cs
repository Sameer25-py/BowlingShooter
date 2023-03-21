using System.Collections.Generic;
using UnityEngine;

public class TrajectoryManager : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private EdgeCollider2D edgeCollider;
    [SerializeField] private Vector2 clampXY = new Vector2(2.25f, 3f);
    [SerializeField] private int pointCount = 5;
    [SerializeField] private int trajectoryResolution = 10;
    [SerializeField] private float lineLength = 5f;

    [SerializeField] private BubbleGrid grid;

    [SerializeField] private LayerMask ignoreLayerMask;

    private Vector2 _initialPos = new Vector2(0f, -4.5f);

    private List<Vector3> _trajectory = new();

    public Vector3[] GetTrajectory()
    {
        return _trajectory.ToArray();
    }

    void InitializeTrajectory(Vector2 nextPos)
    {
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, _initialPos);
        _trajectory = new();
        _trajectory.Add(_initialPos);
        CalculateTrajectory(_initialPos, nextPos);
    }

    void CalculateTrajectory(Vector2 startPos, Vector2 endPos)
    {
        Vector2 direction = endPos - startPos;
        RaycastHit2D hit2D = Physics2D.Raycast(startPos, direction, 10f, ignoreLayerMask);
        if (hit2D.collider)
        {
            endPos = hit2D.point;
        }
        lineRenderer.positionCount += 1;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, endPos);
        _trajectory.Add(endPos);

        if (hit2D.collider && hit2D.collider.CompareTag("Wall"))
        {
            for (int i = 0; i < trajectoryResolution; i++)
            {
                Vector2 newStartPos = endPos + hit2D.normal;
                direction = Vector2.Reflect(direction, hit2D.normal);
                hit2D = Physics2D.Raycast(newStartPos, direction, 10f, ignoreLayerMask);
                if (hit2D.collider)
                {
                    endPos = hit2D.point;

                    if (lineRenderer.positionCount != pointCount)
                    {
                        lineRenderer.positionCount += 1;
                        lineRenderer.SetPosition(lineRenderer.positionCount - 1, endPos);
                    }

                    _trajectory.Add(endPos);

                    if (hit2D.collider.CompareTag("Bubble"))
                    {
                        _trajectory[^1] = AdjustPosition(hit2D.normal.normalized, hit2D.transform, _trajectory[^1]);
                        break;
                    }
                }
            }
        }

        else if (hit2D.collider && hit2D.collider.CompareTag("Bubble"))
        {
            _trajectory[^1] = AdjustPosition(hit2D.normal.normalized, hit2D.transform, _trajectory[^1]);
        }

    }

    void ClearTrajectory()
    {
        lineRenderer.positionCount = 0;
    }

    private Vector2 AdjustPosition(Vector2 normal, Transform bubble, Vector2 vec)
    {
        Vector2 direction = lineRenderer.GetPosition(lineRenderer.positionCount - 1) - lineRenderer.GetPosition(lineRenderer.positionCount - 2);
        float angleInDeg = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        int directionCategory = -1;
        if (angleInDeg < 80f)
        {
            directionCategory = 0;
        }
        else if (angleInDeg >= 80f && angleInDeg < 150f)
        {
            directionCategory = 1;
        }
        else
        {
            directionCategory = 2;
        }
        return grid.GetPositionAtIndexInDirection(bubble.gameObject, directionCategory);
    }



    private void OnEnable()
    {
        Events.TouchBegan.AddListener(InitializeTrajectory);
        Events.TouchMoved.AddListener(InitializeTrajectory);
        Events.TouchEnded.AddListener(ClearTrajectory);
    }
}