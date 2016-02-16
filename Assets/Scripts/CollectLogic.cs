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
        if (other.name == "well_outlet")
        {
            logic.BallLost();
        }
        else if(other.name == "well_central")
        {
            logic.CentralWell();
        }
        else
        {
            logic.ExtraWell();
        }

        Destroy(this.gameObject, 3.0f);
    }

}
