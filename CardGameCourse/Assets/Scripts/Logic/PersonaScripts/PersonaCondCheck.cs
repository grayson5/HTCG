using UnityEngine;
using System.Collections;

public class PersonaConditionEffect
{
    public virtual bool PersonaCheckCondition (Player caster = null, Player ptarget = null)
    {
        Debug.Log("No Persona effect name found! Check for typos in CardAssets");
        return true;
    }
        
}
