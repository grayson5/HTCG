using UnityEngine;
using System.Collections;

public class ConditionEffect
{
    public virtual bool CheckCondition (int specialAmount = 0, ICharacter target = null, Player caster = null, Player ptarget = null, string specialName = null, string cardname = null)
    {
        Debug.Log("No Spell effect with this name found! Check for typos in CardAssets");
        return true;
    }
        
}
