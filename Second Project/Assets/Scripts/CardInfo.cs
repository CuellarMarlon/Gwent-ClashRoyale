using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GwentPlus;
using System.Linq;

public class CardInfo : MonoBehaviour
{
    public Image cardImage; 
    public GameObject powerObject;

    public TextMeshProUGUI cardInfoText, shadowCardInfoText; 
    public TextMeshProUGUI cardPower, shadowCardPower;
    public TextMeshProUGUI cardName, shadowCardName; 
    
    private Card currentCard; 

    public void ShowCardInfo(Card card)
    {
        if (card != null)
        {
            currentCard = card;

            cardImage.enabled = true;
            cardInfoText.enabled = true;
            shadowCardInfoText.enabled = true;
            cardImage.sprite = currentCard.Photo;

            cardName.text = currentCard.Name;
            shadowCardName.text = currentCard.Name;
            
            if (currentCard.Type == CardType.Oro || currentCard.Type == CardType.Plata)
            {
                powerObject.SetActive(true);

                cardPower.text = currentCard.Power.ToString();
                shadowCardPower.text = currentCard.Power.ToString();

                // Construir las cadenas para AttackTypes y Effects
                string attackTypes = string.Join(", ", currentCard.Range.Select(r => "\"" + r.ToString() + "\""));
                string effects = currentCard.OnActivation != null && currentCard.OnActivation.Any() ? string.Join(", ", currentCard.OnActivation.Select(e => e.Name)) : "\"No Effect\"";
                
                cardInfoText.text = $"-Faction: {currentCard.Faction}\n-CardType: {currentCard.Type}\n-AttackType: {attackTypes}\n-Effect: {effects}";
                shadowCardInfoText.text = $"-Faction: {currentCard.Faction}\n-CardType: {currentCard.Type}\n-AttackType: {attackTypes}\n-Effect: {effects}";
            
            }
            else if (currentCard.Type == CardType.Aumento || currentCard.Type == CardType.Senuelo || currentCard.Type == CardType.Despeje || currentCard.Type == CardType.Clima)
            {
                powerObject.SetActive(false);
                
                // Similarmente, verifica si OnActivation es nulo o está vacío antes de intentar acceder a sus elementos
                string effects = currentCard.OnActivation != null && currentCard.OnActivation.Any() ? string.Join(", ", currentCard.OnActivation.Select(e => e.Name)) : "\"No Effect\"";
                
                cardInfoText.text = $"-Faction: {currentCard.Faction}\n-CardType: {currentCard.Type}\n-Effect: {effects}";
                shadowCardInfoText.text = $"-Faction: {currentCard.Faction}\n-CardType: {currentCard.Type}\n-Effect: {effects}";
            }
            else if (card.Type == CardType.Lider)
            {
                 powerObject.SetActive(false);
                // Verifica si OnActivation es nulo o está vacío antes de intentar acceder a sus elementos
                string ability = currentCard.OnActivation != null && currentCard.OnActivation.Any() ? currentCard.OnActivation[0].Name : "\"No Ability\"";
                
                cardInfoText.text = $"-Faction: {currentCard.Faction}\n-CardType: {currentCard.Type}\n-Ability: {ability}";
                shadowCardInfoText.text = $"-Faction: {currentCard.Faction}\n-CardType: {currentCard.Type}\n-Ability: {ability}";

            }
        }
    }

    public void HideCardInfo()
    {
        cardImage.enabled = false;
        cardInfoText.enabled = false;
        cardInfoText.enabled = false;
        shadowCardInfoText.enabled = false;
    }

}


