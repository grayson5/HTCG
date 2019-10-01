using UnityEngine;
using System.Collections;

public class RemoveFromPlayEffect
{
    public virtual void RemoveEffect (int specialAmount = 0, ICharacter target = null, Player caster = null, Player ptarget = null, string specialName = null, string cardname = null)
    {
        Debug.Log("No Spell effect with this name found! Check for typos in CardAssets");
    }
        
}
