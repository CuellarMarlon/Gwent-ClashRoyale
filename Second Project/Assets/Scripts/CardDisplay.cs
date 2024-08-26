using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using GwentPlus;
using UnityEngine.SceneManagement;


public class CardDisplay : MonoBehaviour, IPointerEnterHandler
{
    public Card card;
    public CardZoom cardZoom;
    public CardInfo cardInfo;


    public TextMeshProUGUI cardName;
    public TextMeshProUGUI shadowCardName;
    public TextMeshProUGUI power;
    public TextMeshProUGUI shadowPower;
    public GameObject powerContainer;
    public GameObject cardBack;
    public Image cardPhoto;

    public GameObject deckP1, deckP2;

    void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1) 
        {
            cardInfo = GameObject.Find("InfoCardObject").GetComponent<CardInfo>();

        }
        else if (SceneManager.GetActiveScene().buildIndex == 3) 
        {
            cardZoom = GameObject.Find("CardZoom").GetComponent<CardZoom>();
            deckP1 = GameObject.Find("Deck_P1");
            deckP2 = GameObject.Find("Deck_P2");
            InitializeCard();
        }
    
    }

    public void InitializeCard()
    {
        if (card.Type == CardType.Oro || card.Type == CardType.Plata)
        {
            cardName.text = card.Name;
            shadowCardName.text = card.Name;
            power.text = card.Power.ToString();
            shadowPower.text = card.Power.ToString();
            powerContainer.SetActive(true);
            cardPhoto.sprite = card.Photo;
        }
        else if (card.Type == CardType.Clima || card.Type == CardType.Despeje || card.Type == CardType.Senuelo || card.Type == CardType.Aumento || card.Type == CardType.Lider)
        {
            cardName.text = card.Name;
            shadowCardName.text = card.Name;
            cardPhoto.sprite = card.Photo;
            powerContainer.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (cardZoom != null && card != null)
        {
            cardZoom.ShowCardInfo(card);
            
        }
        if (cardInfo != null && card != null)
        {
            cardInfo.ShowCardInfo(card);
        }
    }

    public void OnPointerExit(PointerEventData evenData)
    {
        if (cardZoom != null && card != null)
        {
            cardZoom.HideCardInfo();
        }
        if (cardInfo != null && card != null)
        {
            cardInfo.HideCardInfo();
        }
    }    

    public void Update()
    {
        if (this.transform.parent == deckP1.transform || this.transform.parent == deckP2.transform)
        {
            cardBack.SetActive(true);
        }  
        else
        {
            cardBack.SetActive(false);
        }      
    }


}
