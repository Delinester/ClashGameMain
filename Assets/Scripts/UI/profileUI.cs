using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class profileUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI headerText;
    [SerializeField]
    private TextMeshProUGUI nameText;
    [SerializeField]
    private TextMeshProUGUI surnameText;
    [SerializeField] 
    private TextMeshProUGUI genderText;
    public TextMeshProUGUI bdayText;
    [SerializeField] 
    private TextMeshProUGUI ageText;
    [SerializeField] 
    private TextMeshProUGUI addressText;
    [SerializeField] 
    private TextMeshProUGUI emailText;
    [SerializeField]
    private Image profileImage;

    public void SetData(PlayerNetworking.UserData userdata)
    {
        headerText.text = $"Profile of {userdata.username}";
        nameText.text = "Name: " + userdata.name ;
        surnameText.text = "Surname: " + userdata.surname;
        genderText.text = "Gender: " + userdata.gender;
        bdayText.text = "BirthDay: " + userdata.b_date;
        ageText.text = "Age: " + userdata.age.ToString();
        addressText.text = "Address: " + userdata.address;
        emailText.text = "Email: " + userdata.email;
        profileImage.sprite = GameManager.instance.avatarsList[userdata.icon_id];
    }

    public void ShowProfile()
    {
        GetComponent<Canvas>().enabled = true;
    }

    public void CloseProfile()
    {
        GetComponent<Canvas>().enabled = false;
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
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
