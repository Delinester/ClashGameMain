using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoadingCanvas : MonoBehaviour
{
    public static LoadingCanvas instance;
    // Start is called before the first frame update
    public TextMeshProUGUI loadingText;
    public GameObject refreshIconObject;

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
        gameObject.SetActive(false);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        refreshIconObject.transform.Rotate(0, 0, 60 * Time.deltaTime);
    }
}
