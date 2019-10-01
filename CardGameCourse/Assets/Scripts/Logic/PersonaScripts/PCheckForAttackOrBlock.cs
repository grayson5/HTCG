using UnityEngine;
using System.Collections;

public class PCheckForAttackOrBlock : PersonaConditionEffect
{
    

    public override bool PersonaCheckCondition (Player caster = null, Player ptarget = null)
    {
        bool condexists = false;

        Debug.Log("In PCheckForAttackOrBlock");

        switch (caster.currentphase)
        {
            case "Defense":
                //Check to see if a Defense is in play

                for (int i = 0; i < caster.table.CreaturesOnTable.Count; i++)
                {
                    if (caster.table.CreaturesOnTable[i].ca.AttackDefense == "D" || caster.table.CreaturesOnTable[i].ca.AttackDefense == "G")
                    {
                        condexists = true;
                        GameObject target = IDHolder.GetGameObjectWithID(caster.table.CreaturesOnTable[i].ID);
                        caster.table.CreaturesOnTable[i].PowerAOrB = true;
                        target.GetComponent<OneCreatureManager>().PowerIcon.enabled = true;
                        target.GetComponent<OneCreatureManager>().GlowImageBlock.enabled = true;
                        break;
                    }
                }

                break;

            case "Attack":
                //Check to see if an Attack is in play

                for (int i = 0; i < caster.table.CreaturesOnTable.Count; i++)
                {
                    if (caster.table.CreaturesOnTable[i].ca.AttackDefense == "A")
                    {
                        condexists = true;
                        GameObject target = IDHolder.GetGameObjectWithID(caster.table.CreaturesOnTable[i].ID);
                        caster.table.CreaturesOnTable[i].PowerAOrB = true;
                        //target.GetComponent<OneCreatureManager>().IncreaseDamage(table.CreaturesOnTable[targetgridid].ca.DamageAmt + 2);
                        target.GetComponent<OneCreatureManager>().IncreaseDamage(caster.charAsset.PowerAttack);
                        target.GetComponent<OneCreatureManager>().PowerIcon.enabled = true;
                        target.GetComponent<OneCreatureManager>().CreatureGlowImage.enabled = true;
                        if (caster.table.CreaturesOnTable[i].HiddenAttack == true)
                        {
                            target.GetComponent<OneCreatureManager>().CardBackGlowImage.enabled = true;
                        }
                        ptarget.NextAttackHidden = true;
                        ptarget.PArea.HiddenIconButton1.image.enabled = true;
                        break;
                    }
                }

                break;
            default:
                Debug.Log("Not in correct phase to activate ability");
                break;

        }




        if (condexists)
        {
            Debug.Log("Attack or Block Exist");
            return true;
        }
        else
        {
            Debug.Log("No Attack or Block exists");
            return false;
        }

    }
}
