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
    private bool isDragging = false;
    private List<GameObject> objectsJoinedToCurrentObject = new();
    private Vector3 leftRotate = new Vector3(0, 0, 0.5f);
    private Vector3 rightRotate = new Vector3(0, 0, -0.5f);

    void Start()
    {
        SceneManagerScript.OnSceneSwitch += OnSceneSwitchTriggered;
        //gameObject.GetComponent<Rigidbody>().isKinematic = false;
        myMainCamera = Camera.main;
        var rigidBody = GetComponent<Rigidbody>();
        rigidBody.isKinematic = false;
    }

    private void OnSceneSwitchTriggered(string obj)
    {
        if (obj == "Tornado Lift Scene")
        {
            gameObject.GetComponent<Rigidbody>().isKinematic = false;
        }
    }

    private void OnEnable()
    {
        var rigidBody = GetComponent<Rigidbody>();
        rigidBody.isKinematic = true;
    }

    private void Update()
    {
        //var rigidBody = GetComponent<Rigidbody>();
        //rigidBody.velocity = Vector3.zero;

        if (isDragging && Input.GetKey(KeyCode.E))
        {
            transform.Rotate(rightRotate);
        }
        else if (isDragging && Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(leftRotate);
        }

        //if (!isDragging)
        //{
        //    transform.Rotate(Vector3.zero);// = new Quaternion(x: startingRotation.x, y: startingRotation.y, z: transform.rotation.z, w: startingRotation.w);
        //}
    }

    void OnMouseDown()
    {
        isDragging = true;
        dragPlane = new Plane(myMainCamera.transform.forward, transform.position); 
        Ray camRay = myMainCamera.ScreenPointToRay(Input.mousePosition);

        dragPlane.Raycast(camRay, out float planeDist);
        offset = transform.position - camRay.GetPoint(planeDist);

        var fixedJoints = gameObject.GetComponents<FixedJoint>();

        // Destory linked joints when player lifts mouse
        for (int i = 0; i < fixedJoints.Length; i++)
        {
            Destroy(fixedJoints[i]);
        }
    }

    void OnMouseDrag()
    {   
        Ray camRay = myMainCamera.ScreenPointToRay(Input.mousePosition);

        dragPlane.Raycast(camRay, out float planeDist);
        transform.position = camRay.GetPoint(planeDist) + offset;

        //Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, 5f);
        //foreach (var collider in colliders.Where(c => c.gameObject.GetComponents<FixedJoint>().Any(fj => fj.connectedBody == gameObject.transform)))
        //{
        //    collider.transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
        //}
    }

    void OnMouseUp()
    {
        isDragging = false;

        // Check if the object being dragged is touching any other objects
        Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, 0.8f);

        // If in range, first try to add the joint to the player object
        Collider playerCollider = colliders.FirstOrDefault(c => c.gameObject.GetComponent<PlayerScript>());
        if (playerCollider != null)
        {
            AddJointToCollider(playerCollider);
            return;
        }

        // Fallback and try and create a joint on another collider that's not the original object and that doesn't already have a joint to this object (don't want joint to joint)
        Collider collider = colliders.FirstOrDefault(c => c.gameObject != gameObject
                                                       && (!c.gameObject.GetComponents<FixedJoint>().Any()
                                                       || !c.gameObject.GetComponents<FixedJoint>().Any(fj => fj.transform == gameObject.transform)));
        if (collider != null)
        {
            AddJointToCollider(collider);
        }
    }

    private void AddJointToCollider(Collider otherObjectToAttachTo)
    {
        // If touching another object, create a joint between them
        FixedJoint joint = gameObject.AddComponent<FixedJoint>();
        joint.connectedBody = otherObjectToAttachTo.attachedRigidbody;
        joint.breakForce = 100f;
        joint.breakTorque = 100f;

        DontDestroyOnLoad(gameObject);
    }

    public void AttachObjectToCurrent(GameObject gameObject)
    {
        objectsJoinedToCurrentObject.Add(gameObject);
    }
}