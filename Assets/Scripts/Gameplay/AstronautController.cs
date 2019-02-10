using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstronautController : MonoBehaviour {


    [HideInInspector]
    public int _astronautIndex = 0;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ball"))
        {
            PlayerController.Instance.CutAstronaut(_astronautIndex);
        }
    }

    public void DestroyObject()
    {
        gameObject.SetActive(false);
    }


}
