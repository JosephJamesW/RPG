using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextWriter : MonoBehaviour
{
    private static TextWriter instance;

    private List<TextWriterSingle> textWriterSingleList;

    private void Awake()
    {
        instance = this;
        textWriterSingleList = new List<TextWriterSingle>();
    }

    public static void AddWrite_Static(TextMeshProUGUI textObj, string textToWrite, float timePerChar, bool invisibleChars)
    {
        instance.AddWriter(textObj, textToWrite, timePerChar, invisibleChars);
    }

    private void AddWriter(TextMeshProUGUI textObj, string textToWrite, float timePerChar, bool invisibleChars)
    {
        textWriterSingleList.Add(new TextWriterSingle(textObj, textToWrite, timePerChar, invisibleChars));
    }

    private void Update()
    {
        for (int i = 0; i < textWriterSingleList.Count; i++)
        {
            bool destroyInstance = textWriterSingleList[i].Update();
            if (destroyInstance)
            {
                textWriterSingleList.RemoveAt(i);
                i--;
            }
        }
    }

    public class TextWriterSingle
    {
        private TextMeshProUGUI textObj;
        private string textToWrite;
        private int charIndex;
        private float timePerChar;
        private float timer;
        private bool invisibleChars;

        public TextWriterSingle(TextMeshProUGUI textObj, string textToWrite, float timePerChar, bool invisibleChars)
        {
            this.textObj = textObj;
            this.textToWrite = textToWrite;
            this.timePerChar = timePerChar;
            this.invisibleChars = invisibleChars;
            charIndex = 0;
        }

        public bool Update()
        {
            timer -= Time.deltaTime;
            while (timer <= 0)
            {
                timer += timePerChar;
                charIndex++;
                string text = textToWrite.Substring(0, charIndex);
                if (invisibleChars)
                {
                    text += "<color=#00000000>" + textToWrite.Substring(charIndex) + "</color>";
                }
                textObj.text = text;

                if (charIndex >= textToWrite.Length)
                {
                    return true;
                }
            }
            return false;
        }
    }
}

/* References:
 * 
 *  - https://www.youtube.com/watch?v=ZVh4nH8Mayg&ab_channel=CodeMonkey
 */