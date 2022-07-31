using System.Collections;
using UnityEngine;
using TMPro;
using static Actions;

public class CanvasHelper : MonoSingleton<CanvasHelper>
{
    [Header("Game Panels")] [SerializeField]
    private GameObject startPanel;
    
    [SerializeField] private GameObject successPanel;
    [SerializeField] private GameObject failPanel;
    
    [Header("Texts")] [SerializeField] private TMP_Text movesText;

    /// <summary>
    /// Set the desired panel as given state.
    /// </summary>
    public void SetPanel(CanvasPanel panel, bool state)
    {
        switch (panel)
        {
            case CanvasPanel.Start:
                startPanel.SetActive(state);
                return;
            case CanvasPanel.Success:
                successPanel.SetActive(state);
                return;
            case CanvasPanel.Fail:
                failPanel.SetActive(state);
                return;
            default:
                return;
        }
    }

    public void SetMovesText()
    {
        movesText.text = GameManager.Instance.moves.ToString();
    }
}

public enum CanvasPanel
{
    Start,
    Success,
    Fail,
}