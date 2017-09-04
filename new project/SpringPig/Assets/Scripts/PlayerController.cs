using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    public float _movementMultiplier;
    public float _jumpMultiplier;

    private Rigidbody player;

    void Start()
    {
        player = GetComponent<Rigidbody>();
    }

    // Applied before physics
    void FixedUpdate()
    {
        float moveHorizortal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        float jump = Input.GetAxis("Jump");

        Vector3 movement = new Vector3(moveHorizortal   *   _movementMultiplier, 
                                       jump             *   _jumpMultiplier, 
                                       moveVertical     *   _movementMultiplier);

        player.AddForce(movement);
    }

    //void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.CompareTag("Pick Up"))
    //    {
    //        other.gameObject.SetActive(false);
    //        Destroy(other.gameObject);
    //    }
    //}
}
