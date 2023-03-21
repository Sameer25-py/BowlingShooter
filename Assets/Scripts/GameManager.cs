using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] public List<Sprites> PinSprites;
    [SerializeField] public List<Sprites> BowlSprites;

    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject background;
    [SerializeField] private GameObject inGameUI;

    public BubbleGrid grid;
    public Shooter shooter;
    public InputManager inputManager;

    public int selectedMode = 0;


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
        inGameUI.SetActive(true);
    }

    public void ShowSettings()
    {
        DisableInput();
        inGameUI.SetActive(false);
        menu.SetActive(false);
        background.SetActive(true);
        settingsMenu.SetActive(true);
    }
    public void StartGame()
    {
        inGameUI.SetActive(true);
        menu.SetActive(false);
        background.SetActive(false);
        settingsMenu.SetActive(false);
        grid.FillGrid(PinSprites[selectedMode].SpriteList);
        shooter.LoadShooter(BowlSprites[selectedMode].SpriteList);
        Invoke(nameof(EnableInput), 0.1f);
    }
}

[System.Serializable]
public class Sprites
{
    public List<Sprite> SpriteList;
}
