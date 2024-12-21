using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TroopTrainingListEntry : MonoBehaviour
{
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI goldCostText;
    public TextMeshProUGUI meatCostText;
    public TextMeshProUGUI moveSpeedText;
    public TextMeshProUGUI attackSpeedText;
    public TextMeshProUGUI trainTimeText;
    public Image troopIcon;

    public Button trainButton;
    public Slider trainSlider;


    private PlayerTown playerTown;
    private TroopData data;

    private string matchID;
    private int teamNum;

    public void InitTroop(TroopData data, PlayerTown playerTown)
    {
        hpText.text = data.hp.ToString();
        damageText.text = data.damage.ToString();
        goldCostText.text = data.goldCost.ToString();
        meatCostText.text = data.meatCost.ToString();
        moveSpeedText.text = data.moveSpeed.ToString();
        attackSpeedText.text = data.attackSpeed.ToString();
        trainTimeText.text = data.trainTimeSeconds.ToString();

        troopIcon.sprite = data.troopIcon;

        this.playerTown = playerTown;
        this.data = data;

        trainButton.onClick.AddListener(OnTrainButtonClick);
    }

    private void OnTrainButtonClick()
    {
        if (LocalStateManager.instance == null)
        {
            Debug.LogError("LocalStateManager.instance is null");
            return;
        }
        if (LocalStateManager.instance.localGameData == null)
        {
            Debug.LogError("LocalStateManager.instance.localGameData is null");
            return;
        }
        if (LocalStateManager.instance.localPlayer == null)
        {
            Debug.LogError("LocalStateManager.instance.localPlayer is null");
            return;
        }

        var playerNetworking = LocalStateManager.instance.localPlayer.GetComponent<PlayerNetworking>();
        if (playerNetworking == null)
        {
            Debug.LogError("PlayerNetworking component is missing from localPlayer");
            return;
        }

        if (data == null)
        {
            Debug.LogError("data is null");
            return;
        }

        if (playerTown == null)
        {
            Debug.LogError("playerTown is null");
            return;
        }
        GameData gameData = LocalStateManager.instance.localGameData;
        int teamNum = LocalStateManager.instance.localPlayer.GetComponent<PlayerNetworking>().synchronizedPlayerGameData.teamNumber;
        if (gameData == null) Debug.LogError("GAME DATA IS NULL");
        
        if (gameData.GetGold(teamNum) >= data.goldCost && gameData.GetFood(teamNum) >= data.meatCost)
        {
            ResourceUpdateMsg msg = new ResourceUpdateMsg(matchID, teamNum, -data.goldCost, Resource.GOLD);
            ResourceUpdateMsg msg2 = new ResourceUpdateMsg(matchID, teamNum, -data.meatCost, Resource.FOOD);
            GameManager.instance.CMDUpdateResource(msg);
            GameManager.instance.CMDUpdateResource(msg2);
            playerTown.TrainTroop(data);
            StartCoroutine(TrainTroopIndicatorStart());
        }
    }

    private IEnumerator TrainTroopIndicatorStart()
    {
        float elapsedTime = 0f;
        float startValue = 0f;
        float endValue = 1f;

        while (elapsedTime < data.trainTimeSeconds)
        {
            elapsedTime += Time.deltaTime;
            trainSlider.value = Mathf.Lerp(startValue, endValue, elapsedTime / (float)data.trainTimeSeconds);
            yield return null;
        }

        trainSlider.value = endValue;

        trainSlider.value = 0f;

    }
    // Start is called before the first frame update
    void Start()
    {
        PlayerNetworking player = LocalStateManager.instance.localPlayer.GetComponent<PlayerNetworking>();
        matchID = player.synchronizedPlayerGameData.matchPtr.matchID;
        teamNum = player.synchronizedPlayerGameData.teamNumber;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
