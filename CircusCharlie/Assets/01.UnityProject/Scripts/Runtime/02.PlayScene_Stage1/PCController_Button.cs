using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCController_Button : MonoBehaviour
{
    private PlayerController PC = default;
    // Start is called before the first frame update
    void Start()
    {
        GameObject gObj = GFunc.GetRootObj("GameObjs");
        PC = gObj.FindChildObj("PlayerCharacter").GetComponentMust<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnDownLeftButton()
    {
        PC.onButtonDownLeftBtn();
    }
    public void OnUpLeftButton()
    {
        PC.onButtonUpLeftBtn();
    }
    public void OnDownRightButton()
    {
        PC.onButtonDownRightBtn();
    }
    public void OnUpRightButton()
    {
        PC.onButtonUpRightBtn();
    }
    public void OnDownJumpButton()
    {
        PC.onButtonDownJumpBtn();
    }
}
