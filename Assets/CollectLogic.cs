using UnityEngine;
using System.Collections;

public class CollectLogic : MonoBehaviour {

    private GameLogic logic;

	// Use this for initialization
	void Start () {
        logic = GameObject.FindGameObjectWithTag("GameLogic").GetComponent<GameLogic>();
    }
	
	// Update is called once per frame
	void Update () {}

    void OnTriggerEnter(Collider other)
    {
        if (other.name == "BallOutlet")
        {
            logic.BallLost();
        }
        else if(other.name == "CentralWell")
        {
            logic.CentralWell();
        }
        else
        {
            Debug.Log(other.name);
        }

        Destroy(this.gameObject);
    }
}
