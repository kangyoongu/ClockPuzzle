using UnityEngine;
using System.Collections.Generic;

public class StaticBlock : MonoBehaviour
{
    internal List<StaticBlock> neighbors = new List<StaticBlock>(); // 이웃 블록 리스트
    internal int blockType; // 구슬의 타입 (색상)

    public List<StaticBlock> Neighbors => neighbors;
    public int BlockType => blockType;

    public void SetNeighbors(List<StaticBlock> blocks)
    {
        neighbors = new List<StaticBlock>(blocks);
    }

    public void SetBlockType(int type)
    {
        blockType = type;
    }
} 