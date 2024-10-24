using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIPortrait : MonoBehaviour
{

    public Image image;
    public TMP_Text healthText;
    public TMP_Text actionPointsText;
    private BattleManager battleManager;
    public int playerIndex;

    private void Awake()
    {
        battleManager = BattleManager.Instance;
    }

    public void SelectPartyMemberInUI()
    {
        battleManager.SelectPartyMember(playerIndex); 
    }

}
