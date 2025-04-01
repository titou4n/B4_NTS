using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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
    private List<GameObject> drawnLines = new List<GameObject>(); // Liste des lignes créées

    private float _sikness; // Épaisseur
    
    // Timer
    public GameObject CptUi;
    public TMP_Text Cpt;
    private float _timing;
    private bool canDraw = true;
    
    public GameObject particleEffectPrefab;
    private string tool = "pen";
    public string[] tools = { "pen", "eraser" };
    
    void Start()
    {
        touchPressAction = PlayerInput.actions["TouchPress"];
        touchPosAction = PlayerInput.actions["TouchPos"];
        _timing = 60;
        CptUi.SetActive(true);
        _sikness = 0.01f;
    }

    public void ChangeTool(string tool)
    {
        this.tool = tool;
        // tool = tools[(Array.FindIndex(tools, t => t == tool) + 1) % tools.Length];
        // Debug.Log("Tool changed to: " + tool);
    }
    
    void Update()
    {
        if (CptUi.activeSelf)
        {
            _timing -= Time.deltaTime;
            if (_timing <= 0)
            {
                Cpt.SetText("This is done ...");
                canDraw = false;
            }
            else
            {
                Cpt.SetText($"{_timing:F1} s");
            }
        }
        
        if (canDraw && touchPressAction.IsPressed())
        {
            OnTouch();
        }
        else
        {
            currentLine = null;
        }
    }

      public void SetSize(int size)
    {
        switch (size) {
            case 1:
                _sikness = 0.01f;
                break;
            case 2:
                _sikness = 0.05f;
                break;
            case 3:
                _sikness = 0.1f;
                break;
            default:
                _sikness = 0.01f;
                break;
        }
    }
    
    private void StartNewLine(Vector3 startPosition)
    {
        GameObject lineObj = new GameObject("Line");
        lineObj.tag = "Line";
        
        currentLine = lineObj.AddComponent<LineRenderer>();
        currentLine.material = new Material(Shader.Find("Sprites/Default"));
        currentLine.startWidth = _sikness;
        currentLine.endWidth = _sikness;
        currentLine.positionCount = 0;
        currentLine.useWorldSpace = true;
        currentLine.startColor = GetColorFromName(colorSelected);
        currentLine.endColor = GetColorFromName(colorSelected);

        drawnLines.Add(lineObj);
        linePoints.Clear();
        AddPointToLine(startPosition);
    }

    private void AddPointToLine(Vector3 newPoint)
    {
        if (currentLine == null) return;
        
        linePoints.Add(newPoint);
        currentLine.positionCount = linePoints.Count;
        currentLine.SetPositions(linePoints.ToArray());

        CreateParticleEffect(newPoint);
    }
    
    private Color GetColorFromName(string color)
    {
        return color switch
        {
            "red" => Color.red,
            "green" => Color.green,
            "blue" => Color.blue,
            "yellow" => Color.yellow,
            "pink" => new Color(1.0f, 0.75f, 0.8f),
            "black" => Color.black,
            "white" => Color.white,
            _ => Color.white,
        };
    }
    
    private GameObject FindClosestLine(Vector3 position)
    {
        GameObject closestLine = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject line in drawnLines)
        {
            LineRenderer lr = line.GetComponent<LineRenderer>();
            if (lr == null) continue;

            for (int i = 0; i < lr.positionCount; i++)
            {
                float distance = Vector3.Distance(lr.GetPosition(i), position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestLine = line;
                }
            }
        }
        return minDistance < 0.05f ? closestLine : null; // pour eviter de tout suprimer d'un coup
    }
    
    private void OnTouch()
    {
        if (EventSystem.current.IsPointerOverGameObject())
    {
        return; // Évite de dessiner quand on clique sur un bouton
    }
    Debug.Log("Touch detected!");

        var touchPos = touchPosAction.ReadValue<Vector2>();
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        RaycastManager.Raycast(touchPos, hits, TypeToTrack);
        
        if (hits.Count > 0)
        {
            Vector3 hitPosition = hits[0].pose.position;
            
            switch (tool)
            {
                case "pen":
                    if (currentLine == null)
                    {
                        StartNewLine(hitPosition);
                    }
                    AddPointToLine(hitPosition);
                    break;
                
                case "eraser":
                    GameObject lineToErase = FindClosestLine(hitPosition);
                    if (lineToErase != null)
                    {
                        drawnLines.Remove(lineToErase);
                        Destroy(lineToErase);
                        Debug.Log("Line erased!");
                    }
                    break;
            }
        }
    }
    
    public void ChangeColor(string color)
    {
        colorSelected = color;
        StatusColor.color = GetColorFromName(color);
    }
    
    private void CreateParticleEffect(Vector3 position)
    {
        if (particleEffectPrefab != null)
        {
            GameObject particleEffect = Instantiate(particleEffectPrefab, position, Quaternion.identity);
            Destroy(particleEffect, 0.5f);
        }
    }
}