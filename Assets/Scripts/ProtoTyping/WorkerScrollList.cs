using MySpace;
using UnityEngine;

public class WorkerScrollList : MonoBehaviour
{
	public Transform contentPanel;
	public WorkerToHireObjectPool hireObjectPool;

	private PanelContainer m_panelContainer;

    public void Awake()
    {
		m_panelContainer = GetComponent<PanelContainer>();
		m_panelContainer?.RegisterOnPanelPreOpen(OnPanelPreOpen);
		m_panelContainer?.RegisterOnPanelPreClose(OnPanelPreClose);
    }

	public void OnPanelPreOpen()
    {
		SetAllChildrenActiveRecursively(true, gameObject);
		DisplayWorkersToHire();
    }

	public void OnPanelPreClose()
	{
		SetAllChildrenActiveRecursively(false, gameObject);
	}

	public void DisplayWorkersToHire()
	{
		RefreshList();
		AddWorkerButtons();
	}

	protected void SetAllChildrenActiveRecursively(bool activeState, GameObject currentObject)
	{
		currentObject.SetActive(activeState);
		foreach (Transform child in currentObject.transform)
		{
			SetAllChildrenActiveRecursively(activeState, child.gameObject);
		}
	}

	private void AddWorkerButtons()
	{
		//	Worker construction data can contain all information that will need to be displayed to the player about a worker they might want to hire
		foreach (WorkerConstructionData man in ManManager.Ref.hireList)
		{
			GameObject newButton = hireObjectPool.GetObject();

			newButton.transform.SetParent(contentPanel, true);

			WorkerToHire workerToHire = newButton.GetComponent<WorkerToHire>();

			if (workerToHire != null)
			{
				workerToHire.Setup(man);

				UnityEngine.UI.Button currentButton = newButton.GetComponent<UnityEngine.UI.Button>();
				currentButton?.onClick.AddListener(() => m_panelContainer.CloseParents(-1));

				GenerateHeadshot headshot = newButton.GetComponent<GenerateHeadshot>();
				if (headshot != null)
				{
                    headshot.CreateHeadshot(man.sprite);
				}
			}
		}
	}

	private void RefreshList()
	{
		foreach (WorkerPooledObject button in contentPanel.GetComponentsInChildren<WorkerPooledObject>())
		{
			button.pool.ReturnObject(button.gameObject);
		}
	}
}
