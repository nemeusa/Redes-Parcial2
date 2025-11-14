using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : NetworkBehaviour
{
    EnemyMovement _movement;
    EnemyWeapon _weapon;

    [SerializeField] Material _material;


    private void Awake()
    {
        _movement = GetComponent<EnemyMovement>();
        _weapon = GetComponent<EnemyWeapon>();
    }

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            var healthSystem = GetComponent<EnemyLife>();

            healthSystem.OnDeadStateUpdate += (isDead) =>
            {
                enabled = !isDead;

                if (!isDead)
                {
                    _movement.Teleport(transform.position + Vector3.up * 3);
                }
            };
            GameManager.Instance.AddPlayer(this);

        }

    }

    public override void FixedUpdateNetwork()
    {
        if (!GetInput(out NetworkInputData inputData)) return;

        _movement.Move(Vector3.right * inputData.xAxi);

        if (inputData.buttons.IsSet(PlayerButtons.Jump))
        {
            _movement.Jump();
        }

        if (inputData.buttons.IsSet(PlayerButtons.Shot))
        {
            _weapon.BulletShot();
            //_weapon.RaycastShot();
        }
    }

    [Rpc]
    public void RPC_SetTeam(Color color)
    {
        _material.color = color;
        _weapon.SetColor(color);
    }
}
