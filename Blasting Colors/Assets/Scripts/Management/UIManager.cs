using UnityEngine;
using UnityEngine.SceneManagement;
using static Actions;


public class UIManager : MonoBehaviour
{
    private CanvasHelper canvasHelper;

    private void Start()
    {
        if (TryGetComponent<CanvasHelper>(out var helper)) canvasHelper = helper;
    }

    private void OnEnable()
    {
        LevelFailed += OnLevelFailed;
        LevelSuccess += OnLevelSuccess;
        TurnEnded += OnTurnEnded;
    }
    
    private void OnDisable()
    {
        LevelFailed -= OnLevelFailed;
        LevelSuccess -= OnLevelSuccess;
        TurnEnded -= OnTurnEnded;
    }

    public void GameStartButton()
    {
        LevelStart?.Invoke();
        ButtonTapped?.Invoke();
        canvasHelper.SetPanel(CanvasPanel.Start, false);
        canvasHelper.SetMovesText();
    }

    private void OnLevelSuccess()
    {
        PlayerPrefs.DeleteAll();
        canvasHelper.SetPanel(CanvasPanel.Success, true);
    }

    private void OnLevelFailed()
    {
        canvasHelper.SetPanel(CanvasPanel.Fail, true);
    }

    public void LoadLevel()
    {
        ButtonTapped?.Invoke();
        SceneManager.LoadScene(sceneBuildIndex: 0);
    }

    private void OnTurnEnded()
    {
        GameManager.Instance.moves--;
        canvasHelper.SetMovesText();
    }
}