using UnityEngine;
using UnityEngine.InputSystem;

public class KillingCamera : MonoBehaviour
{
    public GameObject ParticleEffect;
    private Vector2 touchPos;
    private RaycastHit hit;
    private Camera cam;
    
    public PlayerInput playerInput;
    private InputAction touchPressAction;
    private InputAction touchPosAction;
    
    void Start()
    {
        touchPressAction = playerInput.actions["TouchPress"];
        touchPosAction = playerInput.actions["TouchPos"];
        cam = GetComponent<Camera>();
    }
    
    void Update()
    {
        if (!touchPressAction.WasPerformedThisFrame())
        {
            touchPos = touchPosAction.ReadValue<Vector2>();
            Ray ray = cam.ScreenPointToRay(touchPos);
            if (Physics.Raycast(ray, out hit))
            {
                GameObject hitObj = hit.collider.gameObject;
                if (hitObj.tag == "Enemy")
                {
                    var clone = Instantiate(ParticleEffect, hitObj.transform.position, Quaternion.identity);
                    clone.transform.localScale = hitObj.transform.localScale;
                    Destroy(hitObj);
                }
            }
        }
    }
    
}
