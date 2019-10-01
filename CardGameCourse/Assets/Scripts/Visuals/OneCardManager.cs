using UnityEngine;
using System.Collections;
using UnityEngine.UI;

// holds the refs to all the Text, Images on the card
public class OneCardManager : MonoBehaviour {

    public CardAsset cardAsset;
    public OneCardManager PreviewManager;
    [Header("Text Component References")]
    public Text NameText;
    public Text PersonaText;
    public Text ManaCostText;
    public Text DescriptionText;
    public Text HealthText;
    public Text AttackText;
    [Header("Image References")]
    public Image CardGridImage;
    public Image CardTitleImage;
    public Image CardDescriptionImage;
    public Image CardGraphicImage;
    public Image CardBodyImage;
    public Image CardFaceFrameImage;
    public Image CardFaceGlowImage;
    public Image CardBackGlowImage;


    void Awake()
    {
        if (cardAsset != null)
            ReadCardFromAsset();
    }

    private TargetingOptions _targets = TargetingOptions.AllCharacters;
    public TargetingOptions Targets
    {
        get
        {
            return _targets;
        }

        set
        {
            _targets = value;
        }

    }

    private bool canBePlayedNow = false;
    public bool CanBePlayedNow
    {
        get
        {
            return canBePlayedNow;
        }

        set
        {
            canBePlayedNow = value;

            CardFaceGlowImage.enabled = value;
        }
    }

    public void ReadCardFromAsset()
    {
        // universal actions for any Card
        // 1) apply tint
        //Debug.Log("In ReadCardFromAsset");
        if (cardAsset.characterAsset != null)
        {
            //Debug.Log("Set Persona Specific Color");
            //CardBodyImage.color = cardAsset.characterAsset.ClassCardTint;
            CardFaceFrameImage.color = cardAsset.characterAsset.ClassCardTint;
            //CardFaceFrameImage.color = Color.blue;
            CardTitleImage.sprite = cardAsset.characterAsset.TitleBar;
            CardDescriptionImage.sprite = cardAsset.characterAsset.BodyImage;
            CardBodyImage.color = cardAsset.characterAsset.ClassCardTint;
            PersonaText.text = cardAsset.characterAsset.name;
            PersonaText.color = cardAsset.characterAsset.AvatarBGTint;
            NameText.color = cardAsset.characterAsset.AvatarBGTint;
            DescriptionText.color = cardAsset.characterAsset.AvatarBGTint;
            //CardTopRibbonImage.color = cardAsset.characterAsset.ClassRibbonsTint;
            //CardLowRibbonImage.color = cardAsset.characterAsset.ClassRibbonsTint;
        }
        else
        {
            //CardBodyImage.color = GlobalSettings.Instance.CardBodyStandardColor;
            CardFaceFrameImage.color = Color.white;
            //CardTopRibbonImage.color = GlobalSettings.Instance.CardRibbonsStandardColor;
            //CardLowRibbonImage.color = GlobalSettings.Instance.CardRibbonsStandardColor;
        }
        // 2) add card name
        if (cardAsset.name != null)
        {
            //Debug.Log(cardAsset.name);
            NameText.text = cardAsset.name;
        }

        // 3) add mana cost
        //ManaCostText.text = cardAsset.ManaCost.ToString();
        // 4) add description
        DescriptionText.text = cardAsset.Description;
        // 5) Change the card graphic sprite
        CardGraphicImage.sprite = cardAsset.CardImage;
        // 6) Change Grid icon
        CardGridImage.sprite = cardAsset.GridImage;


        if (cardAsset.MaxHealth != 0)
        {
            // this is a creature
            //AttackText.text = cardAsset.Attack.ToString();
            //HealthText.text = cardAsset.MaxHealth.ToString();
        }

        if (PreviewManager != null)
        {
            // this is a card and not a preview
            // Preview GameObject will have OneCardManager as well, but PreviewManager should be null there
            PreviewManager.cardAsset = cardAsset;
            PreviewManager.ReadCardFromAsset();
        }

        Targets = cardAsset.Targets;

    }
    public void TakeDamage(int amount, int healthAfter)
    {
        if (amount > 0)
        {
            // TODO DamageEffect.CreateDamageEffect(transform.position, amount);
            HealthText.text = healthAfter.ToString();
        }
    }
}
