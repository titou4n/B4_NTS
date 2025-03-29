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
    
    public PlayerInput PlayerInput;
    private InputAction touchPressAction;
    private InputAction touchPosAction;
    
    public Image StatusColor;
    private string colorSelected = "red";
    
    private LineRenderer currentLine;
    private List<Vector3> linePoints = new List<Vector3>();

    void Start()
    {
        touchPressAction = PlayerInput.actions["TouchPress"];
        touchPosAction = PlayerInput.actions["TouchPos"];
    }

    private void StartNewLine(Vector3 startPosition)
    {
        GameObject lineObj = new GameObject("Line");
        currentLine = lineObj.AddComponent<LineRenderer>();
        
        currentLine.material = new Material(Shader.Find("Sprites/Default"));
        currentLine.startWidth = 0.01f;
        currentLine.endWidth = 0.01f;
        currentLine.positionCount = 0;
        currentLine.useWorldSpace = true;
        
        // Appliquer la couleur sélectionnée
        currentLine.startColor = GetColorFromName(colorSelected);
        currentLine.endColor = GetColorFromName(colorSelected);
        
        linePoints.Clear();
        AddPointToLine(startPosition);
    }

    private void AddPointToLine(Vector3 newPoint)
    {
        if (currentLine == null) return;
        
        linePoints.Add(newPoint);
        currentLine.positionCount = linePoints.Count;
        currentLine.SetPositions(linePoints.ToArray());
    }

    private Color GetColorFromName(string color)
    {
      switch (color)
      {
        case "red": return Color.red;
        case "green": return Color.green;
        case "blue": return Color.blue;
        case "yellow": return Color.yellow;
        case "pink": return new Color(1.0f, 0.75f, 0.8f);
        case "black": return Color.black;
        case "white": return Color.white;
        default: return Color.white;
      }
    }

    private void OnTouch()
    {
        var touchPos = touchPosAction.ReadValue<Vector2>();
        
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        RaycastManager.Raycast(touchPos, hits, TypeToTrack);
        
        if (hits.Count > 0)
        {
            Vector3 hitPosition = hits[0].pose.position;

            if (currentLine == null)
            {
                StartNewLine(hitPosition);
            }
            
            AddPointToLine(hitPosition);
        }
    }
    
    public void ChangeColor(string color)
    {
        colorSelected = color;
        StatusColor.color = GetColorFromName(color);
        
        if (currentLine != null)
        {
            currentLine.startColor = GetColorFromName(color);
            currentLine.endColor = GetColorFromName(color);
        }
    }

    void Update()
    {
        if (touchPressAction.IsPressed())
        {
            OnTouch();
        }
        else
        {
            currentLine = null;
        }
    }
}
