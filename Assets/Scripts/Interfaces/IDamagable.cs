using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable
{
    void TakeHit(float damageToTake, RaycastHit hit);
    void TakeDamage(float damageToTake);
}
