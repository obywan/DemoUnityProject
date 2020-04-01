using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARRaycastManager))]
public class PlacementController : MonoBehaviour
{
    public GameObject defaultObject;
    private Transform targetObject;
    public UnityEvent onPlacedFirstTime;

    private ARRaycastManager raycastManager;
    
    private Touch t0;
    private Touch t1;

    private Vector2 t0InitPos;
    private Vector2 t1InitPos;

    private float countdownTimer = 1f;

    private bool placedFirstTime = false;
    List<ARRaycastHit> hitResults = new List<ARRaycastHit>();
    private Vector3 placementPosition = Vector3.zero;

    private void Awake()
    {
        raycastManager = GetComponent<ARRaycastManager>();
    }

    void Update()
    {
        WatchInput();
    }

    public void PlaceObject(GameObject go, Touch t)
    {
        targetObject = go.transform;
        MoveItem(t.position);
        if (!placedFirstTime)
        {
            CallOnFirstTimePlaced();
        }
    }

    private void WatchInput()
    {
        if (!PointerOverUI())
        {
            //SetActionModeBasedOnTouches(Input.touches);

            switch (Input.touchCount)
            {
                case 1:
                    // handle one touch
                    HandleOneTouch();
                    break;
                case 2:
                    // handle two touches
                    HandleTwotouches();
                    break;
                default:
                    //nothing to do
                    break;
            }

        }
    }

    private void HandleOneTouch()
    {
        t0 = Input.GetTouch(0);

        //Check if wee nedd to switch object
        if (t0.phase == TouchPhase.Began)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100))
            {
                targetObject = hit.transform;
            }
            else
            {
                GameObject tmpGO = Instantiate(defaultObject);
                PlaceObject(tmpGO, t0);
            }
        }


        if (t0.phase == TouchPhase.Began || t0.phase == TouchPhase.Moved || t0.phase == TouchPhase.Stationary)
        {
            countdownTimer -= Time.deltaTime;
            if (countdownTimer <= 0f)
            {
                MoveItem(t0.position);
            }
        }
        if (t0.phase == TouchPhase.Ended)
        {
            countdownTimer = 1f;
        }
    }

    private void HandleTwotouches()
    {
        UpdateRotation();
    }

    private void MoveItem(Vector2 position)
    {
        if (targetObject)
        {
            if (!targetObject.gameObject.activeSelf)
                targetObject.gameObject.SetActive(true);

            if (raycastManager.Raycast(position, hitResults))
            {
                targetObject.position = hitResults[0].pose.position;
                placementPosition = targetObject.position;
            }
        }
    }

    private void UpdateRotation()
    {
        if (targetObject)
        {
            t0 = Input.GetTouch(0);
            t1 = Input.GetTouch(1);

            if (t1.phase == TouchPhase.Began)
            {
                t0InitPos = t0.position;
                t1InitPos = t1.position;
            }

            if (t0InitPos != null && t1InitPos != null)
            {
                if (t0.phase == TouchPhase.Moved)
                {
                    float angle = Vector2.Angle(Input.touches[0].position - t1InitPos, Quaternion.Euler(0, 0, 90) * (t0InitPos - t1InitPos));
                    angle -= 90;
                    targetObject.Rotate(Vector3.up, angle * 3f);
                }
                if (t1.phase == TouchPhase.Moved)
                {
                    float angle = Vector2.Angle(Input.touches[1].position - t0InitPos, Quaternion.Euler(0, 0, 90) * (t1InitPos - t0InitPos));
                    angle -= 90;
                    targetObject.Rotate(Vector3.up, angle * 3f);
                }

                //m_targetObj.localScale *= Vector3.Distance(t0.position, t1.position) / Vector3.Distance(t0InitPos, t1InitPos);
            }

            t0InitPos = t0.position;
            t1InitPos = t1.position;
        }
    }

    private void CallOnFirstTimePlaced()
    {
        placedFirstTime = true;
        if(onPlacedFirstTime != null)
        {
            onPlacedFirstTime.Invoke();
        }
    }

    private bool PointerOverUI()
    {
        if (EventSystem.current == null)
            return false;

#if UNITY_IOS || UNITY_ANDROID
        if (Input.touchCount > 0)
        {
            return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
        }
        return false;
#endif
    }
}
