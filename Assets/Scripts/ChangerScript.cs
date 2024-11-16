using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ChangerScript : MonoBehaviour
{

    public Color colorChangeColor;
    public string nameChangeName;

    void Awake()
    {
        GetComponent<SpriteRenderer>().color = colorChangeColor;

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
