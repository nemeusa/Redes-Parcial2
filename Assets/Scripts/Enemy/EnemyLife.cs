using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLife : NetworkBehaviour
{
    [SerializeField] byte _maxLife;
    [Networked, OnChangedRender(nameof(LifeChanged))] byte CurrentLife { get; set; }

    [SerializeField] int _currentSpawns = 3;
    [SerializeField] float _respawnTime = 2.5f;

    TickTimer _respawnTimer;

    [Networked, OnChangedRender(nameof(DeadStateChanged))]
    NetworkBool IsDead { get; set; }

    public event Action<bool> OnDeadStateUpdate;

    public override void Spawned()
    {
        CurrentLife = _maxLife;
    }

    void DeadStateChanged()
    {
        GetComponentInParent<HitboxRoot>().HitboxRootActive = !IsDead;
        OnDeadStateUpdate?.Invoke(IsDead);
    }

    void LifeChanged()
    {
        //Actualizar barras de vida
    }

    public void TakeDamage(byte damage)
    {
        if (IsDead) return;

        if (CurrentLife < damage)
        {
            damage = CurrentLife;
        }

        CurrentLife -= damage;

        if (CurrentLife != 0) return;

        _currentSpawns--;

        ColorManager.Instance.AddBlock(this);

        if (_currentSpawns == 0)
        {
            GameManager.Instance.RPC_GameOver();
            DisconnectObject();
            return;
        }

        _respawnTimer = TickTimer.CreateFromSeconds(Runner, _respawnTime);

        Debug.Log("muerto xd");
        IsDead = true;
    }

    public override void FixedUpdateNetwork()
    {
        if (_respawnTimer.Expired(Runner))
        {
            Debug.Log("procesando");
            _respawnTimer = TickTimer.None;
            CurrentLife = _maxLife;
            IsDead = false;
            OnDeadStateUpdate?.Invoke(IsDead);
            Debug.Log("se reseteo");
        }
    }

    void DisconnectObject()
    {
        if (!HasInputAuthority)
        {
            Runner.Disconnect(Object.InputAuthority);
        }

        Runner.Despawn(Object);
    }
}
