using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;

public class UIManager : MonoBehaviour {

    public static UIManager Instance;

    public Text pickItemsText;
    public Slider healthBar;
    public Text AmmunitionDisplay;
    public GameObject Inventory;
    public GameObject player;
    public GameObject cameraHolder;
    public GameObject PauseMenu;

    public GameObject AI;

    public GameObject QuestDisplay;

    public GameObject QuestJournal;
    public Text QuestText;

    internal int MaxCarryableQuests = 10;
    internal int currentQuests;
 
    bool paused;
    bool inventoryOpen;
    bool questJournalOpen;

    void Awake() { Instance = this; }

    void Update()
    {
        if (Inventory.activeSelf && !paused && !inventoryOpen) Inventory.SetActive(false);
        if (QuestJournal.activeSelf && !paused && !questJournalOpen) QuestJournal.SetActive(false);

        #region Inventory
        if (Input.GetKeyUp(KeyCode.I))
        {
            if (!inventoryOpen && !paused)
            {
                ToggleInput(false);
                Inventory.SetActive(true);
                inventoryOpen = true;
                Pause();
                Cursor.lockState = CursorLockMode.Confined;
            }
            else if (inventoryOpen && !paused)
            {
                ToggleInput(true);
                Inventory.SetActive(false);
                inventoryOpen = false;
                UnPause();
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
        #endregion

        #region Pausing
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (!paused || (paused && inventoryOpen) || (paused && questJournalOpen)) {
                inventoryOpen = false;
                Inventory.SetActive(false);
                questJournalOpen = false;
                QuestJournal.SetActive(false);
                Pause();
                PauseMenu.SetActive(true);
            }
            else
            {
                UnPause();
                PauseMenu.SetActive(false);
            }
        }
        #endregion

        #region Toggle Cursor Lock

        if (Input.GetKeyUp(KeyCode.LeftAlt))
        {
            if(Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.Confined;
            }
            else if (Cursor.lockState == CursorLockMode.Confined)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }


        #endregion

        #region Quest Journal
        if (Input.GetKeyUp(KeyCode.L))
        {
            if (!questJournalOpen && !paused)
            {
                ToggleInput(false);
                QuestJournal.SetActive(true);
                for(int i = 0; i < currentQuests; i++)
                {
                    QuestJournal.transform.GetChild(0).GetChild(i).gameObject.SetActive(true);
                    QuestJournal.transform.GetChild(0).GetChild(i).name = PlayerManager.Instance.localPlayerData.currentQuests[i].ToString();
                }
                for(int i = currentQuests; i < MaxCarryableQuests; i++)
                {
                    QuestJournal.transform.GetChild(0).GetChild(i).gameObject.SetActive(false);
                }
                questJournalOpen = true;
                Pause();
                Cursor.lockState = CursorLockMode.Confined;
            }
            else if (questJournalOpen && !paused)
            {
                ToggleInput(true);
                QuestJournal.SetActive(false);
                questJournalOpen = false;
                UnPause();
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
        #endregion

    }

    void ToggleInput(bool state)
    {
        if (state)
        {
            player.GetComponent<InputHandler>().enabled = true;
            cameraHolder.GetComponent<FreeCameraLook>().enabled = true;
            cameraHolder.GetComponent<FreeCameraLook>().lockCursor = true;
        }
        else
        {
            player.GetComponent<InputHandler>().enabled = false;
            cameraHolder.GetComponent<FreeCameraLook>().lockCursor = false;
            cameraHolder.GetComponent<FreeCameraLook>().enabled = false;
        }
    }

    public void Pause()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Time.timeScale = 0;
        ToggleInput(false);
        paused = true;
    }

    public void UnPause()
    {
        Time.timeScale = 1.0f;
        ToggleInput(true);
        paused = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void UpdateHealthBar(float curHealth)
    {
        healthBar.value = curHealth;
    }

    public void UpdateAmmunition(int curMagazineAmmo, int currentAmmoOnPerson)
    {
        AmmunitionDisplay.text = curMagazineAmmo + " / " + currentAmmoOnPerson;
    }

    public void QuestPopUp(List<SavedQuest> npcQuests, bool popUp, GameObject ai)
    {
        AI = ai;

        if (!popUp)
        {
            QuestDisplay.SetActive(false);
            return;
        }
        bool atLeast1 = false;
        QuestDisplay.SetActive(true);
        for(int i = 0; i < npcQuests.Count; i++)
        {
            if (!npcQuests[i].inProgress)
            {
                atLeast1 = true;
                QuestDisplay.transform.GetChild(0).GetChild(i).GetComponent<Text>().text = npcQuests[i].ToString();

                QuestDisplay.transform.GetChild(0).GetChild(i).name = npcQuests[i].ID.ToString();
            }
        }

        if (!atLeast1) QuestDisplay.SetActive(false);
    }

    public void GiveQuest(GameObject go)
    {
        PlayerManager.Instance.QuestAcquisition(AI.GetComponent<AIManagerRevamp>().FindQuestByID(int.Parse(go.name)));
        UnPause();
        currentQuests++;
    }

    public void DisplayQuestDescription()
    {
        //QuestDisplay.transform.GetChild(1).GetChild(1).GetComponent<Text>().text = 
    }
}
