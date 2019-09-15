using System.Collections;
using UnityEngine;

public class GuiUserInfoSmallScript : MonoBehaviour
{
    public UnityEngine.UI.Text InfoText = null; // To be set by editor

    private bool _IsMoving = false;
    private float _PosY1 = 128.0f * 5;
    private float _PosY2 = 128.0f * 3;

    private readonly float TravelSpeed = 800.0f;
    private readonly float PausingTime = 3.0f;

    void Start ()
    {
        gameObject.SetActive(false);
        Debug.Assert(InfoText != null);
    }

    void Update()
    {
        if (!_IsMoving) return;

        float distance = transform.position.y - _PosY2;
        float travel = TravelSpeed * Time.deltaTime;

        if (travel > distance) // Snap
        {
            transform.position = new Vector3(transform.position.x, _PosY2);
            _IsMoving = false;
            StartCoroutine(updateCoroutine());
        }
        else
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - travel);
        }
    }

    public void StartInfoText(string sInfoText)
    {
        InfoText.text = sInfoText;
        transform.position = new Vector3(transform.position.x, _PosY1);
        StopAllCoroutines();
        gameObject.SetActive(true);
        _IsMoving = true;
    }

    private IEnumerator updateCoroutine()
    {
        yield return new WaitForSeconds(PausingTime);
        gameObject.SetActive(false);
        StopAllCoroutines();
    }
}
