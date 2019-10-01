using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EndDefensePhase : SpellEffect 
{
    

    public override void ActivateEffect(int specialAmount = 0, ICharacter target = null, Player caster = null, Player ptarget = null, string specialData = null)
    {
        Debug.Log("In EndDefensePhase");

        if (caster != null)
        {
            caster.currentphase = "Attack";
            GameObject.FindWithTag("ButtonText").GetComponent<Text>().text = "End Attack Phase";
            GameObject.FindWithTag("PowerButton").GetComponent<Text>().text = "Exert for Power Blow";
            caster.HighlightPlayableCards();
            //caster.abilitygained += specialAmount;
            new ShowMessageCommand(caster.name + " escapes to Holy Ground!", 1f).AddToQueue();
        }

    }
}
