using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using GwentPlus;
using System.Collections.Generic;
using TMPro;

public class CreateCard : MonoBehaviour
{
    public GameObject InputField, CardsCreated, CardPrefab;
    public TextMeshProUGUI Input;
    public List<Card> cardsCreated;
    public List<GameObject> instantiatedCards;

    public void Start()
    {
        cardsCreated = new List<Card>();
        instantiatedCards = new List<GameObject>();
    }

    public void OnRunButtonClicked()
    {
        CodeGenerator(ParserFunction(LexerFunction()));
        if (cardsCreated != null)
        {
            CleanUpInstantiatedCards();
        }
        ShowCards();
    }

    public void OnBackButtonClicked()
    {
        SceneManager.LoadScene(0);
    }

    private List<Token> LexerFunction()
    {
        Lexer lexer = new Lexer(Input.text);
        List<Token> tokens = lexer.Tokenize();
        return tokens;
    }

    private List<ASTNode> ParserFunction(List<Token> tokens)
    {
        Parser parser = new Parser(tokens);
        List<ASTNode> ast = parser.Parse();
        return ast;
    }

    private void CodeGenerator(List<ASTNode> ast)
    {
        CodeGenerator codeGenerator = new CodeGenerator(ast);
        codeGenerator.GenerateCode("EffectCreated.cs");

        //Agregar cada carta creada en codeGenerator a cardsCreated
        foreach (var card in codeGenerator._cards)
        {
            cardsCreated.Add(card);
        }
    }

     private void ShowCards()
    {
        foreach (var card in cardsCreated)
        {
            GameObject clone = Instantiate(CardPrefab, CardsCreated.transform);
            CardDisplay cardDisplay = clone.GetComponent<CardDisplay>();
            cardDisplay.card = card;
            cardDisplay.InitializeCard();

            instantiatedCards.Add(clone); // Agrega la instancia de la carta a la lista de cartas instanciadas
        }
    }

    // Método para limpiar las cartas instanciadas cuando ya no sean necesarias
    public void CleanUpInstantiatedCards()
    {
        foreach (var card in instantiatedCards)
        {
            Destroy(card);
        }
        instantiatedCards.Clear(); // Limpia la lista de cartas instanciadas
    }
}
