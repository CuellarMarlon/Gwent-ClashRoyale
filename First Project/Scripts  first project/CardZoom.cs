using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardZoom : MonoBehaviour
{
    public Image cardImage; 
    public TextMeshProUGUI cardInfoText; 
    
    private Card currentCard; 
     

    public void Start()
    {
        cardImage = GameObject.Find("CardPhoto").GetComponent<Image>();
        cardInfoText = GameObject.Find("CardDescription").GetComponent<TextMeshProUGUI>();
    }

    public void ShowCardInfo(Card card)
    {
        if (card != null)
        {
            currentCard = card;

            cardImage.enabled = true;
            cardInfoText.enabled = true;

            cardImage.sprite = currentCard.cardPhoto;
            switch (currentCard.cardType)
            {
                case Card.CardType.UnitCard:
                cardInfoText.text = $"Name: {currentCard.cardName}\nPower: {currentCard.power}\nFaction: {currentCard.faction}\nCardType: {currentCard.cardType}\nUnitType: {currentCard.unitType}\nAttackType: {currentCard.attackType}\nEffect: {currentCard.effect}";
                break;
                case Card.CardType.SpecialCard:
                cardInfoText.text = $"Name: {currentCard.cardName}\nFaction: {currentCard.faction}\nCardType: {currentCard.cardType}\nSpecialType: {currentCard.specialType}\nEffect: {currentCard.effect}";
                break;
                case Card.CardType.LeaderCard:
                cardInfoText.text = $"Name: {currentCard.cardName}\nFaction: {currentCard.faction}\nCardType: {currentCard.cardType}\nAbility: {currentCard.effect}";
                break;
            }
        }
    }

    public void HideCardInfo()
    {
        cardImage.enabled = false;
        cardInfoText.enabled = false;
    }

}


