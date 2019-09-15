// This script is attached to the elevator rooms

using UnityEngine;

public class RoomElevatorAnimation : MonoBehaviour
{
    private Animator _Animator;

    void Start()
    {
        CheckReferences();
    }

    private void CheckReferences()
    {
        _Animator = GetComponentInChildren<Animator>();
        Debug.Assert(_Animator != null);
    }

    public void SetAnimation_OpenDoor()
    {
        if (!_Animator.GetCurrentAnimatorStateInfo(0).IsName("DoorOpened")) _Animator.SetTrigger("DoorOpenTrigger");
    }

    public void SetAnimation_CloseDoor()
    {
        if (!_Animator.GetCurrentAnimatorStateInfo(0).IsName("DoorClosedg")) _Animator.SetTrigger("DoorCloseTrigger");
    }
}     
