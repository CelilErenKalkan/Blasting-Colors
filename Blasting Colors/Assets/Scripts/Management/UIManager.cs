using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using static Actions;


public class UIManager : MonoBehaviour
{
    private int levelNo;
    private bool isLevelSuccess;
    private CanvasHelper canvasHelper;

    private void Start()
    {
        levelNo = PlayerPrefs.GetInt("LevelNo", 1);
        if (TryGetComponent<CanvasHelper>(out var helper)) canvasHelper = helper;
        canvasHelper.SetLevelText(levelNo);
        canvasHelper.SetMovesText();
    }

    private void OnEnable()
    {
        LevelFailed += OnLevelFailed;
        LevelSuccess += OnLevelSuccess;
        DotDestroyed += OnDotDestroyed;
    }
    
    private void OnDisable()
    {
        LevelFailed -= OnLevelFailed;
        LevelSuccess -= OnLevelSuccess;
        DotDestroyed -= OnDotDestroyed;
    }

    public void GameStartButton()
    {
        LevelStart?.Invoke();
        ButtonTapped?.Invoke();
        canvasHelper.SetPanel(CanvasPanel.Start, false);
        
        //GameManager.Instance.isPlayable = true;
    }

    private void OnLevelSuccess()
    {
        isLevelSuccess = true;
        canvasHelper.SetPanel(CanvasPanel.Success, true);
    }

    private void OnLevelFailed()
    {
        canvasHelper.SetPanel(CanvasPanel.Fail, true);
    }

    public void LoadLevel()
    {
        if (isLevelSuccess)
        {
            levelNo++;
        }

        isLevelSuccess = false;
        PlayerPrefs.SetInt("LevelNo", levelNo);
        
        ButtonTapped?.Invoke();
        SceneManager.LoadScene(sceneBuildIndex: 1);
    }

    private void OnDotDestroyed()
    {
        GameManager.Instance.moves--;
        canvasHelper.SetMovesText();
    }
}