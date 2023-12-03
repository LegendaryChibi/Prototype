using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverScreen : MonoBehaviour
{
   public void PlayAgain()
    {
        GameManager.Instance.PlayerRespawn();
    }

    public void DontPlayAgain()
    {
        GameManager.Instance.ReturnToMainMenu();
    }
}
