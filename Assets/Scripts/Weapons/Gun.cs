using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shooter.Weapons
{
    public class Gun : ObjectPooler
    {
        [Header("Gun Properties")]
        [SerializeField] Transform muzzleLocation;
        [SerializeField] float msBetweenShots = 100f;
        [SerializeField] float muzzleVelocity = 35f;

        float nextShotTime = 0;
        protected override void Start()
        {
            base.Start(); //setup pooled objects
        }

        public void Shoot()
        {
            if (Time.time > nextShotTime)
            {
                nextShotTime = Time.time + msBetweenShots / 1000;
                GameObject bullet = GetPooledObject();
                if(bullet != null)
                {
                    bullet.transform.position = muzzleLocation.transform.position;
                    bullet.transform.rotation = muzzleLocation.transform.rotation;
                    bullet.SetActive(true);
                    try
                    {
                        bullet.GetComponent<Projectile>().Speed = muzzleVelocity;
                    }
                    catch (System.Exception)
                    {
                        Debug.LogError("Object is not a Projectile!");
                    }
                }
            }
        }
    }
}
