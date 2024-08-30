using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GwentPlus;
using TMPro;
using UnityEngine.SceneManagement;


public class FactionButton : MonoBehaviour
{
    public TextMeshProUGUI p1, p2, sp1, sp2;
    public int counter;

    void Start()
    {
        counter = 0;
    }

    public void OnDarkButtonClicked()
    {
        counter += 1;

        if (counter == 1)
        {
            SaveCardsForPlayer(1, "dark");
        }
        else if (counter == 2)
        {
            SaveCardsForPlayer(2, "dark");
            SceneManager.LoadScene(4);
        }
        
    }

    public void OnCelestialButtonClicked()
    {
        counter += 1;

        if (counter == 1)
        {
            SaveCardsForPlayer(1, "celestial");
        }
        else if (counter == 2)
        {
            SaveCardsForPlayer(2, "celestial");
            SceneManager.LoadScene(4);
        }

        
    }
    
    private void SaveCardsForPlayer(int owner, string faction)
    {
        if (owner == 1 && faction == "dark")
        {
            foreach (Card card in DataGame.Instance.darkCards)
            {
                card.Owner = 1;
                DataGame.Instance.p1Cards.Add(card);
            }
            foreach (Card card in DataGame.Instance.specialCards)
            {
                card.Owner = 1;
                DataGame.Instance.p1Cards.Add(card);
            }
            foreach (Card card in DataGame.Instance.createdCards)
            {
                card.Owner = 1;
                if (card.Faction == Faction.Dark)
                DataGame.Instance.p1Cards.Add(card);
            }
            DataGame.Instance.p1Leader = DataGame.Instance.leaderDark;

        }
        else if (owner == 2 && faction == "dark")
        {
            foreach (Card card in DataGame.Instance.darkCards)
            {
                card.Owner = 2;
                DataGame.Instance.p2Cards.Add(card);
            }
            foreach (Card card in DataGame.Instance.specialCards)
            {
                card.Owner = 2;
                DataGame.Instance.p2Cards.Add(card);
            }
            foreach (Card card in DataGame.Instance.createdCards)
            {
                card.Owner = 2;
                if (card.Faction == Faction.Dark)
                DataGame.Instance.p2Cards.Add(card);
            }
            DataGame.Instance.p2Leader = DataGame.Instance.leaderDark;


        }
        else if (owner == 1 && faction == "celestial")
        {
            foreach (Card card in DataGame.Instance.celestialCards)
            {
                card.Owner = 1;
                DataGame.Instance.p1Cards.Add(card);
            }
            foreach (Card card in DataGame.Instance.specialCards)
            {
                card.Owner = 1;
                DataGame.Instance.p1Cards.Add(card);
            }
            foreach (Card card in DataGame.Instance.createdCards)
            {
                card.Owner = 1;
                if (card.Faction == Faction.Celestial)
                DataGame.Instance.p1Cards.Add(card);
            }
            DataGame.Instance.p1Leader = DataGame.Instance.leaderCelestial;

        }
        else if (owner == 2 && faction == "celestial")
        {
            foreach (Card card in DataGame.Instance.celestialCards)
            {
                card.Owner = 2;
                DataGame.Instance.p2Cards.Add(card);
            }
            foreach (Card card in DataGame.Instance.specialCards)
            {
                card.Owner = 2;
                DataGame.Instance.p2Cards.Add(card);
            }
            foreach (Card card in DataGame.Instance.createdCards)
            {
                card.Owner = 2;
                if (card.Faction == Faction.Celestial)
                DataGame.Instance.p2Cards.Add(card);
            }
            DataGame.Instance.p2Leader = DataGame.Instance.leaderCelestial;

        }

        
    }

    // Update is called once per frame
    void Update()
    {
        if (counter == 0)
        {
            p1.enabled = true;
            sp1.enabled = true;
            p2.enabled = false;
            sp2.enabled = false;
        }
        else 
        {
            p2.enabled = true;
            sp2.enabled = true;
            p1.enabled = false;
            sp1.enabled = false;
        }
    }


}
