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
    private int defcon = 6;
    
    //AudioSource to play audio, plus clips to play at different times
    private AudioSource audio;
    private AudioClip backgroundMusic;
    private AudioClip minigameMusic;

    //Animations
    private GameObject screen;
    private GameObject leftTray;
    private GameObject rightTray;
    private Vector3 trayOpenPosition = new Vector3(0, 0, -0.1f);
    private Material[] mat;

    bool trayOpen;

    //UI
    public Text textBallsLeft;
    public Text textBallsGone;

    public GameObject controlWheel;
    public Transform ballLauncher;
    public Rigidbody ballPrefab;

    // Use this for initialization
    void Start () {
        screenWidth = Screen.width;

        screen = GameObject.Find("Large_Screen");
        leftTray = GameObject.Find("Left_Tray");
        rightTray = GameObject.Find("Right_Tray");

        textBallsLeft.text = ballsLeft.ToString("D8");
        textBallsGone.text = ballsLost.ToString("D8");

        backgroundMusic = Resources.Load<AudioClip>("Sounds/standardMode") as AudioClip;
        minigameMusic = Resources.Load<AudioClip>("Sounds/minigame") as AudioClip;

        mat = new Material[7];
        mat[6] = Resources.Load("Materials/screen_normal", typeof(Material)) as Material;
        mat[5] = Resources.Load("Materials/screen_defcon5", typeof(Material)) as Material;
        mat[4] = Resources.Load("Materials/screen_defcon4", typeof(Material)) as Material;
        mat[3] = Resources.Load("Materials/screen_defcon3", typeof(Material)) as Material;
        mat[2] = Resources.Load("Materials/screen_defcon2", typeof(Material)) as Material;
        mat[1] = Resources.Load("Materials/screen_defcon1", typeof(Material)) as Material;
        mat[0] = Resources.Load("Materials/screen_nuke", typeof(Material)) as Material;

        audio = GetComponent<AudioSource>();
        audio.clip = backgroundMusic;
        audio.loop = true;
        audio.Play();

        // We want the balls to fire every 0.7 seconds, after an initial 2 seconds pause
        InvokeRepeating("LaunchBall", 2, 0.7F);
    }

    void openTrays()
    {
        leftTray.transform.localPosition = Vector3.Lerp(leftTray.transform.localPosition, trayOpenPosition, 0.5f * Time.deltaTime);
        rightTray.transform.localPosition = Vector3.Lerp(rightTray.transform.localPosition, trayOpenPosition, 0.5f * Time.deltaTime);
        if ((leftTray.transform.localPosition.z - trayOpenPosition.z) < 0.0001)
            trayOpen = true;
    }

    void closeTrays()
    {
        leftTray.transform.localPosition = Vector3.Lerp(leftTray.transform.localPosition, Vector3.zero, 0.5f * Time.deltaTime);
        rightTray.transform.localPosition = Vector3.Lerp(rightTray.transform.localPosition, Vector3.zero, 0.5f * Time.deltaTime);
        if (leftTray.transform.localPosition.z < 0.0001)
            trayOpen = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Open or close trays if mini game is triggered
        if(miniGame && !trayOpen)
            openTrays();
        else if(!miniGame && trayOpen)
            closeTrays();

        //Determine control wheel angle depending on mouse x position, between 0 and 90 degrees
        ballSpeed = Input.mousePosition.x / screenWidth;
        controlWheel.transform.eulerAngles = new Vector3(0, 0, -Mathf.Floor(90 * ballSpeed));
        ballSpeed = 2200 + 500 * ballSpeed;

        //End game when no balls are left
        if (ballsLeft <= 0)
        {
            CancelInvoke("LaunchBall");
            //Game done ...
        }
    }

    //Instantiate a new ball and give it a velocity
    void LaunchBall()
    {
        Rigidbody ballInstance;
        ballInstance = Instantiate(ballPrefab, ballLauncher.position, ballLauncher.rotation) as Rigidbody;
        ballInstance.AddForce(ballLauncher.up * ballSpeed);

        ballsLeft--;
        textBallsLeft.text = ballsLeft.ToString("D8");
    }

    //Called from CollectLogic script assigned to balls
    public void BallLost()
    {
        ballsLost++;
        textBallsGone.text = ballsLost.ToString("D8");
    }

    //Called from CollectLogic script assigned to balls
    public void CentralWell()
    {
        //Launch the mini game if not yet running
        if (!miniGame)
        {
            StartMiniGame();
        }

    }

    public void ExtraWell()
    {
        if (miniGame)
        {
            defcon--;
            screen.GetComponent<Renderer>().material = mat[defcon];
            if (defcon == 0)
            {
                //yield return new WaitForSeconds(3);
                StopMiniGame();
            }
        }
    }

    //Start mini game: change background music and screen image
    public void StartMiniGame()
    {
        miniGame = true;
        audio.clip = minigameMusic;
        audio.Play();
        defcon = 5;
        screen.GetComponent<Renderer>().material = mat[defcon];
    }

    //End mini game: background music and screen to default
    public void StopMiniGame()
    {
        miniGame = false;
        audio.clip = backgroundMusic;
        audio.Play();
        defcon = 6;
        screen.GetComponent<Renderer>().material = mat[defcon];
    }
}
