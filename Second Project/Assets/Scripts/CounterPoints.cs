using GwentPlus;
using TMPro;
using UnityEngine;

public class CounterPoints : MonoBehaviour
{
    public GameObject p1Row_M, p1Row_R, p1Row_S, p2Row_M, p2Row_R, p2Row_S;

    public TextMeshProUGUI p1Counter_M, p1Counter_R, p1Counter_S, p2Counter_M, p2Counter_R, p2Counter_S;
    public TextMeshProUGUI _p1Counter_M, _p1Counter_R, _p1Counter_S, _p2Counter_M, _p2Counter_R, _p2Counter_S; //Estos para la sombra
    
    public TextMeshProUGUI p1TotalCounter, p2TotalCounter; 
    public TextMeshProUGUI _p1TotalCounter, _p2TotalCounter; //Estos para las sombras

    public TextMeshProUGUI p1RoundCounter, _p1RoundCounter;
    public TextMeshProUGUI p2RoundCounter, _p2RoundCounter;


    private int pointsM_P1 = 0, pointsR_P1 = 0, pointsS_P1 = 0, pointsM_P2 = 0, pointsR_P2 = 0, pointsS_P2 = 0;

    public static int totalPoints_P1;
    public static int totalPoints_P2;

    public static int totalRound_P1;
    public static int totalRound_P2;


    public void ActualizePoints()
    {
         // Actualizar puntos para el jugador 1
        pointsM_P1 = CalculateRowPoints(p1Row_M);
        pointsR_P1 = CalculateRowPoints(p1Row_R);
        pointsS_P1 = CalculateRowPoints(p1Row_S);

        // Actualizar puntos para el jugador 2
        pointsM_P2 = CalculateRowPoints(p2Row_M);
        pointsR_P2 = CalculateRowPoints(p2Row_R);
        pointsS_P2 = CalculateRowPoints(p2Row_S);
    }

    public static int CalculateRowPoints(GameObject row)
    {
        int totalPoints = 0;
        // Obtener todos los objetos hijos de la fila que tengan el componente CardDisplay
        CardDisplay[] cardDisplays = row.GetComponentsInChildren<CardDisplay>();

        foreach (CardDisplay cardDisplay in cardDisplays)
        {
            // Acceder a la tarjeta de cada CardDisplay
            Card card = cardDisplay.card;
            if(card.Type == CardType.Oro || card.Type == CardType.Plata)
            {
                totalPoints += card.Power;
            }
        }

        return totalPoints;
    }

    public void ActualizeVisual()
    {
        // Actualizar visualmente los puntos para el jugador 1 con las sombras tambien
        p1Counter_M.text = pointsM_P1.ToString();
        _p1Counter_M.text = pointsM_P1.ToString();
        p1Counter_R.text = pointsR_P1.ToString();
        _p1Counter_R.text = pointsR_P1.ToString();
        p1Counter_S.text = pointsS_P1.ToString();
        _p1Counter_S.text = pointsS_P1.ToString();

        // Actualizar visualmente los puntos para el jugador 2
        p2Counter_M.text = pointsM_P2.ToString();
        _p2Counter_M.text = pointsM_P2.ToString();
        p2Counter_R.text = pointsR_P2.ToString();
        _p2Counter_R.text = pointsR_P2.ToString();
        p2Counter_S.text = pointsS_P2.ToString();
        _p2Counter_S.text = pointsS_P2.ToString();

        //Actualizar la cantidad de rondas ganadas por  cada jugador 
        p1RoundCounter.text = "Rounds: " + totalRound_P1.ToString();
        _p1RoundCounter.text = "Rounds: " + totalRound_P1.ToString();

        p2RoundCounter.text = "Rounds: " + totalRound_P2.ToString();
        _p2RoundCounter.text = "Rounds: " + totalRound_P2.ToString();
    }

    public void CalculeTotalPoints()
    {
        totalPoints_P1 = CalculateRowPoints(p1Row_M) + CalculateRowPoints(p1Row_R) + CalculateRowPoints(p1Row_S);
        totalPoints_P2 = CalculateRowPoints(p2Row_M) + CalculateRowPoints(p2Row_R) + CalculateRowPoints(p2Row_S); 
        
        ActualizeTotalPoints();
    } 

    public void ActualizeTotalPoints()
    {
        p1TotalCounter.text = "Points: " + totalPoints_P1.ToString();
        _p1TotalCounter.text = "Points: " + totalPoints_P1.ToString();

        p2TotalCounter.text = "Points: " + totalPoints_P2.ToString();
        _p2TotalCounter.text = "Points: " + totalPoints_P2.ToString();
    }

    void Update()
    {
        ActualizePoints();
        ActualizeVisual();
        CalculeTotalPoints();
        ActualizeTotalPoints();
    }
}
