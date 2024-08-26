using TMPro;
using UnityEngine;

public class PointsCounter : MonoBehaviour
{
    public GameObject rowM_P1;
    public GameObject rowR_P1;
    public GameObject rowS_P1;
    public GameObject rowM_P2;
    public GameObject rowR_P2;
    public GameObject rowS_P2;

    private int pointsM_P1 = 0;
    private int pointsR_P1 = 0;
    private int pointsS_P1 = 0;
    private int pointsM_P2 = 0;
    private int pointsR_P2 = 0;
    private int pointsS_P2 = 0;

    public TextMeshProUGUI pointsM_p1;
    public TextMeshProUGUI pointsR_p1;
    public TextMeshProUGUI pointsS_p1;
    public TextMeshProUGUI pointsM_p2;
    public TextMeshProUGUI pointsR_p2;
    public TextMeshProUGUI pointsS_p2;

    public TextMeshProUGUI totalPoints_p1;
    public TextMeshProUGUI totalPoints_p2;
    
    public static int totalPoints_P1;
    public static int totalPoints_P2;




    void Update()
    {
        ActualizePoints();
        ActualizeVisual();
        CalculeTotalPoints();
        ActualizeTotalPoints();
    }

    public void ActualizePoints()
    {
         // Actualizar puntos para el jugador 1
        pointsM_P1 = CalculateRowPoints(rowM_P1);
        pointsR_P1 = CalculateRowPoints(rowR_P1);
        pointsS_P1 = CalculateRowPoints(rowS_P1);

        // Actualizar puntos para el jugador 2
        pointsM_P2 = CalculateRowPoints(rowM_P2);
        pointsR_P2 = CalculateRowPoints(rowR_P2);
        pointsS_P2 = CalculateRowPoints(rowS_P2);
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
            if(card.cardType == Card.CardType.UnitCard)
            {
                totalPoints += card.power;
            }
        }

        return totalPoints;
    }

    public void ActualizeVisual()
    {
        // Actualizar visualmente los puntos para el jugador 1
        pointsM_p1.text = pointsM_P1.ToString();
        pointsR_p1.text = pointsR_P1.ToString();
        pointsS_p1.text = pointsS_P1.ToString();

        // Actualizar visualmente los puntos para el jugador 2
        pointsM_p2.text = pointsM_P2.ToString();
        pointsR_p2.text = pointsR_P2.ToString();
        pointsS_p2.text = pointsS_P2.ToString();
    }

    public void CalculeTotalPoints()
    {
        totalPoints_P1 = CalculateRowPoints(rowM_P1) + CalculateRowPoints(rowR_P1) + CalculateRowPoints(rowS_P1);
        totalPoints_P2 = CalculateRowPoints(rowM_P2) + CalculateRowPoints(rowR_P2) + CalculateRowPoints(rowS_P2); 
        
        ActualizeTotalPoints();
    } 

    public void ActualizeTotalPoints()
    {
        totalPoints_p1.text = "Points: " + totalPoints_P1.ToString();
        totalPoints_p2.text = "Points: " + totalPoints_P2.ToString();

    }


}