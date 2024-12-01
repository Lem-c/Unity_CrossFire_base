using Unity.VisualScripting;
using UnityEngine;

public class PrimeAudioManager : WeaponAudioManager
{
    [SerializeField] private AudioClip fireDynamic;
    [SerializeField] private AudioClip fireDynamic_2;
    [SerializeField] private AudioClip reloadGas;

    public override void PlayFireSound()
    {
        // Generate a random number between 0 and 100
        int chance = Random.Range(0, 100);

        if (chance < 5) // 20% chance to play `fireDynamic`
        {
            PlaySound(fireDynamic);
        }
        else if (chance < 15) // Another 20% chance to play `fireDynamic2`
        {
            PlaySound(fireDynamic_2);
        }
        else // Default case: 60% chance to play the base fire sound
        {
            base.PlayFireSound();
        }
    }

    public void PlayGasEjectSound()
    {
        PlaySound(reloadGas);
    }
}
