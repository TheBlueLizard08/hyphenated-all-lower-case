using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextReplacer : MonoBehaviour
{ 

    private void OnEnable()
    {
        if(GameManager.Instance == null) { return; }

        TextMeshProUGUI text = GetComponent<TextMeshProUGUI>();
        text.text = text.text
            .Replace("[PLAYERNAME]", GameManager.Instance.playerName)
            .Replace("[CURRENTYEAR]", DateTime.Now.Year.ToString());
    }
}
