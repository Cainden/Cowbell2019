using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySpace;
using System;

public class Room_Bedroom : RoomScript
{
    #region Serialized Variables
    public const float cleanTickTime = 2;

    [Tooltip("The cost of this room to stay in for guests")]
    public int RentCost = 50;

    #endregion

    public override Enums.ManRole RoomRole => Enums.ManRole.Guest;

    #region Private Variables
    MeshRenderer thisRend;

    private float fElapsedTime = 0;

    // Room Attributes
    private float cleanliness; //For debugging to the inspector
    public float Cleanliness { get { return cleanliness; } set { cleanliness = value; } }

    public const float cleanlinessThreshhold = 0.4f;

    //Here as a fillable reference instead of creating a local reference-type variable each frame of update
    //ManRef<ManScript_Guest> occupantG;
    //ManRef<ManScript_Worker> occupantW;
    #endregion

    protected override void Start()
    {
        //We are calling start in Self_Initialize, so there's no need to have it called again here.
        //base.Start();
    }

    protected override void Update()
    {
        base.Update();

        fElapsedTime += Time.deltaTime;
        if (RoomIsActive && fElapsedTime > cleanTickTime)
        {
            int numMen = CountMen();
            fElapsedTime = 0.0f;
            bool bStank = false; // If true deteriorates room
            bool bClean = false; // If true cleans room. 
            float cleanFactor = 0, dirtyFactor = 0;
            //Check to see if Room is occupied by Guest and decrease cleanliness. 
            //If room is occupied by Cleaners, increase cleanliness.
            //Make it so Cleaners cannot occupy the rooms at the same time as guests?

            for (int i = 0; i < numMen; i++)
            {
                Guid id = RoomData.ManSlotsAssignments[i];
                if (id == Guid.Empty)
                    continue;

                if (!(manManRef.GetManData(id).ManScript.State == Enums.ManStates.None))
                    continue;

                if (manManRef.IsManTypeOf<ManScript_Guest>(id))
                {
                    bStank = true;
                    dirtyFactor += (manManRef.GetManData(id).ManScript as ManScript_Guest).dirtyFactor;
                }
                else if (manManRef.IsManTypeOf<ManScript_Worker>(id))
                {
                    bClean = true;
                    cleanFactor += (manManRef.GetManData(id).ManScript as ManScript_Worker).GetSpecialtyStatValue(MySpace.Stats.SpecialtyStat.StatType.Physicality);
                }
                //occupant = null;
            }


            if (bStank)
            {
                Cleanliness -= RoomManager.Ref.DirtinessSpeedRatio * dirtyFactor * cleanTickTime * Time.deltaTime;

                if (Cleanliness < 0)
                    Cleanliness = 0;
            }
            if (bClean)
            {
                Cleanliness += RoomManager.Ref.CleanSpeedRatio * cleanFactor * cleanTickTime * Time.deltaTime;
                if (Cleanliness > 1)
                    Cleanliness = 1;
            }



            thisRend.material.SetColor("_Color", Color.Lerp(Color.green, Color.white, Cleanliness));
        }
    }

    public void SelfInitialize(GridIndex leftMostIndex)
    {
        //HAD TO DO ALL OF THE CREATEROOM INITIALIZATION HERE BECAUSE IT CANNOT BE INSTANTIATED IN THE WRONG POSITION THROUGH CREATEROOM
        RoomDefData RoomDefData = new RoomDefData("BedroomBase", gameObject, Enums.RoomSizes.Size2, Enums.RoomTypes.Bedroom, Enums.RoomCategories.Miscellaneous, 2, CreateNewArray(2), "The room in which all Hootel guests stay. These rooms become dirtied by guests over time and need to be cleaned by cleaners.", 0, Enums.RoomOverUnder.Over, false, null);

        RoomData = new RoomInstanceData();
        RoomData.RoomId = Guid.NewGuid();
        RoomData.RoomName = RoomDefData.RoomName;
        RoomData.RoomSize = RoomDefData.RoomSize;
        RoomData.RoomCategory = RoomDefData.RoomCategory;
        RoomData.RoomType = RoomDefData.RoomType;
        RoomData.RoomOverUnder = RoomDefData.RoomOverUnder;
        RoomData.ManSlotCount = RoomDefData.ManSlotCount;
        RoomData.ManSlotsPositions = new Vector3[RoomDefData.ManSlotCount]; // Data will be set by object script on Start()
        RoomData.ManSlotsRotations = new Quaternion[RoomDefData.ManSlotCount]; // Data will be set by object script on Start()
        RoomData.ManSlotsAssignments = new Guid[RoomDefData.ManSlotCount];
        RoomData.OwnerSlotsAssignments = new Guid[RoomDefData.ManSlotCount];
        RoomData.RoomScript = this;
        RoomData.RoomDescription = RoomDefData.RoomDescription;

        for (int i = 0; i < RoomData.ManSlotCount; i++) RoomData.ManSlotsAssignments[i] = Guid.Empty;
        RoomData.ManWorkingStates = RoomDefData.ManWorkingStates;
        RoomData.CoveredIndizes = GridManager.Ref.GetOccupiedindizes(RoomDefData.RoomSize, leftMostIndex);

        for (int i = 0; i < RoomData.ManSlotCount; i++) RoomData.OwnerSlotsAssignments[i] = Guid.Empty;

        GridManager.Ref.RegisterAtGrid(RoomData.RoomSize, RoomData.RoomId, leftMostIndex, false);
        RoomManager.Ref.AddRoom(RoomData.RoomId, new RoomRef(gameObject, this));

        #region Manual assignment of grid index movement directions
        //enable the  way to leave the room through the door
        GridManager.Ref.AddMovementDirectionToGridIndex(RoomData.CoveredIndizes[1], Enums.MoveDirections.Front);
        //enable left movement to the door index
        GridManager.Ref.AddMovementDirectionToGridIndex(RoomData.CoveredIndizes[1], Enums.MoveDirections.Left);
        //enable right movement to the index inside the room
        GridManager.Ref.AddMovementDirectionToGridIndex(RoomData.CoveredIndizes[0], Enums.MoveDirections.Right);
        //disable front movement from the other index
        GridManager.Ref.RemoveMovementDirectionFromGridIndex(RoomData.CoveredIndizes[0], Enums.MoveDirections.Front);
        #endregion

        Cleanliness = 1.0f;
        thisRend = GetComponentInChildren<MeshRenderer>();
        thisRend.material.color = Color.white;
        base.Start();

        Enums.ManStates[] CreateNewArray(int length)
        {
            var ar = new Enums.ManStates[length];
            for (int i = 0; i < length; i++)
            {
                ar[i] = Enums.ManStates.Idle;
            }
            return ar;
        }
    }
}
