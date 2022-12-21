using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectBinder : MonoBehaviour
{
    [SerializeField] Transform objectToBindTo;
    [SerializeField] Vector3 localPositionOffset;
    [SerializeField] Quaternion localRotationOffset;

    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (rb)
        {
            rb.MovePosition(objectToBindTo.transform.position + objectToBindTo.transform.TransformDirection(localPositionOffset));
            rb.MoveRotation(objectToBindTo.transform.rotation * localRotationOffset);
        }

    }

    private void LateUpdate()
    {
        // To have the weapon stay ut in the hand, we could take out the if statement
        // However, that could give us problems if we want to use physics with rigidbody
        // An alternative is to fix the binded object to the hand with an IK
        // Another alternative is to move the rigidbody/collider in FixedUpdate and the visual mesh in LateUpdate
        // Third alternative, check if the weapons is Kinematic. Works when the collision is done through multiple raycasts.
        //      Need to turn dynamic again if we want to throw/drop the weapon
        if (!rb || rb.isKinematic) 
        {
            transform.position = objectToBindTo.transform.position + objectToBindTo.transform.TransformDirection(localPositionOffset);
            transform.rotation = objectToBindTo.transform.rotation * localRotationOffset;
        }
        
    }

    //private void BindObject(Vector3 objectToBindToPosition, Vector3 objectPosition,
    //                        Quaternion objectToBindToRotation, Quaternion objectRotation)
    //{
    //    if (rb)
    //    {
    //        rb.MovePosition(objectToBindTo.transform.position + objectToBindTo.transform.TransformDirection(localPositionOffset));
    //        rb.MoveRotation(objectToBindTo.transform.rotation * localRotationOffset);
    //    }
    //    else
    //    {
    //        transform.position = objectToBindToPosition + objectPosition;
    //        transform.rotation = objectToBindToRotation * objectRotation;
    //    }
    //}
}
