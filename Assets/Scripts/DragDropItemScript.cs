using UnityEngine;
using System.Collections;
using System;

public class DragDropItemScript : MonoBehaviour 
{
    // The plane the object is currently being dragged on, with respect to the camera
    private Plane dragPlane;
    // The difference between where the mouse is on the drag plane and 
    // where the origin of the object is on the drag plane
    private Vector3 offset;
    private Camera myMainCamera; 

    void Start()
    {
        myMainCamera = Camera.main; // Camera.main is expensive - cache it here
    }

    private void Update()
    {
        // Fixing issue where multiple joined items are rotating
        transform.SetPositionAndRotation(transform.position, new Quaternion(transform.rotation.x, transform.rotation.y, 0, transform.rotation.w));
    }

    void OnMouseDown()
    {
        dragPlane = new Plane(myMainCamera.transform.forward, transform.position); 
        Ray camRay = myMainCamera.ScreenPointToRay(Input.mousePosition);

        dragPlane.Raycast(camRay, out float planeDist);
        offset = transform.position - camRay.GetPoint(planeDist);

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

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject != gameObject)
            {
                // If touching another object, create a joint between them
                FixedJoint joint = gameObject.AddComponent<FixedJoint>();
                joint.connectedBody = collider.attachedRigidbody;

                // Optionally set joint parameters like break force, break torque, etc.
                joint.breakForce = 500f;
                joint.breakTorque = 500f;

                break; // We only need one joint, break out of the loop
            }
        }
    }
}