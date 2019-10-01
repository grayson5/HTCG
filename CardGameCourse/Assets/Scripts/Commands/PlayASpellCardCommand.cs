using UnityEngine;
using System.Collections;

public class PlayASpellCardCommand: Command
{
    private CardLogic card;
    private Player p;
    private int index;
    private int SpellID;
    //private ICharacter target;

    public PlayASpellCardCommand(Player p, CardLogic card, int index, int SpellID)
    {
        this.card = card;
        this.p = p;
        this.index = index;
        this.SpellID = SpellID;
    }

    public override void StartCommandExecution()
    {
        //Following Line are NEW REMOVE TO GO BACK TO OLD WAY
        HandVisual PlayerHand = p.PArea.handVisual;
        GameObject scard = IDHolder.GetGameObjectWithID(card.UniqueCardID);
        PlayerHand.RemoveCard(scard);
        GameObject.Destroy(scard);
        // move this card to the spot
        //p.PArea.handVisual.PlayASpecialFromHand(card.UniqueCardID);
        //Uncomment ABOVE LINE TO GO BACK TO OLD WAY
        Debug.Log("OLD PASC ID: " + card.UniqueCardID);
        Debug.Log("NEW PASC ID: " + SpellID);
        p.PArea.tableVisual.AddSpecialAtIndex(card.ca, SpellID, index);
        // do all the visual stuff (for each spell separately????)
    }
}
