namespace GwentPlus
{
public class EffectCreated
{
    public void DamageEffect(CardList targets, GameContext context , int Amount)
    {
        foreach (Card target in targets)
        {
        var i = 0;
        while (i < Amount)
        {
         target.Power -= 1;
         i += 1;
        }
        }
    }

    public void DrawEffect(CardList targets, GameContext context )
    {
        var topCard = context.Deck.Pop();
        context.Hand.Add(topCard);
        context.Hand.Shuffle();
    }

    public void ReturnToDeckEffect(CardList targets, GameContext context )
    {
        foreach (Card target in targets)
        {
        var deck = context.Deck;
        deck.Push(target);
        deck.Shuffle();
        context.Board.Remove(target);
        }
    }

}
}
