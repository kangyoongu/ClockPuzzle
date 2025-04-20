public class GamePresenter
{
    private IGameView view;
    private GameManager model;

    public GamePresenter(IGameView view, GameManager model)
    {
        this.view = view;
        this.model = model;
    }
    public void Start()
    {
        view.BindNext(NextStage);
        view.BindMenu(Menu);
        for (int i = 0; i < model.maps.Count; i++)
        {
            int index = i;
            view.BindStage(i, () => StartGame(index));
        }
    }
    public void Clear()
    {
        view.ShowClearText();
    }
    public void StartGame(int stage)
    {
        model.InitializeGrid(stage);
        view.StartGame(stage);
    }
    public void NextStage()
    {
        model.ClearCubes(true);
        view.NextStage();
    }
    public void Menu()
    {
        model.Menu();
        view.ClickMenu();
    }
} 