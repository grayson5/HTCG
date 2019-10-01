using UnityEngine;
using System.Collections;

public class CheckForObject : ConditionEffect
{
    

    public override bool CheckCondition (int specialAmount = 0, ICharacter target = null, Player caster = null, Player ptarget = null, string specialName = null, string cardname = null)
    {
        bool objectexists = false;

        Debug.Log("In CheckForObject for card: " + cardname);

        //Check to see if a copy of plot card is in play already; if so it can't be played.
        if (cardname != "" || cardname != null)
        {
            for (int i = 0; i < caster.table.CreaturesOnTable.Count; i++)
            {
                if (cardname == caster.table.CreaturesOnTable[i].ca.ObjectName)
                {
                    objectexists = true;
                    break;
                }
            }
        }

        if (objectexists)
            return false;
        else
            return true;

    }
}
