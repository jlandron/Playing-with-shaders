using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Shooter.Weapons
{
    [RequireComponent(typeof(MuzzleFlash))]
    public class Gun : ObjectPooler
    {
        
        [SerializeField] Transform muzzleLocation;
        [SerializeField] Transform ejectionPort;
        MuzzleFlash muzzleFlash;
        [Header("Gun Properties")]
        [SerializeField] float msBetweenShots = 100f;
        [SerializeField] float muzzleVelocity = 35f;
        

        float nextShotTime = 0;
        protected override void Start()
        {
            base.Start(); //setup pooled objects
            muzzleFlash = GetComponent<MuzzleFlash>();
        }

        public void Shoot()
        {
            if (Time.time > nextShotTime)
            {
                nextShotTime = Time.time + msBetweenShots / 1000;
                GameObject bullet = GetPooledObject(0);
                if(bullet != null)
                {
                    bullet.transform.position = muzzleLocation.transform.position;
                    bullet.transform.rotation = muzzleLocation.transform.rotation;
                    bullet.SetActive(true);
                    muzzleFlash.Activate();
                    try
                    {
                        bullet.GetComponent<Projectile>().Speed = muzzleVelocity;
                    }
                    catch (System.Exception)
                    {
                        Debug.LogError("Object is not a Projectile!");
                    }
                }
                GameObject shell = GetPooledObject(1);
                if (shell != null)
                {
                    shell.transform.position = ejectionPort.transform.position;
                    shell.transform.rotation = ejectionPort.transform.rotation;
                    shell.SetActive(true);
                }
            }
        }
    }
}
