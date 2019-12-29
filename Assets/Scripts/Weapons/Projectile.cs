using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shooter.Weapons
{
    public class Projectile : MonoBehaviour
    {
        [Header("Projectile properties")]
        [SerializeField] float speed = 10;
        [SerializeField] float timeAlive = 10f;
        float timeToDie;
        [SerializeField] private LayerMask collisionLayers;
        [SerializeField] float damage = 1;
        [Tooltip("Increase this property if projectiles do not seem to be colliding with fast enemies")]
        [SerializeField] float skinWidth = 0.1f;

        private void OnEnable()
        {
            timeToDie = Time.time + timeAlive;
        }
        void Update()
        {
            CheckTimeAlive();
            float moveDist = speed * Time.deltaTime;
            CheckCollisions(moveDist);
            Move(moveDist);
        }

        private void Move(float moveDist)
        {
            transform.Translate(Vector3.forward * moveDist);
        }

        private void CheckTimeAlive()
        {
            if (Time.time > timeToDie)
            {
                this.gameObject.SetActive(false);
            }
        }

        private void CheckCollisions(float moveDist)
        {
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, moveDist + skinWidth, collisionLayers, QueryTriggerInteraction.Collide))
            {
                OnHitObject(hit.collider, hit.point);
            }
        }
        private void OnHitObject(Collider c, Vector3 hitPoint)
        {
            IDamagable damagable = c.GetComponent<IDamagable>();
            if (damagable != null)
            {
                Debug.Log("Projectile hit damagable!");
                damagable.TakeHit(Damage, hitPoint, transform.forward );
            }

            this.gameObject.SetActive(false);
        }
        //-------properties-------
        public float Speed { get => speed; set => speed = value; }
        public float Damage { get => damage; set => damage = value; }
    }
}
