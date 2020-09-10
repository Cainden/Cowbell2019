using System.Collections;
using UnityEngine;
using TMPro;

public class GuiUserInfoSmallScript : MonoBehaviour
{
    public TextMeshProUGUI TitleText;
    public TextMeshProUGUI InfoText = null; // To be set by editor
    
    Animator anim;
    bool popUp;

    private readonly float PausingAnimTime = 3.5f;//may need to be adjusted based on idle animation
    private readonly float PausingObjOffTime = 5.0f;//if animation gets cut off by object vanishing make this higher number

    void Start ()
    {
        gameObject.SetActive(false);
        Debug.Assert(InfoText != null);
        Debug.Assert(TitleText != null);
        TitleText.text = "";
        //Get Animator and set up popUp to turn Paramater on/off
        anim = gameObject.GetComponent<Animator>();
        popUp = false;
    }

    void Update()
    {
        //set animation bool Activation to true or false based on popUp. 
        if (popUp == false)
            anim.SetBool("Activation", false);
        else if (popUp == true)
            anim.SetBool("Activation", true);
    }

    public void StartInfoText(string sInfoText, string sTitleText = "") //called and filled from GuiManager script
    {
        TitleText.text = sTitleText;
        InfoText.text = sInfoText;
        //When info called by another script also play anim and stop anim.
        ActivateAnim();
        StartCoroutine(DeactivateAnimCoroutine());
    }

    private void ActivateAnim()
    {
        StopAllCoroutines();
        gameObject.SetActive(true);
        popUp = true;
    }

    private IEnumerator DeactivateAnimCoroutine()
    {
        yield return new WaitForSeconds(PausingAnimTime);//Pause time for popup to be legible
        popUp = false;

        yield return new WaitForSeconds(PausingObjOffTime); //Pause before turning off popup
        gameObject.SetActive(false);
    }
}
