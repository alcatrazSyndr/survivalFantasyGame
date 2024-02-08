using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using FishNet.Transporting;
using FishNet.Managing.Client;

public class ClientConnectionController : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private string _defaultConnectButtonText = "Connect";
    [SerializeField] private string _failedConnectButtonText = "Failed";
    [SerializeField] private string _connectingConnectButtonText = "Connecting";
    [SerializeField] private string _connectingConnectButtonTextSuffix = ".";
    [SerializeField] private int _connectingConnectButtonTextSuffixCount = 3;
    [SerializeField] private float _connectingConnectButtonTextSuffixAddageDelay = 0.5f;

    [Header("Component References")]
    [SerializeField] private Transport _clientTransport;
    [SerializeField] private ClientManager _clientManager;
    [SerializeField] private GameObject _connectionPanelGameObject;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI _connectButtonText;
    [SerializeField] private TMP_InputField _serverIPAndPortInputField;
    [SerializeField] private Button _connectButton;

    [Header("Runtime")]
    [SerializeField] private string _serverIP = string.Empty;
    private bool _serverIPValid
    {
        get
        {
            return _serverIP != string.Empty;
        }
    }
    [SerializeField] private ushort _serverPort = 0;
    private bool _serverPortValid
    {
        get
        {
            return _serverPort != 0;
        }
    }
    private IEnumerator _connectingCRT = null;
    [SerializeField] private int _connectingConnectButtonTextSuffixCountCurrent = 1;

    private void Awake()
    {
        _serverIPAndPortInputField.onValueChanged.AddListener(OnPlayerNameAndIPPortInputFieldChanged);

        _connectButton.onClick.AddListener(OnConnectButtonClicked);

        _clientManager.OnClientConnectionState += OnClientConnectionStateChanged;
    }

    private void OnClientConnectionStateChanged(ClientConnectionStateArgs arg)
    {
        if (arg.ConnectionState == LocalConnectionState.Starting)
        {
            if (_connectingCRT != null)
            {
                StopCoroutine(_connectingCRT);
                _connectingCRT = null;
            }

            _connectingCRT = ConnectingCRT();

            StartCoroutine(_connectingCRT);
        }
        else if (arg.ConnectionState == LocalConnectionState.Stopping)
        {
            if (_connectingCRT != null)
            {
                StopCoroutine(_connectingCRT);
                _connectingCRT = null;
            }

            _connectButtonText.text = _failedConnectButtonText;
        }
        else if (arg.ConnectionState == LocalConnectionState.Stopped)
        {
            if (_connectingCRT != null)
            {
                StopCoroutine(_connectingCRT);
                _connectingCRT = null;
            }

            _serverIPAndPortInputField.interactable = true;
            _connectButton.interactable = true;

            _serverIPAndPortInputField.text = string.Empty;

            _connectButtonText.text = _defaultConnectButtonText;

            OnPlayerNameAndIPPortInputFieldChanged(string.Empty);

            _connectionPanelGameObject.SetActive(true);
        }
        else if (arg.ConnectionState == LocalConnectionState.Started)
        {
            if (_connectingCRT != null)
            {
                StopCoroutine(_connectingCRT);
                _connectingCRT = null;
            }

            _serverIPAndPortInputField.interactable = false;
            _connectButton.interactable = false;

            _serverIPAndPortInputField.text = string.Empty;

            _connectButtonText.text = _defaultConnectButtonText;

            OnPlayerNameAndIPPortInputFieldChanged(string.Empty);

            _connectionPanelGameObject.SetActive(false);
        }
    }

    private void OnConnectButtonClicked()
    {
        if (_serverIPValid && _serverPortValid)
        {
            _serverIPAndPortInputField.interactable = false;
            _connectButton.interactable = false;

            AttemptConnection();
        }
    }

    private void AttemptConnection()
    {
        _clientTransport.SetClientAddress(_serverIP);
        _clientTransport.SetPort(_serverPort);

        _clientManager.StartConnection();
    }

    private IEnumerator ConnectingCRT()
    {
        _connectingConnectButtonTextSuffixCountCurrent = 1;
        _connectButtonText.text = _connectingConnectButtonText + _connectingConnectButtonTextSuffix;
        while (true)
        {
            yield return new WaitForSeconds(_connectingConnectButtonTextSuffixAddageDelay);
            if (_connectingConnectButtonTextSuffixCountCurrent >= _connectingConnectButtonTextSuffixCount)
            {
                _connectingConnectButtonTextSuffixCountCurrent = 0;
            }
            _connectingConnectButtonTextSuffixCountCurrent += 1;
            var text = _connectingConnectButtonText;
            for (int i = 0; i < _connectingConnectButtonTextSuffixCountCurrent; i++)
            {
                text += _connectingConnectButtonTextSuffix;
            }
            _connectButtonText.text = text;
            yield return null;
        }
    }

    private void OnPlayerNameAndIPPortInputFieldChanged(string input)
    {
        var serverIPAndPortStringValue = _serverIPAndPortInputField.text;
        var serverIPAndPortStringArray = serverIPAndPortStringValue.Split(":");
        if (serverIPAndPortStringArray.Length != 2)
        {
            _serverIP = string.Empty;
            _serverPort = 0;
        }
        else
        {
            if (ushort.TryParse(serverIPAndPortStringArray[1], out ushort result))
            {
                _serverIP = serverIPAndPortStringArray[0];
                _serverPort = result;
            }
            else
            {
                _serverIP = string.Empty;
                _serverPort = 0;
            }
        }

        _connectButton.interactable = (_serverIPValid && _serverPortValid);
    }
}
