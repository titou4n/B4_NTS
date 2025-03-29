using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
public class Scene2Manager : MonoBehaviour
{
    public ARRaycastManager RaycastManager;
    public TrackableType TypeToTrack = TrackableType.PlaneWithinBounds;
    public GameObject PrefabToInstantiate;
    
    public PlayerInput PlayerInput;
    private InputAction touchPressAction;
    private InputAction touchPosAction;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        touchPressAction = PlayerInput.actions["TouchPress"];
        touchPosAction = PlayerInput.actions["TouchPos"];
    }

    private void OnTouch()
    {
        var touchPos = touchPosAction.ReadValue<Vector2>();
        
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        RaycastManager.Raycast(touchPos, hits, TypeToTrack);
        
        if (hits.Count > 0)
        {
            ARRaycastHit firstHit = hits[0];
            Instantiate(PrefabToInstantiate, firstHit.pose.position, firstHit.pose.rotation);
        }
    }

    void Update()
    {
      if (touchPressAction.IsPressed())
      {
        OnTouch();
      }
    }
}
