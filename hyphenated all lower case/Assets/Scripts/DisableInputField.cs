using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TMP_InputField))]
public class DisableInputField : MonoBehaviour
{
    TMP_InputField inputField;

    public void Disable()
    {
        inputField = GetComponent<TMP_InputField>();
        StartCoroutine(DisableC());
    }

    IEnumerator DisableC()
    {
        yield return null;
        inputField.interactable = false;
    }
}
