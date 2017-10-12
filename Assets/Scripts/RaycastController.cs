using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class RaycastController : MonoBehaviour
{

    public LayerMask collisionMask;

    public const float skinWidth = .015f;
    const float dstBetweenRays = .15f;
    [HideInInspector]
    public int horizontalRayCount;
    [HideInInspector]
    public int verticalRayCount;

    [HideInInspector]
    public float horizontalRaySpacing;
    [HideInInspector]
    public float verticalRaySpacing;

    [HideInInspector]
    public BoxCollider2D collider;
    public RaycastOrigins raycastOrigins;

    // script should be assign to character which will emit raycasts
    // require BoxCollider2D to be present on the object

    // assign collider to gameobject, calculate ray spacing

    public virtual void Awake()
    {
        collider = GetComponent<BoxCollider2D>();
    }



    public virtual void Start()
    {
        CalculateRaySpacing();
    }

    public void UpdateRaycastOrigins()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);


        // assign raycastOrigins corners locations
        // max to get largest vector component
        // min to get smallest
        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    // calculate rays and its spacing
    public void CalculateRaySpacing()
    {
        Bounds bounds = collider.bounds; // get collider measurement
        bounds.Expand(skinWidth * -2); // expand its size by skinWidth * 2, then turn value into negative to shrink

        float boundsWidth = bounds.size.x;
        float boundsHeight = bounds.size.y;

        horizontalRayCount = Mathf.RoundToInt(boundsHeight / dstBetweenRays); // we use mathf class to turn our floats into ints
        verticalRayCount = Mathf.RoundToInt(boundsWidth / dstBetweenRays); // depending on size we get our raycast count

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1); // divide raycast count evenly across the surface
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }

    public struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }
}