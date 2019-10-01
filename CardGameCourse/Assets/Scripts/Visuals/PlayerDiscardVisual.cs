using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

// this class should be attached to the deck
// generates new cards and places them into the hand
public class PlayerDiscardVisual : MonoBehaviour {

    // PUBLIC FIELDS
    public AreaPosition owner;
    public float HeightOfOneCard = 0.012f;
    public bool TakeCardsOpenly = true;
    public SameDistanceChildren slots;

    [Header("Transform References")]
    public Transform DrawPreviewSpot;
    public Transform DeckTransform;
    public Transform OtherCardDrawSourceTransform;
    public Transform PlayPreviewSpot;
    public Transform DiscardSpot;

    // PRIVATE : a list of all card visual representations as GameObjects
    private List<GameObject> CardsInDiscard = new List<GameObject>();

    //void Start()
    //{
    //    NumCardsInDiscard = GlobalSettings.Instance.Players[owner].discardcards.CardsInDiscard.Count;
    //}

    //private int numcardsInDiscard = 0;
    //public int NumCardsInDiscard
    //{
    //    get{ return numcardsInDiscard; }

    //    set
    //    {
    //        numcardsInDiscard = value;
    //        transform.position = new Vector3(transform.position.x, transform.position.y, - HeightOfOneCard * value);
    //    }
    //}

    private bool cursorOverThisDiscard = false;

    // A 3D collider attached to this game object
    private BoxCollider col;

    // PROPERTIES

    // returns true if we are hovering over any player`s table collider
    public static bool CursorOverSomeDiscard
    {
        get
        {
            PlayerDiscardVisual[] bothDiscards = GameObject.FindObjectsOfType<PlayerDiscardVisual>();
            return (bothDiscards[0].CursorOverThisDiscard || bothDiscards[1].CursorOverThisDiscard);
        }
    }

    // returns true only if we are hovering over this table`s collider
    public bool CursorOverThisDiscard
    {
        get { return cursorOverThisDiscard; }
    }

    // METHODS

    // MONOBEHAVIOUR METHODS (mouse over collider detection)
    void Awake()
    {
        col = GetComponent<BoxCollider>();
    }

    // CURSOR/MOUSE DETECTION
    void Update()
    {
        // we need to Raycast because OnMouseEnter, etc reacts to colliders on cards and cards "cover" the table
        // create an array of RaycastHits
        RaycastHit[] hits;
        // raycst to mousePosition and store all the hits in the array
        hits = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition), 30f);

        bool passedThroughDiscardCollider = false;
        foreach (RaycastHit h in hits)
        {
            // check if the collider that we hit is the collider on this GameObject
            if (h.collider == col)
                passedThroughDiscardCollider = true;
        }
        cursorOverThisDiscard = passedThroughDiscardCollider;
    }

    // creates a card and returns a new card as a GameObject
    GameObject CreateACardAtPosition(CardAsset c, Vector3 position, Vector3 eulerAngles)
    {
        // Instantiate a card depending on its type
        GameObject card;
        if (c.MaxHealth > 0)
        {
            // this card is a creature card
            card = GameObject.Instantiate(GlobalSettings.Instance.CreatureCardPrefab, position, Quaternion.Euler(eulerAngles)) as GameObject;
        }
        else
        {
            // this is a spell: checking for targeted or non-targeted spell
            if (c.Targets == TargetingOptions.NoTarget)
                card = GameObject.Instantiate(GlobalSettings.Instance.NoTargetSpellCardPrefab, position, Quaternion.Euler(eulerAngles)) as GameObject;
            else
            {
                card = GameObject.Instantiate(GlobalSettings.Instance.TargetedSpellCardPrefab, position, Quaternion.Euler(eulerAngles)) as GameObject;
                // pass targeting options to DraggingActions
                DragSpellOnTarget dragSpell = card.GetComponentInChildren<DragSpellOnTarget>();
                dragSpell.Targets = c.Targets;
            }

        }

        // apply the look of the card based on the info from CardAsset
        OneCardManager manager = card.GetComponent<OneCardManager>();
        manager.cardAsset = c;
        manager.ReadCardFromAsset();
        //Debug.Log("In Create Card Name: " + card.name);
        return card;
    }
    public void AddDisCard(GameObject card)
    {
        // we allways insert a new card as 0th element in CardsInHand List 
        Debug.Log("In AddDisCard");
        //Debug.Log("Card: " + card.name);
        CardsInDiscard.Insert(0, card);
        //Debug.Log("Count: " + CardsInDiscard.Count);
        

        // parent this card to our Slots GameObject
        //card.transform.SetParent(slots.transform);

        // re-calculate the position of the hand
        //PlaceCardsOnNewSlots();
        //UpdatePlacementOfSlots();
    }


    public void GiveDiscardACard(CardAsset c, int UniqueID, bool fast = false, bool fromDeck = true)
    {
        Debug.Log("In GiveDiscardACard");
        
        //Command.CommandExecutionComplete();
        // return;
        GameObject discardcard;
        if (fromDeck)
            discardcard = CreateACardAtPosition(c, DeckTransform.position, new Vector3(0f, -179f, 0f));
        else
            discardcard = CreateACardAtPosition(c, OtherCardDrawSourceTransform.position, new Vector3(0f, -179f, 0f));

        // Set a tag to reflect where this card is
        foreach (Transform t in discardcard.GetComponentsInChildren<Transform>())
            t.tag = owner.ToString() + "Card";
        // pass this card to HandVisual class
        //TODO: Need to modify below line of code to Add to Discard once that is created.
        AddDisCard(discardcard);
        //NEW LINE BELOW 7-11
        discardcard.SetActive(true);
        discardcard.GetComponent<OneCardManager>().CardFaceGlowImage.enabled = false;
        // Bring card to front while it travels from draw spot to hand
        WhereIsTheCardOrCreature w = discardcard.GetComponent<WhereIsTheCardOrCreature>();
        w.BringToFront();
        w.Slot = 0;
        w.VisualState = VisualStates.Transition;

        // pass a unique ID to this card.
        IDHolder id = discardcard.AddComponent<IDHolder>();
        id.UniqueID = UniqueID;

        // move card to the hand;
        Sequence s = DOTween.Sequence();
        if (!fast)
        {
            Debug.Log("Not fast!!!");
            s.Append(discardcard.transform.DOMove(DrawPreviewSpot.position, GlobalSettings.Instance.CardTransitionTime));
            s.AppendInterval(GlobalSettings.Instance.CardPreviewTime);
            s.Append(discardcard.transform.DOMove(DiscardSpot.position, GlobalSettings.Instance.CardTransitionTime));
            if (TakeCardsOpenly)
                s.Insert(0f, discardcard.transform.DORotate(Vector3.zero, GlobalSettings.Instance.CardTransitionTime));
            //else 
            //s.Insert(0f, card.transform.DORotate(new Vector3(0f, -179f, 0f), GlobalSettings.Instance.CardTransitionTime)); 
            //s.AppendInterval(GlobalSettings.Instance.CardPreviewTime);
            // displace the card so that we can select it in the scene easier.
            //s.Append(card.transform.DOLocalMove(slots.Children[0].transform.localPosition, GlobalSettings.Instance.CardTransitionTime));
        }
        else
        {
            // displace the card so that we can select it in the scene easier.
            s.Append(discardcard.transform.DOMove(DiscardSpot.position, GlobalSettings.Instance.CardTransitionTime));
            if (TakeCardsOpenly)
                s.Insert(0f, discardcard.transform.DORotate(Vector3.zero, GlobalSettings.Instance.CardTransitionTimeFast));
        }
        // Debug.Log("Before Change Status");
        //Commented out below line and added 
        //s.OnComplete(() => ChangeLastCardStatusToInHand(card, w));
        s.OnComplete(() =>
        {
            //Debug.Log("Call CommandExecComplete");       
            Command.CommandExecutionComplete();
            Destroy(discardcard); //????
           
        });
        // Debug.Log("After Change Status");
    }

}
