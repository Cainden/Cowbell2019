// Script for handing the main camera.

using MySpace;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
	public static CameraScript Ref { get; private set; }
	Camera cam;

	Plane panPlane;
	public bool IsCamDragging { get; private set; }
	Vector2 prevMousePos;
	float zoomFactor; // 0-100%, far to near Z-Pos

	/// <summary>
	/// x = left pos, y = right pos
	/// </summary>
	Vector2 xLimit;
	/// <summary>
	/// x = down pos, y = up pos
	/// </summary>
	Vector2 yLimit;

	
	[SerializeField] float CameraZoomMovementSpeed = 5.0f;
	[SerializeField] float CameraDragMovementSpeed = 0.1f;
	[SerializeField] float CameraKeyMovementSpeed = 4.0f;
	[Tooltip("Pixel threshold to detect/invoke camera drag")]
	[SerializeField] int CameraDragThreshold = 6;

	// Variables used in recalculate limits method
	[SerializeField] Vector2 CameraxZPositionLimits = new Vector2(-180.0f, -10.0f);
	[SerializeField] Vector2 CameraxXPositionLimitsLeft = new Vector2(18.0f, -21.0f);
	[SerializeField] Vector2 CameraxXPositionLimitsRight = new Vector2(18.0f, 58.0f);
	[SerializeField] Vector2 CameraxYPositionLimitsDown = new Vector2(18.0f, -12.0f);
	[SerializeField] Vector2 CameraxYPositionLimitsUp = new Vector2(48.0f, 67.0f);

	#region Prebuild Methods

	void Awake()
	{
		#region Singleton Managment
		if (Ref && Ref != this)
		{
			Destroy(this);
			return;
		}
		Ref = this;
		DontDestroyOnLoad(this);
		#endregion

		cam = GetComponent<Camera>();
		panPlane = new Plane();
	}

	void Start()
	{
		RecalculateLimits();
	}

	void Update()
	{
		CheckMouseWheelZoom();
		CheckKeyMovement();
	}

	void LateUpdate()
	{
		CheckScrollScreenborder();
		Limit_CameraPosition();
	}

	#endregion

	#region Camera Dragging

	public void CamPanStart()
	{
		prevMousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		IsCamDragging = false;
	}

	public void CamPanUpdate()
	{
		//PanZoom Tutorial https://youtu.be/KkYco_7-ULA
		Vector3 delta1;
		panPlane.SetNormalAndPosition(transform.forward, new Vector3(transform.position.x, transform.position.y, 0));
		delta1 = PlanePosDelta(Input.mousePosition);
		cam.transform.Translate(-delta1, Space.World);
        Vector2 mPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        if (!IsCamDragging)
        {
            if (mPos != prevMousePos)
            {
                SetCameraDragging();
            }
        }
		prevMousePos = mPos;
	}

	Vector3 PlanePosDelta(Vector2 _screenPos)
	{
		Vector2 deltaPos = prevMousePos - new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		Ray rayBefore = cam.ScreenPointToRay(_screenPos - deltaPos);
		Ray rayCurr = cam.ScreenPointToRay(_screenPos);
		if (panPlane.Raycast(rayBefore, out var rayBeforeOut) && panPlane.Raycast(rayCurr, out var rayCurrOut))
		{
			return rayBefore.GetPoint(rayBeforeOut) - rayCurr.GetPoint(rayCurrOut);
		}
		return Vector3.zero;
	}

	Vector3 PlanePos(Vector2 _screenPos)
	{
		Ray ray = cam.ScreenPointToRay(_screenPos);
		if (panPlane.Raycast(ray, out var rayOut))
		{
			return ray.GetPoint(rayOut);
		}
		return Vector3.zero;
	}

	public void SetCameraDragging()
	{
		IsCamDragging = true;
		GuiManager.Ref.SetCamDragCursor(true);
	}

	public void ResetCameraDragging()
	{
		IsCamDragging = false;
		GuiManager.Ref.SetCamDragCursor(false);
	}

	#endregion

	public static bool ZoomDisabled = false;
	public void CheckMouseWheelZoom()
	{
		if (Input.mouseScrollDelta.y == 0.0f) return;
		if (ZoomDisabled) return;

		transform.Translate(new Vector3(0, 0, Input.mouseScrollDelta.y * CameraZoomMovementSpeed));
		RecalculateLimits();
	}

	public void CheckKeyMovement()
	{
		if (Input.GetKey(KeyCode.D))
		{
			transform.Translate(new Vector3(CameraKeyMovementSpeed * Time.deltaTime, 0, 0));
		}
		if (Input.GetKey(KeyCode.A))
		{
			transform.Translate(new Vector3(-CameraKeyMovementSpeed * Time.deltaTime, 0, 0));
		}
		if (Input.GetKey(KeyCode.S))
		{
			transform.Translate(new Vector3(0, -CameraKeyMovementSpeed * Time.deltaTime, 0));
		}
		if (Input.GetKey(KeyCode.W))
		{
			transform.Translate(new Vector3(0, CameraKeyMovementSpeed * Time.deltaTime, 0));
		}
	}

	void RecalculateLimits()
    {
        float ZPos = Mathf.Clamp(transform.position.z, CameraxZPositionLimits.x, CameraxZPositionLimits.y);
        transform.position = new Vector3(transform.position.x, transform.position.y, ZPos);
        zoomFactor = (ZPos - CameraxZPositionLimits.x) / (CameraxZPositionLimits.y - CameraxZPositionLimits.x);

        xLimit.x = (CameraxXPositionLimitsLeft.y - CameraxXPositionLimitsLeft.x) * zoomFactor + CameraxXPositionLimitsLeft.x;
        xLimit.y = (CameraxXPositionLimitsRight.y - CameraxXPositionLimitsRight.x) * zoomFactor + CameraxXPositionLimitsRight.x;

        yLimit.x = (CameraxYPositionLimitsDown.y - CameraxYPositionLimitsDown.x) * zoomFactor + CameraxYPositionLimitsDown.x;
        yLimit.y = (CameraxYPositionLimitsUp.y - CameraxYPositionLimitsUp.x) * zoomFactor + CameraxYPositionLimitsUp.x;
    }

    void Limit_CameraPosition()
    {
        float XPos = Mathf.Clamp(transform.position.x, xLimit.x, xLimit.y);
        float YPos = Mathf.Clamp(transform.position.y, yLimit.x, yLimit.y);
        transform.position = new Vector3(XPos, YPos, transform.position.z);
    }

    public void CheckScrollScreenborder()
    {
        if (StateManager.Ref.GetGameState() != Enums.GameStates.ManDragging) return;

        float fZFactor = (1.0f - zoomFactor) * 10.0f + 7.5f; 

        if (Input.mousePosition.x < 20)
        {
            transform.Translate(new Vector3(-CameraKeyMovementSpeed * Time.deltaTime * fZFactor, 0, 0));
        }
        else if (Input.mousePosition.x > Screen.width - 20)
        {
            transform.Translate(new Vector3(CameraKeyMovementSpeed * Time.deltaTime * fZFactor, 0, 0));
        }

        if (Input.mousePosition.y < 20)
        {
            transform.Translate(new Vector3(0, -CameraKeyMovementSpeed * Time.deltaTime * fZFactor, 0));
        }
        else if (Input.mousePosition.y > Screen.height - 20)
        {
            transform.Translate(new Vector3(0, CameraKeyMovementSpeed * Time.deltaTime * fZFactor, 0));
        }
    }
}
