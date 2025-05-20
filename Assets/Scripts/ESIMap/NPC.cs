using System;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public KeyCode useKey = KeyCode.E;
    public float interactDistance = 3f;

    public Dialogue dialogue;

    public int dialogueIndex;

    private Camera playerCamera;

    public enum GameStage
    {
        CierrePasilloF,
        Sangre
    }

    private GameObject npcs;

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
        }
    }

    void Update()
    {
        if (playerCamera == null)
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
        for (int g = 0; g < 3 && g <= i; g++)
            ChangeGameStage(g);
    }

    public void ChangeGameStage(int i)
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
            default:
                return;
        }

        dialogue.HideDialogue();

        GameState.Instance.Set("last_gamestage", i);
    }
}
