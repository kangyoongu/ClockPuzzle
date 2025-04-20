using System.Collections.Generic;
using UnityEngine;

public class MapGenerator
{
    private Texture2D texture;
    private List<Material> cubeMats;
    private float spacing;

    // 색상 리스트와 색상-인덱스 매핑을 위한 딕셔너리
    private List<Color> colorList = new List<Color>();
    private Dictionary<Color, int> colorToIndexMap = new Dictionary<Color, int>();

    public MapGenerator(Texture2D texture, List<Material> cubePrefabs, float spacing)
    {
        this.texture = texture;
        this.cubeMats = cubePrefabs;
        this.spacing = spacing;
    }

    public StaticBlock[,] GenerateMap()
    {
        int width = texture.width;
        int height = texture.height;

        // 맵의 중심을 (0, 0, 0)으로 설정하기 위한 오프셋 계산
        Vector3 offset = new Vector3((width - 1) * spacing / 2, (height - 1) * spacing / 2, 0f);

        StaticBlock[,] map = new StaticBlock[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Color pixelColor = texture.GetPixel(x, y);
                int prefabIndex = GetPrefabIndexFromColor(pixelColor);

                if (prefabIndex >= 0 && prefabIndex < cubeMats.Count)
                {
                    GameObject cube;
                    if (prefabIndex == 0)
                        cube = Object.Instantiate(GameManager.Instance.staticBlock);
                    else
                    {
                        cube = Object.Instantiate(GameManager.Instance.block);
                        cube.GetComponent<Renderer>().material = cubeMats[prefabIndex-1];
                    }

                    cube.transform.position = new Vector3(x * spacing - offset.x, y * spacing - offset.y, 0f);
                    cube.transform.localScale = Vector3.zero;
                    cube.name = $"Block[{x},{y}]";
                    StaticBlock block = cube.GetComponent<StaticBlock>();
                    block.SetBlockType(prefabIndex);
                    map[x, y] = block; // Block 컴포넌트를 가져와 맵에 저장
                }
            }
        }
        return map; // 생성된 맵을 반환
    }

    private int GetPrefabIndexFromColor(Color color)
    {
        if (color == Color.black)
            return 0;

        if (colorToIndexMap.TryGetValue(color, out int index))
        {
            return index+1; // 존재하면 해당 인덱스 반환
        }
        else
        {
            if (colorList.Count < cubeMats.Count) // 프리팹 수를 초과하지 않도록 확인
            {
                colorList.Add(color);
                int newIndex = colorList.Count - 1;
                colorToIndexMap[color] = newIndex; // 색상과 인덱스 매핑
                return newIndex+1;
            }
        }

        return -1; // 매핑되지 않은 색상
    }
} 