using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum AreaPosition{Top, Low}

public class PlayerArea : MonoBehaviour 
{
    public AreaPosition owner;
    public bool ControlsON = true;
    public PlayerDeckVisual PDeck;
    //public ManaPoolVisual ManaBar;
    public HandVisual handVisual;
    public PlayerPortraitVisual Portrait;
    public HeroPowerButton HeroPower;
    //public EndTurnButton EndTurnButton;
    public TableVisual tableVisual;
    public Transform PortraitPosition;
    public Transform InitialPortraitPosition;
    public PlayerDiscardVisual discardVisual;
    public Transform HiddenIcon1;
    public Button HiddenIconButton1;
    public Button ShieldIcon;
    public Button PAbilityIcon;
    public Button DisarmIcon;
    public GameObject DiscardPanel;

    public bool AllowedToControlThisPlayer
    {
        get;
        set;
    }      


}
