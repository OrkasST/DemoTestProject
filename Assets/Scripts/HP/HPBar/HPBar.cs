using Assets.Scripts.BusinessLogic;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{

    private CombatController _entityCombatController;
    private Slider _slider;

    void Start()
    {
        _entityCombatController = GetComponent<ActorController>().CombatController;
        _slider = GetComponent<Slider>();

        _slider.maxValue = _entityCombatController.MaxHp;
        _slider.value = _entityCombatController.MaxHp;
    }

    void Update()
    {
        if (_entityCombatController.CurrentHp != _slider.value)
            _slider.value = _entityCombatController.CurrentHp;
    }
}
