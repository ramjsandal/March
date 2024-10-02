using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Party : MonoBehaviour
{
    public List<Player> partyMembers = new List<Player>();
    void Start()
    {
        partyMembers[0].selected = true;
    }

    int CheckPartySelection()
    {
        int retVal = -1;

        for (int i = 0; i < partyMembers.Count; i++)
        {
            if (partyMembers[i].MouseOnPlayer())
            {
                retVal = i; break;
            }
        }

        return retVal;

    }

    public void Update() {
    
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            SelectPartyMember();
        }
    }


    private void SelectPartyMember()
    {
        int index = CheckPartySelection();
        Debug.Log(index);

        if (index != -1) {
            for (int i = 0;i < partyMembers.Count;i++)
            {
                if (i == index)
                {
                    partyMembers[i].selected = true;
                } else
                {
                    partyMembers[i].selected = false;
                }
            }
        }
    }

}
