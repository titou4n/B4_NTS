using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;

public class Scene2Manager : MonoBehaviour
{
    public ARRaycastManager RaycastManager;
    public TrackableType TypeToTrack = TrackableType.PlaneWithinBounds;
    public GameObject PrefabToInstantiate;
    
    public PlayerInput PlayerInput;
    private InputAction touchPressAction;
    private InputAction touchPosAction;
    public List<Button> MaterialsButtons;
    public Image StatusColor;
    string colorSelected = "red";
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
            var obj = Instantiate(PrefabToInstantiate, firstHit.pose.position, firstHit.pose.rotation);
            var renderer = obj.GetComponent<Renderer>();
            switch (colorSelected)
            {
              case "red":
                renderer.material.color = Color.red;
                break;
              case "green":
                renderer.material.color = Color.green;
                break;
              case "blue":
                renderer.material.color = Color.blue;
                break;
              case "yellow":
                renderer.material.color = Color.yellow;
                break;
              case "pink":
                renderer.material.color = new Color(1.0f, 0.75f, 0.8f); // Custom pink color
                break;
              default:
                Debug.LogWarning("Unknown color selected: " + colorSelected);
                break;
            }
        }
    }
    public void ChangeColor(string color) {
      switch (color) {
      case "red":
        colorSelected = "red";
        StatusColor.color = Color.red;
        break;
      case "green":
        colorSelected = "green";
        StatusColor.color = Color.green;
        break;
      case "blue":
        colorSelected = "blue";
        StatusColor.color = Color.blue;
        break;
      case "yellow":
        colorSelected = "yellow";
        StatusColor.color = Color.yellow;
        break;
      case "pink":
        colorSelected = "pink";
        StatusColor.color = new Color(1.0f, 0.75f, 0.8f); // Custom pink color
        break;
      default:
        Debug.LogWarning("Unknown color selected: " + color);
        break;
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
