using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WorldMapWarriorController : CharacterControllerBase
{
    [SerializeField]
    private float clickDurationThreshold = 0.3f;

    private float currentClickDuration = 0f;
    private bool isClicked = false;

    private GameUI gameUI;

    private bool isArmySelected = false;
    private WorldMapArmyAI selectedArmy = null;
    // Start is called before the first frame update
    override protected void Start()
    {
        base.Start();
        gameUI = FindObjectOfType<GameUI>();
    }

    private void DeselectArmy()
    {
        isArmySelected = false;
        if (selectedArmy != null) 
            selectedArmy.SetBaseOutline();
    }

    // Update is called once per frame
    override protected void Update()
    {
        base.Update();

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            isClicked = true;
        }
        if (isClicked)
        {
            currentClickDuration += Time.deltaTime;
        }
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            if (currentClickDuration <= clickDurationThreshold)
            {
                if (!isArmySelected)
                    gameUI.DisplayArmyCreationMenu(mousePos);
                else
                {
                    selectedArmy.MoveToPoint(mousePos);
                    DeselectArmy();
                }
            }
            currentClickDuration = 0;
            isClicked = false;
        }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(mousePos, 0.1f);
            if (colliders.Length == 0)
            {
                DeselectArmy();
            }
            foreach (Collider2D collider in colliders)
            {
                if (collider.gameObject.tag == "Army")
                {
                    isArmySelected = true;
                    selectedArmy = collider.GetComponent<WorldMapArmyAI>();
                    Debug.LogError("Selected army hash is " + selectedArmy.GetHash());
                    int teamNumber = LocalStateManager.instance.localPlayer.GetComponent<PlayerNetworking>().synchronizedPlayerGameData.teamNumber;
                    if (selectedArmy.GetOwnerTeamNum() != teamNumber || selectedArmy.IsInCombat())
                    {
                        isArmySelected = false;
                        return;
                    }
                    selectedArmy.SetThickOutline();
                    //
                    foreach(Troop troop in selectedArmy.GetComponent<WorldMapArmyAI>().GetTroopsInArmy())
                    {
                        Debug.Log("Troop " + troop.data.troopName + " counts " + troop.count);
                    }
                    break;
                }
                else
                {
                    DeselectArmy();
                }
            }
        }
    }
}

