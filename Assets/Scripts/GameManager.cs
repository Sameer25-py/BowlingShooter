using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[Serializable]
public enum Language
{
    English,
    Russian
}

[Serializable]
public enum Theme
{
    LightMode,
    DarkMode
}

public class GameManager : MonoBehaviour
{
    [SerializeField] public List<Sprites> PinSprites;
    [SerializeField] public List<Sprites> BowlSprites;

    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject background;
    [SerializeField] private GameObject inGameUI;
    [SerializeField] private GameObject backDrop;

    public BubbleGrid   grid;
    public Shooter      shooter;
    public InputManager inputManager;

    public  int      Difficulty       = 0;
    public  Language SelectedLanguage = Language.English;
    public  Theme    SelectedTheme    = Theme.LightMode;
    private bool     _isGameStarted   = false;

    public List<MultiText> MultiTexts;
    public List<Sprite>    Themes;
    public Color           LightModeColor  = Color.black;
    public Color           DarkModeColor   = Color.white;
    public bool            EnableSound     = true;
    public bool            EnableVibration = true;

    void EnableInput()
    {
        inputManager.EnableInput = true;
    }

    void DisableInput()
    {
        inputManager.EnableInput = false;
    }

    private void Start()
    {
        inGameUI.SetActive(false);
        settingsMenu.SetActive(false);
    }

    public void ResumeGame()
    {
        menu.SetActive(false);
        inGameUI.SetActive(true);
        background.SetActive(false);
        settingsMenu.SetActive(false);
        EnableInput();
    }

    public void ShowSettings()
    {
        DisableInput();
        inGameUI.SetActive(false);
        menu.SetActive(false);
        background.SetActive(true);
        settingsMenu.SetActive(true);
    }

    public void ShowMenu()
    {
        DisableInput();
        inGameUI.SetActive(false);
        menu.SetActive(true);
        background.SetActive(true);
        settingsMenu.SetActive(false);
    }

    public void BackButtonPressed()
    {
        if (_isGameStarted)
        {
            ResumeGame();
        }
        else
        {
            ShowMenu();
        }
    }

    public void StartGame()
    {
        _isGameStarted = true;
        inGameUI.SetActive(true);
        menu.SetActive(false);
        background.SetActive(false);
        settingsMenu.SetActive(false);
        grid.FillGrid(PinSprites[Difficulty]
            .SpriteList);
        shooter.LoadShooter(BowlSprites[Difficulty]
            .SpriteList);
        Invoke(nameof(EnableInput), 0.1f);
    }

    public void SwitchSound(bool status)
    {
        Debug.Log(status);
    }

    public void SwitchVibration(bool status) { }

    public void ChangeLanguage(Language language)
    {
        SelectedLanguage = language;
        foreach (MultiText multiText in MultiTexts)
        {
            multiText.RenderText(SelectedLanguage);
        }
    }

    void ChangeUIColor(Sprite theme, Color color)
    {
        background.GetComponent<Image>()
            .sprite = theme;
        backDrop.GetComponent<Image>()
            .sprite = theme;
        foreach (MultiText multiText in MultiTexts)
        {
            multiText.gameObject.GetComponent<TMP_Text>()
                .color = color;
        }
    }

    public void ChangeTheme(Theme theme)
    {
        SelectedTheme = theme;
        switch (SelectedTheme)
        {
            case Theme.LightMode:
                ChangeUIColor(Themes[0], LightModeColor);
                break;
            case Theme.DarkMode:
                ChangeUIColor(Themes[1], DarkModeColor);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(theme), theme, null);
        }
    }

    public void ChangeDifficulty(int difficulty)
    {
        Difficulty = difficulty;
        grid.FillGrid(PinSprites[Difficulty]
            .SpriteList);
        shooter.LoadShooter(BowlSprites[Difficulty]
            .SpriteList);
    }
}

[System.Serializable]
public class Sprites
{
    public List<Sprite> SpriteList;
}