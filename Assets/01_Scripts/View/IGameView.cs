using UnityEngine.Events;

public interface IGameView
{
    void ShowClearText();
    void StartGame(int stage);
    void NextStage();
    void BindNext(UnityAction action);
    void BindStage(int index, UnityAction action);
    void BindMenu(UnityAction action);
    void ClickMenu();
} 