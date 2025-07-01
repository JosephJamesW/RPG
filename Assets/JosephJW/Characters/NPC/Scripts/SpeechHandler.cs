using GinjaGaming.FinalCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeechHandler : MonoBehaviour
{
    [SerializeField] private string textBoxOutput;
    [SerializeField] private float resetTime;
    [SerializeField] private GameObject textBoxObj;
    [SerializeField] private Vector3 textBoxOffset;

    private Animator animator;
    private NPCHeadLookAt npcHeadLookAt;

    private bool textActive;
    private TextBox textBox;

    void Awake()
    {
        animator = GetComponent<Animator>();
        npcHeadLookAt = GetComponent<NPCHeadLookAt>();
    }

    public void Talk(Transform interactorTransform)
    {
        if (!textActive)
        {
            textActive = true;

            textBox = Instantiate(textBoxObj, transform.position + textBoxOffset, transform.rotation).GetComponentInChildren<TextBox>();
            textBox.StartWriter(textBoxOutput);

            animator.SetTrigger("Talk");

            npcHeadLookAt.LookAtPosition(interactorTransform.GetComponentInChildren<CharacterInteract>().head);

            Invoke("ResetSpeech", resetTime);
        }
    }

    private void ResetSpeech()
    {
        animator.SetTrigger("Idle");
        npcHeadLookAt.ResetLookAt();
        Destroy(textBox.gameObject);
        textActive = false;
    }

    public bool GetActive()
    {
        return textActive;
    }
}
