using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.EventSystems;

public class Scene2Manager : MonoBehaviour
{
    public ARRaycastManager RaycastManager;
    public TrackableType TypeToTrack = TrackableType.PlaneWithinBounds;
    
    public PlayerInput PlayerInput;
    private InputAction touchPressAction;
    private InputAction touchPosAction;
    
    public UnityEngine.UI.Image StatusColor;
    private string colorSelected = "red";
    
    private LineRenderer currentLine;
    private List<Vector3> linePoints = new List<Vector3>();
    private List<GameObject> drawnLines = new List<GameObject>(); // Liste des lignes créées
    private List<GameObject> ListCube = new List<GameObject>(); // Liste des cubes créées
    
    private int currentPaint; // Quantité de peinture utilisée
    private int maxPaint;

    private float _sikness; // Épaisseur
    
    // Timer
    public GameObject CptUi;
    public TMP_Text Cpt;
    public GameObject PaintBarUI;
    public TMP_Text PaintBar;
    public RectTransform paintBarRect;
    private float _timing;
    private bool canDraw = true;
    
    public GameObject particleEffectPrefab;
    private string tool = "pen";
    public AudioSource audioSource;
    public AudioClip clickSound;
    public AudioClip bestSoundEver;
    public GameObject CubePrefab;
    
    void Start()
    {
        touchPressAction = PlayerInput.actions["TouchPress"];
        touchPosAction = PlayerInput.actions["TouchPos"];
        
        CptUi.SetActive(true);
        PaintBarUI.SetActive(false);
        
        _timing = 60;
        _sikness = 0.01f;
        currentPaint = 0;
        maxPaint = 100;
    }

    void Update()
    {
        if (CptUi.activeSelf)
        {
            _timing -= Time.deltaTime;
            if (_timing <= 0)
            {
                Cpt.SetText("Time's up !");
                canDraw = false;
            }
            else
            {
                Cpt.SetText($"{_timing:F1} s");
            }
        }

        if (currentPaint >= maxPaint)
        {
            Cpt.SetText("YOU WON !!!!");
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

 private void OnTouch()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return; // Évite de dessiner quand on clique sur un bouton
        }

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
                        if (currentPaint < maxPaint)
                        {
                            currentPaint += 1;
                            UpdateScore();
                            StartNewLine(hitPosition);
                        }
                    }
                    AddPointToLine(hitPosition);
                    break;
                
                case "eraser":
                    GameObject lineToErase = FindClosestLine(hitPosition);
                    if (lineToErase != null)
                    {
                        drawnLines.Remove(lineToErase);
                        Destroy(lineToErase);
                        
                        currentPaint -= 1;
                        
                        UpdateScore();
                        
                        if (currentPaint == 0)
                        {
                            PaintBarUI.SetActive(false);
                        } 
                    }

                    if (audioSource != null)
                    {
                        audioSource.Stop();
                        audioSource.PlayOneShot(bestSoundEver);
                    }
                    break;
                
                case "erase all":
                    
                    currentPaint = 0;
                    
                    PaintBarUI.SetActive(false);
                    foreach (GameObject line in drawnLines)
                    {
                        if (line != null)
                        {
                            Destroy(line);
                        }
                    }
                    foreach (GameObject cube in ListCube)
                    {
                        if (cube != null)
                        {
                            Destroy(cube);
                        }
                    }

                    drawnLines = new List<GameObject>();
                    ListCube = new List<GameObject>();
                    break;
                
                case "cube":
                    if (hits.Count > 0)
                    {
                        
                        if (currentPaint < maxPaint)
                        {
                            currentPaint += 1;
                            
                            UpdateScore();
                            
                            ARRaycastHit firstHit = hits[0];
                            var c = Instantiate(CubePrefab, firstHit.pose.position, firstHit.pose.rotation);
                            c.GetComponent<Renderer>().material.color = GetColorFromName(colorSelected);
                            c.transform.localScale = new Vector3(_sikness, _sikness, _sikness);
                            
                            ListCube.Add(c);
                            
                        }
                    }

                    break;
            }
            UpdateScore();
        }
    }


    private void UpdateScore()
    {
        /*
        int paintPercent = (currentPaint * 100 / maxPaint);
        int percent = paintPercent * 660 / 100;

        paintBarRect.sizeDelta = new Vector2(percent, paintBarRect.sizeDelta.y);
        */
        PaintBarUI.SetActive(true);
        PaintBar.SetText(currentPaint + " / " + maxPaint);
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

        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }

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
    
    /***************** change somthing *******************/

    public void ChangeTool(string tool)
    {
        this.tool = tool;
        ClickSound();
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
        ClickSound();
    }

    public void ChangeColor(string color)
    {
        colorSelected = color;
        StatusColor.color = GetColorFromName(color);
        ClickSound();
    }

    /*************** effects *******************/
    
    private void ClickSound() {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }
    private void CreateParticleEffect(Vector3 position)
    {
        if (particleEffectPrefab != null)
        {
            GameObject particleEffect = Instantiate(particleEffectPrefab, position, Quaternion.identity);
            ParticleSystem ps = particleEffect.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ParticleSystem.MainModule main = ps.main;
                main.startColor = GetColorFromName(colorSelected);
            }
            
            Destroy(particleEffect, 0.5f);
        }
    }

}