using System;
using System.Collections.Generic;
using UnityEngine;


public class BubbleGrid : MonoBehaviour
{
    [SerializeField] private int maxBubblePerRow = 6;
    [SerializeField] private GameObject bubble;
    [SerializeField] private int initialRowCount = 100;
    [SerializeField] private float bubbleSize = 1f;

    public int LatestFilledRow = 95;
    public GridData LastIndexedGridData;

    public List<GameObject> BubblePool = new List<GameObject>();

    private List<Sprite> _cachedSprites;

    private GridData[,] _grid;

    public InputManager InputManager;

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
                    Index = new Vector2Int(i, j),
                    Name = ""
                };
                var obj = Instantiate(bubble, transform);
                obj.SetActive(false);
                BubblePool.Add(obj);
            }
        }

        // for (int i = 0; i < initialRowCount; i++)
        // {
        //     for (int j = 0; j < maxBubblePerRow; j++)
        //     {
        //         Debug.Log(_grid[i,j].Index);
        //     }
        // }
    }

    public void FillGrid(List<Sprite> sprites)
    {   
        _cachedSprites = sprites;
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
                obj.transform.localPosition = _grid[i, j].LocalPosition;
                Sprite randomSprite = sprites[UnityEngine.Random.Range(0, sprites.Count)];
                obj.GetComponent<SpriteRenderer>().sprite = randomSprite;
                obj.SetActive(true);
                _grid[i, j].Obj = obj;
                _grid[i, j].Name = randomSprite.name;
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
                    if (!_grid[i, j].Obj) continue;
                    _grid[i, j].Obj.transform.localPosition = _grid[i + 1, j].LocalPosition;
                    _grid[i + 1, j].Obj = _grid[i, j].Obj;
                    _grid[i + 1, j].Name = _grid[i, j].Name;
                }
            }

            LatestFilledRow += 1;
        }
    }

    GridData GetGridDataByObj(GameObject obj)
    {
        for (int i = LatestFilledRow; i >= 0; i--)
        {
            for (int j = 0; j < maxBubblePerRow; j++)
            {
                if (GameObject.ReferenceEquals(obj, _grid[i, j].Obj))
                {
                    return _grid[i, j];
                }
            }
        }

        return null;
    }

    public Vector2 GetPositionAtIndexInDirection(GameObject obj, int direction)
    {
        Vector2 pos = new();
        GridData data = GetGridDataByObj(obj);
        if (data == null)
        {
            return pos;
        }

        if (direction == 0) //left
        {
            if (data.Index.y - 1 >= 0 && !_grid[data.Index.x, data.Index.y - 1].Obj)
            {
                pos = transform.TransformPoint(_grid[data.Index.x, data.Index.y - 1].LocalPosition);
                LastIndexedGridData = _grid[data.Index.x, data.Index.y - 1];
            }
            else
            {
                pos = GetPositionAtIndexInDirection(obj, 1);
            }
        }
        else if (direction == 1) //bottom
        {
            if (data.Index.x + 1 < initialRowCount && !_grid[data.Index.x + 1, data.Index.y].Obj)
            {
                pos = transform.TransformPoint(_grid[data.Index.x + 1, data.Index.y].LocalPosition);
                LastIndexedGridData = _grid[data.Index.x + 1, data.Index.y];
            }
        }
        else if (direction == 2) //right
        {
            if (data.Index.y + 1 < maxBubblePerRow && !_grid[data.Index.x, data.Index.y + 1].Obj)
            {
                pos = transform.TransformPoint(_grid[data.Index.x, data.Index.y + 1].LocalPosition);
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
        foreach(Sprite sprite in _cachedSprites)
        {
            if(sprite.name == name)
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
        obj.GetComponent<SpriteRenderer>().sprite = GetSpriteByName(colorName);
        LastIndexedGridData.Obj = obj;
        LastIndexedGridData.Name = colorName;

        if (LastIndexedGridData.Index.x > LatestFilledRow)
        {
            LatestFilledRow += 1;
        }

        PerformMatching();   
        InputManager.EnableInput = true;
    }

    void PerformMatching()
    {
        int depth = 4;
        for (int i = 0; i < depth; i++)
        {
            if (LastIndexedGridData.Index.y != 0)
            {
                //checkLeft
            }

            if (LastIndexedGridData.Index.y != maxBubblePerRow - 1)
            {
                //check right
            }
        }
    }
    void OnStartGameCalled(List<Sprite> sprites)
    {
        
    }

    private void Start()
    {
        Events.ConvertBubble.AddListener(AddObjectToGrid);
        Events.StartGame.AddListener(OnStartGameCalled);
        GenerateGrid();
    }
}

[Serializable]
public class GridData
{
    public GameObject Obj = null;
    public Vector2 LocalPosition;
    public Vector2Int Index;
    public string Name;
}
