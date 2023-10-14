using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GradingAnimationEvents : MonoBehaviour
{
    public void SwitchToDesktopGame()
    {
        GameManager.Instance.SwitchToDesktopGame();
    }
}
