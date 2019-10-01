using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum TargetingOptions
{
    NoTarget,
    AllCreatures, 
    EnemyCreatures,
    YourCreatures, 
    AllCharacters, 
    EnemyCharacters,
    YourCharacters
}

public enum AttackTypes { Regular, Additional, Modified, Special, Headshot, Dodge, Ranged }
public enum SpecialType { Event, Object, Plot, Situation, Location, Edge}
public enum RarityOptions{  Basic, Common, Uncommon, Rare, Epic }
public enum TypesOfCards{ Creature, Spell }

public class CardAsset : ScriptableObject 
{
    // this object will hold the info about the most general card
    [Header("General info")]
    public CharacterAsset characterAsset;  // if this is null, it`s a neutral card
    [TextArea(2,3)]
    public string Description;  // Description for spell or character
    [TextArea(2, 3)]
    public string Tags;  // tags that can be searched as keywords
    public RarityOptions Rarity;
    public Sprite CardImage;

    public int ManaCost;
    public bool TokenCard = false; // token cards can not be seen in collection
    public int OverrideLimitOfThisCardInDeck = -1;
    public TypesOfCards TypeOfCard;

    public Sprite GridImage;
    public string GridData;

    [Header("General Card Info")]
    public int MaxHealth;   // =0 => Special card
    public int Attack;
    public int AttacksForOneTurn = 1;
    //public bool Charge;

    public DefenseEffect defenseeffect;
    public TargetingOptions Targets;

    [Header("Scripts And Data")]
    public string CreatureScriptName;
    public int specialCreatureAmount;
    public string SpecialScriptName;
    public string SecondSpecialName;
    public string ConditionCheckName;
    public string RemoveEffectName;
    public string DefenseEffectName;
    public string PlotName;
    public string ObjectName;
    public string SpecialData;
    public int specialSpellAmount;
    public int secondSpecialAmount;

    [Header("Attack Defense Info")]
    public string AttackDefense;
    public int ModifiedDamageAmt;
    public int AdditionalDamageAmt;
    public AttackTypes TypeOfAttack;
    public bool CanBePowerBlow;
    public bool SpecialAction;
    public SpecialType TypeOfSpecial;
    public bool SweepAttack;
    public bool SweepDefense;
    public bool SweepSpecial;
    public bool HasConditionCheck;


}
