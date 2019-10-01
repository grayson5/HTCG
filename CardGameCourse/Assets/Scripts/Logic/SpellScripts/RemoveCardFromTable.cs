using UnityEngine;
using System.Collections;

public class RemoveCardFromTable : SpellEffect 
{
    

    public override void ActivateEffect(int specialAmount = 0, ICharacter target = null, Player caster = null, Player ptarget = null, string specialData = null)
    {
        if (target != null)
        {
            Debug.Log("In RemoveCardFromTable");
            Debug.Log("Target ID: " + target.ID);
            Debug.Log("Owner: " + target.Owner());
            
            //Debug.Log("Name: " + g.GetComponent<OneCardManager>().cardAsset.name);

            if (target.Owner() == caster)
            {
                Debug.Log("Owner is: " + caster.name);
                for (int i = 0; i < caster.table.CreaturesOnTable.Count; i++)
                {
                    if (caster.table.CreaturesOnTable[i].UniqueCreatureID == target.ID)
                    {
                        Debug.Log("Found Creature in table: " + i);
                        CardLogic newdisCard = new CardLogic(caster.table.CreaturesOnTable[i].ca);
                        if (specialData == newdisCard.ca.TypeOfSpecial.ToString())
                        {
                            if (newdisCard.removeeffect != null)
                                newdisCard.removeeffect.RemoveEffect(newdisCard.ca.specialSpellAmount, null, caster, null, null);
                            newdisCard.owner = caster;
                            caster.discardcards.CardsInDiscard.Insert(0, newdisCard);
                            caster.PArea.tableVisual.RemoveCreatureWithID(caster.table.CreaturesOnTable[i].UniqueCreatureID);
                            caster.table.CreaturesOnTable.Remove(caster.table.CreaturesOnTable[i]);
                            break;
                        }
                        else
                        {
                            new ShowMessageCommand("Card targeted not a " + specialData, 1f).AddToQueue();
                            break;
                        }
                    }
                }
            }
            else
            {
                Debug.Log("Owner is: " + ptarget.name);
                for (int i = 0; i < ptarget.table.CreaturesOnTable.Count; i++)
                {
                    if (ptarget.table.CreaturesOnTable[i].UniqueCreatureID == target.ID)
                    {
                        Debug.Log("Found Creature in table: " + i);
                        CardLogic newdisCard = new CardLogic(ptarget.table.CreaturesOnTable[i].ca);
                        if (specialData == newdisCard.ca.TypeOfSpecial.ToString())
                        {
                            if (newdisCard.removeeffect != null)
                                newdisCard.removeeffect.RemoveEffect(newdisCard.ca.specialSpellAmount, null, null, ptarget, null);
                            newdisCard.owner = ptarget;
                            ptarget.discardcards.CardsInDiscard.Insert(0, newdisCard);
                            ptarget.PArea.tableVisual.RemoveCreatureWithID(ptarget.table.CreaturesOnTable[i].UniqueCreatureID);
                            ptarget.table.CreaturesOnTable.Remove(ptarget.table.CreaturesOnTable[i]);
                            break;
                        }
                        else
                        {
                            new ShowMessageCommand("Card targeted not a " + specialData, 1f).AddToQueue();
                            break;
                        }
                    }
                }
            }


        }

    }
}
