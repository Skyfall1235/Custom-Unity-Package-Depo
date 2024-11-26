using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static Radar;
using static RadarNotifier;

public class Radar : MonoBehaviour
{
    //unity event to declare a hit
    public UnityEvent<RadarHitCompact> RadarPing = new();
    private List<GameObject> foundRadarTargets = new();

    //struct of data about hit
#nullable enable
    /// <summary>
    /// Class representing a radar hit with detailed information about the object hit and location
    /// </summary>
    public class RadarHit
    {
        public GameObject ObjectHit;  // The object that was hit by the radar (reference type, could be null)
        public Vector3 HitLocation;   // The 3D coordinates of the hit location in the world
        public float LocalAzimuth;    // The azimuth angle (direction) at which the radar hit occurred
        public RadarSignature signature;  // An optional radar signature that may describe the target's properties

        /// <summary>
        /// Constructor to initialize a RadarHit with the specified properties
        /// </summary>
        /// <param name="objectHit">the object that was hit</param>
        /// <param name="hitLocation">the hit location in 3D space</param>
        /// <param name="localAzimuth">the azimuth angle</param>
        /// <param name="signature">the radar signature</param>
        public RadarHit(GameObject objectHit, Vector3 hitLocation, float localAzimuth, RadarSignature signature)
        {
            ObjectHit = objectHit; 
            HitLocation = hitLocation;
            LocalAzimuth = localAzimuth;
            this.signature = signature;
        }

        /// <summary>
        /// Method to check equality between two RadarHit objects
        /// </summary>
        /// <param name="other"></param>
        /// <returns>if the other object is equal this one</returns>
        public bool Equals(RadarHit other)
        {
            // If the other object is null, they are not equal
            if (other == null)
                return false;

            // Compare the properties of the two RadarHit objects for equality
            return ObjectHit == other.ObjectHit &&          // Check if the GameObjects are the same (reference comparison)
                   HitLocation == other.HitLocation &&    // Compare the hit locations (Vector3 equality)
                   LocalAzimuth.Equals(other.LocalAzimuth) &&  // Compare the azimuth angles (float equality)
                   signature.Equals(other.signature);     // Compare the radar signature (nullable equality)
        }
    }

    /// <summary>
    /// Struct representing a more compact form of radar hit information
    /// </summary>
    public struct RadarHitCompact : IEquatable<RadarHitCompact>
    {
        public Vector3 HitLocation;   // The 3D coordinates of the hit location in the world
        public float LocalAzimuth;    // The azimuth angle (direction) at which the radar hit occurred
        public RadarSignature signature;  // An optional radar signature (nullable type)

        // Constructor to initialize a RadarHitCompact with the specified properties
        public RadarHitCompact(Vector3 hitLocation, float localAzimuth, RadarSignature signature)
        {
            HitLocation = hitLocation;   // Set the hit location in 3D space
            LocalAzimuth = localAzimuth; // Set the azimuth angle
            this.signature = signature;  // Set the radar signature
        }

        /// <summary>
        /// Method to check equality between two RadarHitCompact objects
        /// </summary>
        /// <param name="other">The other RadarHitCompact struct</param>
        /// <returns>true if these structs are the same, false if not</returns>
        public bool Equals(RadarHitCompact other)
        {
            // Compare the properties of the two RadarHitCompact structs for equality
            return HitLocation == other.HitLocation &&        // Compare the hit locations (Vector3 equality)
                   LocalAzimuth.Equals(other.LocalAzimuth) &&  // Compare the azimuth angles (float equality)
                   signature.Equals(other.signature);         // Compare the radar signature (nullable equality)
        }

        /// <summary>
        /// Explicit conversion from RadarHit to RadarHitCompact
        /// </summary>
        /// <param name="hit">incoming Hit from the radar notifier</param>
        public static explicit operator RadarHitCompact(RadarHit hit)
        {
            // Create and return a new RadarHitCompact using the properties of RadarHit
            // ObjectHit is ignored here because RadarHitCompact doesn't have a corresponding field
            return new RadarHitCompact(hit.HitLocation, hit.LocalAzimuth, hit.signature);
        }
    }




    //NOT DONE YET
    /*
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     * 
    internal RadarHit CreateRadarPing(CompactPing ping)
    {
        return null;
    }

    internal virtual void InvokeEvents(RadarHit hit)
    {
        //RadarPing.Invoke(hit);

    }

    //sweeping math

    //math to determine where on radar hit point is

    internal bool CheckIfTargetIsAlreadyTracked()
    {
        return false;
    }
    */


}


public abstract class RadarSignature { }


public class RadarNotifier : MonoBehaviour
{
    public UnityEvent<CompactPing> RadarPing = new();

    //collision stuff for collider
    private void OnCollisionEnter(Collision collision)
    {
        CompactPing ping;
        Vector3 hitPoint = Vector3.zero;
        foreach (ContactPoint contact in collision.contacts)
        {
            // Each contact has a point where the collision happened
            hitPoint = contact.point;
            break;
        }
        ping = new CompactPing(hitPoint, collision.gameObject);
        RadarPing.Invoke(ping);
    }
    public struct CompactPing : IEquatable<CompactPing>
    {
        public Vector3 hitPoint;
        public GameObject hitObject;

        public CompactPing(Vector3 hitPoint, GameObject hitObject)
        {
            this.hitPoint = hitPoint;
            this.hitObject = hitObject;
        }

        public bool Equals(CompactPing other)
        {
            return other is CompactPing ping &&
                   hitPoint.Equals(ping.hitPoint) &&
                   EqualityComparer<GameObject>.Default.Equals(hitObject, ping.hitObject);
        }
    }
}
