using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class BattleUIScript : MonoBehaviour
{
    private VisualElement _root;
    private Label _countDown;
    private Label _playerName;
    private ProgressBar _hp;

    [Tooltip("How many time for a battle")]
    [SerializeField]private float totalTime = 300;
    private float _currentTime;

    // Start is called before the first frame update
    void Start()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;
        _countDown = _root.Q<Label>("Timer");
        _playerName = _root.Q<Label>("Name");
        _hp = _root.Q<ProgressBar>("HP");

        _playerName.text = GameState.localPlayerName;
        _currentTime = totalTime;

        StartCoroutine(CountDown());
    }

    private void Update()
    {
        if(GameState.localPlayer != null)
        {
            _hp.value = GameState.localPlayer.healthPoint;
        }       
    }

    private IEnumerator CountDown()
    {
        int min = (int)_currentTime / 60;
        int sec = (int)_currentTime % 60;

        while(true)
        {
            min = (int)_currentTime / 60;
            sec = (int)_currentTime % 60;
            _countDown.text = $"{min:D2}:{sec:D2}";
            _currentTime--;
            yield return new WaitForSeconds(1);

            if (_currentTime == 0)
            {
                break;
            }            
        }

        SwitchLevel();
    }

    [Server]
    private void SwitchLevel()
    {
        NetworkManager.singleton.ServerChangeScene("Lobby");
    }
}
