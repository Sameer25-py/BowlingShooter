using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DifficultyRadioSwitch : MonoBehaviour
{
    public  int    Difficulty = 0;
    public  Image  tickImage;
    private Button _button;

    public UnityEvent<int> DifficultySelected;

    private void Start()
    {
        if (Difficulty != 0)
        {
            tickImage.enabled = false;
        }
    }

    private void OnEnable()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(ChangeDifficulty);
        Events.ChangeDifficultyUI.AddListener(ChangeDifficultyUI);
    }

    private void ChangeDifficulty()
    {
        tickImage.enabled = !tickImage.enabled;
        DifficultySelected?.Invoke(Difficulty);
        Events.ChangeDifficultyUI?.Invoke(Difficulty);
    }

    private void ChangeDifficultyUI(int arg0)
    {
        tickImage.enabled = arg0 == Difficulty;
    }
}