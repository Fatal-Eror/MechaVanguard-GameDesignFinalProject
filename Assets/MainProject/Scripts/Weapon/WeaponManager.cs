using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using UnityEngine.InputSystem;

public class WeaponManager : NetworkBehaviour
{
    // Set of player's weapon, main weapon is rifle, sub is pistol
    // Because players prefab contains both red and blue team models, so we need to make sure
    // weapons[0] and [1] is binded to the activated model's rifle and pistol
    // public List<GameObject> weapons = new();
    public WeaponBehaviour _currentGun;

    private void Start()
    {
/*        // only the main weapon should be activated at the start point        
        weapons[0].SetActive(true);

        int index = 0;
        foreach(var weapon in weapons)
        {
            if(index != 0)
            {
                weapon.SetActive(false);
                index++;
            }
        }*/
    }


}
