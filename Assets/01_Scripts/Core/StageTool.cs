using UnityEngine;
using System.Collections.Generic;

public class StageTool
{
    public void RotateRandomBlocks(GameManager gameManager, int rotationCount)
    {
        List<Block> blocks = new List<Block>();

        // 현재 그리드에서 모든 블록을 가져옵니다.
        for (int y = 0; y < gameManager.maps[gameManager.CurrentStage].height; y++)
        {
            for (int x = 0; x < gameManager.maps[gameManager.CurrentStage].width; x++)
            {
                if (gameManager.Grid[x, y] is Block block)
                {
                    blocks.Add(block);
                }
            }
        }

        // 무작위로 블록을 선택하여 회전 동작을 수행합니다.
        for (int i = 0; i < rotationCount; i++)
        {
            if (blocks.Count == 0) break;

            int randomIndex = Random.Range(0, blocks.Count);
            Block selectedBlock = blocks[randomIndex];
            selectedBlock.HandleBlockClick(false); // 블록의 이웃을 회전시키는 메서드 호출
        }
    }

    public void GenerateTextureFromGrid(StaticBlock[,] grid)
    {
        int width = grid.GetLength(0);
        int height = grid.GetLength(1);
        Texture2D texture = new Texture2D(width, height);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (grid[x, y] != null)
                {
                    Color blockColor = GetColorFromBlockType(grid[x, y].BlockType);
                    texture.SetPixel(x, y, blockColor);
                }
                else
                {
                    texture.SetPixel(x, y, Color.clear); // 빈 블록은 투명으로 설정
                }
            }
        }

        string path = "Assets/Game/04_Texture/Stage/" + System.DateTime.Now.Ticks + ".png";
        byte[] bytes = texture.EncodeToPNG();
        System.IO.File.WriteAllBytes(path, bytes);
    }

    private Color GetColorFromBlockType(int blockType)
    {
        // 블록 타입에 따라 색상을 반환합니다.
        switch (blockType)
        {
            case 0: return Color.black; // 예시: 블록 타입 0은 검정색
            case 1: return new Color(1f, 0.5f, 0.5f); // 예시: 블록 타입 1은 분홍색
            case 2: return new Color(0.5f, 1f, 0.5f); // 예시: 블록 타입 2는 연두색
            case 3: return new Color(0.5f, 0.5f, 1f); // 예시: 블록 타입 3은 하늘색
            case 4: return new Color(1f, 1f, 0f); // 예시: 블록 타입 4은 노란색
            case 5: return new Color(0f, 1f, 1f); // 예시: 블록 타입 5은 청록색
            case 6: return new Color(1f, 0f, 1f); // 예시: 블록 타입 6은 자주색
            case 7: return new Color(0.8f, 0.8f, 0.8f); // 예시: 블록 타입 7은 회색
            case 8: return new Color(0.4f, 0.4f, 0.4f); // 예시: 블록 타입 8은 어두운 회색
            case 9: return new Color(0.2f, 0.2f, 0.2f); // 예시: 블록 타입 9은 매우 어두운 회색
            case 10: return new Color(0.6f, 0.6f, 0.6f); // 예시: 블록 타입 10은 중간 회색
            case 11: return new Color(0.9f, 0.9f, 0.9f); // 예시: 블록 타입 11은 밝은 회색
            case 12: return new Color(0.1f, 0.1f, 0.1f); // 예시: 블록 타입 12은 매우 어두운 회색
            case 13: return new Color(0.7f, 0.7f, 0.7f); // 예시: 블록 타입 13은 중간 회색
            case 14: return new Color(0.3f, 0.3f, 0.3f); // 예시: 블록 타입 14은 어두운 회색
            case 15: return new Color(0.5f, 0.2f, 0.1f); // 예시: 블록 타입 15은 갈색
            case 16: return new Color(0.2f, 0.5f, 0.1f); // 예시: 블록 타입 16은 연두색
            case 17: return new Color(0.1f, 0.2f, 0.5f); // 예시: 블록 타입 17은 하늘색
            case 18: return new Color(0.5f, 0.5f, 0.2f); // 예시: 블록 타입 18은 연두색
            case 19: return new Color(0.2f, 0.2f, 0.5f); // 예시: 블록 타입 19은 하늘색
            case 20: return new Color(0.8f, 0.4f, 0.2f); // 예시: 블록 타입 20은 갈색
            case 21: return new Color(0.4f, 0.8f, 0.2f); // 예시: 블록 타입 21은 연두색
            case 22: return new Color(0.2f, 0.4f, 0.8f); // 예시: 블록 타입 22은 하늘색
            case 23: return new Color(0.6f, 0.3f, 0.1f); // 예시: 블록 타입 23은 갈색
            case 24: return new Color(0.3f, 0.6f, 0.1f); // 예시: 블록 타입 24은 연두색
            case 25: return new Color(0.1f, 0.3f, 0.6f); // 예시: 블록 타입 25은 하늘색
            case 26: return new Color(0.9f, 0.6f, 0.3f); // 예시: 블록 타입 26은 갈색
            case 27: return new Color(0.6f, 0.9f, 0.3f); // 예시: 블록 타입 27은 연두색
            case 28: return new Color(0.3f, 0.6f, 0.9f); // 예시: 블록 타입 28은 하늘색
            case 29: return new Color(0.7f, 0.4f, 0.2f); // 예시: 블록 타입 29은 갈색
            case 30: return new Color(0.4f, 0.7f, 0.2f); // 예시: 블록 타입 30은 연두색
            default: return Color.white; // 기본 색상
        }
    }
} 