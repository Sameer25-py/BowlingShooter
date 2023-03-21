using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private float minSwipeDistance = 0.05f;
    Vector2                        startPos         = Vector2.zero;
    Vector2                        endPos           = Vector2.zero;
    Vector2                        direction        = Vector2.zero;
    Camera                         mainCamera;

    public bool EnableInput = false;
    

    private void OnEnable()
    {   

        mainCamera = Camera.main;
    }

    void Update()
    {   
        if(!EnableInput) return;
        if (Input.touches.Length == 1)
        {
            Touch touch = Input.touches[0];
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    startPos  = mainCamera.ScreenToWorldPoint(touch.position);
                    Events.TouchBegan?.Invoke(startPos);
                    break;
                case TouchPhase.Moved:
                    endPos    = mainCamera.ScreenToWorldPoint(touch.position);
                    direction = endPos - startPos;
                    if (direction.sqrMagnitude >= minSwipeDistance)
                    {
                        Events.TouchMoved?.Invoke(endPos);
                    }
                    break;
                case TouchPhase.Ended:
                    Events.TouchEnded?.Invoke();
                    break;
            }
        }
    }
}