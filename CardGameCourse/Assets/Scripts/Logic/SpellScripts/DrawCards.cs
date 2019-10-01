using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DrawCards : SpellEffect 
{
    

    public override void ActivateEffect(int specialAmount = 0, ICharacter target = null, Player caster = null, Player ptarget = null, string specialData = null)
    {
        Debug.Log("In DrawCards");
        Debug.Log("-----------------> Draw AMOUNT: " + specialAmount);

        if (caster != null)
        {

            caster.MustOrMayAction = true;
            caster.ProcessEventCard = true;
            caster.MMDrawCards = specialAmount;
            caster.MMDiscardCards = "R";
            caster.MayMustMessage = "You must draw " + specialAmount + " cards from your Endurance";
            Debug.Log("Setting MMDrawCards to: " + caster.MMDrawCards);

            new ShowMessageCommand(caster.name + " must draw " + specialAmount + " cards from Endurance", 1f).AddToQueue();

            GameObject.FindWithTag("PowerButton").GetComponent<Text>().text = "Draw 1 Card";
        }
    }
}
