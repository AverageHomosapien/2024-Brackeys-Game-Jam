using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TornadoPhysics : MonoBehaviour
{
    public float forceMultiplier;
    public float internalSideForce;
    public float externalSideForce;

    public float simulatedUpForce;

    const float distanceToExternal = 15f; 

    private Vector3 GetTornadoCentre() 
    {
        return this.transform.position;
    }

    private Vector3 TornadoForce(Vector3 position)
    {
        float dist = Vector3.Distance(position, this.GetTornadoCentre());
        // why can I not import using "using System.Math"? who tf knows
        float sideForceProportion = System.Math.Clamp(dist / distanceToExternal, 0, 1);
        float horizontalForce = ((sideForceProportion * externalSideForce + (1 - sideForceProportion) * internalSideForce) * forceMultiplier) * (UnityEngine.Random.value + 1);
        float verticalForce = simulatedUpForce * forceMultiplier * (UnityEngine.Random.value - 0.5f);

        Vector3 towardsCentre = this.GetTornadoCentre() - position;       

        return new Vector3(horizontalForce * System.Math.Clamp(towardsCentre.x, -1, 1), verticalForce + (System.Math.Clamp(towardsCentre.y, -2, 2)), 0);
    }

    private void TornadoWhoosh(Rigidbody body, Vector3 position)
    {
        Vector3 force = TornadoForce(position);
        body.AddForce(force);
        Debug.Log("Whoosing with force: " + force.ToString());
    }

    // Engine hook
    public void OnCollisionStay(Collision collisionInfo)
    {
        if(collisionInfo.gameObject.tag == "Whooshable")
        {
            this.TornadoWhoosh(collisionInfo.rigidbody, collisionInfo.gameObject.transform.position);
            Debug.Log("Whooshable detected");
        }
        Debug.Log("Collision Detected");
    }
}
