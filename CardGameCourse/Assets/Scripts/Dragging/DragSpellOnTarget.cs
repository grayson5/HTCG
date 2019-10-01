using UnityEngine;
using System.Collections;
using DG.Tweening;

public class DragSpellOnTarget : DraggingActions {

    public TargetingOptions Targets = TargetingOptions.AllCharacters;
    private SpriteRenderer sr;
    private LineRenderer lr;
    private WhereIsTheCardOrCreature whereIsThisCard;
    private VisualStates tempVisualState;
    private Transform triangle;
    private SpriteRenderer triangleSR;
    private GameObject Target;
    private OneCardManager manager;

    public override bool CanDrag
    {
        get
        { 
            // TEST LINE: this is just to test playing creatures before the game is complete 
            // return true;

            // TODO : include full field check
             return base.CanDrag && manager.CanBePlayedNow;
        }
    }

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        lr = GetComponentInChildren<LineRenderer>();
        lr.sortingLayerName = "AboveEverything";
        triangle = transform.Find("Triangle");
        triangleSR = triangle.GetComponent<SpriteRenderer>();


        manager = GetComponentInParent<OneCardManager>();
        whereIsThisCard = GetComponentInParent<WhereIsTheCardOrCreature>();
    }

    public override void OnStartDrag()
    {
        tempVisualState = whereIsThisCard.VisualState;
        whereIsThisCard.VisualState = VisualStates.Dragging;
        sr.enabled = true;
        lr.enabled = true;
        Debug.Log("Start Targets: " + Targets);
        Debug.Log("Start Targets 2: " + manager.Targets);
    }

    public override void OnDraggingInUpdate()
    {
        // This code only draws the arrow
        Vector3 notNormalized = transform.position - transform.parent.position;
        Vector3 direction = notNormalized.normalized;
        float distanceToTarget = (direction*2.3f).magnitude;
        if (notNormalized.magnitude > distanceToTarget)
        {
            // draw a line between the creature and the target
            lr.SetPositions(new Vector3[]{ transform.parent.position, transform.position - direction*2.3f });
            lr.enabled = true;

            // position the end of the arrow between near the target.
            triangleSR.enabled = true;
            triangleSR.transform.position = transform.position - 1.5f*direction;

            // proper rotarion of arrow end
            float rot_z = Mathf.Atan2(notNormalized.y, notNormalized.x) * Mathf.Rad2Deg;
            triangleSR.transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);
        }
        else
        {
            // if the target is not far enough from creature, do not show the arrow
            lr.enabled = false;
            triangleSR.enabled = false;
        }

    }

    public override void OnEndDrag()
    {
        Target = null;
        RaycastHit[] hits;
        // TODO: raycast here anyway, store the results in 
        hits = Physics.RaycastAll(origin: Camera.main.transform.position, 
            direction: (-Camera.main.transform.position + this.transform.position).normalized, 
            maxDistance: 30f) ;

        Debug.Log("In OnEndDrag");

        foreach (RaycastHit h in hits)
        {
            if (h.transform.tag.Contains("Player"))
            {
                // selected a Player
                Debug.Log("Contains Player");
                Target = h.transform.gameObject;
            }
            else if (h.transform.tag.Contains("Creature"))
            {
                // hit a creature, save parent transform
                Debug.Log("Contains Creature");
                Debug.Log("H2: " + h.transform.name);
                Target = h.transform.parent.gameObject;
                Component[] components = h.transform.parent.gameObject.GetComponents(typeof(Component));
                foreach (Component component in components)
                {
                    Debug.Log("Creature GO: " + component.ToString());
                    if (component.ToString().Contains("IDHolder") == true)
                    {
                        Debug.Log("Found IDHolder");
                        Debug.Log("ID: " + component.GetComponent<IDHolder>().UniqueID);
                    }
                }

                Component[] components2 = h.transform.gameObject.GetComponents(typeof(Component));
                foreach (Component component in components2)
                {
                    Debug.Log("Creature GO 2: " + component.ToString());
                    if (component.ToString().Contains("IDHolder") == true)
                    {
                        Debug.Log("Found IDHolder");
                        Debug.Log("ID: " + component.GetComponent<IDHolder>().UniqueID);
                        Target = h.transform.gameObject;
                        Debug.Log("Validate ID: " + Target.GetComponent<IDHolder>().UniqueID);
                    }
                }
            }
            else if (h.transform.tag.Contains("Discard"))
            {
                Debug.Log("Trying TO DISCARD TARGET CARD");
                playerOwner.DiscardACardFromHand(GetComponentInParent<IDHolder>().UniqueID);
                return;
            }
            else
            {
                Debug.Log("In else (tag): " + h.transform.tag);
                Debug.Log("Nothing in TAG!!!");
                Debug.Log("H: " + h.transform.name);
                Component[] components = h.transform.parent.gameObject.GetComponents(typeof(Component));
                foreach (Component component in components)
                {
                    Debug.Log(component.ToString());
                    if (component.ToString().Contains("IDHolder") == true)
                    {
                        Debug.Log("Found IDHolder");
                        Debug.Log("ID: " + component.GetComponent<IDHolder>().UniqueID);
                    }
                }

                Component[] components2 = h.transform.gameObject.GetComponents(typeof(Component));
                foreach (Component component in components2)
                {
                    Debug.Log("GO: " + component.ToString());
                    if (component.ToString().Contains("IDHolder") == true)
                    {
                        Debug.Log("Found IDHolder");
                        Debug.Log("ID: " + component.GetComponent<IDHolder>().UniqueID);
                    }
                }
            }
        }

        bool targetValid = false;

        if (Target != null)
        {

            Debug.Log("Target: " + Target.tag);
            Debug.Log("Targets: " + manager.Targets);
            Player owner = null; 
            if (tag.Contains("Low"))
                owner = GlobalSettings.Instance.LowPlayer;
            else
                owner = GlobalSettings.Instance.TopPlayer;

            // check of we should play this spell depending on targeting options
            int targetID = Target.GetComponent<IDHolder>().UniqueID;
            switch (manager.Targets)
            {
                case TargetingOptions.AllCharacters: 
                    owner.PlayASpecialFromHand(GetComponentInParent<IDHolder>().UniqueID, targetID, 0);
                    targetValid = true;
                    break;
                case TargetingOptions.AllCreatures:
                    if (Target.tag.Contains("Creature"))
                    {
                        owner.PlayASpecialFromHand(GetComponentInParent<IDHolder>().UniqueID, targetID, 0);
                        targetValid = true;
                    }
                    break;
                case TargetingOptions.EnemyCharacters:
                    if (Target.tag.Contains("Creature") || Target.tag.Contains("Player"))
                    {
                        // had to check that target is not a card
                        if ((tag.Contains("Low") && Target.tag.Contains("Top"))
                           || (tag.Contains("Top") && Target.tag.Contains("Low")))
                        {
                            owner.PlayASpecialFromHand(GetComponentInParent<IDHolder>().UniqueID, targetID, 0);
                            targetValid = true;
                        }
                    }
                    break;
                case TargetingOptions.EnemyCreatures:
                    if (Target.tag.Contains("Creature"))
                    {
                        // had to check that target is not a card or a player
                        if ((tag.Contains("Low") && Target.tag.Contains("Top"))
                            || (tag.Contains("Top") && Target.tag.Contains("Low")))
                        {
                            owner.PlayASpecialFromHand(GetComponentInParent<IDHolder>().UniqueID, targetID, 0);
                            targetValid = true;
                        }
                    }
                    break;
                case TargetingOptions.YourCharacters:
                    if (Target.tag.Contains("Creature") || Target.tag.Contains("Player"))
                    {
                        // had to check that target is not a card
                        if ((tag.Contains("Low") && Target.tag.Contains("Low"))
                            || (tag.Contains("Top") && Target.tag.Contains("Top")))
                        {
                            owner.PlayASpecialFromHand(GetComponentInParent<IDHolder>().UniqueID, targetID, 0);
                            targetValid = true;
                        }
                    }
                    break;
                case TargetingOptions.YourCreatures:
                    Debug.Log("In YourCreatures");
                    if (Target.tag.Contains("Creature"))
                    {
                        // had to check that target is not a card or a player
                        if ((tag.Contains("Low") && Target.tag.Contains("Low"))
                            || (tag.Contains("Top") && Target.tag.Contains("Top")))
                        {
                            owner.PlayASpecialFromHand(GetComponentInParent<IDHolder>().UniqueID, targetID, 0);
                            targetValid = true;
                        }
                    }
                    break;
                default:
                    Debug.LogWarning("Reached default case in DragSpellOnTarget! Suspicious behaviour!!");
                    break;
            }
        }

        if (!targetValid)
        {
            // not a valid target, return
            whereIsThisCard.VisualState = tempVisualState;
            whereIsThisCard.SetHandSortingOrder();
        }

        // return target and arrow to original position
        // this position is special for spell cards to show the arrow on top
        transform.localPosition = new Vector3(0f, 0f, 0.4f);
        sr.enabled = false;
        lr.enabled = false;
        triangleSR.enabled = false;

    }

    // NOT USED IN THIS SCRIPT
    protected override bool DragSuccessful()
    {
        return true;
    }

    public override void OnCancelDrag()
    {

    }
}
