using System.Collections;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public KeyCode useKey = KeyCode.E;
    public float interactDistance = 3f;

    public Dialogue dialogue;

    public int dialogueIndex;

    private Camera playerCamera;

    private GameObject npcs;

    public string minigameCheckObjective;
    public int dialogeIndexOnWin;

    public int dialogueIndexOnLose;

    private bool HasFinishedLoading = false;

    void Start()
    {
        // Look for the player camera (which must have the "MainCamera" tag).
        playerCamera = Camera.main;

        if (playerCamera == null)
            Debug.LogWarning("No se ha encontrado ninguna cámara con el tag 'MainCamera'.");

        npcs = GameObject.Find("NPCs");

        if (GameState.Instance.Get("last_gamestage", out int last_gamestage))
        {
            if (GameState.Instance.Get("is_reset", out bool is_reset) && is_reset)
            {
                GameState.Instance.Set("is_reset", false);
                ChangeGameStageCyclic(last_gamestage);
            }

            StartCoroutine(WaitThenCheckGame());
        }
        else
            HasFinishedLoading = true;
    }

    IEnumerator WaitThenCheckGame()
    {
        yield return new WaitForSeconds(0.5f);

        if (GameState.Instance.Get("minigame", out string minigame) && minigame == minigameCheckObjective)
        {
            if (GameState.Instance.Get($"{minigame}_points", out int points) && GameState.Instance.Get($"{minigame}_objective", out int objective) && points >= objective)
                dialogue.TriggerDialogue(dialogeIndexOnWin);
            else
                dialogue.TriggerDialogue(dialogueIndexOnLose);
        }

        HasFinishedLoading = true;
    }

    void Update()
    {
        if (playerCamera == null || !HasFinishedLoading)
            return;

        if (Input.GetKeyDown(useKey) && !dialogue.IsWorking() && IsLookingAtUs())
            dialogue.TriggerDialogue(dialogueIndex);
    }

    bool IsLookingAtUs()
    {
        Ray ray = new(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
            return hit.transform == transform;

        return false;
    }

    public void ChangeGameStageCyclic(int i)
    {
        for (int g = 0; g < 7 && g <= i; g++)
            ChangeGameStage(g);
    }

    public void ChangeGameStage(int i)
    {
        ChangeGameStageNoHide(i);
        dialogue.HideDialogue();
    }

    public void ChangeGameStageNoHide(int i)
    {
        switch (i)
        {
            case 0:
                break;
            case 1:
                npcs.transform.Find("ClosePasilloF").gameObject.SetActive(true);
                break;
            case 2:
                npcs.transform.Find("Sangre").gameObject.SetActive(true);
                GameState.Instance.Set("samegame_objective", 500);
                break;
            case 3:
                npcs.transform.Find("Sangre2").gameObject.SetActive(true);
                dialogueIndex = 30;
                break;
            case 4:
                npcs.transform.Find("PasilloC").gameObject.SetActive(true);
                break;
            case 5:
                npcs.transform.Find("Gallinas").gameObject.SetActive(true);

                npcs.transform.Find("Sangre").gameObject.SetActive(false);
                npcs.transform.Find("ClosePasilloF").gameObject.SetActive(false);
                npcs.transform.Find("Sangre2").gameObject.SetActive(false);
                break;
            case 6:
                npcs.transform.Find("Final1").gameObject.SetActive(true);
                dialogueIndex = 49;
                GameState.Instance.Set("arkanoid_objective", 400);

                break;

            case 7:
                npcs.transform.Find("Gallinas").gameObject.SetActive(false);
                npcs.transform.Find("Final2").gameObject.SetActive(true);

                dialogueIndex = 57;
                break;
            default:
                return;
        }

        GameState.Instance.Set("last_gamestage", i);
    }
}
