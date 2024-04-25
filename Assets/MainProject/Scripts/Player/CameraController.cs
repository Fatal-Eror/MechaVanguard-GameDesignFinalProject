using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CameraController : NetworkBehaviour
{
    private PlayerControls _input;
    [SerializeField] private float maxPitchAngle = 45;
    [SerializeField] private float minPitchAngle= -45;

    private void Start()
    {
        _input = new PlayerControls();

        if (GameNetwork.condition == LevelCondition.Battle)
        {
            _input.Enable();
        }        
    }

    private void OnEnable()
    {
        if (GameNetwork.condition == LevelCondition.Battle && _input != null)
        {
            _input.Enable();
        }
    }

    private void OnDisable()
    {
        if(_input != null)
        {
            _input.Disable();
        }
    }

    private void LateUpdate()
    {
        FocusOnLocalPlayer();
        UpdatePhotographerRotation();
    }

    private void FocusOnLocalPlayer()
    {
        if(GameState.localPlayer != null)
        {
            transform.position = GameState.localPlayer.transform.position;
        }        
    }

    private float NormalizeAngle(float angle)
    {
        while (angle > 180) angle -= 360;
        while (angle < -180) angle += 360;
        return angle;
    }

    private void UpdatePhotographerRotation()
    {
        Vector2 input = _input.Gameplay.Look.ReadValue<Vector2>();
        Vector3 presentAngle = transform.eulerAngles;

        // Convert angle from 0 - 360 to -180 ~ 180
        presentAngle.x = NormalizeAngle(presentAngle.x);

        float yaw = GameState.YawSensitivity / 250 * input.x + presentAngle.y;
        float pitch = Mathf.Clamp(GameState.PitchSensitivity / 250 * input.y * -1 + presentAngle.x, minPitchAngle, maxPitchAngle);

        transform.eulerAngles = new Vector3(pitch, yaw, presentAngle.z);
    }
}
