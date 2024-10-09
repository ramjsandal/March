using System.Collections.Generic;
using UnityEngine;

public class Party : MonoBehaviour
{
    public List<Player> partyMembers = new List<Player>();

    private bool collapsed = false;
    private int _selectedMemberIdx;
    public int SelectedMemberIdx
    {
        get => _selectedMemberIdx;
        set
        {
            _selectedMemberIdx = value;

            for (int i = 0; i < partyMembers.Count; i++)
            {
                if (i == _selectedMemberIdx)
                {
                    partyMembers[i].selected = true;
                }
                else
                {
                    partyMembers[i].selected = false;
                }
            }
        }
    }
    void Start()
    {
        SelectedMemberIdx = 0;
    }

    public void Update()
    {

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            SelectPartyMember();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            if (collapsed) 
            {
                ExpandParty();
            } else
            {
                CollapseParty();
            }

        }
    }

    private void CollapseParty()
    {
        for (int i = 0; i < partyMembers.Count; i++)
        {
            if (i != _selectedMemberIdx)
            {
                partyMembers[i].gridPos = null;
                partyMembers[i].gameObject.SetActive(false);
            }
        }
        collapsed = true;
    }

    private void ExpandParty()
    {
        int partySize = partyMembers.Count;
        List<Vector2Int> closestSquares = partyMembers[SelectedMemberIdx].GetClosestSquares(partySize);

        for (int i = 0; i < partySize; i++)
        {
            if (i == SelectedMemberIdx)
            {
                continue;
            }

            partyMembers[i].Teleport(closestSquares[i]);
            partyMembers[i].gameObject.SetActive(true);
        }

        collapsed = false;
    }

    private void SelectPartyMember()
    {
        int index = CheckPartySelection();

        if (index != -1)
        {
            SelectedMemberIdx = index;
        }
    }

    private int CheckPartySelection()
    {
        int retVal = -1;

        for (int i = 0; i < partyMembers.Count; i++)
        {
            if (partyMembers[i].MouseOnAgent())
            {
                retVal = i; break;
            }
        }

        return retVal;

    }


}
