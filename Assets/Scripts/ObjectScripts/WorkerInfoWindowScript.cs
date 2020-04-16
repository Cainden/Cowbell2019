using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MySpace;
using MySpace.Stats;
using TMPro;

public class WorkerInfoWindowScript : MonoBehaviour
{
    #region Serialized Variables
    public GameObject workerParent;

    public TextMeshProUGUI nameText, roleText;
    public Image workerImage;

    public Image physicalityFrontSlider, physicalityRearSlider;
    public Image professionalismFrontSlider, professionalismRearSlider;
    public Image intelligenceFrontSlider, intelligenceRearSlider;

    public TextMeshProUGUI genstat1Name;
    public Image genstat1FrontSlider, genstat1RearSlider;

    public Slider physicalityExpBar, intelligenceExpBar, professionalismExpBar;

    //public Button fireButton, grabButton, 
    #endregion

    private ManScript_Guest guest;
    private ManScript_Worker worker;
    private bool active = false;

    private void Start()
    {
        DeActivate();
        physicalityRearSlider.fillAmount = 0;
        professionalismRearSlider.fillAmount = 0;
        intelligenceRearSlider.fillAmount = 0;
        genstat1RearSlider.fillAmount = 0;
    }

    public void Activate(System.Guid man)
    {
        worker = ManManager.Ref.GetManData(man).ManScript as ManScript_Worker;
        if (worker != null)
            SetupWorker();
        else
        {
            guest = ManManager.Ref.GetManData(man).ManScript as ManScript_Guest;
            if (guest != null)
                SetupGuest();
        }
    }

    public void Activate(ManScript_Worker worker)
    {
        this.worker = worker;
        SetupWorker();
    }

    public void Activate(ManScript_Guest guest)
    {
        this.guest = guest;
        SetupGuest();
    }

    public void DeActivate()
    {
        active = false;
        gameObject.SetActive(false);
    }

    private void SetupWorker()
    {
        gameObject.SetActive(true);
        workerParent.SetActive(true);
        active = true;
        nameText.text = worker.ManName;
        roleText.text = worker.role.ToString();

        genstat1Name.text = "Speed";
        genstat1FrontSlider.fillAmount = worker.GetGeneralStatValue(GeneralStat.StatType.Speed) / Stat.StatMax;

        //workerImage.sprite = worker.

        StartCoroutine(InfoUpdateWorker());
    }

    private void SetupGuest()
    {
        gameObject.SetActive(true);
        workerParent.SetActive(false);
        active = true;
        nameText.text = guest.ManName;

        genstat1Name.text = "Dirtiness";
        genstat1FrontSlider.fillAmount = guest.dirtyFactor / Stat.StatMax;
    }

    private IEnumerator InfoUpdateWorker()
    {
        while (active && worker)
        {
            physicalityFrontSlider.fillAmount = worker.GetSpecialtyStatValue(SpecialtyStat.StatType.Physicality) / Stat.StatMax;
            //physicalityRearSlider.fillAmount = worker. ???
            intelligenceFrontSlider.fillAmount = worker.GetSpecialtyStatValue(SpecialtyStat.StatType.Intelligence) / Stat.StatMax;
            //intelligenceRearSlider.fillAmount = worker. ???
            professionalismFrontSlider.fillAmount = worker.GetSpecialtyStatValue(SpecialtyStat.StatType.Professionalism) / Stat.StatMax;




            yield return null;
        }
    }
}
