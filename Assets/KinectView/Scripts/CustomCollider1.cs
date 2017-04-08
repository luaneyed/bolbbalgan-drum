using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCollider1 : MonoBehaviour {
    private bool tmp = false;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(transform.position.y < -1)
        {
            if (!tmp) {
                GameObject.Find("Cube").GetComponent<AudioSource>().Play();
                tmp = true;
            }
        } else {
            tmp = false;
        }
	}
}
