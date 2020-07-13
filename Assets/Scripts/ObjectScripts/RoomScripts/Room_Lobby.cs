using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySpace;

public class Room_Lobby : RoomScript
{
    public override Enums.ManRole RoomRole => Enums.ManRole.None;

    [Header("Door Variables")]
    [Header("===============================================================================================================================================================================")]
    public Transform LeftDoor;
    public Transform RightDoor;

    [SerializeField] float L_BaseRot, L_InnerRot;
    [SerializeField] float R_BaseRot, R_InnerRot;

    private float L_XRot, L_YRot, R_XRot, R_YRot;

    private int doorOpening, lastOpen;
    private float t = 0;
    private int charsIn = 0;
    private float doorSpeed;

    public void OpenDoorOutsideEnter(float doorSpeed = 1)
    {
        charsIn++;
        this.doorSpeed = doorSpeed;
        if (t > 0)
        {
            doorOpening = lastOpen;
        }
        if (doorOpening > 0)
            return;
        doorOpening = lastOpen = 2;
    }

    public void OpenDoorInsideEnter(float doorSpeed = 1)
    {
        charsIn++;
        this.doorSpeed = doorSpeed;
        if (t > 0)
        {
            doorOpening = lastOpen;
        }
        if (doorOpening > 0)
            return;
        doorOpening = lastOpen = 1;
    }

    public void CloseDoor()
    {
        charsIn--;
        if (charsIn <= 0)
        {
            doorOpening = 0;
        }
    }

    protected override void Start()
    {
        base.Start();

        L_XRot = LeftDoor.transform.rotation.eulerAngles.x;
        L_YRot = LeftDoor.transform.rotation.eulerAngles.y;
        R_XRot = RightDoor.transform.rotation.eulerAngles.x;
        R_YRot = RightDoor.transform.rotation.eulerAngles.y;
    }

    protected override void Update()
    {
        base.Update();

        if (t > 0 && doorOpening == 0)
        {
            t -= Time.deltaTime * doorSpeed * 2;
            if (t < 0) t = 0;

            LeftDoor.transform.rotation = Quaternion.Euler(Vector3.Lerp(new Vector3(L_XRot, L_YRot, L_BaseRot), new Vector3(L_XRot, L_YRot, (lastOpen == 1 ? -L_InnerRot : L_InnerRot)), t));
            RightDoor.transform.rotation = Quaternion.Euler(Vector3.Lerp(new Vector3(R_XRot, R_YRot, R_BaseRot), new Vector3(R_XRot, R_YRot, (lastOpen == 1 ? -R_InnerRot : R_InnerRot)), t));
        }
        else if (doorOpening == 1 && t < 1)
        {
            t += Time.deltaTime * doorSpeed * 2;
            if (t > 1)
                t = 1;

            LeftDoor.transform.rotation = Quaternion.Euler(Vector3.Lerp(new Vector3(L_XRot, L_YRot, L_BaseRot), new Vector3(L_XRot, L_YRot, -L_InnerRot), t));
            RightDoor.transform.rotation = Quaternion.Euler(Vector3.Lerp(new Vector3(R_XRot, R_YRot, R_BaseRot), new Vector3(R_XRot, R_YRot, -R_InnerRot), t));
        }
        else if (doorOpening == 2 && t < 1)
        {
            t += Time.deltaTime * doorSpeed * 2;
            if (t > 1)
                t = 1;

            LeftDoor.transform.rotation = Quaternion.Euler(Vector3.Lerp(new Vector3(L_XRot, L_YRot, L_BaseRot), new Vector3(L_XRot, L_YRot, L_InnerRot), t));
            RightDoor.transform.rotation = Quaternion.Euler(Vector3.Lerp(new Vector3(R_XRot, R_YRot, R_BaseRot), new Vector3(R_XRot, R_YRot, R_InnerRot), t));

        }
        if (GameManager.Debug && t != 0)
            print(t);
    }
}
