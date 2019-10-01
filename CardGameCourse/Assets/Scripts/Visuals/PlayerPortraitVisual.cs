using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using DG.Tweening;

public class PlayerPortraitVisual : MonoBehaviour {

    public CharacterAsset charAsset;
    [Header("Text Component References")]
    //public Text NameText;
    public Text HealthText;
    [Header("Image References")]
    public Image HeroPowerIconImage;
    public Image HeroPowerBackgroundImage;
    public Image PortraitImage;
    public Image PortraitBackgroundImage;
    public Image PersonaCardBack;

    void Awake()
	{
		if(charAsset != null)
			ApplyLookFromAsset();
	}
	
	public void ApplyLookFromAsset()
    {
        HealthText.text = charAsset.MaxHealth.ToString();
        //HeroPowerIconImage.sprite = charAsset.HeroPowerIconImage;
        //HeroPowerBackgroundImage.sprite = charAsset.HeroPowerBGImage;
        PortraitImage.sprite = charAsset.AvatarImage;
        //PersonaCardBack.sprite = charAsset.PersonaBackImage;
        //PortraitBackgroundImage.sprite = charAsset.AvatarBGImage;

        //HeroPowerBackgroundImage.color = charAsset.HeroPowerBGTint;
        //PortraitBackgroundImage.color = charAsset.AvatarBGTint;

    }

    public void ApplyLookFromAssetDeckBuiding()
    {
        Debug.Log("ApplyCharAssetDeckBuilding");
        Debug.Log("Persona: " + charAsset.name);
        Debug.Log("Avatar: " + charAsset.AvatarImage.name);
        HealthText.text = charAsset.MaxHealth.ToString();
        //HeroPowerIconImage.sprite = charAsset.HeroPowerIconImage;
        //HeroPowerBackgroundImage.sprite = charAsset.HeroPowerBGImage;
        PortraitImage.sprite = charAsset.AvatarImage;
        //PortraitBackgroundImage.sprite = charAsset.AvatarBGImage;

        //HeroPowerBackgroundImage.color = charAsset.HeroPowerBGTint;
        //PortraitBackgroundImage.color = charAsset.AvatarBGTint;

    }

    public void CheckAndUpdateAbility(int currentHealth)
    {
        int pphealth = Int32.Parse(HealthText.text);

        if (pphealth != currentHealth)
        {
            if (pphealth < currentHealth)
            {
                Heal();
                HealthText.text = currentHealth.ToString();
            }
            else if (pphealth > currentHealth)
            {
                DamageEffect.CreateDamageEffect(transform.position, (pphealth - currentHealth));
                HealthText.text = currentHealth.ToString();
            }
        }
    }

    public void AddAbility(int amount, int healthAfter)
    {
        if (amount > 0)
        {
            Heal();
            HealthText.text = healthAfter.ToString();

        }
    }

    public void TakeDamage(int amount, int healthAfter)
    {
        if (amount > 0)
        {
            DamageEffect.CreateDamageEffect(transform.position, amount);
            HealthText.text = healthAfter.ToString();
        }
    }

    public void Explode()
    {
        Instantiate(GlobalSettings.Instance.ExplosionPrefab, transform.position, Quaternion.identity);
        Sequence s = DOTween.Sequence();
        s.PrependInterval(2f);
        s.OnComplete(() => GlobalSettings.Instance.GameOverPanel.SetActive(true));
    }

    public void Heal()
    {
        Instantiate(GlobalSettings.Instance.Heal_AnimPrefab, transform.position, Quaternion.identity);
        Sequence s = DOTween.Sequence();
        s.PrependInterval(2f);
        s.OnComplete(() => { Debug.Log("Healed"); });
    }

}
