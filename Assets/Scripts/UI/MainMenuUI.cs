using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    [Header("GamePanel")]
    [SerializeField]
    private TextMeshProUGUI _playerTurnText;

    [Header("EndGamePanel")] 
    [SerializeField] 
    private GameObject _endgamePanel;
    [SerializeField]
    private TextMeshProUGUI _playerWinText;
    
    // Start is called before the first frame update

    public void ChangePlayerTurnUI(Player player)
    {
        _playerTurnText.text = "Joueur " + player.IndexPlayer;
    }

    public void EndGame(Player player)
    {
        _playerWinText.text = "Joueur " + player.IndexPlayer + " à gagné la partie !";
        DisplayEndGamePanel(true);
    }
    
    public void DisplayEndGamePanel(bool value)
    {
        _endgamePanel.SetActive(value);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
