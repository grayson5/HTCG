using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OneCreatureManager : MonoBehaviour 
{
    public CardAsset cardAsset;
    public OneCardManager PreviewManager;
    [Header("Text Component References")]
    public Text HealthText;
    public Text AttackText;
    [Header("Image References")]
    public Image CreatureGraphicImage;
    public Image CreatureGlowImage;
    public Image GlowImageBlock;
    [Header("Highlander References")]
    public Image CardGridImage;
    public Image CardGraphicImage;
    public Image CardBodyImage;
    public Image CardFaceFrameImage;
    public Image PowerIcon;
    public Image CardBackGlowImage;

    void Awake()
    {
        if (cardAsset != null)
        {
            Debug.Log("Call ReadfromCreatureAsset");
            ReadCreatureFromAsset();
        }
            
    }

    private bool canAttackNow = false;
    public bool CanAttackNow
    {
        get
        {
            return canAttackNow;
        }

        set
        {
            canAttackNow = value;

            CreatureGlowImage.enabled = value;
        }
    }

    public void ReadCreatureFromAsset()
    {
        // Change the card graphic sprite
        //CreatureGraphicImage.sprite = cardAsset.CardImage;
        CardGraphicImage.sprite = cardAsset.CardImage;
        CardGridImage.sprite = cardAsset.GridImage;
        PowerIcon.enabled = false;
        //card

        if (cardAsset.AttackDefense == "A")
        {
            //AttackText.text = cardAsset.DamageAmt.ToString();
        }
        else
            AttackText.text = "";
        
        //HealthText.text = cardAsset.MaxHealth.ToString();
        //DamageAmtText.text = cardAsset.DamageAmt.ToString();

        if (PreviewManager != null)
        {
            PreviewManager.cardAsset = cardAsset;
            Debug.Log("Call ReadCardfromAsset");
            PreviewManager.ReadCardFromAsset();
            Debug.Log("Back from ReadCardFromAsset");
        }
    }	

    public void TakeDamage(int amount, int healthAfter)
    {
        if (amount > 0)
        {
            // TODO DamageEffect.CreateDamageEffect(transform.position, amount);
            HealthText.text = healthAfter.ToString();
        }
    }

    public void IncreaseDamage(int amount)
    {
        if (amount > 0)
        {
            // TODO DamageEffect.CreateDamageEffect(transform.position, amount);
            AttackText.text = amount.ToString();
            
        }
    }
}
