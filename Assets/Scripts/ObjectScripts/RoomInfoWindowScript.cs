using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MySpace.ObjectPooling;
using MySpace;

public class RoomInfoWindowScript : MonoBehaviour
{
    public TextMeshProUGUI RoomNameText;
    public TextMeshProUGUI RoomInfoText;

    public Transform Grid, poolParent;

    public GameObject ManUIPrefab;
    public GameObject StatUIPrefab;
    public GameObject DescUIPrefab;

    private ObjectPool<ManPopUpScript> manPool;
    private ObjectPool<StatPopUpScript> statPool;
    private ObjectPool descPool;

    void Awake ()
    {
        manPool = new ObjectPool<ManPopUpScript>(Grid, poolParent, ManUIPrefab);
        statPool = new ObjectPool<StatPopUpScript>(Grid, poolParent, StatUIPrefab);
        descPool = new ObjectPool(Grid, poolParent, DescUIPrefab);
        Deactivate();
    }

    private Room_Bedroom currRoom;
    private Room_WorkQuarters currQuarters;
    private float tickCounter;
    private void Update()
    {
        float deltaTime;
        TimeManager.Ref.GetScaledDeltaTime(TimeManager.TimeScalar.HOOTEL, out deltaTime);

        if (gameObject.activeInHierarchy)
        {
            tickCounter += deltaTime;
            if (currRoom != null)
            {
                if (tickCounter >= 2)
                {
                    tickCounter = 0;
                    statPool.PoolAllObjects();
                    statPool.GetObject().SetComponents(null/*need to put the dirty sprite here, can use get sprite for any of the other stats that might be on rooms.*/, "Stankiness", Mathf.Round((1 - currRoom.Cleanliness) * 100) + "%");
                }
            }
            else if (currQuarters != null)
            {
                if (tickCounter >= 2)
                {
                    tickCounter = 0;
                    statPool.PoolAllObjects();

                }
            }
            
            
        }
    }

    public void Activate(Guid roomId)
    {
        Activate(RoomManager.Ref.GetRoomData(roomId).RoomScript);
    }

    public void Activate(RoomScript RoomScript)
    {
        if (gameObject.activeInHierarchy)
            ResetInfo();

        RoomNameText.text = RoomScript.RoomData.RoomName;
        if (RoomScript is Room_Bedroom)
        {
            currRoom = RoomScript as Room_Bedroom;
            foreach (ManRef<ManScript_Guest> man in RoomScript.GetAllMenOfType<ManScript_Guest>())
            {
                manPool.GetObject().SetComponents(man.ManScript.GetSprite(), man.ManScript.ManName);
            }

            statPool.GetObject().SetComponents(null/*need to put the dirty sprite here, can use get sprite for any of the other stats that might be on rooms.*/, "Stankiness", Mathf.Round((1 - currRoom.Cleanliness) * 100) + "%");
        }
        else if (RoomScript is Room_WorkQuarters)
        {
            currQuarters = RoomScript as Room_WorkQuarters;
            foreach (ManRef<ManScript_Worker> man in RoomScript.GetAllMenOfType<ManScript_Worker>())
            {
                manPool.GetObject().SetComponents(man.ManScript.GetSprite(), man.ManScript.ManName);
            }

            string temp = "Specialty Stats Used:";

            for (int i = 0; i < currQuarters.specialStatsUsed.Length; i++)
            {
                temp += (i > 0 ? ", " : " ") + currQuarters.specialStatsUsed[i];
            }
            temp += ".";

            descPool.GetObject().GetComponentInChildren<TextMeshProUGUI>().text = temp;
        }

        RoomInfoText.text = RoomScript.RoomData.RoomDescription;
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        if (!gameObject.activeInHierarchy)
            return;
        ResetInfo();
        if (GameManager.Debug)
            print("Disabling object");
        gameObject.SetActive(false);
        
    }

    public void ResetInfo()
    {
        manPool.PoolAllObjects();
        statPool.PoolAllObjects();
        descPool.PoolAllObjects();
        currRoom = null;
        currQuarters = null;
        tickCounter = 0;
    }
}

namespace MySpace
{
    namespace ObjectPooling
    {
        public class ObjectPool<T> where T : MonoBehaviour
        {
            private List<T> objectPool;

            private Transform parent, pooledParent;
            private GameObject prefab;

            public ObjectPool(Transform UIParent, Transform poolParent, GameObject prefab, T[] premades)
            {
                parent = UIParent;
                objectPool = new List<T>(premades);
                this.prefab = prefab;
                pooledParent = poolParent;
            }

            public ObjectPool(Transform UIParent, Transform poolParent, GameObject prefab)
            {
                parent = UIParent;
                objectPool = new List<T>();
                this.prefab = prefab;
                pooledParent = poolParent;
            }

            public T GetObject()
            {
                for (int i = 0; i < objectPool.Count; i++)
                {
                    if (objectPool[i].gameObject.activeInHierarchy)
                        continue;
                    objectPool[i].transform.parent = parent;
                    objectPool[i].gameObject.SetActive(true);
                    return objectPool[i];
                }
                objectPool.Add(GameObject.Instantiate(prefab, parent).GetComponent<T>());
                return objectPool[objectPool.Count - 1];
            }

            public T GetObject(Vector3 spawnPosition)
            {
                for (int i = 0; i < objectPool.Count; i++)
                {
                    if (objectPool[i].gameObject.activeInHierarchy)
                        continue;
                    objectPool[i].transform.parent = parent;
                    objectPool[i].transform.position = spawnPosition;
                    return objectPool[i];
                }
                objectPool.Add(GameObject.Instantiate(prefab, parent).GetComponent<T>());
                objectPool[objectPool.Count - 1].transform.position = spawnPosition;
                return objectPool[objectPool.Count - 1];
            }

            public void PoolAllObjects()
            {
                for (int i = 0; i < objectPool.Count; i++)
                {
                    if (!objectPool[i].gameObject.activeInHierarchy)
                        continue;
                    objectPool[i].gameObject.SetActive(false);
                    objectPool[i].transform.SetParent(pooledParent);
                }
            }

            public void PoolObject(T thing)
            {
                if (!objectPool.Contains(thing))
                    objectPool.Add(thing);
                thing.gameObject.SetActive(false);
                thing.transform.parent = pooledParent;
            }
        }

        public class ObjectPool
        {
            private List<GameObject> objectPool;

            private Transform parent, pooledParent;
            private GameObject prefab;

            public ObjectPool(Transform UIParent, Transform poolParent, GameObject prefab, GameObject[] premades)
            {
                parent = UIParent;
                objectPool = new List<GameObject>(premades);
                this.prefab = prefab;
                pooledParent = poolParent;
            }

            public ObjectPool(Transform UIParent, Transform poolParent, GameObject prefab)
            {
                parent = UIParent;
                objectPool = new List<GameObject>();
                this.prefab = prefab;
                pooledParent = poolParent;
            }

            public GameObject GetObject()
            {
                for (int i = 0; i < objectPool.Count; i++)
                {
                    if (objectPool[i].gameObject.activeInHierarchy)
                        continue;
                    objectPool[i].transform.parent = parent;
                    objectPool[i].SetActive(true);
                    return objectPool[i];
                }
                objectPool.Add(GameObject.Instantiate(prefab, parent));
                
                return objectPool[objectPool.Count - 1];
            }

            public GameObject GetObject(Vector3 spawnPosition)
            {
                for (int i = 0; i < objectPool.Count; i++)
                {
                    if (objectPool[i].gameObject.activeInHierarchy)
                        continue;
                    objectPool[i].transform.parent = parent;
                    objectPool[i].transform.position = spawnPosition;
                    return objectPool[i];
                }
                objectPool.Add(GameObject.Instantiate(prefab, parent));
                objectPool[objectPool.Count - 1].transform.position = spawnPosition;
                return objectPool[objectPool.Count - 1];
            }

            public void PoolAllObjects()
            {
                for (int i = 0; i < objectPool.Count; i++)
                {
                    if (!objectPool[i].activeInHierarchy)
                        continue;
                    objectPool[i].gameObject.SetActive(false);
                    objectPool[i].transform.SetParent(pooledParent);
                }
            }

            public void PoolObject(GameObject thing)
            {
                if (!objectPool.Contains(thing))
                    objectPool.Add(thing);
                thing.gameObject.SetActive(false);
                thing.transform.parent = pooledParent;
            }
        }
    }
}
