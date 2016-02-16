using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameLogic : MonoBehaviour {

    private int ballsLeft = 1000;
    private int screenWidth;
    private float ballSpeed;

    //Mini game variables
    private bool miniGame = false;
    private int defcon = 6;
    
    //AudioSource to play audio, plus clips to play at different times
    private AudioSource audioMusic;
    private AudioSource audioSounds;

    private AudioClip backgroundMusic;
    private AudioClip minigameMusic;
    private AudioClip soundYeah;
    private AudioClip soundBetterLuck;
    private AudioClip soundBang;

    //Animations
    private GameObject screen;
    private GameObject leftTray;
    private GameObject rightTray;
    private Vector3 trayOpenPosition = new Vector3(0, 0, -0.13f);
    private Material[] mat;
    bool trayOpen;

    //UI
    public Text textBallsLeft;
    public Text textInfo;
    private string info;

    //Countdown
    float endTime;
    float showCountdown=2;
    int timespan = 20;

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
        info = "PEACE";

        backgroundMusic = Resources.Load<AudioClip>("Sounds/standardMode") as AudioClip;
        minigameMusic = Resources.Load<AudioClip>("Sounds/minigame") as AudioClip;
        soundYeah = Resources.Load<AudioClip>("Sounds/yeah") as AudioClip;
        soundBetterLuck = Resources.Load<AudioClip>("Sounds/betterLuck") as AudioClip;
        soundBang = Resources.Load<AudioClip>("Sounds/bang") as AudioClip;

        mat = new Material[7];
        mat[6] = Resources.Load("Materials/screen_normal", typeof(Material)) as Material;
        mat[5] = Resources.Load("Materials/screen_defcon5", typeof(Material)) as Material;
        mat[4] = Resources.Load("Materials/screen_defcon4", typeof(Material)) as Material;
        mat[3] = Resources.Load("Materials/screen_defcon3", typeof(Material)) as Material;
        mat[2] = Resources.Load("Materials/screen_defcon2", typeof(Material)) as Material;
        mat[1] = Resources.Load("Materials/screen_defcon1", typeof(Material)) as Material;
        mat[0] = Resources.Load("Materials/screen_nuke", typeof(Material)) as Material;

        audioMusic = GetComponents<AudioSource>()[0];
        audioSounds = GetComponents<AudioSource>()[1];
        audioMusic.clip = backgroundMusic;
        audioMusic.loop = true;
        audioMusic.Play();

        // We want the balls to fire every 0.7 seconds, after an initial 2 seconds pause
        InvokeRepeating("LaunchBall", 2, 0.7F);
    }

    void openTrays()
    {
        leftTray.transform.localPosition = Vector3.Lerp(leftTray.transform.localPosition, trayOpenPosition, 2f * Time.deltaTime);
        rightTray.transform.localPosition = Vector3.Lerp(rightTray.transform.localPosition, trayOpenPosition, 2f * Time.deltaTime);
        if (Mathf.Abs(leftTray.transform.localPosition.z - trayOpenPosition.z) < 0.0001)
            trayOpen = true;
    }

    void closeTrays()
    {
        leftTray.transform.localPosition = Vector3.Lerp(leftTray.transform.localPosition, Vector3.zero, 2f * Time.deltaTime);
        rightTray.transform.localPosition = Vector3.Lerp(rightTray.transform.localPosition, Vector3.zero, 2f * Time.deltaTime);
        if (Mathf.Abs(leftTray.transform.localPosition.z) < 0.00001)
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
            screen.GetComponent<Renderer>().material = mat[6];
            textBallsLeft.text = "LOST!";
            info = "PEACE";
        }

        //Update countdown
        int timeLeft = (int) (endTime - Time.time);
        if (timeLeft < 0 && miniGame) Deescalate();

        if (miniGame) { 
            if (showCountdown < 0) { 
                textInfo.text = string.Format("T-{0} S", timeLeft);
                showCountdown -= Time.deltaTime;
                if (showCountdown < -2)
                    showCountdown = 2;
            }
            else { 
                textInfo.text = info;
                showCountdown -= Time.deltaTime;
            }
        }
        else
        {
            textInfo.text = info;
        }
    }

    //Instantiate a new ball and give it a velocity
    void LaunchBall()
    {
        Rigidbody ballInstance;
        ballInstance = Instantiate(ballPrefab, ballLauncher.position, ballLauncher.rotation) as Rigidbody;
        ballInstance.AddForce(ballLauncher.up * (ballSpeed + Random.value * 5));
        ballsLeft--;
        textBallsLeft.text = ballsLeft.ToString("D8");
    }
    
    //Called from CollectLogic script assigned to balls
    public void CentralWell()
    {
        //Give 10 additional balls
        textBallsLeft.text = "+10";
        ballsLeft += 10;

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
            endTime = Time.time + timespan;
            screen.GetComponent<Renderer>().material = mat[defcon];
            audioSounds.PlayOneShot(soundYeah);
            if (defcon == 0)
            {
                textBallsLeft.text = "+1000";
                ballsLeft += 1000;

                miniGame = false;
                audioSounds.PlayOneShot(soundBang);
                StartCoroutine(Nuke());
            }
            else
            {
                info = string.Format("DEFCON {0}", defcon);
                textBallsLeft.text = "+100";
                ballsLeft += 100;
            }
        }
    }

    public void Deescalate()
    {
        audioSounds.PlayOneShot(soundBetterLuck);
        defcon++;
        screen.GetComponent<Renderer>().material = mat[defcon];
        if (defcon != 6) { 
            endTime = Time.time + timespan;
            info = string.Format("DEFCON {0}", defcon);
        }
        else
        {
            StopMiniGame();
        }
    }

    public IEnumerator Nuke()
    {
        info = "YEAH";
        yield return new WaitForSeconds(3);
        StopMiniGame();
    }

    //Start mini game: change background music and screen image
    public void StartMiniGame()
    {
        miniGame = true;
        GetComponent<AudioSource>().clip = minigameMusic;
        GetComponent<AudioSource>().Play();
        defcon = 5;
        endTime = Time.time + timespan;
        screen.GetComponent<Renderer>().material = mat[defcon];
        info = string.Format("DEFCON {0}", defcon);
    }

    //End mini game: background music and screen to default
    public void StopMiniGame()
    {
        GetComponent<AudioSource>().clip = backgroundMusic;
        GetComponent<AudioSource>().Play();
        defcon = 6;
        screen.GetComponent<Renderer>().material = mat[defcon];
        info = "PEACE";
    }
}
