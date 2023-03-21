using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LanguageRadioSwitch : MonoBehaviour
{
    public  Language Language;
    public  Image    Frame;
    private Button   _button;

    public UnityEvent<Language> LanguageSelected;

    private void Start()
    {
        if (Language == Language.Russian)
        {
            Frame.enabled = false;
        }
    }

    private void OnEnable()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(ChangeLanguage);
        Events.ChangeLanguageUI.AddListener(ChangeLanguageUI);
    }

    private void ChangeLanguageUI(Language arg0)
    {
        Frame.enabled = arg0 == Language;
    }

    private void ChangeLanguage()
    {
        if (Frame.enabled)
        {
            Frame.enabled = false;
        }
        else
        {
            Frame.enabled = true;
        }

        LanguageSelected?.Invoke(Language);
        Events.ChangeLanguageUI?.Invoke(Language);
    }
}