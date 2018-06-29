using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The <see cref="NoController"/> class detects whether an Oculus Remote or Touch is connected.
/// <para/>
/// Modified by: Austin Che
/// Modified Date: June 28, 2018
/// </summary>
public class NoController : MonoBehaviour {

    protected OVRInput.Controller leftTouch = OVRInput.Controller.LTouch;
    protected OVRInput.Controller rightTouch = OVRInput.Controller.RTouch;
	protected OVRInput.Controller left = OVRInput.Controller.LTrackedRemote;
	protected OVRInput.Controller right = OVRInput.Controller.RTrackedRemote;
	protected Canvas canvas = null;

	private bool m_prevControllerConnected = false;
	private bool m_prevControllerConnectedCached = false;

    private bool _prevTouchControllerConnected = false;
    private bool _prevTouchControllerConnectedCached = false;

	void Awake() {
		canvas = GetComponent<Canvas> ();
	}

	void Update()
	{
		bool controllerConnected = OVRInput.IsControllerConnected(left) || OVRInput.IsControllerConnected(right);
        bool touchControllerConnected = OVRInput.IsControllerConnected(leftTouch) || OVRInput.IsControllerConnected(rightTouch);

		if ((controllerConnected != m_prevControllerConnected) || !m_prevControllerConnectedCached)
		{
			canvas.enabled = !controllerConnected;
			m_prevControllerConnected = controllerConnected;
			m_prevControllerConnectedCached = true;
		}

        // Determines if the touch controllers are connected...
        // If they are connected, then we want to cache the boolean results
        if ((touchControllerConnected != _prevTouchControllerConnected) || !_prevTouchControllerConnectedCached)
        {
            // we want to disable the "No Controller" canvas because a controller is connected
            canvas.enabled = !touchControllerConnected;
            _prevTouchControllerConnected = touchControllerConnected;
            _prevTouchControllerConnectedCached = true;
        }

		if (!controllerConnected)
		{
			return;
		}
        if (!touchControllerConnected)
        {
            return;
        }
	}
}
