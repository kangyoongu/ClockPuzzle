using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[Serializable]
public struct UI
{
    public RectTransform changeUI;
    public CanvasGroup fadeUI;
    public Dir dir;
    public Vector2 inAndOut;
    public float time;
    public bool setActive;
    public float fadeFloat;
}


public class UIManager : MonoBehaviour, IGameView
{
    [SerializeField] private Button nextBtn;
    [SerializeField] private Button stageBtnPref;
    [SerializeField] private Button menuBtn;
    [SerializeField] private Transform container;
    private List<Button> stageBtns;

    public UI[] stage;
    public UI[] clear;
    public UI[] play;

    public GameObject block;

    private void Start()
    {
        stageBtns = new List<Button>();
    }
    public void ShowClearText()
    {
        In(clear);
    }
    public void StartGame(int stage)
    {
        Out(this.stage);
        In(play);
    }
    public void NextStage()
    {
        Out(clear);
    }
    public void BindNext(UnityAction action)
    {
        nextBtn.onClick.AddListener(action);
    }

    public void BindStage(int index, UnityAction action)
    {
        if (index >= stageBtns.Count)
        {
            Button newStageBtn = Instantiate(stageBtnPref);
            newStageBtn.transform.SetParent(container);
            TextMeshProUGUI tmpText = newStageBtn.GetComponentInChildren<TextMeshProUGUI>();
            tmpText.text = (index + 1).ToString();
            stageBtns.Add(newStageBtn);
        }

        stageBtns[index].onClick.AddListener(action);
    }

    public void BindMenu(UnityAction action)
    {
        menuBtn.onClick.AddListener(action);
    }
    public void ClickMenu()
    {
        Out(play);
        Out(clear);
        In(stage);
    }

    #region UI_FUNCTION
    private void In(UI[] lst)
    {
        block.SetActive(true);
        float max = 0;
        for (int i = 0; i < lst.Length; i++)
        {
            if (max < lst[i].time) max = lst[i].time;
            if (lst[i].changeUI != null)
            {
                if (lst[i].setActive) lst[i].changeUI.gameObject.SetActive(true);
                if (lst[i].dir == Dir.y) lst[i].changeUI.DOAnchorPosY(lst[i].inAndOut.x, lst[i].time).SetUpdate(true).SetEase(Ease.Linear);
                else lst[i].changeUI.DOAnchorPosX(lst[i].inAndOut.x, lst[i].time).SetUpdate(true).SetEase(Ease.Linear);
            }
            else if (lst[i].fadeUI != null)
            {
                if (lst[i].setActive) lst[i].fadeUI.gameObject.SetActive(true);
                lst[i].fadeUI.DOFade(lst[i].fadeFloat / 255f, lst[i].time).SetUpdate(true).SetEase(Ease.Linear);
            }
        }
        StartCoroutine(BlockTime(max));
    }

    private void Out(UI[] lst)
    {
        block.SetActive(true);
        float max = 0;
        for (int i = 0; i < lst.Length; i++)
        {
            if (max < lst[i].time) max = lst[i].time;
            int index = i;
            if (lst[i].changeUI != null)
            {
                if (lst[i].dir == Dir.y)
                {
                    lst[i].changeUI.DOAnchorPosY(lst[i].inAndOut.y, lst[i].time).SetEase(Ease.Linear).SetUpdate(true).OnComplete(() =>
                    {
                        if (lst[index].setActive) lst[index].changeUI.gameObject.SetActive(false);
                    });
                }
                else
                {
                    lst[i].changeUI.DOAnchorPosX(lst[i].inAndOut.y, lst[i].time).SetEase(Ease.Linear).SetUpdate(true).OnComplete(() =>
                    {
                        if (lst[index].setActive) lst[index].changeUI.gameObject.SetActive(false);
                    });
                }
            }
            else if (lst[i].fadeUI != null)
            {
                lst[i].fadeUI.DOFade(0, lst[i].time).SetEase(Ease.Linear).SetUpdate(true).OnComplete(() =>
                {
                    if (lst[index].setActive) lst[index].fadeUI.gameObject.SetActive(false);
                });
            }
        }
        StartCoroutine(BlockTime(max));
    }
    IEnumerator BlockTime(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        block.SetActive(false);
    }


    #endregion
}