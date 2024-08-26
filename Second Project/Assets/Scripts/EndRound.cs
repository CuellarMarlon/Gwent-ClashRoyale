using TMPro;
using UnityEngine;

public class EndRound : MonoBehaviour
{
    public GameObject winPanel; //Panel que mostrara mensaje de quien gano 
    public TextMeshProUGUI winText; // texto para actualizar

    public static int counter = 0; // para contar los dos end round de los jugadores  
    public int numRoundPlayed = 0; // para contar la cantidad de  rondas jugadas 

    void Start()
    {
        winPanel.SetActive(false);
    }

    public void OnButtonClick()
    {
        counter += 1;
        if (counter == 2)
        {
            GameManager.Instance.isFirst = true;
            GameManager.Instance.DrawnCard(2);
            VerifyWinRound();
            VerifyWinGame();
            GameManager.Instance.SendCardsToCementery();
        }
        
        GameManager.Instance.StartTurn();
    }

    private void VerifyWinRound()
    {
        if (CounterPoints.totalPoints_P1 > CounterPoints.totalPoints_P2)
        {
            CounterPoints.totalRound_P1 += 1;
        }
        else if (CounterPoints.totalPoints_P2 > CounterPoints.totalPoints_P1)
        {
            CounterPoints.totalRound_P2 += 1;
        }
        else 
        {
            CounterPoints.totalRound_P1 += 1;
            CounterPoints.totalRound_P2 += 1; 
        }
    }

    private void VerifyWinGame()
    {
        if (CounterPoints.totalRound_P1 == 2 && CounterPoints.totalRound_P2 == 2)
        {
            winPanel.SetActive(true);
            winText.text = "Game Over: 'Is a Draw!'";
        }
        else if (CounterPoints.totalRound_P1 == 2)
        {
            winPanel.SetActive(true);
            winText.text = "Game Over: 'Player1 Win!'";
        }
        else if (CounterPoints.totalRound_P2 == 2)
        {
            winPanel.SetActive(true);
            winText.text = "Game Over: 'Player2 Win!'";
        }
    }

}