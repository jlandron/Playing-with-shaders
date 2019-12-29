using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    [Header("Shell Properties")]
    [SerializeField] Rigidbody rigidbody;
    [SerializeField] Vector2 ejectionForceRange;
    [SerializeField] float lifeTime = 4f;
    void OnEnable()
    {
        float ejectionForce = Random.Range(ejectionForceRange.x, ejectionForceRange.y);
        rigidbody.AddForce(transform.right * ejectionForce);
        rigidbody.AddTorque(Random.insideUnitSphere * ejectionForce);
        StartCoroutine(Cull());
    }

    IEnumerator Cull()
    {
        yield return new WaitForSeconds(lifeTime);
        this.gameObject.SetActive(false);
    }

}
