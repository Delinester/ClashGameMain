using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArmyCreateListEntry : MonoBehaviour
{
    private TroopData data;

    [SerializeField]
    private Image troopIcon;
    [SerializeField]
    private TextMeshProUGUI troopNameText;
    [SerializeField]
    private Button increaseButton;
    [SerializeField]
    private Button decreaseButton;
    [SerializeField]
    private TextMeshProUGUI troopsCountText;

    private int troopCount = 0;

    public void InitTroop(TroopData data)
    {
        this.data = data;
        troopIcon.sprite = data.troopIcon;
        troopNameText.text = data.troopName;
        troopsCountText.text = troopCount.ToString();
    }

    public void ResetTroopsCount()
    {
        troopCount = 0;
    }

    public int GetTroopCount()
    {
        return troopCount;
    }

    public TroopData GetTroopData()
    {
        return data;
    }

    public void OnIncreaseButtonPressed()
    {
        troopCount += 1;
        troopsCountText.text = troopCount.ToString();
    }

    public void OnDecreseButtonPressed()
    {
        if (troopCount > 0)
        {
            troopCount -= 1;
            troopsCountText.text = troopCount.ToString();
        }
    }

    void Awake()
    {
        increaseButton.onClick.AddListener(() => OnIncreaseButtonPressed());
        decreaseButton.onClick.AddListener(() => OnDecreseButtonPressed());
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
