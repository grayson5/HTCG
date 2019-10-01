using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public class AddAbilityCommand : Command {

    private int targetID;
    private int amount;
    private int healthAfter;

    public AddAbilityCommand( int targetID, int amount, int healthAfter, Player target)
    {
        this.targetID = targetID;
        this.amount = amount;
        this.healthAfter = healthAfter;
        //Debug.Log("amount: " + amount);
        //Debug.Log("healthafter: " + healthAfter);
    }

    public override void StartCommandExecution()
    {
        Debug.Log("Add ability command!");

        GameObject target = IDHolder.GetGameObjectWithID(targetID);
        if (targetID == GlobalSettings.Instance.LowPlayer.PlayerID || targetID == GlobalSettings.Instance.TopPlayer.PlayerID)
        {
            // target is a hero
            target.GetComponent<PlayerPortraitVisual>().AddAbility(amount,healthAfter);
        }
        else
        {
            // target is a creature
            target.GetComponent<OneCreatureManager>().TakeDamage(amount, healthAfter);
        }
        CommandExecutionComplete();
    }
}
