using System;
using System.Collections.Generic;
using UnityEngine;


public class BubbleGrid : MonoBehaviour
{
    [SerializeField] private int        maxBubblePerRow = 6;
    [SerializeField] private GameObject bubble;
    [SerializeField] private int        initialRowCount = 100;
    [SerializeField] private float      bubbleSize      = 1f;

    public int      LatestFilledRow = 95;
    public GridData LastIndexedGridData;

    public List<GameObject> BubblePool = new List<GameObject>();

    private List<Sprite> _cachedSprites;

    private GridData[,] _grid;

    public InputManager InputManager;

    private List<GridData> _matched;

    public AudioSource AudioSource;

    void GenerateBubblePool()
    {
        for (int i = 0; i < initialRowCount; i++)
        {
            for (int j = 0; j < maxBubblePerRow; j++)
            {
                var obj = Instantiate(bubble, transform);
                obj.SetActive(false);
                BubblePool.Add(obj);
            }
        }
    }

    void ResetAllBubblePositions()
    {
        for (int i = 0; i < initialRowCount; i++)
        {
            for (int j = 0; j < maxBubblePerRow; j++)
            {
                if (_grid[i, j]
                    .Obj)
                {
                    _grid[i, j]
                        .Obj.transform.position = bubble.transform.position;
                    _grid[i, j]
                        .Obj = null;
                    _grid[i, j]
                        .Name = "";
                }
            }
        }
    }

    void GenerateGrid()
    {
        _grid = new GridData[initialRowCount, maxBubblePerRow];
        for (int i = 0; i < initialRowCount; i++)
        {
            for (int j = 0; j < maxBubblePerRow; j++)
            {
                _grid[i, j] = new GridData()
                {
                    LocalPosition = new Vector2(j * bubbleSize + 1f, (initialRowCount - i) * bubbleSize),
                    Index         = new Vector2Int(i, j),
                    Name          = ""
                };
            }
        }
    }

    public void FillGrid(List<Sprite> sprites)
    {
        ResetAllBubblePositions();
        LatestFilledRow = 91;
        _cachedSprites  = sprites;
        int currentIndex = 0;
        for (int i = LatestFilledRow; i >= 0; i--)
        {
            for (int j = 0; j < maxBubblePerRow; j++)
            {
                if (i == LatestFilledRow && (j == 0 || j == 5))
                {
                    continue;
                }

                GameObject obj = BubblePool[currentIndex++];
                obj.transform.localPosition = _grid[i, j]
                    .LocalPosition;
                Sprite randomSprite = sprites[UnityEngine.Random.Range(0, sprites.Count)];
                obj.GetComponent<SpriteRenderer>()
                    .sprite = randomSprite;
                obj.SetActive(true);
                _grid[i, j]
                    .Obj = obj;
                _grid[i, j]
                    .Name = randomSprite.name;
            }
        }
    }

    void ShowNextRow()
    {
        if (LatestFilledRow + 1 < initialRowCount)
        {
            for (int i = LatestFilledRow; i >= 1; i--)
            {
                for (int j = 0; j < maxBubblePerRow; j++)
                {
                    if (!_grid[i, j]
                            .Obj) continue;
                    _grid[i, j]
                        .Obj.transform.localPosition = _grid[i + 1, j]
                        .LocalPosition;
                    _grid[i + 1, j]
                        .Obj = _grid[i, j]
                        .Obj;
                    _grid[i + 1, j]
                        .Name = _grid[i, j]
                        .Name;
                }
            }

            LatestFilledRow += 1;
        }
        else
        {
            FillGrid(_cachedSprites);
        }
    }

    GridData GetGridDataByObj(GameObject obj)
    {
        for (int i = LatestFilledRow; i >= 0; i--)
        {
            for (int j = 0; j < maxBubblePerRow; j++)
            {
                if (GameObject.ReferenceEquals(obj, _grid[i, j]
                        .Obj))
                {
                    return _grid[i, j];
                }
            }
        }

        return null;
    }

    public Vector2 GetPositionAtIndexInDirection(GameObject obj, int direction)
    {
        Vector2  pos  = new();
        GridData data = GetGridDataByObj(obj);
        if (data == null)
        {
            return pos;
        }

        if (direction == 0) //left
        {
            if (data.Index.y - 1 >= 0 && !_grid[data.Index.x, data.Index.y - 1]
                    .Obj)
            {
                pos = transform.TransformPoint(_grid[data.Index.x, data.Index.y - 1]
                    .LocalPosition);
                LastIndexedGridData = _grid[data.Index.x, data.Index.y - 1];
            }
            else
            {
                pos = GetPositionAtIndexInDirection(obj, 1);
            }
        }
        else if (direction == 1) //bottom
        {
            if (data.Index.x + 1 < initialRowCount && !_grid[data.Index.x + 1, data.Index.y]
                    .Obj)
            {
                pos = transform.TransformPoint(_grid[data.Index.x + 1, data.Index.y]
                    .LocalPosition);
                LastIndexedGridData = _grid[data.Index.x + 1, data.Index.y];
            }
        }
        else if (direction == 2) //right
        {
            if (data.Index.y + 1 < maxBubblePerRow && !_grid[data.Index.x, data.Index.y + 1]
                    .Obj)
            {
                pos = transform.TransformPoint(_grid[data.Index.x, data.Index.y + 1]
                    .LocalPosition);
                LastIndexedGridData = _grid[data.Index.x, data.Index.y + 1];
            }
            else
            {
                pos = GetPositionAtIndexInDirection(obj, 1);
            }
        }

        return pos;
    }

    private Sprite GetSpriteByName(string name)
    {
        foreach (Sprite sprite in _cachedSprites)
        {
            if (sprite.name == name)
            {
                return sprite;
            }
        }

        return null;
    }

    public void AddObjectToGrid(string colorName)
    {
        GameObject obj = Instantiate(bubble, transform);
        BubblePool.Add(obj);
        obj.transform.localPosition = LastIndexedGridData.LocalPosition;
        obj.GetComponent<SpriteRenderer>()
            .sprite = GetSpriteByName(colorName);
        LastIndexedGridData.Obj  = obj;
        LastIndexedGridData.Name = colorName;

        if (LastIndexedGridData.Index.x > LatestFilledRow)
        {
            LatestFilledRow += 1;
        }

        _matched = new();
        PerformMatching(LastIndexedGridData.Index, LastIndexedGridData.Name, 0);
        if (_matched.Count >= 3)
        {   
            AudioSource.Play();
            Events.TriggerVibration?.Invoke();
            foreach (GridData data in _matched)
            {
                if (!data.Obj) continue;
                data.Obj.transform.position = bubble.transform.position;
                data.Obj                    = null;
                data.Name                   = "";
            }
        }
        else
        {
            ShowNextRow();
        }

        InputManager.EnableInput = true;
    }

    void AddUniqueElementToList(GridData data)
    {
        if (!_matched.Contains(data))
        {
            _matched.Add(data);
        }
    }

    void PerformMatching(Vector2Int Index, string Name, int currentDepth)
    {
        if (currentDepth == 4) return;
        bool foundMatch = false;
        if (Index.y != 0)
        {
            for (int i = Index.y; i >= 0; i--)
            {
                if (_grid[Index.x, i]
                        .Obj && _grid[Index.x, i]
                        .Name == Name)
                {
                    AddUniqueElementToList(_grid[Index.x, i]);
                    foundMatch = true;
                }
                else
                {
                    break;
                }
            }
        }

        if (Index.y != maxBubblePerRow - 1)
        {
            for (int i = Index.y; i < maxBubblePerRow; i++)
            {
                if (_grid[Index.x, i]
                        .Obj && _grid[Index.x, i]
                        .Name == Name)
                {
                    AddUniqueElementToList(_grid[Index.x, i]);
                    foundMatch = true;
                }
                else
                {
                    break;
                }
            }
        }

        if (foundMatch)
        {
            AddUniqueElementToList(_grid[Index.x, Index.y]);
        }

        if (Index.x != 0)
        {
            if (_grid[Index.x - 1, Index.y]
                    .Obj && Name == _grid[Index.x - 1, Index.y]
                    .Name)
            {
                PerformMatching(new Vector2Int(Index.x - 1, Index.y), Name, currentDepth + 1);
            }
        }

        if (Index.x != initialRowCount - 1)
        {
            if (_grid[Index.x + 1, Index.y]
                    .Obj && Name == _grid[Index.x + 1, Index.y]
                    .Name)
            {
                PerformMatching(new Vector2Int(Index.x + 1, Index.y), Name, currentDepth + 1);
            }
        }
    }

    void OnStartGameCalled(List<Sprite> sprites) { }

    private void Start()
    {
        Events.ConvertBubble.AddListener(AddObjectToGrid);
        Events.StartGame.AddListener(OnStartGameCalled);
        GenerateBubblePool();
        GenerateGrid();
    }
}

[Serializable]
public class GridData
{
    public GameObject Obj = null;
    public Vector2    LocalPosition;
    public Vector2Int Index;
    public string     Name;
}