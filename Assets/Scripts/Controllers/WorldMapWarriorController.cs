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
    // Start is called before the first frame update
    override protected void Start()
    {
        base.Start();
        gameUI = FindObjectOfType<GameUI>();
    }

    // Update is called once per frame
    override protected void Update()
    {
        base.Update();

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
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                gameUI.DisplayArmyCreationMenu(mousePosition);
            }
            currentClickDuration = 0;
            isClicked = false;
        }
    }
}
