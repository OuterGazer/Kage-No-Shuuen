using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Sight : MonoBehaviour
{
    public Collider[] interestingTargets;
    [SerializeField] Vector3 extents = new Vector3(10f, 30f, 10f);
    [SerializeField] Transform sightPoint;
    [SerializeField] float angle = 120f;
    [SerializeField] float refreshFrequency = 10.0f; // Times per second
    [SerializeField] LayerMask interestingTargetsLayers = Physics.DefaultRaycastLayers;
    [SerializeField] LayerMask occludingLayers = Physics.DefaultRaycastLayers;
    [SerializeField] string[] interestingTags;

    private void OnEnable()
    {
        StartCoroutine(UpdateSight());
    }

    IEnumerator UpdateSight()
    {
        while (true)
        {

            Collider[] collidersInRange = Physics.OverlapBox(sightPoint.position + sightPoint.forward * extents.z / 2f,
                                                             extents / 2f,
                                                             sightPoint.rotation,
                                                             interestingTargetsLayers);

            List<Collider> interestingTargetsList = new List<Collider>();

            foreach (Collider item in collidersInRange)
            {
                if (IsInterestingTargetWithinVisionAngle(item) &&
                    interestingTags.Contains(item.tag) &&
                    IsTargetOccludedByAnObstacle(item))
                {
                    interestingTargetsList.Add(item);
                }
            }

            interestingTargets = interestingTargetsList.ToArray();

            yield return new WaitForSeconds(1f / refreshFrequency);
        }
    }

    private bool IsInterestingTargetWithinVisionAngle(Collider item)
    {
        return Vector3.Angle(sightPoint.forward, (item.transform.position - sightPoint.position)) < angle / 2f;
    }

    private bool IsTargetOccludedByAnObstacle(Collider item)
    {
        return !Physics.Linecast(sightPoint.position, item.transform.position, occludingLayers);
    }
}
