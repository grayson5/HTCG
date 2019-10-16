using UnityEngine;
using System.Collections;

public class ExertForDefOrAtt : Command {

    private Player p;
    private CardLogic cl;
    private bool fast;
    private bool fromDeck;
    private string exerttype;

    public ExertForDefOrAtt(CardLogic cl, Player p, bool fast, string exerttype, bool fromDeck)
    {        
        this.cl = cl;
        this.p = p;
        this.fast = fast;
        this.fromDeck = fromDeck;
        this.exerttype = exerttype;
    }

    public override void StartCommandExecution()
    {
        p.PArea.PDeck.CardsInDeck--;
        //p.PArea.discardVisual.GiveDiscardACard(cl.ca, cl.UniqueCardID, fast, fromDeck);
        p.PArea.ExertPanel.SetActive(true);
        p.PArea.ExertPanel.GetComponentInChildren<ExertListControl>().AddExertCardToList(cl.ca, cl.UniqueCardID);
    }
}
