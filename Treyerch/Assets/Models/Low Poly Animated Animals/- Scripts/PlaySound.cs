using UnityEngine;
using Sirenix.OdinInspector;

namespace LowPolyAnimalPack
{
    public class PlaySound : MonoBehaviour
    {
        public Animator animator;

        [BoxGroup("animalSound", false)]
        public AudioObject animalSound;

        [BoxGroup("walking", false)]
        public AudioObject walking;

        [BoxGroup("running", false)]
        public AudioObject running;

        [BoxGroup("attacking", false)]
        public AudioObject attacking;

        [SerializeField]
        private AudioClip eating;
        [SerializeField]
        private AudioClip death;
        [SerializeField]
        private AudioClip sleeping;

        void AnimalSound()
        {
            if (animalSound.audioClip)
            {
                AudioManager.PlaySound(animalSound.audioClip, transform.position, animalSound.volume, animalSound.pitch);
            }
        }

        void Walking()
        {
            if (walking.audioClip)
            {
                if(animator.GetFloat(walking.animatorVariable) >= 1)
                AudioManager.PlaySound(walking.audioClip, transform.position, walking.volume, animator.GetFloat(walking.animatorVariable)+ walking.floatOffset);
            }
        }

        void Eating()
        {
            if (eating)
            {
                AudioManager.PlaySound(eating, transform.position, 1, 1);
            }
        }

        void Running()
        {
            if (running.audioClip)
            {
                if (animator.GetFloat(running.animatorVariable) >= 1)
                    AudioManager.PlaySound(running.audioClip, transform.position, running.volume, animator.GetFloat(running.animatorVariable)+ running.floatOffset);
            }
        }

        public void Attacking()
        {
            if (attacking.audioClip)
            {
                AudioManager.PlaySound(attacking.audioClip, transform.position, attacking.volume, Random.Range(attacking.pitchRange.x, attacking.pitchRange.y));
            }
        }

        void Death()
        {
            if (death)
            {
                AudioManager.PlaySound(death, transform.position, 1, 1);
            }
        }

        void Sleeping()
        {
            if (sleeping)
            {
                AudioManager.PlaySound(sleeping, transform.position, 1, 1);
            }
        }
    }

    [System.Serializable]
    public class AudioObject
    {
        public AudioClip audioClip;
        public float volume;
        public PitchMode pitchMode = PitchMode.Random;

        [ShowIf("@pitchMode == PitchMode.Static")]
        public float pitch;
        [ShowIf("@pitchMode == PitchMode.Random")]
        public Vector2 pitchRange;
        [ShowIf("@pitchMode == PitchMode.AnimatorFloat")]
        public string animatorVariable;
        [ShowIf("@pitchMode == PitchMode.AnimatorFloat")]
        public float floatOffset = 0;

        public enum PitchMode { Static, Random, AnimatorFloat }
    }
}