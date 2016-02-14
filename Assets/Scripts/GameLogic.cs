using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameLogic : MonoBehaviour {

    private int ballsLeft = 1000;
    private int ballsLost = 0;
    private int ballsWell1 = 0;
    private int ballsWell2 = 0;
    private int ballsWell3 = 0;
    private int screenWidth;
    private float ballSpeed;

    //Mini game variables
    private bool miniGame = false;
    
    //AudioSource to play audio, plus clips to play at different times
    private AudioSource audio;
    private AudioClip backgroundMusic;
    private AudioClip minigameMusic;

    //Animations
    private Animation animAdditionalWells;

    //UI
    public Text textBallsLeft;
    public Text textBallsGone;

    public GameObject controlWheel;
    public Transform ballLauncher;
    public Rigidbody ballPrefab;

    // Use this for initialization
    void Start () {
        screenWidth = Screen.width;

        textBallsLeft.text = ballsLeft.ToString("D8");
        textBallsGone.text = ballsLost.ToString("D8");

        backgroundMusic = Resources.Load<AudioClip>("Sounds/standardMode") as AudioClip;
        minigameMusic = Resources.Load<AudioClip>("Sounds/minigame") as AudioClip;

        //animAdditionalWells = Resources.Load<Animation>("Animation/animAdditionalWells") as Animation;
        //animAdditionalWells.Play();

        audio = GetComponent<AudioSource>();
        audio.clip = backgroundMusic;
        audio.loop = true;
        audio.Play();

        // We want the balls to fire every 0.7 seconds, after an initial 2 seconds pause
        InvokeRepeating("LaunchBall", 2, 0.7F);
    }

    // Update is called once per frame
    void Update()
    {
        //Angle between 0 and 270 degrees
        ballSpeed = Input.mousePosition.x / screenWidth;
        controlWheel.transform.eulerAngles = new Vector3(0, 0, -Mathf.Floor(90 * ballSpeed));
        Debug.Log(controlWheel.transform.eulerAngles);
        ballSpeed = 2200 + 500 * ballSpeed;

        if (ballsLeft <= 0)
        {
            CancelInvoke("LaunchBall");
            //Game done ...
        }
    }

    void LaunchBall()
    {
        Rigidbody ballInstance;
        ballInstance = Instantiate(ballPrefab, ballLauncher.position, ballLauncher.rotation) as Rigidbody;
        ballInstance.AddForce(ballLauncher.up * ballSpeed);

        ballsLeft--;
        textBallsLeft.text = ballsLeft.ToString("D8");
    }

    public void BallLost()
    {
        ballsLost++;
        textBallsGone.text = ballsLost.ToString("D8");
    }

    public void CentralWell()
    {
        //Increase points?
        if (!miniGame)
        {
            StartMiniGame();
        }

    }

    public void StartMiniGame()
    {
        miniGame = true;

        audio.clip = minigameMusic;
        audio.Play();
    }

    public void StopMiniGame()
    {
        miniGame = false;

        audio.clip = backgroundMusic;
        audio.Play();
    }
}
