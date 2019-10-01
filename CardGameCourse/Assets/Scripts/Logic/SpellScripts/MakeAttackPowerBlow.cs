using UnityEngine;
using System.Collections;

public class MakeAttackPowerBlow : SpellEffect 
{
    

    public override void ActivateEffect(int specialAmount = 0, ICharacter target = null, Player caster = null, Player ptarget = null, string specialData = null)
    {
        if (target != null)
        {
            Debug.Log("In MakeAttackPowerBlow");
            GameObject g = IDHolder.GetGameObjectWithID(target.ID);
            Debug.Log("Name: " + g.GetComponent<OneCreatureManager>().cardAsset.name);
            g.GetComponent<OneCreatureManager>().PowerIcon.enabled = true;
            g.GetComponent<OneCreatureManager>().CreatureGlowImage.enabled = true;
            for (int i = 0; i < caster.table.CreaturesOnTable.Count; i++)
            {
                if (caster.table.CreaturesOnTable[i].UniqueCreatureID == target.ID)
                {
                    Debug.Log("Found Creature in table: " + i);
                    caster.table.CreaturesOnTable[i].PowerAOrB = true;
                    //g.GetComponent<OneCreatureManager>().IncreaseDamage(caster.table.CreaturesOnTable[i].ca.DamageAmt + 2);
                    g.GetComponent<OneCreatureManager>().IncreaseDamage(caster.PowerAttack);
                    break;
                }
            }
            caster.otherPlayer.NextAttackHidden = true;
            caster.otherPlayer.PArea.HiddenIconButton1.image.enabled = true;
            //new ShowMessageCommand("Power Blow!", 2f).AddToQueue();
        }

    }
}
