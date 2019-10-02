using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using DG.Tweening;

public class Player : MonoBehaviour, ICharacter
{
    // PUBLIC FIELDS
    // int ID that we get from ID factory
    public int PlayerID;
    // a Character Asset that contains data about this Hero
    public CharacterAsset charAsset;
    // a script with references to all the visual game objects for this player
    public PlayerArea PArea;
    // a script of type Spell effect that will be used for our hero power
    // (essenitially, using hero power is like playing a spell in a way)
    public SpellEffect HeroPowerEffect;
    // a flag not to use hero power twice
    public bool usedHeroPowerThisTurn = false;
    public bool AttackAreasBlocked = false;
    public int TempAddAbility;
    public int NormalAttack;
    public int PowerAttack;
    public int attacksplayed;
    public int defensesplayed;
    public string currentphase;
    public int damageaccrued;
    public int abilitygained;
    public int EventDamage;
    public bool NextAttackHidden = false;
    public bool playedSpecialCardThisTurn = false;
    public bool GuardInPlay = false;
    public bool MustOrMayAction;
    public bool PlayOnlyRangedAttacks = false;
    public bool CanAttackThisTurn = true;
    public bool CanPlaySpecials;
    public bool CanPlayDodges = true;
    public bool Disarmed = false;
    public bool Dicerolled = false;
    //public bool TurnCounterOn = false;
    public int DodgeTurnCounter = 0;
    public int CanPlaySpecialsCounter = 0;
    public int BothPlayersCantAttackCounter = 0;
    public string MMDiscardCards;
    public int MMDiscardAttacks;
    public int MMDiscardAnyCards;
    public int MMExertAnyCards;
    public int MMDrawCards;
    public string rollreason;
    public int rollamount;
    public PersonaConditionEffect personacondcheck;

    // REFERENCES TO LOGICAL STUFF THAT BELONGS TO THIS PLAYER
    public Deck deck;
    public Hand hand;
    public Table table;
    public Discard discardcards;
    public Transform DiscardSpot;

    // a static array that will store both players, should always have 2 players
    public static Player[] Players;

    // list of all the creature cards on the table as GameObjects
    private List<GameObject> CreaturesOnTable = new List<GameObject>();
    private Canvas canvas;
    private CardLogic SavedPlayedCard;

    // PROPERTIES 
    // this property is a part of interface ICharacter
    public int ID
    {
        get { return PlayerID; }
    }

    // opponent player
    public Player otherPlayer
    {
        get
        {
            if (Players[0] == this)
                return Players[1];
            else
                return Players[0];
        }
    }

    private int attackslefthisturn;
    public int AttacksLeftThisTurn
    {
        get { return attackslefthisturn; }
        set { attackslefthisturn = value; }
    }


    private string maymustmessage;
    public string MayMustMessage
    {
        get { return maymustmessage;  }
        set { maymustmessage = value; }
    }

    private bool processeventcard;
    public bool ProcessEventCard
    {
        get { return processeventcard; }
        set { processeventcard = value; }
    }

    private int health;
    public int Health
    {
        get { return health + TempAddAbility; }
        set
        {
            Debug.Log("In HEALTH: value + TempAddability" + (value + TempAddAbility));

            if (value > (charAsset.MaxHealth + TempAddAbility))
            {
                Debug.Log("Trying to exceed MaxHealth + TempAddAbility");
                health = charAsset.MaxHealth;
            }
            else
            {
                Debug.Log("Adjusting Health");
                health = value - TempAddAbility;
            }

            Debug.Log("Health Result: " + Health);

            if (Health <= 0)
                Die();
        }
    }

    private int hardexertionsleft;
    public int HardExertionsLeft
    {
        get { return hardexertionsleft; }
        set { hardexertionsleft = value; }
    }


    // CODE FOR EVENTS TO LET CREATURES KNOW WHEN TO CAUSE EFFECTS
    public delegate void VoidWithNoArguments();
    //public event VoidWithNoArguments CreaturePlayedEvent;
    //public event VoidWithNoArguments SpellPlayedEvent;
    //public event VoidWithNoArguments StartTurnEvent;
    public event VoidWithNoArguments EndTurnEvent;

    public enum blocktype
    {
        noblock,
        halfblock,
        fullblock
    };

    // ALL METHODS
    void Awake()
    {
        // find all scripts of type Player and store them in Players array
        // (we should have only 2 players in the scene)
        Players = GameObject.FindObjectsOfType<Player>();
        // obtain unique id from IDFactory
        PlayerID = IDFactory.GetUniqueID();
    }

    public virtual void OnTurnStart()
    {
        // add one mana crystal to the pool;
        Debug.Log("In ONTURNSTART for: " + this.name);
        Debug.Log("MustMayAction: " + MustOrMayAction);
        Debug.Log("CanAttack: " + CanAttackThisTurn);

        canvas = PArea.PDeck.GetComponentInChildren<Canvas>();
        Debug.Log("-----> Deck: " + canvas.sortingLayerName);
        usedHeroPowerThisTurn = false;
        playedSpecialCardThisTurn = false;
        ProcessEventCard = false;
        PlayOnlyRangedAttacks = false;
        Dicerolled = false;
        HardExertionsLeft = 1;
        AttacksLeftThisTurn = charAsset.MaxAttacksPerTurn;
        AttackAreasBlocked = charAsset.AttackAreasBlocked;
        //damageaccrued = 0;
        foreach (CreatureLogic cl in table.CreaturesOnTable)
            cl.OnTurnStart();

        if (charAsset.PersonaAbilityButton == true)
        {
            PArea.PAbilityIcon.interactable = true;
            PArea.PAbilityIcon.image.enabled = true;
        }
        otherPlayer.PArea.PAbilityIcon.interactable = false;

        //If a Guard is in play at start of turn, if PB need to undo it
        if (GuardInPlay == true)
            CheckGuardForPowerBlock();

        if (BothPlayersCantAttackCounter > 0 || otherPlayer.BothPlayersCantAttackCounter > 0)
        {
            Debug.Log("B: " + BothPlayersCantAttackCounter + " oB: " + otherPlayer.BothPlayersCantAttackCounter);
            CanAttackThisTurn = false;
        }
        else
            Debug.Log("Both Counters at 0");

        Debug.Log("CanAttack2: " + CanAttackThisTurn);
        Debug.Log("DONE with OnTurnStart");
    }

    public void OnTurnEnd()
    {
        Debug.Log("In OnTurnEnd");
        Debug.Log("health: " + Health);
        Debug.Log("Cards in Hand: " + hand.CardsInHand.Count);

        //Now draw up to current ability
        while (hand.CardsInHand.Count < Health)
        {
            DrawACard();
        }

        if (EndTurnEvent != null)
            EndTurnEvent.Invoke();

        if (CanPlaySpecialsCounter > 0)
            CanPlaySpecialsCounter--;

        if (CanPlaySpecialsCounter == 0)
            CanPlaySpecials = true;

        GetComponent<TurnMaker>().StopAllCoroutines();

        //Reset attributes that opponent can effect through cards
        CanAttackThisTurn = true;

        if (BothPlayersCantAttackCounter > 0)
        {
            BothPlayersCantAttackCounter--;
            if (BothPlayersCantAttackCounter > 0)
            {
                otherPlayer.CanAttackThisTurn = false;
                CanAttackThisTurn = false;
            }
            else
            {
                otherPlayer.CanAttackThisTurn = true;
                CanAttackThisTurn = true;
                RemoveCardFromTable(otherPlayer, "Pedestrian");
            }
        }
        else
        {
            otherPlayer.CanAttackThisTurn = true;
            CanAttackThisTurn = true;
        }

        if (DodgeTurnCounter > 0)
        {
            Debug.Log("DodgeTurnCounter: " + DodgeTurnCounter);
            DodgeTurnCounter--;
            if (DodgeTurnCounter == 0)
            {
                CanPlayDodges = true;
                otherPlayer.CanPlayDodges = true;
            }
        }

    }

    // STUFF THAT OUR PLAYER CAN DO

    // draw a single card from the deck
    public void DrawACard(bool fast = false)
    {
        if (deck.cards.Count > 1)
        {
            if (hand.CardsInHand.Count < PArea.handVisual.slots.Children.Length)
            {
                // 1) logic: add card to hand
                CardLogic newCard = new CardLogic(deck.cards[0]);
                newCard.owner = this;
                hand.CardsInHand.Insert(0, newCard);
                //Debug.Log("In DrawACard");
                // Debug.Log(hand.CardsInHand.Count);
                // 2) logic: remove the card from the deck
                deck.cards.RemoveAt(0);
                // 2) create a command
                new DrawACardCommand(hand.CardsInHand[0], this, fast, fromDeck: true).AddToQueue();
                Debug.Log("After DrawACardCommand");
                Debug.Log("Count: " + deck.cards.Count);
                PArea.PDeck.NumOfCardsInDeck.text = deck.cards.Count.ToString();
            }
            else
            {
                Debug.Log("WARNING!!!: No more cards slots to draw card");
            }
        }
        else
        {
            // there are no cards in the deck, take exhaust damage.
            ExhaustPlayer();
            DrawACard(true);
        }

    }

    // get card NOT from deck (a token or a coin)
    public void GetACardNotFromDeck(CardAsset cardAsset)
    {
        if (hand.CardsInHand.Count < PArea.handVisual.slots.Children.Length)
        {
            // 1) logic: add card to hand
            CardLogic newCard = new CardLogic(cardAsset);
            newCard.owner = this;
            hand.CardsInHand.Insert(0, newCard);
            // 2) send message to the visual Deck
            new DrawACardCommand(hand.CardsInHand[0], this, fast: true, fromDeck: false).AddToQueue();
        }
        // no removal from deck because the card was not in the deck
    }

    // 2 METHODS FOR PLAYING SPECIALS
    // 1st overload - takes ids as arguments
    // it is cnvenient to call this method from visual part
    public void PlayASpecialFromHand(int SpecialCardUniqueID, int TargetUniqueID, int tablePos)
    {
        Debug.Log("PSFH SpecialCard ID: " + SpecialCardUniqueID);
        Debug.Log("PSFH Target ID: " + TargetUniqueID);

        if (TargetUniqueID < 0)
            PlayASpecialFromHand(CardLogic.CardsCreatedThisGame[SpecialCardUniqueID], null, tablePos);
        else if (TargetUniqueID == ID)
        {
            PlayASpecialFromHand(CardLogic.CardsCreatedThisGame[SpecialCardUniqueID], this, tablePos);
        }
        else if (TargetUniqueID == otherPlayer.ID)
        {
            PlayASpecialFromHand(CardLogic.CardsCreatedThisGame[SpecialCardUniqueID], this.otherPlayer, tablePos);
        }
        else
        {
            // target is a creature
            PlayASpecialFromHand(CardLogic.CardsCreatedThisGame[SpecialCardUniqueID], CreatureLogic.CreaturesCreatedThisGame[TargetUniqueID], tablePos);
        }

    }

    // 2nd overload - takes CardLogic and ICharacter interface - 
    // this method is called from Logic, for example by AI
    public void PlayASpecialFromHand(CardLogic playedCard, ICharacter target, int tablePos)
    {
        //Debug.Log("In 2 PASFH target ID: " + target);

        if (currentphase == "Attack" || currentphase == "Defense")
        {
            //ManaLeft -= playedCard.CurrentManaCost;
            playedSpecialCardThisTurn = true;
            SavedPlayedCard = playedCard;
            // cause effect instantly:
            if (playedCard.effect != null)
                playedCard.effect.ActivateEffect(playedCard.ca.specialSpellAmount, target, this, this.otherPlayer, playedCard.ca.SpecialData);
            else
            {
                Debug.LogWarning("No effect found on card " + playedCard.ca.name);
            }
            // no matter what happens, move this card to PlayACardSpot

            Debug.Log("table ConT:" + table.CreaturesOnTable.Count);
            CreatureLogic newCreature = new CreatureLogic(this, playedCard.ca);
            //Debug.Log("New Creature ID: " + newCreature.UniqueCreatureID);
            //Debug.Log("playedCard ID: " + playedCard.UniqueCardID);

            //newCreature.UniqueCreatureID = playedCard.UniqueCardID;
            table.CreaturesOnTable.Insert(tablePos, newCreature);
            new PlayASpellCardCommand(this, playedCard, tablePos, newCreature.UniqueCreatureID).AddToQueue();
            //Debug.Log("After PlaySpell ConT: " + CreaturesOnTable.Count);
            // remove this card from hand
            hand.CardsInHand.Remove(playedCard);
            // check if this is a creature or a spell
        }
    }

    // METHODS TO PLAY CREATURES 
    // 1st overload - by ID
    public void PlayAttackDefenseFromHand(int UniqueID, int tablePos)
    {
        PlayAttackDefenseFromHand(CardLogic.CardsCreatedThisGame[UniqueID], tablePos);
    }

    // 2nd overload - by logic units
    public void PlayAttackDefenseFromHand(CardLogic playedCard, int tablePos)
    {
        Debug.Log("In PACFH Case: " + playedCard.ca.AttackDefense);
        switch (playedCard.ca.AttackDefense)
        {
            case "D":
                defensesplayed++;
                //playedCard.ca.PowerAorB = false;
                break;
            case "A":
                attacksplayed++;
                AttacksLeftThisTurn--;
                //playedCard.ca.HiddenAttack = false;
                break;
            case "G":
                //Debug.Log("Played Guard!");
                GuardInPlay = true;
                PArea.ShieldIcon.image.enabled = true;
                break;
        }

        // create a new creature object and add it to Table
        CreatureLogic newCreature = new CreatureLogic(this, playedCard.ca);

        if (NextAttackHidden == true && currentphase == "Attack")
        {
            newCreature.HiddenAttack = true;
        }

        table.CreaturesOnTable.Insert(tablePos, newCreature);
        // 
        //Debug.Log("Call PlayACreatureCommand");
        new PlayACreatureCommand(playedCard, this, tablePos, newCreature.UniqueCreatureID).AddToQueue();
        //Debug.Log("Back from PlayACreatureCommand");
        // cause battlecry Effect
        if (newCreature.effect != null)
            newCreature.effect.WhenACreatureIsPlayed();

        // remove this card from hand
        hand.CardsInHand.Remove(playedCard);
        HighlightPlayableCards();
    }

    public Player Owner()
    {
        return this;
    }

    public void Die()
    {
        // game over
        // block both players from taking new moves 
        PArea.ControlsON = false;
        otherPlayer.PArea.ControlsON = false;
        TurnManager.Instance.StopTheTimer();
        HoverPreview.PreviewsAllowed = false;
        new GameOverCommand(this).AddToQueue();
    }

    public void Heal()
    {
        new HealCommand(this).AddToQueue();
    }

    // use hero power - activate is effect like you`ve payed a spell
    public void UseHeroPower()
    {
        usedHeroPowerThisTurn = true;
        HeroPowerEffect.ActivateEffect();
    }


    // METHOD TO SHOW GLOW HIGHLIGHTS
    public void HighlightPlayableCards(bool removeAllHighlights = false)
    {
        // Debug.Log("HighlightPlayable remove: "+ removeAllHighlights);
        //Debug.Log("In HighlightPlayableCards CP: " + currentphase);

        string lastdefensegrid = null;
        string[] lastdefensevalues = null;
        bool blockedplayed = false;

        //Check to see if Attack Phase and if so, get last defense played
        if (currentphase == "Attack")
        {
            for (int i = table.CreaturesOnTable.Count; i > 0; i--)
            {
                //Debug.Log("Checking Defenses in Play: " + table.CreaturesOnTable[i].ca.name);
                Debug.Log("Checking Defenses in Play i - 1: " + table.CreaturesOnTable[i-1].ca.name);
                if (table.CreaturesOnTable[i - 1].ca.AttackDefense == "D" || table.CreaturesOnTable[i - 1].ca.AttackDefense == "G")
                {
                    lastdefensegrid = table.CreaturesOnTable[i - 1].ca.GridData;
                    lastdefensevalues = lastdefensegrid.Split(',');
                    blockedplayed = true;
                }
            }
            //Debug.Log("Def Grid: " + lastdefensegrid);
        }

        //NOTE:
        //DEFENSE PHASE
        //Can play ONLY 1 defense card for each attacked played by opponent

        foreach (CardLogic cl in hand.CardsInHand)
        {
            GameObject g = IDHolder.GetGameObjectWithID(cl.UniqueCardID);
            if (g != null)
            {
                bool canbeplayed = false;

                if (ProcessEventCard)
                {
                    //Debug.Log("In Highlight for ProcessEventCard");
                    switch (MMDiscardCards)
                    {
                        case "A":
                            if (cl.ca.AttackDefense == "A")
                            {
                                g.GetComponent<OneCardManager>().CanBePlayedNow = true;
                            }
                            else
                            {
                                g.GetComponent<OneCardManager>().CanBePlayedNow = false;
                            }
                            break;
                        case "C":
                            g.GetComponent<OneCardManager>().CanBePlayedNow = true;
                            break;
                        case "P":
                            if (cl.ca.PlotName.Length > 0)
                            {

                                g.GetComponent<OneCardManager>().CanBePlayedNow = true;
                            }
                            else
                            {
                                g.GetComponent<OneCardManager>().CanBePlayedNow = false;
                            }
                            break;
                        case "Z":
                            g.GetComponent<OneCardManager>().CanBePlayedNow = false;
                            break;
                    }
                }
                else
                {
                    switch (currentphase)
                    {
                        case "Sweep":
                            if (g != null)
                                g.GetComponent<OneCardManager>().CanBePlayedNow = false;
                            break;

                        case "Defense":
                            //Debug.Log("other player attacks: " + otherPlayer.attacksplayed);
                            //Debug.Log("defenses played: " + defensesplayed);
                            if(CanPlayDodges == false && cl.ca.TypeOfAttack == AttackTypes.Dodge)
                            {
                                g.GetComponent<OneCardManager>().CanBePlayedNow = false;
                            }
                            else if (Disarmed == true && (cl.ca.AttackDefense == "D" || cl.ca.AttackDefense == "G" ))
                            {
                                g.GetComponent<OneCardManager>().CanBePlayedNow = false;
                            }
                            else if (cl.ca.AttackDefense == "D" || cl.ca.AttackDefense == "G")
                            {
                                if ((otherPlayer.attacksplayed > 0) && (otherPlayer.attacksplayed > defensesplayed) && (GuardInPlay == false))
                                {
                                    for (int i = 0; i < otherPlayer.table.CreaturesOnTable.Count; i++)
                                    {
                                        if (otherPlayer.table.CreaturesOnTable[i].ca.AttackDefense == "A" &&
                                           otherPlayer.table.CreaturesOnTable[i].HiddenAttack == true)
                                        {
                                            g.GetComponent<OneCardManager>().CanBePlayedNow = true;
                                            break;
                                        }
                                        else
                                        {
                                            if (otherPlayer.table.CreaturesOnTable[i].ca.AttackDefense == "A")
                                            {
                                                string attackgrid = otherPlayer.table.CreaturesOnTable[i].ca.GridData;
                                                string[] attackvalues = attackgrid.Split(',');

                                                string defensegrid = cl.ca.GridData;
                                                string[] defensevalues = defensegrid.Split(',');

                                                //FOR LOOP WAS HERE
                                                g.GetComponent<OneCardManager>().CanBePlayedNow = BlockCanBePlayed(attackvalues, defensevalues);
                                            }
                                        }
                                    }
                                }
                                else if ((otherPlayer.attacksplayed == 0) && (GuardInPlay == false) && (cl.ca.AttackDefense == "G"))
                                {
                                    g.GetComponent<OneCardManager>().CanBePlayedNow = true;
                                }
                                else
                                {
                                    g.GetComponent<OneCardManager>().CanBePlayedNow = false;
                                }
                            }
                            else if (cl.ca.AttackDefense == "S")
                            {
                                if (playedSpecialCardThisTurn == false  && ( (CanPlaySpecials) && (otherPlayer.CanPlaySpecials)))
                                {
                                    if (cl.ca.HasConditionCheck == true)
                                    {
                                        if (cl.checkcond != null)
                                        {
                                            if (cl.ca.TypeOfSpecial == SpecialType.Event || cl.ca.TypeOfSpecial == SpecialType.Situation)
                                            {
                                                if (cl.checkcond.CheckCondition(0, null, this, otherPlayer, cl.ca.SpecialData, cl.ca.PlotName))
                                                    g.GetComponent<OneCardManager>().CanBePlayedNow = true;
                                                else
                                                    g.GetComponent<OneCardManager>().CanBePlayedNow = false;
                                            }
                                            else if (cl.ca.TypeOfSpecial == SpecialType.Object)
                                            {
                                                if (cl.checkcond.CheckCondition(0, null, this, otherPlayer, cl.ca.SpecialData, cl.ca.ObjectName))
                                                    g.GetComponent<OneCardManager>().CanBePlayedNow = true;
                                                else
                                                    g.GetComponent<OneCardManager>().CanBePlayedNow = false;
                                            }
                                        }
                                        else
                                            Debug.Log("WARNING: Card Has No Condition Check");
                                    }
                                    else
                                    {
                                        g.GetComponent<OneCardManager>().CanBePlayedNow = true;
                                        Debug.Log("Highlight Name: " + cl.ca.name);
                                    }
                                }
                                else
                                {
                                    g.GetComponent<OneCardManager>().CanBePlayedNow = false;
                                }
                            }
                            else
                            {
                                g.GetComponent<OneCardManager>().CanBePlayedNow = false;
                            }
                            break;
                        case "Attack":
                            //if (attacksplayed < AttacksLeftThisTurn)
                            //{
                            Debug.Log("Highlight Attack Phase: " + cl.ca.name);
                            //Debug.Log("attacksplayed: " + attacksplayed + " - AttacksLeft: " + AttacksLeftThisTurn);
                            //Debug.Log("Can Attack? " + CanAttackThisTurn);

                            if(CanAttackThisTurn == false && cl.ca.AttackDefense == "A")
                            {
                                Debug.Log("Can Attack this TURN is FALSE");
                                canbeplayed = false;
                                break;
                            }

                            if (Disarmed == true && cl.ca.AttackDefense == "A")
                            {
                                Debug.Log("Disarmed - Cannot play attacks");
                                canbeplayed = false;
                                break;
                            }

                            if (PlayOnlyRangedAttacks == true && cl.ca.AttackDefense == "A")
                            {
                                Debug.Log("CAN ONLY PLAY RANGED ATTACKS");
                                if (cl.ca.TypeOfAttack != AttackTypes.Ranged)
                                {
                                    Debug.Log("Not Ranged");
                                    canbeplayed = false;
                                    break;
                                }
                                else
                                {
                                    Debug.Log("Ranged!");
                                    canbeplayed = true;
                                    //break;
                                }
                            }
                                if (cl.ca.AttackDefense == "A" && attacksplayed < AttacksLeftThisTurn)
                                {
                                //Now check to see if attack is to area just blocked
                                    //Debug.Log("Checking Attack Card");
                                    if (cl.ca.AttackDefense == "A" && blockedplayed == true && AttackAreasBlocked == false &&
                                        PArea.HiddenIconButton1.image.enabled == false)
                                    {
                                        string attackgrid = cl.ca.GridData;
                                        string[] attackvalues = attackgrid.Split(',');

                                        for (int x = 0; x < 9; x++)
                                        {
                                            if (int.Parse(attackvalues[x]) == 1)
                                            {
                                                int defresult = int.Parse(lastdefensevalues[x]) + int.Parse(attackvalues[x]);
                                                if (defresult > 0)
                                                {
                                                    //Attack NOT to area just blocked
                                                    canbeplayed = true;
                                                    //break;
                                                }
                                                else
                                                {
                                                    if (int.Parse(attackvalues[x]) == 1 && defresult == 0)
                                                    {
                                                        //Attack too AREA just blocked CANNOT play
                                                        canbeplayed = false;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                        g.GetComponent<OneCardManager>().CanBePlayedNow = canbeplayed;
                                    }
                                    else
                                        g.GetComponent<OneCardManager>().CanBePlayedNow = true;
                                }
                                else if (cl.ca.AttackDefense == "S")
                                {
                                    //Debug.Log("Checking Special Card");
                                    //Debug.Log("Played Special: " + playedSpecialCardThisTurn);
                                    //Debug.Log("Can play specials: " + CanPlaySpecials);
                                    //Debug.Log("Other Can play specials: " + otherPlayer.CanPlaySpecials);
                                    if (playedSpecialCardThisTurn == false && ((CanPlaySpecials) && (otherPlayer.CanPlaySpecials)))
                                    {
                                        if (cl.ca.HasConditionCheck == true)
                                        {
                                            if (cl.checkcond != null)
                                            {
                                                //Debug.Log("Thinks there is a check cond");
                                                if (cl.ca.TypeOfSpecial == SpecialType.Event || cl.ca.TypeOfSpecial == SpecialType.Situation)
                                                {
                                                    if (cl.checkcond.CheckCondition(0, null, this, otherPlayer, cl.ca.SpecialData, cl.ca.PlotName))
                                                        g.GetComponent<OneCardManager>().CanBePlayedNow = true;
                                                    else
                                                        g.GetComponent<OneCardManager>().CanBePlayedNow = false;
                                                }
                                                else if (cl.ca.TypeOfSpecial == SpecialType.Object)
                                                {
                                                    if (cl.checkcond.CheckCondition(0, null, this, otherPlayer, cl.ca.SpecialData, cl.ca.ObjectName))
                                                        g.GetComponent<OneCardManager>().CanBePlayedNow = true;
                                                    else
                                                        g.GetComponent<OneCardManager>().CanBePlayedNow = false;
                                                }
                                            }
                                            else
                                                Debug.Log("WARNING: Card Has No Condition Check");
                                        }
                                        else
                                        {
                                            g.GetComponent<OneCardManager>().CanBePlayedNow = true;
                                            Debug.Log("Highlight Name: " + cl.ca.name);
                                        }
                                    }
                                    else
                                    {
                                        //Debug.Log("Else of 1st IF");
                                        g.GetComponent<OneCardManager>().CanBePlayedNow = false;
                                    }
                                }
                                else
                                {
                                    //Debug.Log("Card not Attack or Special");
                                    g.GetComponent<OneCardManager>().CanBePlayedNow = false;
                                }
                            break;
                        case "Discard":
                            g.GetComponent<OneCardManager>().CanBePlayedNow = true;
                            break;
                    }
                }
            }
        }

        if (removeAllHighlights == true)
        {
            foreach (CardLogic cl in hand.CardsInHand)
            {
                GameObject g = IDHolder.GetGameObjectWithID(cl.UniqueCardID);
                if (g != null)
                    g.GetComponent<OneCardManager>().CanBePlayedNow = false;
            }
        }

        Debug.Log("Done with HighlightPlayableCards");
        //foreach (CreatureLogic crl in table.CreaturesOnTable)
        //{
        //    GameObject g = IDHolder.GetGameObjectWithID(crl.UniqueCreatureID);
        //    if (g != null)
        //        g.GetComponent<OneCreatureManager>().CanAttackNow = (crl.AttacksLeftThisTurn > 0) && !removeAllHighlights;
        //}
        // highlight hero power
        //PArea.HeroPower.Highlighted = (!usedHeroPowerThisTurn) && (ManaLeft > 1) && !removeAllHighlights;
    }

    public bool BlockCanBePlayed(string[] attackvalues, string[] defensevalues)
    {
        bool canbeplayed = false;

        for (int x = 0; x < 9; x++)
        {
            if (int.Parse(attackvalues[x]) == 1)
            {
                int defresult = int.Parse(defensevalues[x]) + int.Parse(attackvalues[x]);
                if (defresult > 0)
                {
                    //Need to check previous block values
                    //example: 1 block failed, 2nd succeed on same attack;
                    canbeplayed = false;

                    break;

                }
                else
                {
                    if (int.Parse(attackvalues[x]) == 1 && defresult == 0)
                    {
                        canbeplayed = true;
                    }
                }
            }
        }

        if (canbeplayed == false)
            return false;
        else
            return true;
    }

    // START GAME METHODS
    public void LoadCharacterInfoFromAsset()
    {
        //Next lines are new
        TempAddAbility = 0;

        Health = charAsset.MaxHealth;
        // change the visuals for portrait, hero power, etc...
        PArea.Portrait.charAsset = charAsset;
        PArea.Portrait.ApplyLookFromAsset();
        AttackAreasBlocked = charAsset.AttackAreasBlocked;
        NormalAttack = charAsset.NormalAttack;
        PowerAttack = charAsset.PowerAttack;
        CanPlaySpecials = true;

        if (charAsset.HeroPowerName != null && charAsset.HeroPowerName != "")
        {
            HeroPowerEffect = System.Activator.CreateInstance(System.Type.GetType(charAsset.HeroPowerName)) as SpellEffect;
        }
        else
        {
            Debug.LogWarning("Check hero powr name for character " + charAsset.ClassName);
        }
    }

    public void TransmitInfoAboutPlayerToVisual()
    {
        PArea.Portrait.gameObject.AddComponent<IDHolder>().UniqueID = PlayerID;
        if (GetComponent<TurnMaker>() is AITurnMaker)
        {
            // turn off turn making for this character
            PArea.AllowedToControlThisPlayer = false;
        }
        else
        {
            // allow turn making for this character
            PArea.AllowedToControlThisPlayer = true;
        }
    }

    public void ExhaustPlayer()
    {
        //No cards left in deck so must take 5 exhaust damage
        //new DealDamageCommand(TurnManager.Instance.whoseTurn.PlayerID, 5, TurnManager.Instance.whoseTurn.Health - 5).AddToQueue();
        //TurnManager.Instance.whoseTurn.Health -= 5;
        damageaccrued += 5;

        //Now need to add discard cards back to deck
        while (discardcards.CardsInDiscard.Count > 0)
        {
            deck.cards.Insert(0, discardcards.CardsInDiscard[0].ca);
            discardcards.CardsInDiscard.RemoveAt(0);
        }
    }

    public void AbilityAdjustment()
    {
        //MODIFY - Do calculations for ability adjustments then call appropriate Command
        Debug.Log("In AbilityAdjustment");

        int startHealth = Health;
        int abilityadjustment = (abilitygained - damageaccrued);

        Debug.Log("Health: " + Health + " abilityadjustment: " + abilityadjustment);

        if (abilityadjustment > 0 )
        {
            if ((abilityadjustment + Health) > (charAsset.MaxHealth + TempAddAbility))
            {
                Health = charAsset.MaxHealth;
                abilityadjustment = ((charAsset.MaxHealth + TempAddAbility) - Health);
                Debug.Log("updated aa: " + abilityadjustment);
            }
            else
            {
                Health = Health + abilityadjustment;
            }

            new AddAbilityCommand(TurnManager.Instance.whoseTurn.PlayerID, abilityadjustment, Health, this).AddToQueue();
        }
        else if (abilityadjustment < 0)
        {
            Health = (Health + abilityadjustment);
            new DealDamageCommand(TurnManager.Instance.whoseTurn.PlayerID, damageaccrued, Health).AddToQueue();
            damageaccrued = 0;
            EventDamage = 0;
        }

        GameObject target = IDHolder.GetGameObjectWithID(PlayerID);
        target.GetComponent<PlayerPortraitVisual>().CheckAndUpdateAbility(Health);

        Debug.Log("New Health: " + Health);

        //if (damageaccrued > 0)
        //{
        //    Health = (Health - damageaccrued);
        //    new DealDamageCommand(TurnManager.Instance.whoseTurn.PlayerID, damageaccrued, Health).AddToQueue();
        //    damageaccrued = 0;
        //    EventDamage = 0;
        //}

        //if (abilitygained > 0)
        //{
        //    //Wait();
        //    Health = (Health + abilitygained);
        //    new AddAbilityCommand(TurnManager.Instance.whoseTurn.PlayerID, abilitygained, Health, this).AddToQueue();
        //    Debug.Log("After AddAbilityCommand - Health: " + Health);
        //    abilitygained = 0;
        //}
    }

    IEnumerator Wait()
    {
        Debug.Log("In Wait");
        yield return new WaitForSecondsRealtime(2f);
    }

    public void SweepPhase()
    {
        currentphase = "Sweep";
        //defensesplayed = 0;
        attacksplayed = 0;
        Debug.Log("In Sweep Phase");
        Debug.Log("I am Player: " + this.name);


        Debug.Log("Creatures on Table: " + table.CreaturesOnTable.Count);

        if (table.CreaturesOnTable.Count > 0)
        {
            while (MoreCardsToSweep())
            {
                for (int i = 0; i < table.CreaturesOnTable.Count; i++)
                {
                    Debug.Log("Card Name: " + table.CreaturesOnTable[i].ca.name);
                    if (((table.CreaturesOnTable[i].ca.AttackDefense == "A") &&
                        (table.CreaturesOnTable[i].ca.SweepAttack == true)) ||
                        ((table.CreaturesOnTable[i].ca.AttackDefense == "D") &&
                        (table.CreaturesOnTable[i].ca.SweepDefense == true)) ||
                        ((table.CreaturesOnTable[i].ca.AttackDefense == "S") &&
                        (table.CreaturesOnTable[i].ca.SweepSpecial == true)))
                    {
                        Debug.Log("This card should be REMOVED");
                        Debug.Log("Sweep IDCr: " + table.CreaturesOnTable[i].UniqueCreatureID);
                        Debug.Log("Sweep ID: " + table.CreaturesOnTable[i].ID);
                        //Sequence s = DOTween.Sequence();
                        //discard.cards.Add(table.CreaturesOnTable[i].ca);

                        //GameObject sweepcard = IDHolder.GetGameObjectWithID(table.CreaturesOnTable[i].ID);
                        //s.Append(sweepcard.transform.DOMove(DiscardSpot.position, GlobalSettings.Instance.CardTransitionTime));
                        //s.AppendInterval(GlobalSettings.Instance.CardPreviewTime);
                        ////s.Insert(0f, sweepcard.transform.DORotate(Vector3.zero, GlobalSettings.Instance.CardTransitionTime));
                        ////s.AppendInterval(GlobalSettings.Instance.CardPreviewTime);
                        //s.Complete();
                        if (table.CreaturesOnTable[i].ca.AttackDefense == "D")
                            defensesplayed--;

                        CardLogic newdisCard = new CardLogic(table.CreaturesOnTable[i].ca);
                        newdisCard.owner = this;
                        discardcards.CardsInDiscard.Insert(0, newdisCard);
                        PArea.tableVisual.RemoveCreatureWithID(table.CreaturesOnTable[i].UniqueCreatureID);
                        table.CreaturesOnTable.Remove(table.CreaturesOnTable[i]);
                    }
                    else
                    {
                        if (table.CreaturesOnTable[i].ca.AttackDefense == "G" && table.CreaturesOnTable[i].PowerAOrB == true)
                        {
                            //Need to check if PowerBlock and remove visual indictors
                            GameObject target = IDHolder.GetGameObjectWithID(table.CreaturesOnTable[i].ID);
                            target.GetComponent<OneCreatureManager>().PowerIcon.enabled = false;
                            target.GetComponent<OneCreatureManager>().CreatureGlowImage.enabled = false;
                        }
                    }
                }
            }


        }
        else
        {
            Debug.Log("No Card on Table to process Sweep");
        }

        if (MustOrMayAction)
        {
            currentphase = "MayMust";
            GameObject.FindWithTag("ButtonText").GetComponent<Text>().text = "End May/Must Phase";
            CheckMustOrMayAction();
        }
        else
        {
            currentphase = "Defense";
            GameObject.FindWithTag("ButtonText").GetComponent<Text>().text = "End Defense Phase";
            GameObject.FindWithTag("PowerButton").GetComponent<Text>().text = "Exert for Power Block";
        }

    }

    public bool MoreCardsToSweep()
    {
        bool morecards = false;

        for (int i = 0; i < table.CreaturesOnTable.Count; i++)
        {
            if (table.CreaturesOnTable[i].ca.SweepAttack == true ||
                table.CreaturesOnTable[i].ca.SweepDefense == true ||
                table.CreaturesOnTable[i].ca.SweepSpecial == true)
            {
                morecards = true;
                break;
            }
            else
            {
                morecards = false;
            }
        }

        Debug.Log("More Cards to Sweep: " + morecards);
        if (morecards)
            return true;
        else
            return false;
    }

    public void ProcessDefenses()
    {
        Debug.Log("In ProcessDefenses");
        Debug.Log("Player Defending: " + name);
        Debug.Log("Player Attacking: " + otherPlayer.name);

        blocktype blockedit;

        int numofblocks = GetNumberOfBlocks();
        Debug.Log("Numofblocks: " + numofblocks);
        int numofattacks = 0;

        CreatureLogic[] HiddenAttacks = TurnManager.Instance.whoseTurn.otherPlayer.table.CreaturesOnTable.ToArray();
        foreach (CreatureLogic cl in HiddenAttacks)
        {
            if (cl.HiddenAttack == true)
            {
                Debug.Log("Name: " + cl.ca.name);
                Debug.Log("HA?: " + cl.HiddenAttack);
                GameObject hiddencard = IDHolder.GetGameObjectWithID(cl.UniqueCreatureID);
                Debug.Log("Tag: " + tag);
                if (tag == "LowPlayer")
                {
                    Debug.Log("Positions Low");
                    Vector3 oldvector = hiddencard.transform.position;
                    Debug.Log(oldvector);
                    hiddencard.transform.SetPositionAndRotation(new Vector3(transform.position.x + .5f, transform.position.y + 2.8f, transform.position.z), Quaternion.Euler(new Vector3(0, 0, 0)));
                    hiddencard.transform.position = oldvector;
                    Debug.Log(hiddencard.transform.position);
                    PArea.tableVisual.ShiftSlotsGameObjectAccordingToNumberOfCreatures();
                    PArea.tableVisual.PlaceCreaturesOnNewSlots();
                }
                else
                {
                    //Good
                    Debug.Log("Positions High");
                    Vector3 oldvector = hiddencard.transform.position;
                    Debug.Log(oldvector);
                    hiddencard.transform.SetPositionAndRotation(new Vector3(transform.position.x + .5f, transform.position.y - 1.9f, transform.position.z), Quaternion.Euler(new Vector3(0, 0, 0)));
                    hiddencard.transform.position = oldvector;
                    Debug.Log(hiddencard.transform.position);
                    PArea.tableVisual.ShiftSlotsGameObjectAccordingToNumberOfCreatures();
                    PArea.tableVisual.PlaceCreaturesOnNewSlots();
                }

            }
        }

        if (otherPlayer.table.CreaturesOnTable.Count > 0)
        {

            for (int i = 0; i < otherPlayer.table.CreaturesOnTable.Count; i++)
            {
                Debug.Log("Card Name: " + otherPlayer.table.CreaturesOnTable[i].ca.name);
                if (otherPlayer.table.CreaturesOnTable[i].ca.AttackDefense == "A")
                {
                    Debug.Log("This card needs to be defended");
                    numofattacks++;
                    blockedit = blocktype.noblock;
                    bool powerblow = otherPlayer.table.CreaturesOnTable[i].PowerAOrB;

                    //First check to see if defending player has played a block.
                    if (numofblocks > 0)
                    {
                        Debug.Log("Blocks played - check'em");
                        //Now loop through blocks to see if any block the attack
                        
                        for (int y = 0; y < table.CreaturesOnTable.Count; y++)
                        {
                            if (table.CreaturesOnTable[y].ca.AttackDefense == "D" ||
                                table.CreaturesOnTable[y].ca.AttackDefense == "G")
                            {
                                Debug.Log("Processing Block: " + table.CreaturesOnTable[y].ca.name);

                                string attackgrid = otherPlayer.table.CreaturesOnTable[i].ca.GridData;
                                string[] attackvalues = attackgrid.Split(',');

                                string defensegrid = table.CreaturesOnTable[y].ca.GridData;
                                string[] defensevalues = defensegrid.Split(',');
                                bool powerblock = table.CreaturesOnTable[y].PowerAOrB;

                                if (table.CreaturesOnTable[y].ca.DefenseEffectName != null &&
                                    table.CreaturesOnTable[y].ca.DefenseEffectName.Length > 0 &&
                                    table.CreaturesOnTable[y].ActivatedDefEffect == false)
                                {
                                    Debug.Log("DefenseEffectName: " + table.CreaturesOnTable[y].ca.DefenseEffectName);
                                    //ActivateDefenseEffect(table.CreaturesOnTable[y].UniqueCreatureID, table.CreaturesOnTable[y].ca);
                                    table.CreaturesOnTable[y].ca.defenseeffect.ActivateEffect
                                        (table.CreaturesOnTable[y].ca.specialSpellAmount, null, this, this.otherPlayer, table.CreaturesOnTable[y].ca.SpecialData);
                                    table.CreaturesOnTable[y].ActivatedDefEffect = true;
                                }
                                //ActivateDefenseEffect(table.CreaturesOnTable[y].UniqueCreatureID, table.CreaturesOnTable[y].ca);

                                for (int x = 0; x < 9; x++)
                                {
                                    if (int.Parse(attackvalues[x]) == 1)
                                    {
                                        int defresult = int.Parse(defensevalues[x]) + int.Parse(attackvalues[x]);
                                        if (defresult > 0)
                                        {
                                            //Need to check previous block values
                                            //example: 1 block failed, 2nd succeed on same attack;
                                            //attacksuccessful = true;
                                            blockedit = blocktype.noblock;

                                            break;

                                        }
                                        else
                                        {
                                            if (int.Parse(attackvalues[x]) == 1 && defresult == 0)
                                            {
                                                Debug.Log("Attack Defended!");
                                                Debug.Log("PBlow: " + powerblow);
                                                Debug.Log("PBlock: " + powerblock);
                                                //attacksuccessful = false;
                                                if (table.CreaturesOnTable[y].ca.TypeOfAttack != AttackTypes.Dodge)
                                                {
                                                    if (powerblow && powerblock)
                                                        blockedit = blocktype.fullblock;
                                                    else if (powerblow)
                                                        blockedit = blocktype.halfblock;
                                                    else
                                                        blockedit = blocktype.fullblock;
                                                }
                                                else
                                                {
                                                    blockedit = blocktype.fullblock;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        //Now all blocks have been processed for this attack so check if successful

                        //if (attacksuccessful == true)
                        if (blockedit == blocktype.noblock || blockedit == blocktype.halfblock)
                        {
                            Debug.Log("Attack Succeeded - Block Unsuccessful");
                            //Debug.Log("Player: " + name + " takes " + otherPlayer.table.CreaturesOnTable[i].ca.DamageAmt + " Damage");
                            Debug.Log("Player: " + name + " takes " + GetDamageValue(otherPlayer.table.CreaturesOnTable[i].ca,
                                                                otherPlayer.table.CreaturesOnTable[i].PowerAOrB, blockedit) + " Damage");
                            Debug.Log("Health: " + TurnManager.Instance.whoseTurn.Health);
                            Debug.Log("Block: " + blockedit);
                            if (otherPlayer.table.CreaturesOnTable[i].PowerAOrB == false)
                            {
                                //damageaccrued += otherPlayer.table.CreaturesOnTable[i].ca.DamageAmt;
                                damageaccrued += GetDamageValue(otherPlayer.table.CreaturesOnTable[i].ca,
                                                                otherPlayer.table.CreaturesOnTable[i].PowerAOrB, blockedit);
                                new ShowMessageCommand("Slice!", 2.0f).AddToQueue();
                            }
                            else
                            {
                                Debug.Log("PowerBlow!");
                                Debug.Log("PB? " + otherPlayer.table.CreaturesOnTable[i].PowerAOrB);
                                Debug.Log("Block Type: " + blockedit);
                                if (blockedit == blocktype.noblock)
                                {
                                    if (otherPlayer.table.CreaturesOnTable[i].Headshot == true)
                                    {
                                        new ShowMessageCommand("Headshot!!!!", 2.0f).AddToQueue();
                                        Die();
                                    }
                                    else
                                    {
                                        //damageaccrued += otherPlayer.table.CreaturesOnTable[i].ca.DamageAmt + 2;
                                        damageaccrued += GetDamageValue(otherPlayer.table.CreaturesOnTable[i].ca,
                                                                        otherPlayer.table.CreaturesOnTable[i].PowerAOrB, blockedit);
                                        new ShowMessageCommand("Arghh....Power Blow!", 2.0f).AddToQueue();
                                        otherPlayer.table.CreaturesOnTable[i].PowerAOrB = false;
                                    }
                                }
                                else //regular block against PowerBlow = 2 damage instead of 4
                                {
                                    otherPlayer.table.CreaturesOnTable[i].PowerAOrB = false;
                                    //damageaccrued += otherPlayer.table.CreaturesOnTable[i].ca.DamageAmt;
                                    damageaccrued += GetDamageValue(otherPlayer.table.CreaturesOnTable[i].ca,
                                                                    otherPlayer.table.CreaturesOnTable[i].PowerAOrB, blockedit);
                                    new ShowMessageCommand("Urghhh....Partial Block of Power Blow", 2.0f).AddToQueue();
                                }
                            }
                        }
                        else
                        {
                            new ShowMessageCommand(numofattacks + " Attack BLOCKED!", 2.0f).AddToQueue();
                            Debug.Log("Block Type: " + blockedit);
                            otherPlayer.table.CreaturesOnTable[i].PowerAOrB = false;
                        }
                    }
                    else
                    {
                        //No cards on table so attack is succesful by default
                        Debug.Log("Attack Succeeded - No Blocks Played");
                        Debug.Log("Player: " + name + " takes " + GetDamageValue(otherPlayer.table.CreaturesOnTable[i].ca,
                                                            otherPlayer.table.CreaturesOnTable[i].PowerAOrB, blockedit) + " Damage");

                        Debug.Log("Health: " + TurnManager.Instance.whoseTurn.Health);
                        if (otherPlayer.table.CreaturesOnTable[i].PowerAOrB == false)
                        {
                            //damageaccrued += otherPlayer.table.CreaturesOnTable[i].ca.DamageAmt;
                            damageaccrued += GetDamageValue(otherPlayer.table.CreaturesOnTable[i].ca,
                                                            otherPlayer.table.CreaturesOnTable[i].PowerAOrB, blockedit);
                            new ShowMessageCommand("Stab!", 2.0f).AddToQueue();
                        }
                        else
                        {
                            Debug.Log("PowerBlow!");
                            Debug.Log("PB? " + otherPlayer.table.CreaturesOnTable[i].PowerAOrB);
                            if (otherPlayer.table.CreaturesOnTable[i].Headshot == true)
                            {
                                new ShowMessageCommand("Headshot!!!!", 2.0f).AddToQueue();
                                Die();
                            }
                            else
                            {
                                //damageaccrued += otherPlayer.table.CreaturesOnTable[i].ca.DamageAmt + 2;
                                damageaccrued += GetDamageValue(otherPlayer.table.CreaturesOnTable[i].ca,
                                                                otherPlayer.table.CreaturesOnTable[i].PowerAOrB, blockedit);
                                new ShowMessageCommand("Arghh....Power Blow!", 2.0f).AddToQueue();
                                otherPlayer.table.CreaturesOnTable[i].PowerAOrB = false;
                            }
                        }
                    }
                }
            } //end for loop for processing attacks 
              //should now be able to result all block to non-Powerblocks
            for (int y = 0; y < table.CreaturesOnTable.Count; y++)
            {
                if (table.CreaturesOnTable[y].ca.AttackDefense == "D" || table.CreaturesOnTable[y].ca.AttackDefense == "G")
                    table.CreaturesOnTable[y].PowerAOrB = false;
            }
        }
        else
        {
            Debug.Log("No Card on Table so nothing to Defend");
        }
    }

    public int GetNumberOfBlocks()
    {
        int numofblocks = 0;

        for (int i = 0; i < table.CreaturesOnTable.Count; i++)
        {
            if (table.CreaturesOnTable[i].ca.AttackDefense == "D" || table.CreaturesOnTable[i].ca.AttackDefense == "G")
                numofblocks++;
        }
        return numofblocks;
    }

    public int GetDamageValue(CardAsset attackcard, bool PowerBlow, blocktype block)
    {
        int damagevalue = 0;
        Debug.Log("N = " + otherPlayer.NormalAttack + "  P = " + otherPlayer.PowerAttack);
        switch (attackcard.TypeOfAttack)
        {
            case AttackTypes.Regular:
                if (PowerBlow)
                {
                    if (block == blocktype.halfblock)
                        damagevalue += otherPlayer.PowerAttack - 2;
                    else
                        damagevalue += otherPlayer.PowerAttack;
                }
                else
                    damagevalue += otherPlayer.NormalAttack;
                break;

            case AttackTypes.Additional:
                if (PowerBlow)
                {
                    if (block == blocktype.halfblock)
                        damagevalue += otherPlayer.PowerAttack - 2 + attackcard.AdditionalDamageAmt;
                    else
                        damagevalue += otherPlayer.PowerAttack + attackcard.AdditionalDamageAmt;
                }
                else
                    damagevalue += otherPlayer.NormalAttack + attackcard.AdditionalDamageAmt;
                break;
        }

        Debug.Log("GetDamageValue = " + damagevalue);

        return damagevalue;
    }

        public void ProcessPowerButton()
    {
        Debug.Log("In ProcessPowerButton");
        Debug.Log("Player: " + name + " Phase: " + currentphase + " HE left: " + HardExertionsLeft);
        int targetgridid = -1;

        if (ProcessEventCard)
        {
            switch (MMDiscardCards)
            {
                case "R":
                    MMDrawCards--;
                    DrawACard();
                    MayMustMessage = "You must draw " + MMDrawCards + " more cards from your Endurance";
                    if (MMDrawCards == 0)
                    {
                        ProcessEventCard = false;
                        MustOrMayAction = false;
                        if (currentphase == "MayMust")
                        {
                            currentphase = "Defense";
                            GameObject.FindWithTag("ButtonText").GetComponent<Text>().text = "End Defense Phase";
                            GameObject.FindWithTag("PowerButton").GetComponent<Text>().text = "Exert for Power Block";
                        }
                        else if (currentphase == "Defense")
                        {
                            GameObject.FindWithTag("ButtonText").GetComponent<Text>().text = "End Defense Phase";
                            GameObject.FindWithTag("PowerButton").GetComponent<Text>().text = "Exert for Power Block";
                        }
                        else if (currentphase == "Attack")
                        {
                            GameObject.FindWithTag("ButtonText").GetComponent<Text>().text = "End Attack Phase";
                            GameObject.FindWithTag("PowerButton").GetComponent<Text>().text = "Exert for Power Blow";
                        }
                        HighlightPlayableCards();
                    }                  
                    break;
                case "Z":
                    MMExertAnyCards--;
                    ExertACard();
                    MayMustMessage = "You must exert " + MMExertAnyCards + " more cards from your Endurance";
                    if (MMExertAnyCards == 0)
                    {
                        ProcessEventCard = false;
                        MustOrMayAction = false;
                        if (currentphase == "MayMust")
                        {
                            currentphase = "Defense";
                            GameObject.FindWithTag("ButtonText").GetComponent<Text>().text = "End Defense Phase";
                            GameObject.FindWithTag("PowerButton").GetComponent<Text>().text = "Exert for Power Block";
                        }
                        HighlightPlayableCards();
                    }
                    break;
            }
        }
        else if (table.CreaturesOnTable.Count > 0 && HardExertionsLeft > 0)
        {
            Debug.Log("Creature on table > 0");
            if (currentphase == "Attack")
            {
                //Find last creature played on table
                for (int i = 0; i < table.CreaturesOnTable.Count; i++)
                {
                    if (table.CreaturesOnTable[i].ca.AttackDefense == "A")
                    {
                        targetgridid = i;
                    }
                }

                Debug.Log("After For");

                if (targetgridid >= 0)
                {
                    Debug.Log("going to for loop for PowerBlow");
                    for (int x = 0; x < 3; x++)
                        ExertACard(false);
                    GameObject target = IDHolder.GetGameObjectWithID(table.CreaturesOnTable[targetgridid].ID);
                    table.CreaturesOnTable[targetgridid].PowerAOrB = true;
                    //target.GetComponent<OneCreatureManager>().IncreaseDamage(table.CreaturesOnTable[targetgridid].ca.DamageAmt + 2);
                    target.GetComponent<OneCreatureManager>().IncreaseDamage(PowerAttack);
                    target.GetComponent<OneCreatureManager>().PowerIcon.enabled = true;
                    target.GetComponent<OneCreatureManager>().CreatureGlowImage.enabled = true;
                    if (table.CreaturesOnTable[targetgridid].HiddenAttack == true)
                    {
                        target.GetComponent<OneCreatureManager>().CardBackGlowImage.enabled = true;
                    }
                    HardExertionsLeft = hardexertionsleft - 1;
                    otherPlayer.NextAttackHidden = true;
                    otherPlayer.PArea.HiddenIconButton1.image.enabled = true;
                }
            }
            else
            {
                if (currentphase == "Defense")
                {
                    for (int i = 0; i < table.CreaturesOnTable.Count; i++)
                    {
                        if (table.CreaturesOnTable[i].ca.AttackDefense == "D" || table.CreaturesOnTable[i].ca.AttackDefense == "G")
                        {
                            targetgridid = i;
                        }
                    }

                    if (targetgridid >= 0)
                    {
                        Debug.Log("going to for loop for PowerBlock");
                        for (int x = 0; x < 3; x++)
                            ExertACard(false);
                        GameObject target = IDHolder.GetGameObjectWithID(table.CreaturesOnTable[targetgridid].ID);
                        table.CreaturesOnTable[targetgridid].PowerAOrB = true;
                        target.GetComponent<OneCreatureManager>().PowerIcon.enabled = true;
                        target.GetComponent<OneCreatureManager>().GlowImageBlock.enabled = true;
                        HardExertionsLeft = HardExertionsLeft - 1;

                    }
                }
            }
        }
        else
        {
            Debug.Log("Trying to Exert with No Creatures or No Extersions left: " + table.CreaturesOnTable.Count + " - " + HardExertionsLeft);
            if (HardExertionsLeft == 0)
            {
                new ShowMessageCommand("No Hard Extertions Left", 1f).AddToQueue();
            }
        }

    }

    public void ExertACard(bool fast = false)
    {
        if (deck.cards.Count > 1)
        {
            CardLogic newdisCard = new CardLogic(deck.cards[0]);
            newdisCard.owner = this;
            discardcards.CardsInDiscard.Insert(0, newdisCard);
            Debug.Log("In ExertACard");
            // Debug.Log(hand.CardsInHand.Count);
            // 2) logic: remove the card from the deck
            deck.cards.RemoveAt(0);
            // 2) create a command
            new ExertACardCommand(discardcards.CardsInDiscard[0], this, fast, fromDeck: true).AddToQueue();
            PArea.PDeck.NumOfCardsInDeck.text = deck.cards.Count.ToString();
            // Debug.Log("After DrawACardCommand");
        }
        else
        {
            Debug.Log("No more cards in deck - take 5 damage ");
            //new DealDamageCommand(TurnManager.Instance.whoseTurn.PlayerID, 5, TurnManager.Instance.whoseTurn.Health - 5).AddToQueue();
            //TurnManager.Instance.whoseTurn.Health -= 5;
            ExhaustPlayer();
            ExertACard(false);
        }

    }

    public void DiscardACardFromHand(int UniqueID)
    {
        Debug.Log("In DiscardCard 1");
        Debug.Log("CinH: " + hand.CardsInHand.Count);
        GameObject discardcard = IDHolder.GetGameObjectWithID(UniqueID);
        PArea.handVisual.RemoveCard(discardcard);
        DiscardACardFromHand(CardLogic.CardsCreatedThisGame[UniqueID]);
        Debug.Log("CinH 2: " + hand.CardsInHand.Count);
        Destroy(discardcard);
        Debug.Log("CinH 3: " + hand.CardsInHand.Count);
    }

    public void DiscardACardFromHand(CardLogic playedCard)
    {
        Debug.Log("In DiscardCard 2");
        CardLogic newdisCard = new CardLogic(playedCard.ca);
        newdisCard.owner = this;
        discardcards.CardsInDiscard.Insert(0, newdisCard);
        hand.CardsInHand.Remove(playedCard);

        if (ProcessEventCard)
        {
            switch (MMDiscardCards)
            {
                case "A":
                    MMDiscardAttacks--;
                    if (MMDiscardAttacks == 0 || MustMayConditionMet())
                    {
                        ProcessEventCard = false;
                        MustOrMayAction = false;
                        if (currentphase == "MayMust")
                        {
                            currentphase = "Defense";
                            GameObject.FindWithTag("ButtonText").GetComponent<Text>().text = "End Defense Phase";
                            GameObject.FindWithTag("PowerButton").GetComponent<Text>().text = "Exert for Power Block";
                        }
                    }
                    Debug.Log("SavedPlayedCard Name: " + SavedPlayedCard.ca.name);
                    if (SavedPlayedCard.secondeffect != null)
                    {
                        Debug.Log("Activating SECOND SPECIAL effect");
                        SavedPlayedCard.secondeffect.ActivateEffect(SavedPlayedCard.ca.secondSpecialAmount, null, this, this.otherPlayer, SavedPlayedCard.ca.SpecialData);
                    }

                    HighlightPlayableCards();
                    break;
                case "C":
                    MMDiscardAnyCards--;
                    if (MMDiscardAnyCards == 0 || MustMayConditionMet())
                    {
                        ProcessEventCard = false;
                        MustOrMayAction = false;
                        if (currentphase == "MayMust")
                        {
                            currentphase = "Defense";
                            GameObject.FindWithTag("ButtonText").GetComponent<Text>().text = "End Defense Phase";
                            GameObject.FindWithTag("PowerButton").GetComponent<Text>().text = "Exert for Power Block";
                        }
                    }
                    HighlightPlayableCards();
                    break;
                case "P":
                    Debug.Log("In DiscardACardFromHard for PLOTS");
                    if(MustMayConditionMet())
                    {
                        ProcessEventCard = false;
                        MustOrMayAction = false;
                        if (currentphase == "MayMust")
                        {
                            currentphase = "Defense";
                            GameObject.FindWithTag("ButtonText").GetComponent<Text>().text = "End Defense Phase";
                            GameObject.FindWithTag("PowerButton").GetComponent<Text>().text = "Exert for Power Block";
                        }
                    }

                    HighlightPlayableCards();
                    break;
            }
        }
        else
        {
            if (hand.CardsInHand.Count <= Health)
            {
                if (currentphase == "Discard")
                {
                    currentphase = "End Phase";
                    TurnManager.Instance.EndTurn();
                }
            }
        }
    }

    public void DropGuard()
    {
        Debug.Log("In DropGuard");
        if (table.CreaturesOnTable.Count > 0)
        {

            for (int i = 0; i < table.CreaturesOnTable.Count; i++)
            {
                Debug.Log("Card Name: " + table.CreaturesOnTable[i].ca.name);
                if (table.CreaturesOnTable[i].ca.AttackDefense == "G")
                {
                    Debug.Log("This card should be REMOVED");
                    Debug.Log("Sweep IDCr: " + table.CreaturesOnTable[i].UniqueCreatureID);
                    Debug.Log("Sweep ID: " + table.CreaturesOnTable[i].ID);
                    //Sequence s = DOTween.Sequence();
                    //discard.cards.Add(table.CreaturesOnTable[i].ca);

                    //GameObject sweepcard = IDHolder.GetGameObjectWithID(table.CreaturesOnTable[i].ID);
                    //s.Append(sweepcard.transform.DOMove(DiscardSpot.position, GlobalSettings.Instance.CardTransitionTime));
                    //s.AppendInterval(GlobalSettings.Instance.CardPreviewTime);
                    ////s.Insert(0f, sweepcard.transform.DORotate(Vector3.zero, GlobalSettings.Instance.CardTransitionTime));
                    ////s.AppendInterval(GlobalSettings.Instance.CardPreviewTime);
                    //s.Complete();
                    if (table.CreaturesOnTable[i].ca.AttackDefense == "G")
                    {
                        GuardInPlay = false;
                        PArea.ShieldIcon.image.enabled = false;
                        //This is NOT being incremented Yet so leave uncommented --> defensesplayed--;
                    }
                    CardLogic newdisCard = new CardLogic(table.CreaturesOnTable[i].ca);
                    newdisCard.owner = this;
                    discardcards.CardsInDiscard.Insert(0, newdisCard);
                    PArea.tableVisual.RemoveCreatureWithID(table.CreaturesOnTable[i].UniqueCreatureID);
                    table.CreaturesOnTable.Remove(table.CreaturesOnTable[i]);
                    HighlightPlayableCards();
                }
            }
        }
        else
        {
            Debug.Log("No GUARD on Table to Remove");
        }
    }

    public void CheckMustOrMayAction()
    {
        Debug.Log("In CheckMustOrMayAction");
        if (MustOrMayAction)
        {
            Debug.Log("There IS A MAY/MUST Action");
            ProcessEventCard = true;

            if (MustMayConditionMet())
            {
                ProcessEventCard = false;
                MustOrMayAction = false;
                if (currentphase == "MayMust")
                {
                    currentphase = "Defense";
                    GameObject.FindWithTag("ButtonText").GetComponent<Text>().text = "End Defense Phase";
                    GameObject.FindWithTag("PowerButton").GetComponent<Text>().text = "Exert for Power Block";
                    HighlightPlayableCards();
                }
                else
                {

                }
            }
            else
            {
                new ShowMessageCommand(MayMustMessage, 2f).AddToQueue();
            }
        }
        else
        {
            Debug.Log("No MustOrMayAction Required");
        }
    }

    public bool MustMayConditionMet()
    {
        bool MMCondMet = true;
        Debug.Log("In MustMayConditionMet");

        if (ProcessEventCard == true)
        {
            Debug.Log("ProcessEventCard = true");
            switch (MMDiscardCards)
            {
                case "A":
                    for(int i = 0; i < hand.CardsInHand.Count; i++)
                    {
                        if (hand.CardsInHand[i].ca.AttackDefense == "A" && MMDiscardAttacks > 0)
                        {
                            MMCondMet = false;
                            break;
                        }
                    }
                    break;
                case "C":
                    if (hand.CardsInHand.Count > 0 && MMDiscardAnyCards > 0)
                        MMCondMet = false;
                    break;
                case "P":
                    if (MMDiscardAnyCards == -1)
                    {
                        int numplotcards = 0;
                        for (int i = 0; i < hand.CardsInHand.Count; i++)
                        {
                            if (hand.CardsInHand[i].ca.PlotName.Length > 0)
                            {
                                numplotcards++;
                                MMCondMet = false;
                                break;
                            }
                        }
                    }
                    break;
                case "R":
                    Debug.Log("In Draw MM CHECK");
                    GameObject.FindWithTag("PowerButton").GetComponent<Text>().text = "Draw 1 Card";
                    Debug.Log("MMDrawCards: " + MMDrawCards);
                    if (MMDrawCards == 0)
                    {
                        MMCondMet = true;
                        break;
                    }
                    else
                        MMCondMet = false;
                    break;
                case "Z":
                    Debug.Log("In Exert MM CHECK");
                    GameObject.FindWithTag("PowerButton").GetComponent<Text>().text = "Exert 1 Card";
                    Debug.Log("MMExertAnyCard: " + MMExertAnyCards);
                    if (MMExertAnyCards == 0)
                    {
                        MMCondMet = true;
                        break;
                    }
                    else
                        MMCondMet = false;
                    break;
                case "0": //Disarmed Condition - ONLY MUST MAY PHASE
                    if (Dicerolled == false)
                    {
                        GameObject.FindWithTag("ButtonText").GetComponent<Text>().text = "End Must/May Phase";
                        GameObject.FindWithTag("Dice1").GetComponent<SpriteRenderer>().enabled = true;
                        GameObject.FindWithTag("Dice1").GetComponent<BoxCollider2D>().enabled = true;
                        rollreason = "Rearm";
                        Debug.Log("In MM Disarm CHECK");
                        MMCondMet = false;
                        //GameObject.FindWithTag("PowerButton").GetComponent<Text>().text = "Exert 1 Card";
                    }
                    else
                    {
                        GameObject.FindWithTag("Dice1").GetComponent<SpriteRenderer>().enabled = false;
                        GameObject.FindWithTag("Dice1").GetComponent<BoxCollider2D>().enabled = false;
                        MMCondMet = true;
                        //currentphase = "Defense";
                    }
                    break;                    
            }
        }

        return MMCondMet;
    }

    public void ProcessPersonaAbility()
    {
        Debug.Log("In ProcessPersonaAbility");
        if (personacondcheck != null)
            personacondcheck.PersonaCheckCondition(this, this.otherPlayer);
        else
        {
            Debug.LogWarning("No Persona Ability found for " + charAsset.name);
        }
    }

    public void CheckGuardForPowerBlock()
    {
        Debug.Log("In CheckGuardForPowerBlock");
        for (int i = table.CreaturesOnTable.Count; i > 0; i--)
        {
            if (table.CreaturesOnTable[i - 1].ca.AttackDefense == "G" && table.CreaturesOnTable[i-1].PowerAOrB == true)
            {
                table.CreaturesOnTable[i - 1].PowerAOrB = false;
                GameObject target = IDHolder.GetGameObjectWithID(table.CreaturesOnTable[i-1].ID);
                target.GetComponent<OneCreatureManager>().PowerIcon.enabled = false;
                target.GetComponent<OneCreatureManager>().GlowImageBlock.enabled = false;
            }
        }

    }

    public void EvaluteDiceRoll()
    {
        Debug.Log("In EvaluteDiceRoll");
        Debug.Log("Rollreason: " + rollreason);
        Debug.Log("Dice: " + TurnManager.Instance.gamedie1result);

        switch (rollreason)
        {
            case "Rearm":
                Dicerolled = true;
                if (TurnManager.Instance.gamedie1result == 1)
                {
                    Disarmed = false;
                    PArea.DisarmIcon.image.enabled = false;
                    new ShowMessageCommand("REARMED!!!!!", 2f).AddToQueue();
                }
                break;
            case "Disarm":
                if (TurnManager.Instance.gamedie1result == 1)
                {
                    Debug.Log("Result is 1 - Other player is Disarmed");
                    otherPlayer.Disarmed = true;
                    otherPlayer.PArea.DisarmIcon.image.enabled = true;
                    otherPlayer.MustOrMayAction = true;
                    otherPlayer.MMDiscardCards = "0";
                    otherPlayer.ProcessEventCard = true;
                    ProcessEventCard = false;
                    otherPlayer.MayMustMessage = "You may roll dice to rearm (1 in 6) or exer 5 cards for (2 in 6)";
                    if(otherPlayer.GuardInPlay == true)
                    {
                        Debug.Log("Need to remove Opponent's Guard in Play");
                        otherPlayer.DropGuard();
                    }
                    new ShowMessageCommand(otherPlayer.name+" is now DISARMED!", 2f).AddToQueue();
                }
                else
                {
                    new ShowMessageCommand("Roll failed...."+otherPlayer.name+" keeps their weapon", 2f).AddToQueue();
                }
                break;
            case "Mugging":
                ProcessEventCard = false;
                if (TurnManager.Instance.gamedie1result <= 2)
                {
                    otherPlayer.damageaccrued += rollamount;
                    new ShowMessageCommand("Mugging does "+rollamount + " Damage to " + otherPlayer.name, 1f).AddToQueue();
                }
                else
                {
                    new ShowMessageCommand(otherPlayer.name + " avoided the mugging", 2f).AddToQueue();
                }
                break;
        }

        rollreason = "";
        rollamount = 0;
        GameObject.FindWithTag("Dice1").GetComponent<SpriteRenderer>().enabled = false;
        GameObject.FindWithTag("Dice1").GetComponent<BoxCollider2D>().enabled = false;
        HighlightPlayableCards();
    }

    public void RemoveCardFromTable(Player cardowner, string cardtoremove)
    {
        for (int i = 0; i < cardowner.table.CreaturesOnTable.Count; i++)
        {

            if (cardtoremove == cardowner.table.CreaturesOnTable[i].ca.name)
            {
                Debug.Log("Remove card: " + cardtoremove);
                Debug.Log("ID to Remove: " + cardowner.table.CreaturesOnTable[i].UniqueCreatureID);
                Debug.Log(" i: " + i);

                CardLogic newdisCard = new CardLogic(cardowner.table.CreaturesOnTable[i].ca);
                newdisCard.owner = cardowner;
                cardowner.discardcards.CardsInDiscard.Insert(0, newdisCard);
                cardowner.PArea.tableVisual.RemoveCreatureWithID(cardowner.table.CreaturesOnTable[i].UniqueCreatureID);
                cardowner.table.CreaturesOnTable.Remove(cardowner.table.CreaturesOnTable[i]);
                break;
            }
            
        }
    }
}
