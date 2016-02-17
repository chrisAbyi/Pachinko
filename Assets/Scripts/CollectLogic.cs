using UnityEngine;
using System.Collections;

public class CollectLogic : MonoBehaviour {

    private GameLogic logic;
    private AudioSource audioSounds;
    private AudioClip soundClick;
    private AudioClip soundBonus;
    private AudioClip soundExplosion;

    public GameObject explosion;

    // Use this for initialization
    void Start () {
        logic = GameObject.FindGameObjectWithTag("GameLogic").GetComponent<GameLogic>();
        audioSounds = GetComponent<AudioSource>();
        soundClick = Resources.Load<AudioClip>("Sounds/click") as AudioClip;
        soundBonus = Resources.Load<AudioClip>("Sounds/bonus") as AudioClip;
        soundExplosion = Resources.Load<AudioClip>("Sounds/explosion") as AudioClip;
    }

    // Update is called once per frame
    void Update () {}

    void OnCollisionEnter(Collision collision)
    {
         if (collision.collider.name == "pin")
            audioSounds.PlayOneShot(soundClick);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.name == "well_central")
        {
            audioSounds.PlayOneShot(soundBonus);
            logic.CentralWell();
        }
        else if (other.name == "well_left")
        {
            gameObject.tag = "BallInLeftWell";
            audioSounds.PlayOneShot(soundBonus);
            logic.ExtraWell();
        }
        else if (other.name == "well_right"){
            gameObject.tag = "BallInRightWell";
            audioSounds.PlayOneShot(soundBonus);
            logic.ExtraWell();
        }
        StartCoroutine(Destroy(5));
    }
    
    public IEnumerator Destroy(int sec)
    {
        yield return new WaitForSeconds(sec);
        GameObject newexplosion = (GameObject)Instantiate(explosion, transform.position, transform.rotation);
        Destroy(newexplosion.gameObject, 1);
        //audioSounds.PlayOneShot(soundExplosion);
        Destroy(this.gameObject);
    }

}
