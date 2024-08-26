using UnityEngine;
using GwentPlus;

public class TriggerLogger : MonoBehaviour
{
    public bool _enabled = true; //
    public static bool isColling = false;
    public static CardDisplay otherCard;

    public void OnTriggerEnter2D(Collider2D other)
    {   
        
        if (_enabled && DragDrop.isDrag)
        {
            otherCard = other.GetComponent<CardDisplay>();
            CardDisplay onMyPointerCard = DragDrop.GetCurrentDraggingCard();
            
            if (otherCard != null && (otherCard.card.Type == CardType.Clima || otherCard.card.Type == CardType.Oro || otherCard.card.Type == CardType.Plata))
            {
                if (otherCard.card.Type == CardType.Clima && onMyPointerCard.card.Type == CardType.Despeje)
                {
                    isColling = true;
                }
                else if ((otherCard.card.Type == CardType.Oro || otherCard.card.Type == CardType.Plata) && onMyPointerCard.card.Type == CardType.Senuelo)
                {
                    isColling = true;
                }
            }
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        isColling = false;
        otherCard = null;
    }
}