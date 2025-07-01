using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextBox : MonoBehaviour
{
    [SerializeField] private float timePerChar = 0.1f;
    private TextMeshProUGUI text;

    private void Awake()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void StartWriter(string textOutput)
    {
        TextWriter.AddWrite_Static(text, textOutput, timePerChar, true);
    }
}

/* References:
 * 
 *  - https://www.youtube.com/watch?v=ZVh4nH8Mayg&ab_channel=CodeMonkey
 */
