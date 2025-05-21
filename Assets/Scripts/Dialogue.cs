using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Dialogue : MonoBehaviour
{
    [System.Serializable]
    public struct DialogueItem
    {
        public string displayName;

        public List<string> strings;

        public UnityEvent onClose;
    }

    public List<DialogueItem> dialogues;

    public TMP_Text textDisplayName;
    public TMP_Text textString;

    public Button nextString;

    public float textSpeed = 0.1f;

    private int currentDialogueItem = 0;
    private int currentDialogueItemString = 0;

    private CanvasGroup canvasGroup;

    private bool isWorking = false;

    private bool isAddingText = false;

    private bool canvasIsFullyShown = false;

    public bool IsWorking() => isWorking;

    public Coordinator coordinator;

    public void HideDialogue()
    {
        if (!isWorking)
            return;

        StartCoroutine(FadeOutCanvas(1f));
        nextString.gameObject.SetActive(false);
    }

    public void TriggerDialogue(int index)
    {
        if (index >= 0 && index < dialogues.Count)
        {
            if (dialogues[index].strings.Count == 0)
                return;

            currentDialogueItem = index;

            currentDialogueItemString = 0;

            textDisplayName.text = dialogues[currentDialogueItem].displayName;
            textString.text = "";

            nextString.gameObject.SetActive(true);

            StartCoroutine(AnimateText(isWorking ? 0f : 1f));

            if (!isWorking)
                StartCoroutine(FadeInCanvas(1f));
        }
    }

    public void TriggerNextString()
    {
        if (!isWorking)
            return;

        if (currentDialogueItemString + 1 >= dialogues[currentDialogueItem].strings.Count)
        {
            if (dialogues[currentDialogueItem].onClose == null || dialogues[currentDialogueItem].onClose.GetPersistentEventCount() == 0)
            {
                StartCoroutine(FadeOutCanvas(1f));
                nextString.gameObject.SetActive(false);
            }
            else
                dialogues[currentDialogueItem].onClose?.Invoke();
        }
        else
        {
            ++currentDialogueItemString;
            textString.text = "";

            StartCoroutine(AnimateText(0f));
        }
    }

    public void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        nextString.onClick.AddListener(TryTriggerNextString);
    }

    private void TryTriggerNextString()
    {
        // Ignore button presses if the canvas is still transitioning.
        if (!canvasIsFullyShown)
            return;

        if (isAddingText)
        {
            isAddingText = false;

            textString.text = dialogues[currentDialogueItem].strings[currentDialogueItemString];
        }
        else
            TriggerNextString();
    }

    private IEnumerator AnimateText(float waitTime)
    {
        if (waitTime != 0)
            yield return new WaitForSeconds(waitTime);

        isAddingText = true;

        foreach (char letter in dialogues[currentDialogueItem].strings[currentDialogueItemString].ToCharArray())
        {
            // If the user doesn't want to wait for the animation to complete, just end here.
            if (!isAddingText)
                break;

            textString.text += letter;

            yield return new WaitForSeconds(textSpeed);
        }

        isAddingText = false;
    }

    private IEnumerator FadeInCanvas(float t)
    {
        canvasGroup.interactable = true;
        coordinator.SetBeingAnimated(true);
        coordinator.ToggleUI(false);

        canvasGroup.alpha = 0f;

        isWorking = true;

        while (canvasGroup.alpha < 1.0f)
        {
            canvasGroup.alpha += Time.deltaTime / t;
            yield return null;
        }

        canvasIsFullyShown = true;
        coordinator.SetBeingAnimated(false);
    }

    private IEnumerator FadeOutCanvas(float t)
    {
        canvasGroup.interactable = false;
        coordinator.SetBeingAnimated(true);

        canvasIsFullyShown = false;

        while (canvasGroup.alpha > 0.0f)
        {
            canvasGroup.alpha -= Time.deltaTime / t;
            yield return null;
        }

        isWorking = false;
        coordinator.ToggleUI(true);
        coordinator.SetBeingAnimated(false);

    }
}
