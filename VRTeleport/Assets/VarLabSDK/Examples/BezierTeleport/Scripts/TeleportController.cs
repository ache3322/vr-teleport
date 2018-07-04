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
    public class TeleportController : MonoBehaviour
    {
        [Header("Activation Settings")]

        [Tooltip("The button used to execute the activate action.")]
        public OVRInput.Button activationButton = OVRInput.Button.Four;

        [Header("Selection Settings")]

        [Tooltip("The button used to execute the select the action.")]
        public OVRInput.Button selectionButton = OVRInput.Button.PrimaryIndexTrigger;

        [Header("Other Settings")]

        /// <summary>
        /// The bezier curve that projects the path for teleporting.
        /// </summary>
        [Tooltip("A bezier curve object.")]
        public Bezier bezier;

        /// <summary>
        /// Teleport sprite indicating where the player will land.
        /// </summary>
        [Tooltip("The teleport landing sprite (pad, cursor, etc).")]
        public GameObject teleportSprite;

        public bool TeleportEnabled
        {
            get { return _teleportEnabled; }
        }

        private bool _teleportEnabled;
        private bool _firstClick;
        private float _firstClickTime;
        private float doubleClickTimeLimit = 0.5f;


        void Start()
        {
            _teleportEnabled = false;
            _firstClick = false;
            _firstClickTime = 0f;
            teleportSprite.SetActive(false);
        }

        void Update()
        {
            UpdateTeleportEnabled();

            if (_teleportEnabled)
            {
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
                if (!_firstClick)
                { // The first click is detected.
                    _firstClick = true;
                    _firstClickTime = Time.unscaledTime; // Store the time when click occurs
                }
                else
                { // The second click detected, so toggle teleport mode.
                    _firstClick = false;
                    ToggleTeleportMode();
                }
            }

            if (Time.unscaledTime - _firstClickTime > doubleClickTimeLimit)
            { // Time for the double click has run out.
                _firstClick = false;
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

                if (OVRInput.GetDown(selectionButton))
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
            _teleportEnabled = !_teleportEnabled;
            bezier.ToggleDraw(_teleportEnabled);
            if (!_teleportEnabled)
            {
                teleportSprite.SetActive(false);
            }
        }
    }
}
