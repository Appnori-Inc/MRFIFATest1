using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class CheckEventSystem : MonoBehaviour
{
    private EventSystem eventSystem;
    private GameObject currentSelected;
    private Button button;
    private Toggle toggle;
    private Dropdown dropdown;
    // Start is called before the first frame update
    void Start()
    {
        eventSystem = transform.GetComponent<EventSystem>();
        eventSystem.pixelDragThreshold = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (eventSystem.currentSelectedGameObject == currentSelected)
        {
            return;
        }

        currentSelected = eventSystem.currentSelectedGameObject;

        if (currentSelected == null)
        {
            eventSystem.pixelDragThreshold = 1;
        }
        else if (currentSelected.TryGetComponent(out button))
        {
            eventSystem.pixelDragThreshold = 30;
        }
        else if (currentSelected.TryGetComponent(out toggle))
        {
            eventSystem.pixelDragThreshold = 30;
        }
        else if (currentSelected.TryGetComponent(out dropdown))
        {
            eventSystem.pixelDragThreshold = 30;
        }
        else
        {
            eventSystem.pixelDragThreshold = 1;
        }
    }
}
