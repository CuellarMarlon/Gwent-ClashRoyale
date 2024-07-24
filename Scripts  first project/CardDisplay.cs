using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;


public class CardDisplay : MonoBehaviour, IPointerEnterHandler
{
    public Card card;
    public CardZoom cardZoom;

    public TextMeshProUGUI cardName;
    public TextMeshProUGUI power;
    public GameObject powerContainer;
    public Image cardPhoto;

    public List<string> validZonesTag;

    void Start()
    {
        InitializeCard();
        
    }

    public void InitializeCard()
    {
        switch (card.cardType)
        {
            case Card.CardType.UnitCard:
            cardName.text = card.cardName;
            power.text = card.power.ToString();
            powerContainer.SetActive(true);
            cardPhoto.sprite = card.cardPhoto;
            validZonesTag = card.validZonesTag;
            break;

            case Card.CardType.SpecialCard:
            cardName.text = card.cardName;
            cardPhoto.sprite = card.cardPhoto;
            powerContainer.SetActive(false);
            validZonesTag = card.validZonesTag;
            break;

            case Card.CardType.LeaderCard:
            cardName.text = card.cardName;
            cardPhoto.sprite = card.cardPhoto;
            powerContainer.SetActive(false);
            break;
            

        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (cardZoom != null && card != null)
        {
            cardZoom.ShowCardInfo(card);
            
        }
    }

    public void OnPointerExit(PointerEventData evenData)
    {
        if (cardZoom != null && card != null)
        {
            cardZoom.HideCardInfo();
        }
    }    

    public void Update()
    {
        
    }


}
