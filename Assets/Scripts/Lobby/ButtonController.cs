using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ButtonController : MonoBehaviour
{
    private Renderer buttonRenderer;

    // Start is called before the first frame update
    void Start()
    {
        buttonRenderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsMouseOverButton())
        {
            buttonRenderer.material.color = Color.red;
        }
        else
        {
            buttonRenderer.material.color = Color.black;
        }
    }

    private bool IsMouseOverButton()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        return Physics.Raycast(ray, out hit) && hit.collider.gameObject == gameObject;
    }
}