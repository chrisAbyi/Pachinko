using UnityEngine;
using System.Collections;

public class CollectLogic : MonoBehaviour {

    private GameLogic logic;

    private AudioSource audioSounds;
    private AudioClip soundExplosion;
    private AudioClip soundClick;
    private AudioClip soundBonus;

    // Use this for initialization
    void Start () {
        logic = GameObject.FindGameObjectWithTag("GameLogic").GetComponent<GameLogic>();
        audioSounds = GetComponent<AudioSource>();
        soundClick = Resources.Load<AudioClip>("Sounds/click") as AudioClip;
        soundExplosion = Resources.Load<AudioClip>("Sounds/explosion") as AudioClip;
        soundBonus = Resources.Load<AudioClip>("Sounds/bonus") as AudioClip;
    }

    // Update is called once per frame
    void Update () {}

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.name == "pin")
        {
            audioSounds.PlayOneShot(soundClick);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.name == "well_central")
        {
            audioSounds.PlayOneShot(soundBonus);
            logic.CentralWell();
        }
        else if (other.name == "well_left" || other.name == "well_right")
        {
            audioSounds.PlayOneShot(soundBonus);
            logic.ExtraWell();
        }

        audioSounds.PlayOneShot(soundExplosion);
        Destroy(this.gameObject, 10.0f);
    }

}
