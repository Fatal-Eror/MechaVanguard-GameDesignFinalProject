using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuState : MonoBehaviour
{
    private void Start()
    {
        GameNetwork.condition = LevelCondition.Login;
    }
}
