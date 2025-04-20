using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class Block : StaticBlock
{
    [SerializeField] private float rotationDuration = 0.3f;

    private Tween currentTween;
    [HideInInspector] public bool tweening = false;

    private void Update()
    {
        // 모바일에서 클릭 감지
        if (Input.touchCount > 0)
        {
            if(!GameManager.Instance.playing)return;

            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    // 맞은 오브젝트가 자신인지 확인
                    if (hit.transform == transform)
                    {
                        HandleBlockClick();
                    }
                }
            }
        }
    }

    public void HandleBlockClick(bool checkEnd = true)
    {
        if (tweening) return;

        for (int i = 0; i < neighbors.Count; i++)
        {
            for (int j = 0; j < neighbors[i].neighbors.Count; j++)
            {
                if(neighbors[i].neighbors[j] is Block)
                    ((Block)neighbors[i].neighbors[j]).StopTween();
            }
            if (neighbors[i] is Block)
                ((Block)neighbors[i]).StopTween();
        }
        StopTween();

        RotateNeighbors();
        
        // 게임 클리어 체크
        if(checkEnd)
            GameManager.Instance.CheckGameClear();
    }

    public void StopTween()
    {
        if (currentTween != null && currentTween.IsActive())
        {
            currentTween.Complete();
        }
    }

    private void RotateNeighbors()
    {
        if (neighbors.Count < 2) return;

        tweening = true;

        Sequence moveSequence = DOTween.Sequence();
        for (int i = 0; i < neighbors.Count; i++)
        {
            StaticBlock currentBlock = neighbors[i];
            StaticBlock nextBlock = i < neighbors.Count - 1 ? neighbors[i + 1] : neighbors[0];

            moveSequence.Join(
                currentBlock.transform.DOMove(nextBlock.transform.position, rotationDuration)
                .SetEase(Ease.InOutQuad)
            );
        }

        moveSequence.OnComplete(() => {
            tweening = false;
        });

        currentTween = moveSequence;
        UpdateNeighbors();
    }

    private void UpdateNeighbors()
    {
        Vector2Int blockPos = GameManager.Instance.RotateGridPositions(this);

        if (blockPos.x != -1 && blockPos.y != -1)
        {
            List<Vector2Int> neighborPos = GameManager.Instance.GetNeighborPosWithGrid(blockPos.x, blockPos.y);
            neighbors = GameManager.Instance.GetBlocksWithPos(neighborPos);
            for (int i = 0; i < neighborPos.Count; i++)
            {
                List<Vector2Int> pos = GameManager.Instance.GetNeighborPosWithGrid(neighborPos[i].x, neighborPos[i].y);
                neighbors[i].neighbors = GameManager.Instance.GetBlocksWithPos(pos);

                for (int j = 0; j < neighbors[i].neighbors.Count; j++)
                {
                    List<Vector2Int> pos2 = GameManager.Instance.GetNeighborPosWithGrid(pos[j].x, pos[j].y);
                    neighbors[i].neighbors[j].neighbors = GameManager.Instance.GetBlocksWithPos(pos2);
                }
            }
        }
    }
}
