using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkedPlayerController : NetworkBehaviour
{
    [Header("Component References - Local")]
    [SerializeField] private ClientInputController _playerInputController;

    [Header("Prefabs - Networked")]
    [SerializeField] private GameObject _playerCharacterPrefab;

    [Header("Runtime - Networked")]
    [SerializeField] private NetworkedPlayerCharacterController _playerCharacterController = null;

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (!base.IsOwner)
        {
            return;
        }

        _playerInputController.ToggleMovementInput(true);
        _playerInputController.ToggleCameraInput(true);
    }

    public override void OnStopClient()
    {
        base.OnStopClient();

        if (!base.IsOwner)
        {
            return;
        }

        _playerInputController.ToggleMovementInput(false);
        _playerInputController.ToggleCameraInput(false);
    }
}
