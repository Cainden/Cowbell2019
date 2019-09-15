// Script for handing the main camera.

using MySpace;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [HideInInspector]
    public static CameraScript Ref { get; private set; } // For external access of script

    private bool    _CameraDragging;
    private Vector2 _DragMousePosition;
    private Vector2 _DragSum;

    private float   _ZoomFactor; // 0..100%, far to near Z-Pos
    private Vector2 _XLimitLeftRight;
    private Vector2 _YLimitDownUp;    

    void Awake()
    {
        if (Ref == null) Ref = GetComponent<CameraScript>();
    }

    void Start ()
    {
        RecalculateLimits();
    }
	
	void Update ()
    {
        CheckMouseWheelZoom();
        CheckKeyMovement();
    }

    void LateUpdate()
    {
        CheckScrollScreenborder();
        Limit_CameraPosition();
    }

    public void CheckMouseWheelZoom()
    {
        if (Input.mouseScrollDelta.y == 0.0f) return;

        transform.Translate(new Vector3(0, 0, Input.mouseScrollDelta.y * Constants.CameraZoomMovementSpeed));
        RecalculateLimits();        
    }

    public void CheckKeyMovement()
    {
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(new Vector3(Constants.CameraKeyMovementSpeed * Time.deltaTime, 0, 0));
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(new Vector3(-Constants.CameraKeyMovementSpeed * Time.deltaTime, 0, 0));
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(new Vector3(0, -Constants.CameraKeyMovementSpeed * Time.deltaTime, 0));
        }
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(new Vector3(0, Constants.CameraKeyMovementSpeed * Time.deltaTime, 0));
        }
    }

    public void SetDragStartMousePosition(float x, float y)
    {
        _DragMousePosition.x = x;
        _DragMousePosition.y = y;
        _DragSum = new Vector2();
        _CameraDragging = false;
    }

    public void MoveDragCamera()
    {
        float fDeltaX = Input.mousePosition.x - _DragMousePosition.x;
        float fDeltaY = Input.mousePosition.y - _DragMousePosition.y;
        _DragSum += new Vector2(Mathf.Abs(fDeltaX), Mathf.Abs(fDeltaY));

        Camera.main.transform.Translate(new Vector3(-fDeltaX * Constants.CameraDragMovementSpeed,
                                                    -fDeltaY * Constants.CameraDragMovementSpeed, 0));

        _DragMousePosition.x = Input.mousePosition.x;
        _DragMousePosition.y = Input.mousePosition.y;

        if (_CameraDragging) return;

        if ((_DragSum.x > Constants.CameraDragThreshold) || (_DragSum.y > Constants.CameraDragThreshold))
        {
            SetCameraDragging();
        }
    }

    public bool IsCameraDragging()
    {
        return (_CameraDragging);
    }

    public void SetCameraDragging()
    {
        _CameraDragging = true;
        GuiManager.Ref.SetCamDragCursor(true);
    }

    public void ResetCameraDragging()
    {
        _CameraDragging = false;
        GuiManager.Ref.SetCamDragCursor(false);
    }

    void RecalculateLimits()
    {
        float ZPos = Mathf.Clamp(transform.position.z, Constants.CameraxZPositionLimits.x, Constants.CameraxZPositionLimits.y);
        transform.position = new Vector3(transform.position.x, transform.position.y, ZPos);
        _ZoomFactor = (ZPos - Constants.CameraxZPositionLimits.x) / (Constants.CameraxZPositionLimits.y - Constants.CameraxZPositionLimits.x);

        _XLimitLeftRight.x = (Constants.CameraxXPositionLimitsLeft.y - Constants.CameraxXPositionLimitsLeft.x) * _ZoomFactor + Constants.CameraxXPositionLimitsLeft.x;
        _XLimitLeftRight.y = (Constants.CameraxXPositionLimitsRight.y - Constants.CameraxXPositionLimitsRight.x) * _ZoomFactor + Constants.CameraxXPositionLimitsRight.x;

        _YLimitDownUp.x = (Constants.CameraxYPositionLimitsDown.y - Constants.CameraxYPositionLimitsDown.x) * _ZoomFactor + Constants.CameraxYPositionLimitsDown.x;
        _YLimitDownUp.y = (Constants.CameraxYPositionLimitsUp.y - Constants.CameraxYPositionLimitsUp.x) * _ZoomFactor + Constants.CameraxYPositionLimitsUp.x;
    }

    void Limit_CameraPosition()
    {
        float XPos = Mathf.Clamp(transform.position.x, _XLimitLeftRight.x, _XLimitLeftRight.y);
        float YPos = Mathf.Clamp(transform.position.y, _YLimitDownUp.x, _YLimitDownUp.y);
        transform.position = new Vector3(XPos, YPos, transform.position.z);
    }

    public void CheckScrollScreenborder()
    {
        if (StateManager.Ref.GetGameState() != Enums.GameStates.ManDragging) return;

        float fZFactor = (1.0f - _ZoomFactor) * 10.0f + 7.5f; 

        if (Input.mousePosition.x < 20)
        {
            transform.Translate(new Vector3(-Constants.CameraKeyMovementSpeed * Time.deltaTime * fZFactor, 0, 0));
        }
        else if (Input.mousePosition.x > Screen.width - 20)
        {
            transform.Translate(new Vector3(Constants.CameraKeyMovementSpeed * Time.deltaTime * fZFactor, 0, 0));
        }

        if (Input.mousePosition.y < 20)
        {
            transform.Translate(new Vector3(0, -Constants.CameraKeyMovementSpeed * Time.deltaTime * fZFactor, 0));
        }
        else if (Input.mousePosition.y > Screen.height - 20)
        {
            transform.Translate(new Vector3(0, Constants.CameraKeyMovementSpeed * Time.deltaTime * fZFactor, 0));
        }
    }
}
