using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VarLab.Teleport
{
    /// <summary>
    /// Animates the teleportation bezier curve line.
    /// <para/>
    /// Created by: Hassank
    /// Credit to: Hassank
    /// Link to: https://github.com/FusedVR/GearTeleporter
    /// <para/>
    /// Modified by: Austin Che
    /// Modified on: June 28, 2018
    /// </summary>
    public class AnimateLine : MonoBehaviour
    {
        public Material m;

        private Material _matInstance;
        private Vector2 uvAnimationRate = new Vector2(-2.0f, 0.0f);
        private string textureName = "_MainTex";
        private Vector2 uvOffset = Vector2.zero;


        void Start()
        {
            // Get local copy of bezier material
            _matInstance = GetComponent<Renderer>().material;
        }


        void LateUpdate()
        {
            uvOffset += (Time.deltaTime * uvAnimationRate);
            _matInstance.SetTextureOffset(textureName, uvOffset);
        }
    }
}