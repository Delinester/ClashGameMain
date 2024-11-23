using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameRole
{
    TOWN_MANAGER,
    WARRIOR,
    MINER
}
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField]
    private GameObject townTeam1;
    [SerializeField]
    private GameObject townTeam2;

    public Vector2 GetTownTeamPosition(int teamNum)
    {
        if (teamNum == 1)
            return townTeam1.gameObject.transform.position;
        else return townTeam2.gameObject.transform.position;
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
