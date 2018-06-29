using UnityEngine;
using System.Collections.Generic;

namespace VarLab.Teleport
{
    /// <summary>
    /// The <see cref="Bezier"/> class contains logic to calculate values
    /// for the Quadratic Bezier curve.
    /// <para/>
    /// Created by: Hassank
    /// Credit to: Hassank
    /// Link to: https://github.com/FusedVR/GearTeleporter
    /// <para/>
    /// Modified by: Austin Che
    /// Modified on: June 28, 2018
    /// </summary>
    [RequireComponent(typeof(LineRenderer))]
    public class Bezier : MonoBehaviour
    {
        public bool endPointDetected;

        /// <summary>
        /// The endpoint vector of the bezier curve.
        /// </summary>
        public Vector3 EndPoint
        {
            get { return endpoint; }
        }

        public float ExtensionFactor
        {
            set { extensionFactor = value; }
        }

        /// <summary>
        /// Modify the forward projection to extend forward distance of bezier curve.
        /// </summary>
        [Tooltip("Increase value to increase distance of bezier curve.")]
        public float forwardProjectionExtend;

        private Vector3 endpoint;
        private float extensionFactor;
        private Vector3[] controlPoints;
        private LineRenderer lineRenderer;
        private float extendStep;
        private int SEGMENT_COUNT = 50;


        void Start()
        {
            // initialize the bezier control points (p0, p1, p2)
            controlPoints = new Vector3[3];
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.enabled = false;
            extendStep = 5f;
            extensionFactor = 0.1f;

            forwardProjectionExtend = 0.0f;
        }

        void Update()
        {
            UpdateControlPoints();
            HandleExtension();
            DrawCurve();
        }

        public void ToggleDraw(bool draw)
        {
            lineRenderer.enabled = draw;
        }

        void HandleExtension()
        {
            if (extensionFactor == 0f)
                return;

            float finalExtension = extendStep + Time.deltaTime * extensionFactor * 2f;
            extendStep = Mathf.Clamp(finalExtension, 2.5f, 7.5f);
        }

        /// <summary>
        /// First control point is the Oculus remote. Second is a foward project. The third is a forward and downward projection.
        /// </summary>
        void UpdateControlPoints()
        {
            controlPoints[0] = gameObject.transform.position; // Get Controller Position
            controlPoints[1] = controlPoints[0] + (gameObject.transform.forward * (extendStep + forwardProjectionExtend) * 2f / 5f);
            controlPoints[2] = controlPoints[1] + (gameObject.transform.forward * (extendStep + forwardProjectionExtend) * 3f / 5f) + Vector3.up * -2f;
        }


        /// <summary>
        /// Draw the bezier curve.
        /// </summary>
        void DrawCurve()
        {
            if (!lineRenderer.enabled)
                return;
            lineRenderer.positionCount = 1;
            //
            // Sets the 1st control point on the position of the controller
            lineRenderer.SetPosition(0, controlPoints[0]);

            Vector3 prevPosition = controlPoints[0];
            Vector3 nextPosition = prevPosition;
            for (int i = 1; i <= SEGMENT_COUNT; i++)
            {
                float t = i / (float)SEGMENT_COUNT;
                lineRenderer.positionCount = i + 1;

                if (i == SEGMENT_COUNT)
                { // For the last point, project out the curve two more meters.
                    Vector3 endDirection = Vector3.Normalize(prevPosition - lineRenderer.GetPosition(i - 2));
                    nextPosition = prevPosition + endDirection * 2f;
                }
                else
                {
                    nextPosition = CalculateBezierPoint(t, controlPoints[0], controlPoints[1], controlPoints[2]);
                }

                if (CheckColliderIntersection(prevPosition, nextPosition))
                { // If the segment intersects a surface, draw the point and return.
                    lineRenderer.SetPosition(i, endpoint);
                    endPointDetected = true;
                    return;
                }
                else
                { // If the point does not intersect, continue to draw the curve.
                    lineRenderer.SetPosition(i, nextPosition);
                    endPointDetected = false;
                    prevPosition = nextPosition;
                }
            }
        }


        /// <summary>
        /// Check if the line between start and end intersect a collider.
        /// </summary>
        /// <param name="start">Vector3 start position.</param>
        /// <param name="end">Vector3 end position.</param>
        /// <returns>Returns true if line intersects collider.</returns>
        bool CheckColliderIntersection(Vector3 start, Vector3 end)
        {
            Ray r = new Ray(start, end - start);
            RaycastHit hit;
            if (Physics.Raycast(r, out hit, Vector3.Distance(start, end)))
            {
                endpoint = hit.point;
                return true;
            }

            return false;
        }


        /// <summary>
        /// Calculates a point on the bezier curve using the
        /// Quadratic Bezier curve equation.
        /// <para/>
        /// https://en.wikipedia.org/wiki/B%C3%A9zier_curve
        /// <para/>
        /// Credit to: Hassank
        /// <para/>
        /// Modified by: Austin Che
        /// </summary>
        /// <param name="t">Where t the amount of segments divided by time.</param>
        /// <param name="p0">Control point 1</param>
        /// <param name="p1">Control point 2</param>
        /// <param name="p2">Control point 3</param>
        /// <returns>Returns a Vector3.</returns>
        Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
        {
            // B(t) = [(1 - t)^2 * p0] + [2(1 - t) * t * p1] + [t^2 * p2]
            return
                Mathf.Pow((1f - t), 2) * p0 +
                2f * (1f - t) * t * p1 +
                Mathf.Pow(t, 2) * p2;
        }
    }
}