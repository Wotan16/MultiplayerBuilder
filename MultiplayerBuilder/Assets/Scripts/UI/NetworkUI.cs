using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkUI : MonoBehaviour
{
    [SerializeField]
    private Button hostBtn;
    [SerializeField]
    private Button clientBtn;
    [SerializeField]
    private Button closeButton;

    private bool isOnline;

    private void Start()
    {
        InitializeButtons();
    }

    private void InitializeButtons()
    {
        hostBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
            UpdateButtonsActive();
        });
        clientBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
            UpdateButtonsActive();
        });
        closeButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }

    private void UpdateButtonsActive()
    {
        hostBtn.gameObject.SetActive(!isOnline);
        clientBtn.gameObject.SetActive(!isOnline);
        //disconnectBtn.gameObject.SetActive(isOnline);
    }
}
