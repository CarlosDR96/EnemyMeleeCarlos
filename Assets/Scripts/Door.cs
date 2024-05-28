using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private Transform previousRoom;
    [SerializeField] private Transform nextRoom;

    [SerializeField] private CameraController cam;
    // Start is called before the first frame update

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Hello ");
        if (other.tag == "Player")
        {
            Debug.Log("Hello Player");
            if (other.transform.position.x < transform.position.x)
            {
                cam.MoveToNewRoom(nextRoom);
            }
            else
            {
                cam.MoveToNewRoom(previousRoom);
            }
        }
    }
}
