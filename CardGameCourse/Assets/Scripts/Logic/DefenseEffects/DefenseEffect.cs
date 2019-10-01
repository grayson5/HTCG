using UnityEngine;
using System.Collections;

public class DefenseEffect
{
    public virtual void ActivateEffect(int specialAmount = 0, ICharacter target = null, Player caster = null, Player ptarget = null, string specialData = null)
    {
        Debug.Log("No Defense effect with this name found! Check for typos in CardAssets");
    }
        
}
