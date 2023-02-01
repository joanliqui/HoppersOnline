using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InputFieldController : MonoBehaviour
{
    private TMP_InputField inputField;
    [SerializeField] Color letterColor;

    private void Start()
    {
        inputField = GetComponent<TMP_InputField>();
        inputField.onValueChanged.AddListener(ForceUppercase);

        
    }

    private void ForceUppercase(string t)
    {

        inputField.text = inputField.text.ToUpper();
    }

}
