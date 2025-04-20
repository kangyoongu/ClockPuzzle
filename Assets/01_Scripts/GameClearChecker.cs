using UnityEngine;
using System.Collections.Generic;

public class GameClearChecker
{
    private int gridWidth;
    private int gridHeight;
    private StaticBlock[,] grid;
    private bool[,] visited;

    public GameClearChecker(int width, int height, StaticBlock[,] gameGrid)
    {
        gridWidth = width;
        gridHeight = height;
        grid = gameGrid;
    }

    public bool CheckGameClear()
    {
        HashSet<int> allTypes = new HashSet<int>();
        
        // 모든 블록의 타입을 수집
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if (grid[x, y] != null && grid[x, y].BlockType != 0)
                {
                    allTypes.Add(grid[x, y].BlockType);
                }
            }
        }

        // 각 색상별로 연결 상태 확인
        foreach (int type in allTypes)
        {
            if (!IsColorConnected(type))
            {
                return false;
            }
        }

        return true;
    }

    private bool IsColorConnected(int targetType)
    {
        visited = new bool[gridWidth, gridHeight];
        bool foundFirst = false;
        Vector2Int startPos = Vector2Int.zero;

        // 해당 색상의 첫 번째 블록 찾기
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if (grid[x, y] != null && grid[x, y].BlockType == targetType)
                {
                    startPos = new Vector2Int(x, y);
                    foundFirst = true;
                    break;
                }
            }
            if (foundFirst) break;
        }

        if (!foundFirst) return true;

        int connectedCount = DFS(startPos.x, startPos.y, targetType);
        int totalTypeCount = CountBlocksOfType(targetType);

        return connectedCount == totalTypeCount;
    }

    private int DFS(int x, int y, int targetType)
    {
        if (!IsValidCoordinate(x, y) || 
            visited[x, y] || 
            grid[x, y] == null || 
            grid[x, y].BlockType != targetType)
        {
            return 0;
        }

        visited[x, y] = true;
        int count = 1;

        // 상하좌우 탐색
        int[] dx = { 0, 1, 0, -1 };
        int[] dy = { 1, 0, -1, 0 };

        for (int i = 0; i < 4; i++)
        {
            count += DFS(x + dx[i], y + dy[i], targetType);
        }

        return count;
    }

    private int CountBlocksOfType(int targetType)
    {
        int count = 0;
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if (grid[x, y] != null && grid[x, y].BlockType == targetType)
                {
                    count++;
                }
            }
        }
        return count;
    }

    private bool IsValidCoordinate(int x, int y)
    {
        return x >= 0 && x < gridWidth && y >= 0 && y < gridHeight;
    }
} 