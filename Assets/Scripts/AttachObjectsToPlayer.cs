using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachObjectsToPlayer : MonoBehaviour
{
    public GameObject objectToAttach;
    private bool isAttached = false;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isAttached && collision.gameObject.tag == "IsAttachable")
        {
            // Add a Fixed Joint to the object
            FixedJoint2D joint = objectToAttach.AddComponent<FixedJoint2D>();
            // Connect the joint to the object you collided with
            joint.connectedBody = collision.rigidbody;

            isAttached = true;
        }
    }
}
