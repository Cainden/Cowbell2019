using System.Collections;
using UnityEngine;
using TMPro;

public class GuiUserInfoSmallScript : MonoBehaviour
{
    public TextMeshProUGUI TitleText;
    public TextMeshProUGUI InfoText = null; // To be set by editor
    private bool _IsMoving = false;
    private float _PosY1;
    private float _PosX1;
    private float _PosX2 = 1500.0f;

    private readonly float TravelSpeed = 7000.0f;
    private readonly float PausingTime = 3.0f;

    void Start ()
    {
        gameObject.SetActive(false);
        Debug.Assert(InfoText != null);
        Debug.Assert(TitleText != null);
        TitleText.text = "";

        //Start slightly above the screen
        _PosY1 = Screen.height * 0.30f;
        _PosX1 = transform.position.x;
        //Make sure no matter the screen dimensions that the text becomes visible
    }

    void Update()
    {
        if (!_IsMoving) return;

        float distance = transform.position.x - _PosX2;
        float travel = TravelSpeed * Time.deltaTime;

        if (travel > distance) // Snap
        {
            transform.position = new Vector3(_PosX2, transform.position.y);
            _IsMoving = false;
            StartCoroutine(updateCoroutine());
        }
        else
        {
            transform.position = new Vector3(transform.position.x - travel, transform.position.y);
        }
    }

    public void StartInfoText(string sInfoText, string sTitleText = "")
    {
        TitleText.text = sTitleText;
        InfoText.text = sInfoText;
        transform.position = new Vector3(_PosX1, _PosY1);
        StopAllCoroutines();
        gameObject.SetActive(true);
        _IsMoving = true;
    }
    
    private IEnumerator updateCoroutine()
    {
        yield return new WaitForSeconds(PausingTime);
        gameObject.SetActive(false);
    }
}
