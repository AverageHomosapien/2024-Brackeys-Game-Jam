using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using Unity.VisualScripting;

public class DragDropItemScript : MonoBehaviour 
{
    // The plane the object is currently being dragged on, with respect to the camera
    private Plane dragPlane;
    // The difference between where the mouse is on the drag plane and 
    // where the origin of the object is on the drag plane
    private Vector3 offset;
    private Camera myMainCamera; 
    private float LastSetZ;
    private bool isDragging = false;

    void Start()
    {
        myMainCamera = Camera.main; // Camera.main is expensive - cache it here
        LastSetZ = 0;
    }

    private void Update()
    {
        // Fixing issue where multiple joined items are rotating
        //transform.SetPositionAndRotation(transform.position, new Quaternion(transform.rotation.x, transform.rotation.y, LastSetZ, transform.rotation.w));

        if (isDragging && Input.GetKey(KeyCode.E))
        {
            LastSetZ = Math.Min(1, LastSetZ + 0.003f);
            transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.w, LastSetZ, transform.rotation.w);
        }
        else if (isDragging && Input.GetKey(KeyCode.Q))
        {
            LastSetZ = Math.Max(-1, LastSetZ - 0.003f);
            transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.w, LastSetZ, transform.rotation.w); 
        }
    }

    void OnMouseDown()
    {
        isDragging = true;
        dragPlane = new Plane(myMainCamera.transform.forward, transform.position); 
        Ray camRay = myMainCamera.ScreenPointToRay(Input.mousePosition);

        dragPlane.Raycast(camRay, out float planeDist);
        offset = transform.position - camRay.GetPoint(planeDist);

        // Remove object parents
        var parents = gameObject.GetComponentsInParent<Transform>();
        for (int i = 0; i < parents.Length; i++)
        {
            gameObject.transform.SetParent(parents[i], false);
        }

        // Destroy joint if it exists
        if (gameObject.TryGetComponent(out FixedJoint specificJoint))
        {
            // Destroy the specific joint
            Destroy(specificJoint);
        }
    }

    void OnMouseDrag()
    {   
        Ray camRay = myMainCamera.ScreenPointToRay(Input.mousePosition);

        dragPlane.Raycast(camRay, out float planeDist);
        transform.position = camRay.GetPoint(planeDist) + offset;
    }

    void OnMouseUp()
    {
        // Check if the object being dragged is touching any other objects
        Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, 1f);

        var colliderChildren = gameObject.GetComponentsInChildren<Transform>();
        foreach (Collider collider in colliders.Where(c => c.gameObject != gameObject && !colliderChildren.Contains(c.transform)))
        {
            // If touching another object, create a joint between them
            //FixedJoint joint = gameObject.AddComponent<FixedJoint>();
            //joint.connectedBody = collider.attachedRigidbody;
            //joint.breakForce = 500f;
            //joint.breakTorque = 500f;

            gameObject.transform.SetParent(collider.transform, false);

            break; // We only need one joint, break out of the loop
        }
        isDragging = false;
    }
}