using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayExitBtn : MonoBehaviour
{
    public void OnPressPlayButton()
    {
        GFunc.LoadScene(GData.SCENE_NAME_STAGE_1);
    }
    public void OnPressExitButton()
    {
        GFunc.QuitThisGame();
    }
}
