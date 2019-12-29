using Shooter.Weapons;
using System;
using UnityEngine;

namespace Shooter.Characters
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(GunController))]
    public class Player : LivingEntity
    {
        [Header("Player properties")]
        [SerializeField] float moveSpeed = 5f;
        Rigidbody rigidbody;
        GunController gunController;
        Camera camera;
        Vector3 velocity;
        protected override void Start()
        {
            base.Start();
            rigidbody = GetComponent<Rigidbody>();
            gunController = GetComponent<GunController>();
            camera = Camera.main;
        }

        protected override void Update()
        {
            base.Update();
            CheckMovement();
            CheckWeaponInput();
        }
        private void CheckWeaponInput()
        {
            if (Input.GetMouseButton(0))
            {
                gunController.Shoot();
            }
        }

        private void CheckMovement()
        {
            Vector3 mousePos = camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, camera.transform.position.y));
            transform.LookAt(mousePos + Vector3.up * transform.position.y);
            velocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized * moveSpeed;
        }

        private void FixedUpdate()
        {
            
            rigidbody.MovePosition(rigidbody.position + velocity * Time.fixedDeltaTime);
        }
    }
}