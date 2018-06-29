using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VarLab.Teleport
{
    /// <summary>
    /// The TeleportController has the functionality to handle 'Arc' teleportation.
    /// It uses the Quadratic Bezier equation to calculate 3 control points (p0, p1, p2).
    /// 
    /// This controller allows the user to toggle teleport mode: "ON" and "OFF.
    /// <para/>
    /// Created by: Hassank
    /// Credit to: Hassank
    /// Link to: https://github.com/FusedVR/GearTeleporter
    /// <para/>
    /// Modified by: Austin Che
    /// Modified on: June 28, 2018
    /// </summary>
    public class Teleport : MonoBehaviour
    {
        public bool TeleportEnabled
        {
            get { return teleportEnabled; }
        }

        /// <summary>
        /// The bezier curve that projects the path for teleporting.
        /// </summary>
        public Bezier bezier;

        /// <summary>
        /// Teleport sprite indicating where the player will land.
        /// </summary>
        public GameObject teleportSprite;

        private bool teleportEnabled;
        private bool firstClick;
        private float firstClickTime;
        private float doubleClickTimeLimit = 0.5f;



        void Start()
        {
            teleportEnabled = false;
            firstClick = false;
            firstClickTime = 0f;
            teleportSprite.SetActive(false);
        }

        void Update()
        {
            UpdateTeleportEnabled();

            if (teleportEnabled)
            {
                //HandleBezier();
                HandleTeleport();
            }
        }


        /// <summary>
        /// On double-click, toggle teleport mode on and off.
        /// </summary>
        void UpdateTeleportEnabled()
        {
            // Evaluates if the 'y' button was pressed two times in quick succession.
            // --> This enables teleportation mode
            if (OVRInput.GetDown(OVRInput.Button.Four))
            {
                if (!firstClick)
                { // The first click is detected.
                    firstClick = true;
                    firstClickTime = Time.unscaledTime; // Store the time when click occurs
                }
                else
                { // The second click detected, so toggle teleport mode.
                    firstClick = false;
                    ToggleTeleportMode();
                }
            }

            if (Time.unscaledTime - firstClickTime > doubleClickTimeLimit)
            { // Time for the double click has run out.
                firstClick = false;
            }
        }


        /// <summary>
        /// Handles the logic for if teleporting is possible.
        /// </summary>
        void HandleTeleport()
        {
            if (bezier.endPointDetected)
            { // There is a point to teleport to.
              // Display the teleport point.
                teleportSprite.SetActive(true);
                teleportSprite.transform.position = bezier.EndPoint;

                if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch))
                {
                    // Teleport to the position.
                    TeleportToPosition(bezier.EndPoint);
                }
            }
            else
            {
                teleportSprite.SetActive(false);
            }
        }


        /// <summary>
        /// Sets the position of the player to the "projected" teleportation area.
        /// </summary>
        /// <param name="teleportPos">Vector3 of the intended teleporation area.</param>
        void TeleportToPosition(Vector3 teleportPos)
        {
            // teleport the player upwards, so they cannot get stuck
            gameObject.transform.position = teleportPos + Vector3.up * 0.5f;
        }


        /// <summary>
        /// Toggles teleport mode.
        /// </summary>
        void ToggleTeleportMode()
        {
            teleportEnabled = !teleportEnabled;
            bezier.ToggleDraw(teleportEnabled);
            if (!teleportEnabled)
            {
                teleportSprite.SetActive(false);
            }
        }


        // Optional: use the touchpad to move the teleport point closer or further.
        //void HandleBezier()
        //{
        //    Vector2 touchCoords = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad);

        //    if (Mathf.Abs(touchCoords.y) > 0.8f) {
        //        bezier.ExtensionFactor = touchCoords.y > 0f ? 1f : -1f;
        //    } else {
        //        bezier.ExtensionFactor = 0f;
        //    }
        //}
    }
}
