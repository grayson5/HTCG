using UnityEngine;
using System.Collections;

public class MakeBlockPowerBlock : SpellEffect 
{
    

    public override void ActivateEffect(int specialAmount = 0, ICharacter target = null, Player caster = null, Player ptarget = null, string specialData = null)
    {
        if (target != null)
        {
            Debug.Log("In MakeBlockPowerBlock");
            GameObject g = IDHolder.GetGameObjectWithID(target.ID);
            Debug.Log("Name: " + g.GetComponent<OneCreatureManager>().cardAsset.name);
            g.GetComponent<OneCreatureManager>().PowerIcon.enabled = true;
            g.GetComponent<OneCreatureManager>().GlowImageBlock.enabled = true; ;
            for (int i = 0; i < caster.table.CreaturesOnTable.Count; i++)
            {
                if (caster.table.CreaturesOnTable[i].UniqueCreatureID == target.ID)
                {
                    Debug.Log("Found Creature in table: " + i);
                    caster.table.CreaturesOnTable[i].PowerAOrB = true;
                    break;
                }
            }
            //new ShowMessageCommand("Power Block", 2f).AddToQueue();
        }

    }
}
