using UnityEngine;
using TMPro;
using System.Collections;

public class InteractionUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI guideText;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private float dialogueDuration = 3.0f;

    private Coroutine dialogueRoutine;

    // 시작 할 때 UI를 끈다.
    void Start()
    {
        HideGuide();
        dialoguePanel.SetActive(false);
    }

    // UIPanel을 보여주고 닫는 함수
    public void ShowGuide(string message)
    {
        guideText.gameObject.SetActive(true);
        guideText.text = message;
    }

    public void HideGuide()
    {
        guideText.gameObject.SetActive(false);
    }

    // 다이얼로그를 보여주는 함수
    public void ShowDialogue(string npcName, string message)
    {
        if (dialogueRoutine != null)
        {
            StopCoroutine(dialogueRoutine);
        }

        dialogueRoutine = StartCoroutine(DialogueRoutine(npcName, message));
    }

    // 다이얼로그를 보여주고 끄는 코루틴
    private IEnumerator DialogueRoutine(string npcName, string message)
    {
        dialoguePanel.SetActive(true);
        dialogueText.text = npcName + "\n" + message;

        yield return new WaitForSeconds(dialogueDuration);

        dialoguePanel.SetActive(false);
        dialogueRoutine = null;
    }
}