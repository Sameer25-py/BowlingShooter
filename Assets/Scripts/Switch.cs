using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Switch : MonoBehaviour
{
    public  GameObject       On, Off;
    private Button           _button;
    public  UnityEvent<bool> CallBack;

    private void OnEnable()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(ChangeSwitchState);
    }

    private void ChangeSwitchState()
    {
        if (On.activeSelf)
        {
            Off.SetActive(true);
            On.SetActive(false);
            CallBack?.Invoke(false);
        }
        else
        {
            On.SetActive(true);
            Off.SetActive(false);
            CallBack?.Invoke(true);
        }
    }
}