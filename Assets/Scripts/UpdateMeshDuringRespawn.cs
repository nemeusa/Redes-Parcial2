using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateMeshDuringRespawn : MonoBehaviour
{
    [SerializeField] GameObject _mesh;

    private void Awake()
    {
        var health = GetComponent<EnemyLife>();

        health.OnDeadStateUpdate += UpdateMesh;
    }

    void UpdateMesh(bool isDead)
    {
        _mesh.SetActive(!isDead);
    }
}
