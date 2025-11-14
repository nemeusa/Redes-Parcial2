using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Lava : NetworkBehaviour
{


    private void OnTriggerEnter(Collider other)
    {
        if (!HasStateAuthority) return;

        if (other.TryGetComponent(out EnemyLife enemyHealth))
        {
            enemyHealth.TakeDamage(100);
        }
    }
}
