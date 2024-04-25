using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPacket : NetworkBehaviour
{
    [SerializeField] private ParticleSystem _healthTips;
    [SerializeField] private ParticleSystem _healingEffect;
    [SerializeField] private float coolDownTime = 20;
    [SerializeField] private float healValue = 100;
    private BoxCollider _collider;

    private void Start()
    {
        _collider= GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("RedPlayer") || other.CompareTag("BluePlayer"))
        {
            print("enter");
            PlayerAttributes att = other.GetComponent<PlayerAttributes>();
            if(att.healthPoint >= att.maxHealPoint)
            {
                return;
            }

            CallHealOnServer(att);


        }
    }

    [Server]
    private void CallHealOnServer(PlayerAttributes att)
    {
        print("heal on server");
        att.GetDamage(new PlayerAttributes(), -healValue);
        CallAllClientsStartCoolDown();
    }

    [ClientRpc]
    private void CallAllClientsStartCoolDown()
    {
        print("start coroutine on clients");
        StartCoroutine(HealingCoroutine());
    }

    private IEnumerator HealingCoroutine()
    {
        _healthTips.Stop();
        _healingEffect.Play();
        _collider.enabled = false;

        yield return new WaitForSeconds(coolDownTime);

        _healthTips.Play();
        _collider.enabled = true;
    }
}
