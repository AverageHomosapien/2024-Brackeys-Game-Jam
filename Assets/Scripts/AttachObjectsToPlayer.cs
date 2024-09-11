using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachObjectsToPlayer : MonoBehaviour
{
    //public GameObject objectToAttach;
    private bool isAttached = false;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!isAttached && collision.gameObject.tag == "IsAttachable")
        {
            // Add a Fixed Joint to the object
            FixedJoint joint = collision.gameObject.AddComponent<FixedJoint>();
            // Connect the joint to the object you collided with
            joint.connectedBody = collision.rigidbody;

            isAttached = true;
        }
    }
    
    void OnTriggerEnter(Collider collision)
    {
        if (!isAttached && collision.gameObject.tag == "IsAttachable")
        {
            // Add a Fixed Joint to the object
            FixedJoint joint = collision.gameObject.AddComponent<FixedJoint>();
            // Connect the joint to the object you collided with
            joint.connectedBody = collision.GetComponent<Rigidbody>();

            isAttached = true;
        }
    }
}
