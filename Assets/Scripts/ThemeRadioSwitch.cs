using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ThemeRadioSwitch : MonoBehaviour
{
    public  Theme  Theme;
    public  Image  tickImage;
    private Button _button;

    public UnityEvent<Theme> ThemeSelected;

    private void Start()
    {
        if (Theme == Theme.DarkMode)
        {
            tickImage.enabled = false;
        }
    }

    private void OnEnable()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(ChangeTheme);
        Events.ChangeThemeUI.AddListener(ChangeThemeUI);
    }

    private void ChangeTheme()
    {
        tickImage.enabled = !tickImage.enabled;
        ThemeSelected?.Invoke(Theme);
        Events.ChangeThemeUI?.Invoke(Theme);
    }

    private void ChangeThemeUI(Theme arg0)
    {
        tickImage.enabled = arg0 == Theme;
    }
}