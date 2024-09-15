using System.Collections;
using System.Collections.Generic;
using static System.Math;
using UnityEngine;

// TODO: add length of tornado (maybe 60 seconds or something to start?)
// when it ends, gravity needs to reengage and things need to fall off the screen again :D

public class TornadoPhysics : MonoBehaviour
{
    public float forceMultiplier;
    public float internalSideForce;
    public float externalSideForce;

    public ParticleSystem particles;
    public ParticleSystem particles2;

    public float simulatedUpForce;

    const float distanceToExternal = 15f;

    public float gustChancePerTick;
    public float gustMinV;
    public float gustMaxV;
    public float gustMinH;
    public float gustMaxH;
    public float gustNonCentreMultiplier;

    private float gustForceX;
    private float gustForceY;
    private float gustNonCentreX;
    private float gustNonCentreY;

    System.Func<float, float, float, Vector3> TornadoForce;

    private Vector3 GetTornadoCentre() 
    {
        return this.transform.position;
    }

    private Vector3 TornadoForce_Normal(float xComponentSign, float yComponentToCentre, float distanceToCentre)
    {
        float sideForceProportion = Clamp(distanceToCentre / distanceToExternal, 0, 1);
        float horizontalForce = ((sideForceProportion * externalSideForce + (1 - sideForceProportion) * internalSideForce) * forceMultiplier) * (Random.value + 1);
        float verticalForce = simulatedUpForce * forceMultiplier * (Random.value - 0.5f);

        return new Vector3(horizontalForce * xComponentSign, verticalForce + (Clamp(yComponentToCentre, -2, 2)), 0);
    }

    private Vector3 TornadoForce_Stop(float a, float b, float c)
    {
        return new Vector3(0.0f, -9.8f, 0.0f);
    }

    private void TornadoWhoosh(Rigidbody body, Vector3 position)
    {
        Vector3 vecToCentre = this.GetTornadoCentre() - position;
        float distanceToCentre = Vector3.Magnitude(vecToCentre);
        float vecToCentreXComponentSign = vecToCentre.x / Abs(vecToCentre.x);

        Vector3 force = this.TornadoForce(vecToCentreXComponentSign, vecToCentre.y, distanceToCentre);
        Vector3 gustForce = new Vector3(
            this.gustForceX * (vecToCentreXComponentSign + this.gustNonCentreX),
            this.gustForceY * (vecToCentreXComponentSign + this.gustNonCentreY),
            0.0f
        );

        body.AddForce(force, ForceMode.Acceleration);
        body.AddForce(gustForce, ForceMode.Impulse);
        Debug.Log("Whoosing with force: " + force.ToString());
    }

    public void OnStopScoring()
    {
        this.TornadoForce = this.TornadoForce_Stop;
        this.gustChancePerTick = 0.0f;
        this.particles.Stop();
        this.particles2.Stop();
    }

    public void DoGust()
    {
        // Gust physics updates
        this.gustForceX = Mathf.Lerp(gustMinH, gustMaxH, Random.value);
        this.gustForceY = Mathf.Lerp(gustMinV, gustMaxV, Random.value);
        gustNonCentreX = gustNonCentreMultiplier * (Random.value - 0.5f);
        gustNonCentreY = gustNonCentreMultiplier * (Random.value - 0.5f);

        // Animation

    }

    // ======================================
    // ENGINE HOOKS
    // ======================================

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

    public void Start()
    {
        this.TornadoForce = this.TornadoForce_Normal;
    }

    // Update gusts
    public void Update() 
    {
        if (Random.value < gustChancePerTick)
        {
            this.DoGust();
        }
        else 
        {   
            this.gustForceX = 0.0f;
            this.gustForceY = 0.0f;
        }
    }
}
