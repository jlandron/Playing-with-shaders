using UnityEngine;

namespace Shooter.Weapons
{
    public class GunController : MonoBehaviour
    {
        [Header("GunController properties")]
        [SerializeField] Transform weaponHoldPosition;
        [SerializeField] Gun startingGun;
        Gun equippedGun;

        private void Start()
        {
            if (startingGun != null)
            {
                EquipGun(startingGun);
            }
        }
        public void EquipGun(Gun gunToEquip)
        {
            if (equippedGun != null)
            {
                Destroy(equippedGun.gameObject);
            }
            equippedGun = Instantiate(gunToEquip, weaponHoldPosition) as Gun;
            equippedGun.transform.parent = weaponHoldPosition;
        }

        internal void Shoot()
        {
            if (equippedGun != null)
            {
                equippedGun.Shoot();
            }
        }
    }
}
