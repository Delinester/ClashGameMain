using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Controller : NetworkBehaviour
{
    [SerializeField]
    private float speed = 5.0f;
    [SerializeField]
    private TextMeshProUGUI text;
    [SerializeField]
    private GameObject stick;

    private NetworkAnimator netAnimator;

    [SyncVar(hook = nameof(OnColorUpdate))]
    public Color currColor;
    [SyncVar(hook = nameof(OnNameUpdate))]
    public string currName;
    void OnColorUpdate(Color _old, Color _new)
    {
        GetComponent<SpriteRenderer>().color = _new;
    }

    void OnNameUpdate(string _old, string _new)
    {
        text.text = _new;
    }
    // Start is called before the first frame update
    void Awake()
    {
        Application.targetFrameRate = 60;
        netAnimator = stick.GetComponent<NetworkAnimator>();

        
    }
    void Start()
    {
       
    }

    [Command]
    private void changeCharacter(Color color, string name)
    {
        currColor = color;
        currName = name;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
       if (collision.gameObject.tag == "Changer")
        {
            
            //changeCharacter(data.colorChangeColor, data.nameChangeName);
            Debug.Log("Changer detected");
        }
        Debug.Log("Got trigger");
    }

    [Command]
    void playBeatAnim()
    {
        netAnimator.SetTrigger("Beat");
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer) { return; } 
        
        float movementX = Input.GetAxis("Horizontal");
        float movementY = Input.GetAxis("Vertical");

        transform.Translate(new Vector2(movementX * speed * Time.deltaTime, movementY * speed* Time.deltaTime));

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            //playBeatAnim();
            netAnimator.SetTrigger("Beat");
        }
    }
}
