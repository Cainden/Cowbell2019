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

    public Image happinessImage;

    public Image physicalityFrontSlider, physicalityRearSlider;
    public Image professionalismFrontSlider, professionalismRearSlider;
    public Image intelligenceFrontSlider, intelligenceRearSlider;

    public TextMeshProUGUI genstat1Name;
    public Image genstat1FrontSlider, genstat1RearSlider;
    public TextMeshProUGUI genstat2Name;
    public Image genstat2FrontSlider, genstat2RearSlider;

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
        genstat2RearSlider.fillAmount = 0;
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

        genstat1Name.gameObject.SetActive(true);
        genstat1Name.text = "Speed";
        genstat1FrontSlider.fillAmount = worker.GetGeneralStatValue(GeneralStat.StatType.Speed) / Stat.StatMax;

        genstat2Name.gameObject.SetActive(true);
        genstat2Name.text = "Tiredness";
        genstat2FrontSlider.fillAmount = worker.currentTiredness / ManScript_Worker.tirednessMax;

        //The base value won't change, so there's no need to have it call this multiple times.
        professionalismFrontSlider.fillAmount = worker.GetSpecialtyStat(SpecialtyStat.StatType.Professionalism).BaseValue / Stat.StatMax;
        intelligenceFrontSlider.fillAmount = worker.GetSpecialtyStat(SpecialtyStat.StatType.Intelligence).BaseValue / Stat.StatMax;
        physicalityFrontSlider.fillAmount = worker.GetSpecialtyStat(SpecialtyStat.StatType.Physicality).BaseValue / Stat.StatMax;

        //workerImage.sprite = worker.

        happinessImage.gameObject.SetActive(false);
        //happinessImage.gameObject.SetActive(true);
        //happinessImage.sprite = worker.GetSpriteFromMood();

        StartCoroutine(InfoUpdate());
    }

    private void SetupGuest()
    {
        gameObject.SetActive(true);
        workerParent.SetActive(false);
        active = true;
        nameText.text = guest.ManName;

        genstat1Name.gameObject.SetActive(true);
        genstat1Name.text = "Dirtiness";
        genstat1FrontSlider.fillAmount = guest.GetGeneralStatValue(GeneralStat.StatType.Dirtiness) / Stat.StatMax;

        genstat2Name.gameObject.SetActive(true);
        genstat2Name.text = "Hapiness";
        genstat2FrontSlider.fillAmount = guest.GetHappiness() * 0.01f;//multiply by 0.01 here because mood is from 0 to 100, fill amount goes from 0 to 1.

        //happinessImage.gameObject.SetActive(false);
        happinessImage.gameObject.SetActive(true);
        happinessImage.sprite = guest.GetSpriteFromMood();




        //genstat2Name.gameObject.SetActive(false);
    }

    private IEnumerator InfoUpdate()
    {
        while (active)
        {
            if (worker != null)
            {
                physicalityRearSlider.fillAmount = worker.GetSpecialtyStatValue(SpecialtyStat.StatType.Physicality) / Stat.StatMax;
                intelligenceRearSlider.fillAmount = worker.GetSpecialtyStatValue(SpecialtyStat.StatType.Intelligence) / Stat.StatMax;
                professionalismRearSlider.fillAmount = worker.GetSpecialtyStatValue(SpecialtyStat.StatType.Professionalism) / Stat.StatMax;
                genstat2FrontSlider.fillAmount = worker.currentTiredness / ManScript_Worker.tirednessMax;

                physicalityExpBar.value = worker.GetSpecialtyStat(SpecialtyStat.StatType.Physicality).GetCurrentExp;
                intelligenceExpBar.value = worker.GetSpecialtyStat(SpecialtyStat.StatType.Intelligence).GetCurrentExp;
                professionalismExpBar.value = worker.GetSpecialtyStat(SpecialtyStat.StatType.Professionalism).GetCurrentExp;

                //Workers currently contain a happiness value, but it is not currently used, so the code is commented out.
                //if (happinessImage.gameObject.activeInHierarchy) happinessImage.fillAmount = worker.GetHappiness() * 0.01f;//multiply by 0.01 here because mood is from 0 to 100, fill amount goes from 0 to 1.
                //happinessImage.sprite = worker.GetSpriteFromMood();
            }

            if (guest != null)
            {
                genstat2FrontSlider.fillAmount = guest.GetHappiness() * 0.01f;//multiply by 0.01 here because mood is from 0 to 100, fill amount goes from 0 to 1.
                happinessImage.sprite = guest.GetSpriteFromMood();
            }



            yield return new WaitForSecondsRealtime(0.5f);
        }
    }
}
