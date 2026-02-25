using System;
using UnityEngine;

public class GameLoader: MonoBehaviour
{
    public Action OnClick;

    public void DoSomething()
    {
        OnClick();
    }
}
