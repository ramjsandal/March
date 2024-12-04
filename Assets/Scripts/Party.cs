using System;
using System.Collections.Generic;
using UnityEngine;
using static Agent;

public class Party : MonoBehaviour
{
    public List<Player> partyMembers = new List<Player>();

    public bool collapsed = false;

    private bool _battling;
    public bool battling
    {
        get
        {
            return _battling;
        }

        set
        {
            _battling = value;
        }
    }

    private int _selectedMemberIdx;
    public int SelectedMemberIdx
    {
        get => _selectedMemberIdx;
        set
        {
            _selectedMemberIdx = value;
        }
    }
    void Start()
    {
        SelectedMemberIdx = 0;
    }

    public void Update()
    {

        // if its dead, we should swap party members
        if (!partyMembers[SelectedMemberIdx].alive)
        {
            SelectPartyMember(NextPartyMemberIdx());
        }

        // if its moving, we cannot put in any inputs
        if (partyMembers[SelectedMemberIdx].moving || partyMembers[SelectedMemberIdx].animating)
        {
            return;
        }

        // give the player the option to swap party members
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            SelectPartyMember();
        }


        if (!battling)
        {
            if (DialogueManager.Instance.inDialogue)
            {
                return;
            }

            // collapse / uncollapse party
            if (Input.GetKeyDown(KeyCode.P))
            {
                if (collapsed)
                {
                    ExpandParty();
                }
                else
                {
                    CollapseParty();
                }
            }

            // move
            partyMembers[SelectedMemberIdx].MoveAction(false);
        }
        else if (battling)
        {
            partyMembers[SelectedMemberIdx].BattleActions();
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

    public void ExpandParty()
    {
        int partySize = partyMembers.Count;
        List<Vector2Int> closestSquares = partyMembers[SelectedMemberIdx].GetClosestSquares(partySize);

        for (int i = 0; i < partySize; i++)
        {
            if (i == SelectedMemberIdx)
            {
                continue;
            }

            if (!partyMembers[i].alive)
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
        int index = -1;

        for (int i = 0; i < partyMembers.Count; i++)
        {
            if (partyMembers[i].MouseOnAgent())
            {
                index = i; break;
            }
        }

        if (index != -1)
        {
            SelectPartyMember(index);
        }
    }

    public void SelectPartyMember(int index)
    {
        if (!partyMembers[index].alive)
        {
            return;
        }
        IndexEventArgs args = new IndexEventArgs();
        args.OldIndex = SelectedMemberIdx;
        SelectedMemberIdx = index;
        args.NewIndex = index;
        OnSelectedPartyMember(args);
        Camera.main.transform.position = partyMembers[SelectedMemberIdx].transform.position + new Vector3(0, 0, -1);
    }

    public event EventHandler<IndexEventArgs> SelectedPartyMember;
    public void OnSelectedPartyMember(IndexEventArgs e)
    {
        if (SelectedPartyMember != null)
        {
            SelectedPartyMember(this, e);
        }
    }
    public class IndexEventArgs : EventArgs
    {
        public int NewIndex { get; set; }
        public int OldIndex { get; set; }
    }

    public List<Vector2Int> GetPartyPositions()
    {
        List<Vector2Int> positions = new List<Vector2Int>();
        foreach (var partyMember in partyMembers)
        {
            if (partyMember.gridPos != null)
            {
                positions.Add(partyMember.gridPos.Value);
            }
        }

        return positions;
    }

    public void ResetActionPoints()
    {
        foreach (var partyMember in partyMembers) { partyMember.ResetActionPoints(); }
    }

    public void ReplenishActionPoints()
    {
        foreach (var partyMember in partyMembers) { partyMember.ReplenishActionPoints(); }
    }

    // gets the currently selected player's grid position 
    public Vector2Int GetSelectedPlayerPosition()
    {
        return partyMembers[SelectedMemberIdx].gridPos.Value;
    }

    public void SetSelectedPartyMemberMode(SelectedAction action)
    {
        partyMembers[SelectedMemberIdx].SetSelectedAction(action);
    }

    public void FocusCameraOnSelectedPlayer()
    {
        Camera.main.transform.position = partyMembers[SelectedMemberIdx].transform.position + new Vector3(0, 0, -1);
    }

    private int NextPartyMemberIdx()
    {
        if (SelectedMemberIdx < 3)
        {
            return SelectedMemberIdx + 1;
        } else
        {
            return 0;
        }
    }
}
