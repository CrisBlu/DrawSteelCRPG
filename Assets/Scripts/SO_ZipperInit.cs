
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

[CreateAssetMenu(fileName = "SO_ZipperInit", menuName = "Scriptable Objects/ZipperInit")]
public class SO_ZipperInit : ScriptableObject
{
    [SerializeField] List<SO_User> users;
    private int userIndex;
    private Dictionary<SO_User, bool> roundTracker;


    public SO_User activeUser
    {
        get
        {
            return users[userIndex];
        }
    }

    private void OnEnable()
    {
        userIndex = 0;
        roundTracker = new Dictionary<SO_User, bool>();
        foreach(SO_User user in users)
        {
            roundTracker.Add( user, false);
            user.TurnManager.EventPassInitative.AddListener(ShiftInitiative);
        }

        
    }

   private void ShiftInitiative()
    {
        
        userIndex = (userIndex + 1) % users.Count;
        int currentUserIndex = userIndex;
        roundTracker[users[userIndex]] = CheckIfUserDone(users[userIndex]);

       

        while(roundTracker[users[userIndex]])
        {
            userIndex = (userIndex + 1) % users.Count;
            roundTracker[users[userIndex]] = CheckIfUserDone(users[userIndex]);

            //If we've looped back around without finding a false
            if(userIndex == currentUserIndex)
            {
                TopOfRound();
                break;
            }
        }

        if (activeUser.AI)
        {
            activeUser.EnableAI(users[0].actorsUnderControl);
        }
       
    }

    public void TopOfRound()
    {
        foreach (SO_User user in users)
        {

            foreach (MB_Actor actor in user.actorsUnderControl)
            {
                actor.turnTaken = false;
            }

            roundTracker[user] = false;
        }
    }

    public bool CheckIfUserDone(SO_User user)
    {
        foreach (MB_Actor actor in user.actorsUnderControl)
        {
            if (!actor.turnTaken)
            {
                return false;
            }
        }

        return true;
    }

    private void OnDisable()
    {
        roundTracker.Clear();
    }

}
