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

    public float gustChancePerTick;
    public float gustMinV;
    public float gustMaxV;
    public float gustMinH;
    public float gustMaxH;
    public float gustNonCentreMultiplier;

    private Vector3 GetTornadoCentre() 
    {
        return this.transform.position;
    }

    private Vector3 TornadoForce(float xComponentSign, float yComponentToCentre, float distanceToCentre)
    {
        // why can I not import using "using System.Math"? who tf knows
        float sideForceProportion = System.Math.Clamp(distanceToCentre / distanceToExternal, 0, 1);
        float horizontalForce = ((sideForceProportion * externalSideForce + (1 - sideForceProportion) * internalSideForce) * forceMultiplier) * (UnityEngine.Random.value + 1);
        float verticalForce = simulatedUpForce * forceMultiplier * (UnityEngine.Random.value - 0.5f);

        return new Vector3(horizontalForce * xComponentSign, verticalForce + (System.Math.Clamp(yComponentToCentre, -2, 2)), 0);
    }

    private Vector3 GustForce(float xComponentSign)
    {
        if (UnityEngine.Random.value < gustChancePerTick)
        {
            Vector3 gustForce = new Vector3(
                (Mathf.Lerp(gustMinH, gustMaxH, UnityEngine.Random.value) * (xComponentSign + gustNonCentreMultiplier * (UnityEngine.Random.value - 0.5f))),
                (Mathf.Lerp(gustMinV, gustMaxV, UnityEngine.Random.value) * (xComponentSign + gustNonCentreMultiplier * (UnityEngine.Random.value - 0.5f))),
                0.0f
            );

            return gustForce;
        }
        else 
        {   
            return new Vector3(0.0f, 0.0f, 0.0f);
        }
    }

    private void TornadoWhoosh(Rigidbody body, Vector3 position)
    {
        Vector3 vecToCentre = this.GetTornadoCentre() - position;
        float distanceToCentre = Vector3.Magnitude(vecToCentre);
        float vecToCentreXComponentSign = vecToCentre.x / System.Math.Abs(vecToCentre.x);

        Vector3 force = this.TornadoForce(vecToCentreXComponentSign, vecToCentre.y, distanceToCentre);
        Vector3 gustForce = this.GustForce(vecToCentreXComponentSign);

        body.AddForce(force, ForceMode.Acceleration);
        body.AddForce(gustForce, ForceMode.Impulse);
        // Debug.Log("Whoosing with force: " + force.ToString());
    }

    // Engine hook
    public void OnTriggerStay(Collider collider)
    {
        if(collider.gameObject.tag == "Whooshable")
        {
            this.TornadoWhoosh(collider.attachedRigidbody, collider.gameObject.transform.position);
        }
    }

    public void OnTriggerExit(Collider collider)
    {
        Destroy(collider.gameObject);
    }
}
