using UnityEngine;
using System.Collections;

public enum CharClass{ Elf, Monk, Warrior}

public class CharacterAsset : ScriptableObject 
{
	public CharClass Class;
	public string ClassName;
	public int MaxHealth = 30;
    public int MaxAttacksPerTurn;
    public bool AttackAreasBlocked;
    public bool PersonaAbilityButton;
    public bool GuardInPlay;
    public int NormalAttack;
    public int PowerAttack;
	public string HeroPowerName;
    public string PersonaAblConditionChk;
    public string PersonaAbility;
	public Sprite AvatarImage;
    public Sprite PersonaBackImage;
    public Sprite HeroPowerIconImage;
    public Sprite AvatarBGImage;
    public Sprite HeroPowerBGImage;
    public Color32 AvatarBGTint;
    public Color32 HeroPowerBGTint;
    public Color32 ClassCardTint;
    public Color32 ClassRibbonsTint;
    [Header("Persona Colors")]
    public Sprite TitleBar;
    public Sprite BodyImage;
}
