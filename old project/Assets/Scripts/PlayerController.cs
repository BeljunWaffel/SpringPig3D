using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float _speed;

    private Rigidbody player;

    void Start()
    {
        player = GetComponent<Rigidbody>();
        //player.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    // Applied before physics
    void FixedUpdate()
    {
        float moveHorizortal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizortal, 0.0f, moveVertical);

        player.AddForce(movement * _speed);
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
