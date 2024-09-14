using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

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
    private List<GameObject> objectsJoinedToCurrentObject = new();

    void Start()
    {
        gameObject.GetComponent<Rigidbody>().isKinematic = false;
        myMainCamera = Camera.main;
        LastSetZ = 0;
    }

    private void Update()
    {
        var rigidBody = GetComponent<Rigidbody>();
        rigidBody.velocity = Vector3.zero;

        if (isDragging && Input.GetKey(KeyCode.E))
        {
            LastSetZ = Math.Min(1, LastSetZ + 0.001f);
            transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.w, LastSetZ, transform.rotation.w);
        }
        else if (isDragging && Input.GetKey(KeyCode.Q))
        {
            LastSetZ = Math.Max(-1, LastSetZ - 0.001f);
            transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.w, LastSetZ, transform.rotation.w); 
        }
        else
        {
            rigidBody.rotation = Quaternion.identity;
        }
    }

    void OnMouseDown()
    {
        isDragging = true;
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
        var rigidBody = GetComponent<Rigidbody>();
        Ray camRay = myMainCamera.ScreenPointToRay(Input.mousePosition);

        dragPlane.Raycast(camRay, out float planeDist);
        transform.position = camRay.GetPoint(planeDist) + offset;

        rigidBody.velocity = Vector3.zero;
        if (!Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.E))
        { 
            rigidBody.rotation = Quaternion.identity;
        }
        var joints = gameObject.GetComponents<FixedJoint>();

        Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, 5f);
        foreach (var collider in colliders.Where(c => c.gameObject.GetComponents<FixedJoint>().Any(fj => fj.connectedBody == gameObject.transform)))
        {
            collider.transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }

    void OnMouseUp()
    {
        // Check if the object being dragged is touching any other objects
        Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, 1f);

        // Create only a single child
        Collider collider = colliders.FirstOrDefault(c => c.gameObject != gameObject
                                                       && (!c.gameObject.GetComponents<FixedJoint>().Any()
                                                       || !c.gameObject.GetComponents<FixedJoint>().Any(fj => fj.transform == gameObject.transform)));

        if (collider != null)
        {
            // If touching another object, create a joint between them
            FixedJoint joint = gameObject.AddComponent<FixedJoint>();
            joint.connectedBody = collider.attachedRigidbody;
            joint.breakForce = float.PositiveInfinity;
            joint.breakTorque = float.PositiveInfinity;
        }

        isDragging = false;
    }

    public void AttachObjectToCurrent(GameObject gameObject)
    {
        objectsJoinedToCurrentObject.Add(gameObject);
    }
}