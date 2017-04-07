using UnityEngine;

//Add this Script Directly to The Death Zone

public class drum : MonoBehaviour {
    void Start()
    {
        //GetComponent<AudioSource>().playOnAwake = false;
        //GetComponent<AudioSource>().clip = saw;
    }

    void OnTriggerEnter(Collider col)  //Plays Sound Whenever collision detected
    {
        GetComponent<AudioSource>().Play();
    }
    // Make sure that deathzone has a collider, box, or mesh.. ect..,
    // Make sure to turn "off" collider trigger for your deathzone Area;
    // Make sure That anything that collides into deathzone, is rigidbody;
}