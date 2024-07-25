using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagingInput
{
    // Start is called before the first frame update
    public static bool blockInputs = false;
    public static bool GetKeyDown(KeyCode keyCode)
    {
        if (blockInputs)
            return false;

        return UnityEngine.Input.GetKeyDown(keyCode);
    }
    public static bool GetKeyUp(KeyCode keyCode) 
    {
        if (blockInputs)
            return false;
        
        return UnityEngine.Input.GetKeyUp(keyCode);
    }
    public static bool GetMouseButton(int mouseButton)
    {
        if (blockInputs)
            return false;

        return UnityEngine.Input.GetMouseButton(mouseButton);
    }
    public static float GetAxis(string axisName)
    {
        if (blockInputs)
            return 0;

        return UnityEngine.Input.GetAxis(axisName);
    }
    public static bool GetMouseButtonDown(int mouseButton)
    {
        if (blockInputs)
            return false;

        return UnityEngine.Input.GetMouseButtonDown(mouseButton);
    }
}
