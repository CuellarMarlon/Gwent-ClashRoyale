using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GwentPlus;
using System.Linq;

public class CardZoom : MonoBehaviour
{
    public Image cardImage; 
    public TextMeshProUGUI cardInfoText; 
    public TextMeshProUGUI shadowCardInfoText; 
    
    private Card currentCard; 
     

    public void Start()
    {
        cardImage = GameObject.Find("CardZoomPhoto").GetComponent<Image>();
        cardInfoText = GameObject.Find("CardZoomInfo").GetComponent<TextMeshProUGUI>();
        shadowCardInfoText = GameObject.Find("ShadowCardZoomInfo").GetComponent<TextMeshProUGUI>();

    }

    public void ShowCardInfo(Card card)
    {
        if (card != null)
        {
            currentCard = card;

            cardImage.enabled = true;
            cardInfoText.enabled = true;
            shadowCardInfoText.enabled = true;
            cardImage.sprite = currentCard.Photo;

            if (currentCard.Type == CardType.Oro || currentCard.Type == CardType.Plata)
            {
                // Construir las cadenas para AttackTypes y Effects
                string attackTypes = string.Join(", ", currentCard.Range.Select(r => "\"" + r.ToString() + "\""));
                string effects = currentCard.OnActivation != null && currentCard.OnActivation.Any() ? string.Join(", ", currentCard.OnActivation.Select(e => e.Name)) : "\"No Effect\"";
        
                cardInfoText.text = $"Name: {currentCard.Name}\nPower: {currentCard.Power}\nFaction: {currentCard.Faction}\nCardType: {currentCard.Type}\nAttackType: {attackTypes}\nEffect: {effects}";
                shadowCardInfoText.text = $"Name: {currentCard.Name}\nPower: {currentCard.Power}\nFaction: {currentCard.Faction}\nCardType: {currentCard.Type}\nAttackType: {attackTypes}\nEffect: {effects}";
            }
            else if (currentCard.Type == CardType.Aumento || currentCard.Type == CardType.Senuelo || currentCard.Type == CardType.Despeje || currentCard.Type == CardType.Clima)
            {
                // Similarmente, verifica si OnActivation es nulo o está vacío antes de intentar acceder a sus elementos
                string effects = currentCard.OnActivation != null && currentCard.OnActivation.Any() ? string.Join(", ", currentCard.OnActivation.Select(e => e.Name)) : "\"No Effect\"";

                cardInfoText.text = $"Name: {currentCard.Name}\nFaction: {currentCard.Faction}\nCardType: {currentCard.Type}\nEffect: {effects}";
                shadowCardInfoText.text = $"Name: {currentCard.Name}\nFaction: {currentCard.Faction}\nCardType: {currentCard.Type}\nEffect: {effects}";
            }
            else if (currentCard.Type == CardType.Lider)
            {
                // Verifica si OnActivation es nulo o está vacío antes de intentar acceder a sus elementos
                string ability = currentCard.OnActivation != null && currentCard.OnActivation.Any() ? currentCard.OnActivation[0].Name : "\"No Ability\"";

                cardInfoText.text = $"Name: {currentCard.Name}\nFaction: {currentCard.Faction}\nCardType: {currentCard.Type}\nAbility: {ability}";
                shadowCardInfoText.text = $"Name: {currentCard.Name}\nFaction: {currentCard.Faction}\nCardType: {currentCard.Type}\nAbility: {ability}";

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


