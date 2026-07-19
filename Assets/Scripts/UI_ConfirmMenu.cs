
using System;
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;
using static GF_PlayerInput;

public class UI_ConfirmMenu : MonoBehaviour
{
    Vector3 OffPosition = new Vector3(-600, 0, 0);
    Vector3 OnPosition = new Vector3(50, 0, 0);

    [SerializeField] private Button ConfirmButton;
    [SerializeField] private Button RejectButton;

    [NonSerialized] public UserService currentRequest;

    void OnEnable()
    {
        ConfirmQueue.test += UpdateMenu;
    }

    private void OnDisable()
    {
        ConfirmQueue.ClearQueue();
    }

    void SetUpButtons(UserService request)
    {
        currentRequest = request;

        ConfirmButton.onClick.AddListener(delegate { OnButtonClicked(true); });
        RejectButton.onClick.AddListener(delegate { OnButtonClicked(false); });

    }

    void OnButtonClicked(bool confirm)
    {

        inputEnabled = true;
        currentRequest.OnUserActionCompleted(confirm);

        ConfirmButton.onClick.RemoveAllListeners();
        RejectButton.onClick.RemoveAllListeners();

        currentRequest = null;

        
        ConfirmQueue.NotifyRequestCompletion();
    }



    void UpdateMenu(Queue<UserService> requests)
    {
        if(requests.Count > 0)
        {
            inputEnabled = false;
            transform.localPosition = OnPosition;

            if(currentRequest == null)
            {
                
                UserService request = requests.Dequeue();
                SetUpButtons(request);
            }
                
        }
        else
        {
            transform.localPosition = OffPosition;
            
        }
    }
}


public static class ConfirmQueue
{
    private static Queue<UserService> confirmRequests = new Queue<UserService>();
    public static event Action<Queue<UserService>> test;

    public static void AddToConfirmQueue(UserService confirmReq)
    {
        confirmRequests.Enqueue(confirmReq);
        test.Invoke(confirmRequests);

    }

    public static void NotifyRequestCompletion()
    {
        test.Invoke(confirmRequests);
    }

    public static void ClearQueue()
    {
        test = null;
        confirmRequests.Clear();

    }


}
