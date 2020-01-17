using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverHeadTextScript : MonoBehaviour
{
    private Text myText;

    [SerializeField]
    public float moveAmt;
    [SerializeField]
    public float moveSpeed;

    private Vector3[] moveDirs;
    private Vector3 myMoveDir;

    private bool canMove = false;

    private void Start()
    {

    }

    private void Update()
    {
        if (canMove) { transform.position = Vector3.MoveTowards(transform.position, transform.position + myMoveDir, moveAmt * (moveSpeed * Time.deltaTime)); }

    }

   // public void SetOverheadText(string textStr, Color textColor)
   // {
   //     myText = GetComponentInChildren<Text>();
   //     myText.color = textColor;
   //     myText.text = textStr;
   //     canMove = false;
   // }

    public void SetOverheadText(string textStr, Color textColor, OverheadTextManager.eMoveDirections moveDirection, float textMoveSpd, float textMoveAmt)
    {
        myText = GetComponentInChildren<Text>();
        myText.color = textColor;
        myText.text = textStr;

        moveSpeed = textMoveSpd;
        moveAmt = textMoveAmt;

        // Sets Text movement. For stationary text, send STATIC, otherwise
        //  choose a cardinal direction to send the text

        switch (moveDirection)
        {
            case OverheadTextManager.eMoveDirections.STATIC:
                canMove = false;
                break;
            case OverheadTextManager.eMoveDirections.N:
                myMoveDir = transform.up;
                canMove = true;
                break;
            case OverheadTextManager.eMoveDirections.NE:
                myMoveDir = (transform.up + transform.right);
                canMove = true;
                break;
            case OverheadTextManager.eMoveDirections.E:
                myMoveDir = transform.right;
                canMove = true;
                break;
            case OverheadTextManager.eMoveDirections.SE:
                myMoveDir = (-transform.up + transform.right);
                canMove = true;
                break;
            case OverheadTextManager.eMoveDirections.S:
                myMoveDir = -transform.up;
                canMove = true;
                break;
            case OverheadTextManager.eMoveDirections.SW:
                myMoveDir = (-transform.up + -transform.right);
                canMove = true;
                break;
            case OverheadTextManager.eMoveDirections.W:
                myMoveDir = -transform.right;
                canMove = true;
                break;
            case OverheadTextManager.eMoveDirections.NW:
                myMoveDir = (transform.up + -transform.right);
                canMove = true;
                break;
        }
        
    }


}
