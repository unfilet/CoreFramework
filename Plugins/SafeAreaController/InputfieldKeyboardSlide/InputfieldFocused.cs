using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InputfieldFocused : MonoBehaviour
{

    [SerializeField] InputfieldSlideScreen slideScreen;
    [SerializeField] TMP_InputField inputField;

    private void Awake()
    {
        if (!inputField)
            inputField = GetComponent<TMP_InputField>();
    }

    void Update()
    {
        if (inputField.isFocused)
        {
            slideScreen.InputFieldActive = true;
            slideScreen.childRectTransform = (RectTransform)transform;
        }
    }
}