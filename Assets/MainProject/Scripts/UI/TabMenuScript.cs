using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.VFX;

public class TabMenuScript : MonoBehaviour
{
    private PlayerControls _input;
    private UIDocument _tabUI;
    private VisualElement _root;
    private List<Label> _redNames = new List<Label>();
    private List<Label> _redKills = new List<Label>();
    private List<Label> _redDeaths = new List<Label>();
    private List<Label> _blueNames = new List<Label>();
    private List<Label> _blueKills = new List<Label>();
    private List<Label> _blueDeaths = new List<Label>();

    private bool _isAbleToShow = false;

    private void Awake()
    {

    }

    private void Start()
    {
        _input = new PlayerControls();
        _tabUI = GetComponent<UIDocument>();
        _root = _tabUI.rootVisualElement;

        _redNames = _root.Query<Label>("RedName").ToList();
        _redKills = _root.Query<Label>("RedKill").ToList();
        _redDeaths = _root.Query<Label>("RedDeath").ToList();
        _blueNames = _root.Query<Label>("BlueName").ToList();
        _blueKills = _root.Query<Label>("BlueKill").ToList();
        _blueDeaths = _root.Query<Label>("BlueDeath").ToList();

        _input.Enable();
        _input.UI.CallTabMenu.started += TabToggle;

        _isAbleToShow = false;

        // print(_redNames.Count + " " + _redKills.Count + " " + _redDeaths.Count);
    }

    private void OnDestroy()
    {
        _input.Disable();
        _input.UI.CallTabMenu.started -= TabToggle;
    }

    private void OnDisable()
    {
       
    }

    private void Update()
    {
        _root.visible = _isAbleToShow;
        _root.style.display = _isAbleToShow ? DisplayStyle.Flex : DisplayStyle.None;
        _root.SetEnabled(_isAbleToShow);
    }

    private void TabToggle(InputAction.CallbackContext obj)
    {
        _isAbleToShow = !_isAbleToShow;

        if (_isAbleToShow)
        {
            PlayerAttributes[] attributes = FindObjectsOfType<PlayerAttributes>();
            int redIndex = 0, blueIndex = 0;

            print(attributes.Length);

            foreach (var player in attributes)
            {
                if (player != null)
                {
                    if (player.team == PlayerTeam.Red)
                    {
                        _redNames[redIndex].text = player.playerName;
                        _redKills[redIndex].MarkDirtyRepaint();
                        _redKills[redIndex].text = player.killNum.ToString();
                        _redDeaths[redIndex].text = player.deathNum.ToString();

                        redIndex++;

                    }
                    if (player.team == PlayerTeam.Blue)
                    {
                        _blueNames[blueIndex].text = player.playerName;
                        _blueKills[blueIndex].text = player.killNum.ToString();
                        _blueDeaths[blueIndex].text = player.deathNum.ToString();
                        print(_blueNames[blueIndex].text + " " + _blueKills[blueIndex].text + " " + _blueDeaths[blueIndex].text + "blue");
                        blueIndex++;
                    }
                }
            }



            for (int index = redIndex; index <= 4; index += 1)
            {
                _redNames[index].text = "---";
                _redKills[index].text = "-";
                _redDeaths[index].text = "-";
            }

            for (int index = blueIndex; index <= 4; index += 1)
            {
                _blueNames[index].text = "---";
                _blueKills[index].text = "-";
                _blueDeaths[index].text = "-";
            }
        }        
    }
}


