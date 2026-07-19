
using System.Collections.Generic;
using TMPro;
using static GF_PlayerInput;



public class ZipperInit
{
    public int userIndex;
    public Dictionary<SO_User, bool> roundTracker;




    public SO_User EnableInitative(List<SO_User> users)
    {
        // hardcoded, player goes first
        isPlayerTurn = true;
        userIndex = 0;
        roundTracker = new Dictionary<SO_User, bool>();
        foreach(SO_User user in users)
        {
            roundTracker.Add( user, false);
            
        }

        //Temp way of deciding who goes first
        return users[userIndex];

    }

   public SO_User ShiftInitiative(List<SO_User> users)
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
                TopOfRound(users);
                break;
            }
        }

        if (users[userIndex].AI)
        {
            isPlayerTurn = false;

            //Hardcoded, target the first user (player)'s characters
            users[userIndex].EnableAI(users[0].actorsUnderControl);
        }
        else
        {
            isPlayerTurn = true;
        }

        return users[userIndex];


    }

    public void TopOfRound(List<SO_User> users)
    {
        foreach (SO_User user in users)
        {

            foreach (MB_Actor actor in user.actorsUnderControl)
            {
                actor.turnTaken = false;
                actor.trigger = true;
            }

            roundTracker[user] = false;
        }

        userIndex = 0;
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
