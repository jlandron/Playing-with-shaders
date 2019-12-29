using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable
{
    void TakeHit(float damageToTake, Vector3 hitPoint, Vector3 hitDirection);
    void TakeDamage(float damageToTake);
}
