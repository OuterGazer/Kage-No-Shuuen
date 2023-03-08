using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PushCharacterFromLedge : MonoBehaviour
{
    [SerializeField] float raycastingRefreshRate = 2f;
    [SerializeField] float pushingForce = 1f;
    private float checkingDownwardDistance = 0.3f;
    private ControllerColliderHit lastContactPoint;

    private CharacterController charController;

    private void Awake()
    {
        charController = GetComponent<CharacterController>();
    }

    // Start is called before the first frame update
    private IEnumerator Start()
    {
        checkingDownwardDistance = charController.stepOffset;

        while (true)
        {
            bool areFeetOnGround = Physics.Raycast(transform.position, -Vector2.up, checkingDownwardDistance + 0.05f);

            if (!areFeetOnGround && charController.isGrounded) 
            {
                Vector3 voidDirection = transform.position - lastContactPoint.point;

                charController.Move(pushingForce * Time.deltaTime * voidDirection.normalized);
            }

            yield return new WaitForSeconds(1 / raycastingRefreshRate);
        }        
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        lastContactPoint = hit;
    }
}
