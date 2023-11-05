using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssassinAnimationEventHandler : MonoBehaviour
{
    [SerializeField]
    private AssassinControllerAI assassin;

    public void SwordUsed()
    {
        Debug.Log("Sword Extended!");
    }

}
