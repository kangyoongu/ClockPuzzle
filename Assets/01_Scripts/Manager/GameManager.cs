using UnityEngine;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using DG.Tweening;
using System.Collections;

public class GameManager : SingleTon<GameManager>
{
    public List<MapSO> maps;
    public GameObject staticBlock;
    public GameObject block;

    [SerializeField] private float spacing = 1.2f;

    public float duration = 3f;
    public float tweenDur = 0.3f;

    public int rotateCnt = 10;

    [HideInInspector] public bool playing = false;

    private GameClearChecker clearChecker;
    private GamePresenter presenter;
    private StageTool stageTool;

    private StaticBlock[,] grid;
    public StaticBlock[,] Grid { get => grid; private set => value = grid; }

    public GamePresenter Presenter { get => presenter; private set => value = presenter; }

    int currentStage = 0;
    public int CurrentStage { get => currentStage; private set => value = currentStage; }
    private void Awake()
    {
        Application.targetFrameRate = 120;
        for(int i = 0; i < maps.Count; i++)
        {
            maps[i].width = maps[i].texture.width;
            maps[i].height = maps[i].texture.height;
        }
    }
    private void Start()
    {
        InitializeMVP();
        presenter.Start();
    }

    private void InitializeMVP()
    {
        UIManager uiManager = FindAnyObjectByType<UIManager>();
        presenter = new GamePresenter(uiManager, this);
        stageTool = new StageTool();
    }
    [ContextMenu("Rotate")]
    private void Rotate()
    {
        stageTool.RotateRandomBlocks(this, rotateCnt);
    }

    [ContextMenu("Export")]
    private void Export()
    {
        stageTool.GenerateTextureFromGrid(grid);
    }

    #region MAIN_FUNCTIONS
    public void InitializeGrid(int index)
    {
        ClearAllBlocks();
        currentStage = index;
        StartCoroutine(InitializeGridCoroutine(index)); // 코루틴으로 초기화 시작
    }

    private IEnumerator InitializeGridCoroutine(int index)
    {
        MapGenerator mapGenerator = new MapGenerator(maps[index].texture, maps[index].materials, spacing);
        grid = mapGenerator.GenerateMap();

        clearChecker = new GameClearChecker(maps[index].width, maps[index].height, grid);

        // 카메라 크기 조정
        AdjustCameraSize(maps[index].width);
        float instDelay = duration / (maps[index].height * maps[index].width);

        for (int y = 0; y < maps[index].height; y++)
        {
            for (int x = 0; x < maps[index].width; x++)
            {
                grid[x, y].SetNeighbors(GetNeighborClassWithGrid(x, y));
                    // 블록 생성 및 스케일 애니메이션 적용
                GameObject blockObject = grid[x, y].gameObject;

                // 스케일 애니메이션
                blockObject.transform.DOScale(Vector3.one, tweenDur).SetEase(Ease.OutBack);
                yield return new WaitForSeconds(instDelay); // 0.1초 대기
            }
        }
        yield return new WaitForSeconds(tweenDur - instDelay); // 0.1초 대기
        // 모든 블록이 생성된 후 playing 상태를 true로 설정
        playing = true;
    }

    private void AdjustCameraSize(int width)
    {
        float rightEnd;
        if (width % 2 == 0)
            rightEnd = width * 0.5f * spacing;
        else
            rightEnd = (width - 1) * 0.5f * spacing + spacing * 0.5f;

        float aspectRatio = Definder.MainCam.aspect;
        Definder.MainCam.orthographicSize = rightEnd / aspectRatio;
    }

    public Vector2Int RotateGridPositions(Block centerBlock)
    {
        // 그리드에서 블록의 위치 찾기
        Vector2Int blockPos = new Vector2Int(-1, -1);
        for (int y = 0; y < maps[currentStage].height; y++)
        {
            for (int x = 0; x < maps[currentStage].width; x++)
            {
                if (grid[x, y] == centerBlock)
                {
                    blockPos = new Vector2Int(x, y);
                    break;
                }
            }
            if (blockPos.x != -1) break;
        }

        if (blockPos.x != -1 && blockPos.y != -1)
        {
            List<Vector2Int> neighbors = GetNeighborPosWithGrid(blockPos.x, blockPos.y);

            if (neighbors.Count >= 2)
            {
                StaticBlock[] tempBlocks = new StaticBlock[neighbors.Count];
                for (int i = 0; i < neighbors.Count; i++)
                {
                    Vector2Int pos = neighbors[i];
                    StaticBlock block = grid[pos.x, pos.y];
                    tempBlocks[i] = block;
                }

                for (int i = 0; i < neighbors.Count; i++)
                {
                    Vector2Int newPos = neighbors[(i + 1) % neighbors.Count];
                    grid[newPos.x, newPos.y] = tempBlocks[i];
                }
            }
        }
        return blockPos;
    }
    #endregion

    #region HELPER_FUNCTIONS
    public List<StaticBlock> GetNeighborClassWithGrid(int x, int y) => GetBlocksWithPos(GetNeighborPosWithGrid(x, y));

    public List<Vector2Int> GetNeighborPosWithGrid(int x, int y)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        if (y < maps[currentStage].height - 1 && grid[x, y+1] is Block) neighbors.Add(new Vector2Int(x, y + 1));
        if (x < maps[currentStage].width - 1 && grid[x+1, y] is Block) neighbors.Add(new Vector2Int(x + 1, y));
        if (y > 0 && grid[x, y - 1] is Block) neighbors.Add(new Vector2Int(x, y - 1)); 
        if (x > 0 && grid[x-1, y] is Block) neighbors.Add(new Vector2Int(x - 1, y));

        return neighbors;
    }
    public List<StaticBlock> GetBlocksWithPos(List<Vector2Int> pos)
    {
        List<StaticBlock> blocks = new List<StaticBlock>();
        foreach (Vector2Int p in pos)
        {
            blocks.Add(grid[p.x, p.y]);
        }
        return blocks;
    }
    public StaticBlock GetBlockWithPos(Vector2Int pos)
    {
        return grid[pos.x, pos.y];
    }

    public void CheckGameClear()
    {
        if (clearChecker.CheckGameClear())
        {
            Presenter.Clear();
            playing = false;
        }
    }
    #endregion

    [ContextMenu("Print Grid Names")]
    private void PrintGridNames()
    {
        if (grid == null)
        {
            Debug.Log("Grid is not initialized!");
            return;
        }

        StringBuilder gridMap = new StringBuilder();
        gridMap.AppendLine("Current Grid Names:");

        // 그리드를 위에서 아래로 출력 (y축 역순)
        for (int y = maps[currentStage].height - 1; y >= 0; y--)
        {
            for (int x = 0; x < maps[currentStage].width; x++)
            {
                StaticBlock block = grid[x, y];
                if (block != null)
                {
                    gridMap.Append($"{block.gameObject.name} ");
                }
                else
                {
                    gridMap.Append("null ");
                }
            }
            gridMap.AppendLine(); // 줄바꿈
        }

        Debug.Log(gridMap.ToString());
    }

    private void ClearAllBlocks()
    {
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }

        grid = null;
    }

    public void ClearCubes(bool loadNext = false)
    {
        StartCoroutine(ClearCubesCoroutine(loadNext));
    }

    private IEnumerator ClearCubesCoroutine(bool loadNext)
    {
        List<StaticBlock> sortedCubes = GetSortedCubes(); // 큐브를 정렬하여 가져오기

        float instDelay = duration / sortedCubes.Count;

        foreach (StaticBlock cube in sortedCubes)
        {
            // 큐브 사이즈를 0으로 설정하고 애니메이션 적용
            cube.transform.DOScale(Vector3.zero, tweenDur).SetEase(Ease.OutExpo)
                .OnComplete(() => Destroy(cube.gameObject));
            yield return new WaitForSeconds(instDelay); // 0.5초 대기
        }
        yield return new WaitForSeconds(tweenDur - instDelay); // 0.5초 대기

        if (loadNext)
            LoadNextLevel();
    }

    private List<StaticBlock> GetSortedCubes()
    {
        List<StaticBlock> cubes = new List<StaticBlock>();
        for (int y = 0; y < maps[currentStage].height; y++)
        {
            for (int x = 0; x < maps[currentStage].width; x++)
            {
                if (grid[x, y] != null)
                {
                    cubes.Add(grid[x, y]);
                }
            }
        }
        return cubes.OrderBy(cube => cube.transform.position.y).ThenBy(cube => cube.transform.position.x).ToList();
    }

    private void LoadNextLevel()
    {
        // 다음 레벨 로드 로직
        currentStage++;
        if (currentStage < maps.Count)
        {
            InitializeGrid(currentStage);
        }
    }

    internal void Menu()
    {
        ClearCubes();
    }
} 