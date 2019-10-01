﻿using UnityEngine;
using System.Collections;

public class SpellEffect
{
    public virtual void ActivateEffect(int specialAmount = 0, ICharacter target = null, Player caster = null, Player ptarget = null, string specialData = null)
    {
        Debug.Log("No Spell effect with this name found! Check for typos in CardAssets");
    }
        
}
