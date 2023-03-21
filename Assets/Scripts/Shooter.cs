using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    [SerializeField] private TrajectoryManager trajectoryManager;
    [SerializeField] private GameObject objectToShoot;
    [SerializeField] private float travelTime = 1f;

    private List<Sprite> _shootSprites;

    [SerializeField] private List<SpriteRenderer> _magUI;
    private int _currentIndex = 0;

    [SerializeField] private Queue<Sprite> shooterMag = new();

    private void OnEnable()
    {
        Events.TouchEnded.AddListener(OnTouchEnded);
    }

    void UpdateMagUI()
    {    
        _magUI[0].sprite = shooterMag.ToArray()[0];
        _magUI[1].sprite = shooterMag.ToArray()[1];
    }

    private void ConvertToBubble()
    {   
        Events.ConvertBubble?.Invoke(objectToShoot.GetComponent<ShootBubble>().ColorName);
        objectToShoot.transform.position = new Vector2(0f, -4.5f);
        shooterMag.Enqueue(_shootSprites[Random.Range(0, _shootSprites.Count)]);
        Sprite nextSprite = shooterMag.Dequeue();
        UpdateMagUI();
        objectToShoot.GetComponent<SpriteRenderer>().sprite = nextSprite;
        objectToShoot.GetComponent<ShootBubble>().ColorName = nextSprite.name;
    }

    public void LoadShooter(List<Sprite> sprites)
    {
        _shootSprites = sprites;
        shooterMag = new();
        for (int i = 0; i < 3; i++)
        {
            shooterMag.Enqueue(_shootSprites[Random.Range(0, _shootSprites.Count)]);
        }

        Sprite nextSprite = shooterMag.Dequeue();
        UpdateMagUI();
        objectToShoot.GetComponent<SpriteRenderer>().sprite = nextSprite;
        objectToShoot.GetComponent<ShootBubble>().ColorName = nextSprite.name;

    }

    private void OnTouchEnded()
    {   
        if (shooterMag.Count == 0) return;
        Vector3[] trajectory = trajectoryManager.GetTrajectory();
        Invoke(nameof(ConvertToBubble),1f);
        for (int i = 1; i < trajectory.Length; i++)
        {
            LeanTween.moveLocal(objectToShoot, trajectory[i], 1f / trajectory.Length)
                .setDelay((i - 1) * 1f / trajectory.Length);
        }
    }
}
