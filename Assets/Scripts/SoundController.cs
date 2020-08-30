using UnityEngine;
using System.Collections;

public class SoundController : MonoBehaviour 
{
    public enum GameSounds
    {
        SlotHit,
        Fanfare,
        PrizeBounce,
        HandleRelease,
        SlotFailure,
        PrizeChoosed,
    }

    public AudioClip slotHitSound;
    public AudioClip prizeBounceSound;
    public AudioClip fanfareSound;
    public AudioClip handleReleaseSound;
    public AudioClip slotFailureSound;
    public AudioClip prizeChoosedSound;

    public static SoundController instance;

	// Use this for initialization
	void Start () 
    {
        instance = this;
	}
	
	public static void PlaySound(GameSounds sound)
    {
        switch (sound)
	    {
		    case GameSounds.SlotHit:
                instance.GetComponent<AudioSource>().PlayOneShot(instance.slotHitSound);
                break;

            case GameSounds.Fanfare:
                instance.GetComponent<AudioSource>().PlayOneShot(instance.fanfareSound);
                break;

            case GameSounds.PrizeBounce:
                instance.GetComponent<AudioSource>().PlayOneShot(instance.prizeBounceSound);
                break;

            case GameSounds.HandleRelease:
                instance.GetComponent<AudioSource>().PlayOneShot(instance.handleReleaseSound);
                break;

            case GameSounds.SlotFailure:
                instance.GetComponent<AudioSource>().PlayOneShot(instance.slotFailureSound);
                break;

            case GameSounds.PrizeChoosed:
                instance.GetComponent<AudioSource>().PlayOneShot(instance.prizeChoosedSound);
                break;

            default:
                break;
	    }
    }
}