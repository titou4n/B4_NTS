using System;
using System.Collections.Generic;
using TMPro;
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
    
    //________________________________________//
    //_______________For_CPT__________________//
    //________________________________________//
    
    public GameObject CptUi;
    public TMP_Text Cpt;
    
    private float _timing;
    private bool canDraw = true;
    
    //________________________________________//
    //________________________________________//
    //________________________________________//
    
    // Effet de particules
    public GameObject particleEffectPrefab;
    
    void Start()
    {
        touchPressAction = PlayerInput.actions["TouchPress"];
        touchPosAction = PlayerInput.actions["TouchPos"];
        _timing = 60;
        CptUi.SetActive(true);
    }
    
    void Update()
    {
        if (CptUi.activeSelf)
        {
            _timing -= Time.deltaTime;
            if (_timing <= 0)
            {
                Cpt.SetText($"This is done ...");
                canDraw = false; // Désactive le dessin lorsque le compteur est à 0
            }
            else
            {
                Cpt.SetText($"{_timing:F1} s");
            }
        }
        
        if (canDraw && touchPressAction.IsPressed()) // On permet le dessin uniquement si canDraw est vrai
        {
            OnTouch();
        }
        else
        {
            currentLine = null;
        }
    }
    
    public void SetSize1()
    {
        if (currentLine != null)
        {
            currentLine.startWidth = 0.01f;
            currentLine.endWidth = 0.01f;
        }
    }

    public void SetSize2()
    {
        if (currentLine != null)
        {
            currentLine.startWidth = 0.05f;
            currentLine.endWidth = 0.05f;
        }
    }

    public void SetSize3()
    {
        if (currentLine != null)
        {
            currentLine.startWidth = 0.1f;
            currentLine.endWidth = 0.1f;
        }
    }

    private void StartNewLine(Vector3 startPosition)
    {
        GameObject lineObj = new GameObject("Line");
        currentLine = lineObj.AddComponent<LineRenderer>();
        
        currentLine.material = new Material(Shader.Find("Sprites/Default"));
        currentLine.startWidth = 0.01f; // Valeur par défaut
        currentLine.endWidth = 0.01f; // Valeur par défaut
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
        
        // Ajouter un effet de particules à la position du nouveau point
        CreateParticleEffect(newPoint);
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
    
    private void CreateParticleEffect(Vector3 position)
    {
        if (particleEffectPrefab != null)
        {
            // Instancier le prefab d'effet de particules à la position
            GameObject particleEffect = Instantiate(particleEffectPrefab, position, Quaternion.identity);
            Destroy(particleEffect, 0.5f);
        }
    }
}

