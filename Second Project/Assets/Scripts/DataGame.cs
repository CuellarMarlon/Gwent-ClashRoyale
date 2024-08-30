using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GwentPlus;

public class DataGame : MonoBehaviour
{
    public static DataGame Instance { get; private set; }

    public List<Card> darkCards = new List<Card>();
    public List<Card> celestialCards = new List<Card>();
    public Card leaderDark;
    public Card leaderCelestial;
    
    

    public List<Card> specialCards = new List<Card>();
    public List<Card> createdCards = new List<Card>();

    public List<Card> p1Cards = new List<Card>();
    public List<Card> p2Cards = new List<Card>();
    public Card p1Leader;
    public Card p2Leader;


    void Awake()
    {
        if (Instance == null) 
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }
}