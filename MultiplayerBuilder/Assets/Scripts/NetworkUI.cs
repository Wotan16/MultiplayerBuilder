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
    private Button disconnectBtn;

    private bool isOnline;

    private void Start()
    {
        InitializeButtons();
        isOnline = false;
        UpdateButtonsActive();
    }

    private void InitializeButtons()
    {
        hostBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
            isOnline = true;
            UpdateButtonsActive();
        });
        clientBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
            isOnline = true;
            UpdateButtonsActive();
        });
        //disconnectBtn.onClick.AddListener(() =>
        //{
        //    //NetworkManager.Singleton.dis();
        //    isOnline = false;
        //    UpdateButtonsActive();
        //});
    }

    private void UpdateButtonsActive()
    {
        hostBtn.gameObject.SetActive(!isOnline);
        clientBtn.gameObject.SetActive(!isOnline);
        //disconnectBtn.gameObject.SetActive(isOnline);
    }
}
