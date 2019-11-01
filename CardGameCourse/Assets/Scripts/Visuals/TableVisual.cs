using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class TableVisual : MonoBehaviour 
{
    // PUBLIC FIELDS

    // an enum that mark to whish caracter this table belongs. The alues are - Top or Low
    public AreaPosition owner;

    // a referense to a game object that marks positions where we should put new Creatures
    public SameDistanceChildren slots;

    // PRIVATE FIELDS

    // list of all the creature cards on the table as GameObjects
    private List<GameObject> CreaturesOnTable = new List<GameObject>();

    // are we hovering over this table`s collider with a mouse
    private bool cursorOverThisTable = false;

    // A 3D collider attached to this game object
    private BoxCollider col;

    // PROPERTIES

    // returns true if we are hovering over any player`s table collider
    public static bool CursorOverSomeTable
    {
        get
        {
            TableVisual[] bothTables = GameObject.FindObjectsOfType<TableVisual>();
            return (bothTables[0].CursorOverThisTable || bothTables[1].CursorOverThisTable);
        }
    }

    // returns true only if we are hovering over this table`s collider
    public bool CursorOverThisTable
    {
        get{ return cursorOverThisTable; }
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

        bool passedThroughTableCollider = false;
        foreach (RaycastHit h in hits)
        {
            // check if the collider that we hit is the collider on this GameObject
            if (h.collider == col)
                passedThroughTableCollider = true;
        }
        cursorOverThisTable = passedThroughTableCollider;
    }
   
    // method to create a new creature and add it to the table
    public void AddCreatureAtIndex(CardAsset ca, int UniqueID ,int index, Player player)
    {
        // create a new creature from prefab
        //Debug.Log("Before GameObject creature set");
        GameObject creature = GameObject.Instantiate(GlobalSettings.Instance.CreaturePrefab, slots.Children[index].transform.position, Quaternion.identity) as GameObject;

        // apply the look from CardAsset
       // Debug.Log("Before OneCreatureManager set");
        OneCreatureManager manager = creature.GetComponent<OneCreatureManager>();
        manager.cardAsset = ca;
        Debug.Log("Call ReadCreatureFromAsset");
        manager.ReadCreatureFromAsset();

        //TODO Modify based on attack type
        manager.IncreaseDamage(player.NormalAttack);

        // add tag according to owner
        foreach (Transform t in creature.GetComponentsInChildren<Transform>())
            t.tag = owner.ToString()+"Creature";
        
        // parent a new creature gameObject to table slots
        creature.transform.SetParent(slots.transform);

        // add a new creature to the list
        CreaturesOnTable.Insert(index, creature);
        CreaturesOnTable[index].SetActive(true);
        // let this creature know about its position
        //Debug.Log("Before w assignment");
        WhereIsTheCardOrCreature w = creature.GetComponent<WhereIsTheCardOrCreature>();
        //Debug.Log("After w assignment");
        //Debug.Log("Index: " + index);
        //Debug.Log("W name:" + w.name);
        //Debug.Log("VisualState = " + w.VisualState);
        //Debug.Log("owner = " + owner);
        // TODO - why does this work when commented out????
        //Debug.Log("Before Index");
        w.Slot = index;
        //Debug.Log("After Index");
        if (owner == AreaPosition.Low)
            w.VisualState = VisualStates.LowTable;
        else
            w.VisualState = VisualStates.TopTable;
        //Debug.Log("After Visual State");
        // add our unique ID to this creature
        IDHolder id = creature.AddComponent<IDHolder>();
        id.UniqueID = UniqueID;

        Debug.Log("Check new if");
        if (TurnManager.Instance.whoseTurn.NextAttackHidden == true && TurnManager.Instance.whoseTurn.currentphase == "Attack")
        {
            Debug.Log("Turn card over to back for hidden attack");
            creature.transform.SetPositionAndRotation(new Vector3(transform.position.x, transform.position.y + .5f, transform.position.z), Quaternion.Euler(new Vector3(0, -180, 0)));
            if (TurnManager.Instance.whoseTurn.NumHiddenAttacks == 0)
            {
                TurnManager.Instance.whoseTurn.NextAttackHidden = false;
                TurnManager.Instance.whoseTurn.PArea.HiddenIconButton1.image.enabled = false;
            }
            else
            {
                TurnManager.Instance.whoseTurn.NumHiddenAttacks--;
            }
            creature.GetComponent<OneCreatureManager>().AttackText.enabled = false;
        }
        if (ca.AttackDefense == "D" || ca.AttackDefense == "G" || ca.AttackDefense == "D")
        {
            creature.GetComponent<OneCreatureManager>().AttackText.enabled = false;
        }

        if (ca.AttackDefense == "A")
        {
            Debug.Log("In 'A'");

            if (ca.TypeOfAttack == AttackTypes.Special)
                creature.GetComponent<OneCreatureManager>().AttackText.text = ca.ModifiedDamageAmt.ToString();

            if (ca.TypeOfAttack == AttackTypes.Additional)
            {
                Debug.Log("Modifying Attack value for Modified");
                Debug.Log("Additional: " + ca.AdditionalDamageAmt);

                creature.GetComponent<OneCreatureManager>().AttackText.text = (ca.AdditionalDamageAmt + player.NormalAttack).ToString();
            }

        }
        // after a new creature is added update placing of all the other creatures
        ShiftSlotsGameObjectAccordingToNumberOfCreatures();
        PlaceCreaturesOnNewSlots();

        // end command execution
        Command.CommandExecutionComplete();
    }

    public void AddSpecialAtIndex(CardAsset ca, int UniqueID, int index)
    {
        // create a new creature from prefab
        Debug.Log("AddSpecialAtIndex");
        GameObject creature = GameObject.Instantiate(GlobalSettings.Instance.NoTargetSpellCardPrefab, slots.Children[index].transform.position, Quaternion.identity) as GameObject;

        // apply the look from CardAsset
        // Debug.Log("Before OneCreatureManager set");
        OneCardManager manager = creature.GetComponent<OneCardManager>();
        manager.cardAsset = ca;
        Debug.Log("Call ReadCard(Special)FromAsset");
        manager.ReadCardFromAsset();

        // add tag according to owner
        foreach (Transform t in creature.GetComponentsInChildren<Transform>())
            t.tag = owner.ToString() + "Creature";

        // parent a new creature gameObject to table slots
        creature.transform.SetParent(slots.transform);

        Debug.Log("Before Insert ConT:" + CreaturesOnTable.Count);
        // add a new creature to the list
        CreaturesOnTable.Insert(index, creature);
        CreaturesOnTable[index].SetActive(true);
        Debug.Log("After Insert ConT:" + CreaturesOnTable.Count);
        // let this creature know about its position
        //Debug.Log("Before w assignment");
        WhereIsTheCardOrCreature w = creature.GetComponent<WhereIsTheCardOrCreature>();

        w.Slot = index;
        //Debug.Log("After Index");
        if (owner == AreaPosition.Low)
            w.VisualState = VisualStates.LowTable;
        else
            w.VisualState = VisualStates.TopTable;
        //Debug.Log("After Visual State");
        // add our unique ID to this creature\
        Debug.Log("ID Before: " + UniqueID);
        IDHolder id = creature.AddComponent<IDHolder>();
        id.UniqueID = UniqueID;
        Debug.Log("ID After: " + id.UniqueID);

        // after a new creature is added update placing of all the other creatures
        ShiftSlotsGameObjectAccordingToNumberOfCreatures();
        PlaceCreaturesOnNewSlots();
        manager.CardFaceGlowImage.enabled = false;
        w.PrintSortOrder();
        // end command execution
        Debug.Log("2 After Insert ConT:" + CreaturesOnTable.Count);
        Command.CommandExecutionComplete();
    }

    // returns an index for a new creature based on mousePosition
    // included for placing a new creature to any positon on the table
    public int TablePosForNewCreature(float MouseX)
    {
        // if there are no creatures or if we are pointing to the right of all creatures with a mouse.
        // right - because the table slots are flipped and 0 is on the right side.
        if (CreaturesOnTable.Count == 0 || MouseX > slots.Children[0].transform.position.x)
            return 0;
        else if (MouseX < slots.Children[CreaturesOnTable.Count - 1].transform.position.x) // cursor on the left relative to all creatures on the table
            return CreaturesOnTable.Count;
        for (int i = 0; i < CreaturesOnTable.Count; i++)
        {
            if (MouseX < slots.Children[i].transform.position.x && MouseX > slots.Children[i + 1].transform.position.x)
                return i + 1;
        }
        Debug.Log("Suspicious behavior. Reached end of TablePosForNewCreature method. Returning 0");
        return 0;
    }

    // Destroy a creature
    public void RemoveCreatureWithID(int IDToRemove)
    {
        // TODO: This has to last for some time
        // Adding delay here did not work because it shows one creature die, then another creature die. 
        // 
        //Sequence s = DOTween.Sequence();
        //s.AppendInterval(1f);
        //s.OnComplete(() =>
        //   {

        //    });
        Debug.Log("ConT Visual: " + CreaturesOnTable.Count);
        Debug.Log("IDTORemove: " + IDToRemove);
        GameObject creatureToRemove = IDHolder.GetGameObjectWithID(IDToRemove);
        CreaturesOnTable.Remove(creatureToRemove);
        Destroy(creatureToRemove);

        ShiftSlotsGameObjectAccordingToNumberOfCreatures();
        PlaceCreaturesOnNewSlots();
        Command.CommandExecutionComplete();
    }

    /// <summary>
    /// Shifts the slots game object according to number of creatures.
    /// </summary>
    public void ShiftSlotsGameObjectAccordingToNumberOfCreatures()
    {
        float posX;
        if (CreaturesOnTable.Count > 0)
            posX = (slots.Children[0].transform.localPosition.x - slots.Children[CreaturesOnTable.Count - 1].transform.localPosition.x) / 2f;
        else
            posX = 0f;

        slots.gameObject.transform.DOLocalMoveX(posX, 0.3f);  
    }

    /// <summary>
    /// After a new creature is added or an old creature dies, this method
    /// shifts all the creatures and places the creatures on new slots.
    /// </summary>
    public void PlaceCreaturesOnNewSlots()
    {
        foreach (GameObject g in CreaturesOnTable)
        {
            g.transform.DOLocalMoveX(slots.Children[CreaturesOnTable.IndexOf(g)].transform.localPosition.x, 0.3f);
            // apply correct sorting order and HandSlot value for later 
            // TODO: figure out if I need to do something here:
            // g.GetComponent<WhereIsTheCardOrCreature>().SetTableSortingOrder() = CreaturesOnTable.IndexOf(g);
        }
        Debug.Log("PConNS After Insert ConT:" + CreaturesOnTable.Count);
    }

}
