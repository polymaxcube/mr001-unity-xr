using UnityEngine;
using UnityEngine.InputSystem;

public class InputTest : MonoBehaviour
{
    public InputActionProperty testActionValue;
    public InputActionProperty testActionButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float InputValue = testActionValue.action.ReadValue<float>();
        bool InputButton = testActionButton.action.IsPressed();

        Debug.Log("Input Value: " + InputValue);
        Debug.Log("Input Button:" + InputButton);
    }
}
