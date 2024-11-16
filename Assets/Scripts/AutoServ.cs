using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using kcp2k;

public class AutoServ : MonoBehaviour
{
    // Start is called before the first frame update
    private NetworkManager manager;
    private KcpTransport kcp;
    private void Awake()
    {
        manager = GetComponent<NetworkManager>();
        kcp  = GetComponent<KcpTransport>();
    }
    void Start()
    {
        manager.networkAddress = "0.0.0.0";
        kcp.port = 4567;
        manager.StartServer();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
