using UnityEngine;
using System.Collections;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Events;

[RequireComponent(typeof(ARPlaneManager))]
public class AREventsReporter : MonoBehaviour
{
    public UnityEvent onPlaneDetected;

    private ARPlaneManager arPlaneManager;
    private bool planeDetectionRepotred = false;
    
    private void Awake()
    {
        arPlaneManager = GetComponent<ARPlaneManager>();
    }

    private void OnEnable()
    {
        arPlaneManager.planesChanged += OnPlaneDetected;
    }

    private void OnDisable()
    {
        arPlaneManager.planesChanged -= OnPlaneDetected;
    }

    private void OnPlaneDetected(ARPlanesChangedEventArgs args)
    {
        if (planeDetectionRepotred)
            return;

        if (onPlaneDetected != null && args.added.Count > 0)
        {
            planeDetectionRepotred = true;
            onPlaneDetected.Invoke();
        }
    }


}
