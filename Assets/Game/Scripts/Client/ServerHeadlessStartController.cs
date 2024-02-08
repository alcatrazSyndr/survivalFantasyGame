using FishNet.Managing.Server;
using FishNet.Transporting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ServerHeadlessStartController : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private bool _startHeadless = true;
    [SerializeField] private string _serverConfigFileName = "server-config.txt";

    [Header("Component References")]
    [SerializeField] private Transport _serverTransport;
    [SerializeField] private ServerManager _serverManager;

    [Header("Runtime")]
    [SerializeField] private string[] _serverConfig;
    [SerializeField] private string _serverIP;
    [SerializeField] private ushort _serverPort;

    private void Start()
    {
        if (_startHeadless)
        {
            var serverConfigFilePath = Application.dataPath + "/" + _serverConfigFileName; // -- EDITOR
            //var serverConfigFilePath = _serverConfigFileName; // -- BUILD
            _serverConfig = File.ReadAllLines(serverConfigFilePath);

            bool validIP = false;
            bool validPort = false;

            for (int i = 0; i < _serverConfig.Length; i++)
            {
                var serverData = _serverConfig[i].Split(":");
                if (serverData[0] == "ServerIP")
                {
                    var ip = serverData[1];
                    if (!string.IsNullOrEmpty(ip))
                    {
                        validIP = true;
                        _serverIP = ip;
                    }
                }
                else if (serverData[0] == "ServerPort")
                {
                    var port = serverData[1];
                    if (ushort.TryParse(port, out ushort result))
                    {
                        validPort = true;
                        _serverPort = result;
                    }
                }
            }

            if (validIP && validPort)
            {
                _serverTransport.SetServerBindAddress(_serverIP, IPAddressType.IPv4);
                _serverTransport.SetPort(_serverPort);

                _serverManager.StartConnection();
            }

            //_serverManager.StartConnection();
        }
    }
}
