using UnityEngine;

public class InGameUIManager : MonoBehaviour
{
    private GameObject _player;
    public HPBar HPBar;

    public void GetPlayer(GameObject player)
    {
        _player = player;
        HPBar.Entity = player;
        HPBar.Initialize();
    }
}
