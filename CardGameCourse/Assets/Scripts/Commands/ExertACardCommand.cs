using UnityEngine;
using System.Collections;

public class ExertACardCommand : Command {

    private Player p;
    private CardLogic cl;
    private bool fast;
    private bool fromDeck;

    public ExertACardCommand(CardLogic cl, Player p, bool fast, bool fromDeck)
    {        
        this.cl = cl;
        this.p = p;
        this.fast = fast;
        this.fromDeck = fromDeck;
    }

    public override void StartCommandExecution()
    {
        p.PArea.PDeck.CardsInDeck--;
        p.PArea.discardVisual.GiveDiscardACard(cl.ca, cl.UniqueCardID, fast, fromDeck);
    }
}
