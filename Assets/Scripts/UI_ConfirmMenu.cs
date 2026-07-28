
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GF_PlayerInput;

public class UI_ConfirmMenu : MonoBehaviour
{
    Vector3 OffPosition = new Vector3(-600, 0, 0);
    Vector3 OnPosition = new Vector3(50, 0, 0);

    [SerializeField] private Button ConfirmButton;
    [SerializeField] private Button RejectButton;
    [SerializeField] private TMPro.TMP_Text ConfirmInfo;

    [NonSerialized] public AwaitConfirm currentRequest;

    void OnEnable()
    {
        ConfirmQueue.test += UpdateMenu;
    }

    private void OnDisable()
    {
        ConfirmQueue.ClearQueue();
    }

    void SetUpButtons(AwaitConfirm request)
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



    void UpdateMenu(Queue<AwaitConfirm> requests)
    {
        if(requests.Count > 0)
        {
            inputEnabled = false;
            transform.localPosition = OnPosition;

            if(currentRequest == null)
            {
                
                AwaitConfirm request = requests.Dequeue();
                ConfirmInfo.text = request.reason;
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
    private static Queue<AwaitConfirm> confirmRequests = new Queue<AwaitConfirm>();
    public static event Action<Queue<AwaitConfirm>> test;

    public static void AddToConfirmQueue(AwaitConfirm confirmReq)
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
