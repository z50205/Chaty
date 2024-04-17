using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PanelToggle : MonoBehaviour
{
    [SerializeField]private GameObject _panel;
    private void Start() {
        _panel.SetActive(false);
    }
    public void TogglePanel()
    {
        if (_panel!=null)
        {
            bool isActive=_panel.activeSelf;
            _panel.SetActive(!isActive);
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}
