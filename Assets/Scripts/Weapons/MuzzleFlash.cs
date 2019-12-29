using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shooter.Weapons
{
    public class MuzzleFlash : MonoBehaviour
    {
        public GameObject flashHolder;
        [SerializeField] float flashTime = 0.25f;
        [SerializeField] Sprite[] flashSprites;
        [SerializeField] SpriteRenderer spriteRenderer;

        private void Start()
        {
            flashHolder.SetActive(false);
        }
        public void Activate()
        {
            flashHolder.SetActive(true);

            int flashSpriteIndex = Random.Range(0, flashSprites.Length);
            spriteRenderer.sprite = flashSprites[flashSpriteIndex];
            Invoke("Deactivate", flashTime);
        }
        public void Deactivate()
        {
            flashHolder.SetActive(false);
        }
    }
}
