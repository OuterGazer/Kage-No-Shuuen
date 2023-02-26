using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushCharacterFromLedge : MonoBehaviour
{
    [SerializeField] float raycastingRefreshRate = 2f;
    [SerializeField] float pushingDistance = 1f;
    private ControllerColliderHit lastContactPoint;

    private CharacterController charController;

    void Awake()
    {
        charController = GetComponent<CharacterController>();
    }

    // Start is called before the first frame update
    private IEnumerator Start()
    {
        while (true)
        {
            bool areFeetOnGround = Physics.Raycast(transform.position, -Vector2.up, 0.1f);

            if (!areFeetOnGround && charController.isGrounded) 
            {
                Vector3 voidDirection = transform.position - lastContactPoint.point;

                charController.Move(pushingDistance * Time.deltaTime * voidDirection.normalized);
            }

            yield return new WaitForSeconds(1 / raycastingRefreshRate);
        }        
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        lastContactPoint = hit;
    }
}
