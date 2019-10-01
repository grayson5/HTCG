using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[System.Serializable]
public class CardLogic: IIdentifiable
{
    // reference to a player who holds this card in his hand
    public Player owner;
    // an ID of this card
    public int UniqueCardID; 
    // a reference to the card asset that stores all the info about this card
    public CardAsset ca;
    // a script of type spell effect that will be attached to this card when it`s created
    public SpellEffect effect;
    public SpellEffect secondeffect;
    public ConditionEffect checkcond;
    public RemoveFromPlayEffect removeeffect;
    //public DefenseEffect defenseeffect;


    // STATIC (for managing IDs)
    public static Dictionary<int, CardLogic> CardsCreatedThisGame = new Dictionary<int, CardLogic>();


    // PROPERTIES
    public int ID
    {
        get{ return UniqueCardID; }
    }

    public int CurrentManaCost{ get; set; }

    public bool CanBePlayed
    {
        get
        {
            bool ownersTurn = (TurnManager.Instance.whoseTurn == owner);
            // for spells the amount of characters on the field does not matter
            bool fieldNotFull = true;
            // but if this is a creature, we have to check if there is room on board (table)
            if (ca.MaxHealth > 0)
                fieldNotFull = (owner.table.CreaturesOnTable.Count < 7);
            //Debug.Log("Card: " + ca.name + " has params: ownersTurn=" + ownersTurn + "fieldNotFull=" + fieldNotFull + " hasMana=" + (CurrentManaCost <= owner.ManaLeft));
            return ownersTurn && fieldNotFull;
        }
    }

    // CONSTRUCTOR
    public CardLogic(CardAsset ca)
    {
        // set the CardAsset reference
        //Debug.Log("In CL Constructor");
        this.ca = ca;
        // get unique int ID
        UniqueCardID = IDFactory.GetUniqueID();
        //UniqueCardID = IDFactory.GetUniqueID();
        ResetManaCost();
        // create an instance of SpellEffect with a name from our CardAsset
        // and attach it to 
        //Debug.Log("CardName: " + ca.name);
        if (ca.SpecialScriptName!= null && ca.SpecialScriptName!= "")
        {
            //Debug.Log("SpecialScript: " + ca.SpecialScriptName);
            effect = System.Activator.CreateInstance(System.Type.GetType(ca.SpecialScriptName)) as SpellEffect;
        }

        if (ca.SecondSpecialName != null && ca.SecondSpecialName != "")
        {
            //Debug.Log("SpecialScript: " + ca.SpecialScriptName);
            secondeffect = System.Activator.CreateInstance(System.Type.GetType(ca.SecondSpecialName)) as SpellEffect;
        }

        if (ca.ConditionCheckName != null && ca.ConditionCheckName != "")
        {
            checkcond = System.Activator.CreateInstance(System.Type.GetType(ca.ConditionCheckName)) as ConditionEffect;
        }

        if (ca.RemoveEffectName != null && ca.RemoveEffectName != "")
        {
            removeeffect = System.Activator.CreateInstance(System.Type.GetType(ca.RemoveEffectName)) as RemoveFromPlayEffect;
        }
        if (ca.DefenseEffectName != null && ca.DefenseEffectName != "")
        {
            ca.defenseeffect = System.Activator.CreateInstance(System.Type.GetType(ca.DefenseEffectName)) as DefenseEffect;
        }

        // add this card to a dictionary with its ID as a key
        CardsCreatedThisGame.Add(UniqueCardID, this);
    }

    // method to set or reset mana cost
    public void ResetManaCost()
    {
       //NOTE: No mana costs in Highlander CurrentManaCost = ca.ManaCost;
    }

}
