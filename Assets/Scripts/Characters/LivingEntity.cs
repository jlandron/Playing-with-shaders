using UnityEngine;

namespace Shooter.Characters
{
    public class LivingEntity : MonoBehaviour, IDamagable
    {
        [Header("Living Entity properties")]
        [SerializeField] float startingHealth = 3;
        [SerializeField] protected float currentHealth;
        [SerializeField] protected bool dead = false;
        [SerializeField] protected bool tookDamage = false;
        [SerializeField] float memoryTime = 5;
        [SerializeField] float timeDamageWillBeForgotten;

        public event System.Action OnDeath;
        protected virtual void Start()
        {
            currentHealth = startingHealth;
        }
        protected virtual void Update()
        {
            if (tookDamage && Time.time > timeDamageWillBeForgotten)
            {
                tookDamage = false;
            }
        }
        public virtual void TakeHit(float damageToTake, Vector3 hitPoint, Vector3 hitDirection)
        {
            TakeDamage(damageToTake);
        }
        public virtual void TakeDamage(float damageToTake)
        {
            print(gameObject.name + " took " + damageToTake + " damage.");
            currentHealth -= damageToTake;
            if (currentHealth <= 0 && !dead)
            {
                Die();
            }
            tookDamage = true;
            timeDamageWillBeForgotten = Time.time + memoryTime;
        }

        [ContextMenu("Self Destruct")]
        private void Die()
        {
            if (!dead)
            {
                dead = true;
                if (OnDeath != null)
                {
                    OnDeath();
                }

                Destroy(gameObject);


            }
        }
        public void Revive()
        {
            dead = false;
            gameObject.SetActive(true);
        }
    }
}