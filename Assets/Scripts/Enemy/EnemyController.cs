using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class EnemyController : NetworkBehaviour
{
    EnemyMovement _movement;
    WeaponArm _weapon;

    [SerializeField] List<GameObject> _materials;

    Vector3 initialPost;

    private bool _doublejumped;


    private void Awake()
    {
        _movement = GetComponent<EnemyMovement>();
        _weapon = GetComponentInChildren<WeaponArm>();
    }

    public override void Spawned()
    {
        initialPost = transform.position;

        if (HasStateAuthority)
        {
            var healthSystem = GetComponent<EnemyLife>();

            healthSystem.OnDeadStateUpdate += (isDead) =>
            {
                enabled = !isDead;

                if (!isDead)
                {
                    _movement.Teleport(initialPost);
                    Debug.Log("volvio");
                }
            };
        }

        GameManager.Instance.AddPlayer(this);

    }

    public override void FixedUpdateNetwork()
    {
        if (!GetInput(out NetworkInputData inputData)) return;

        _movement.Move(Vector3.right * inputData.xAxi);

        if (inputData.buttons.IsSet(PlayerButtons.Jump))
        {
            if (_movement.Grounded == true)
            {
                _movement.Jump(false, 15);
            }
            else if (_doublejumped == false)
            {
                //EnemyMovement.moveVelocity.y = 0f;
                _movement.Jump(true, 15);
                _doublejumped = true;
            }
            
        }

        _weapon.RPC_Rotate(inputData.direction);

        //if (inputData.buttons.IsSet(PlayerButtons.Shot))
        if (GameManager.Instance.myPlayers.Count < 2) return;

        if (inputData.isFirePressed)
        {
            _weapon.Shoot();
            //_weapon.RaycastShot();
        }


        if (_movement.Grounded == true)
        {
            _doublejumped = false;
        }
    }

    [Rpc]
    public void RPC_SetTeam(Color color)
    {
        foreach (var mat in _materials)
            mat.GetComponent<Renderer>().material.color = color;
        _weapon.SetColor(color);
    }
}
