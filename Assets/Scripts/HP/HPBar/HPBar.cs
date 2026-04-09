using Assets.Scripts.BusinessLogic;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    public GameObject Entity = null;
    private CombatController _entityCombatController;
    private Slider _slider;
    [SerializeField] private RectTransform _smoothFillTransform;

    public void Initialize()
    {
        if (Entity != null)
        {
            _entityCombatController = Entity.GetComponent<ActorController>().CombatController;
            _slider = GetComponent<Slider>();

            _slider.maxValue = _entityCombatController.MaxHp;
            _slider.value = _entityCombatController.MaxHp;
        }
    }

    void Update()
    {
        if (Entity != null && _entityCombatController.CurrentHp != _slider.value)
        {
            _slider.value = _entityCombatController.CurrentHp;
        }
        if (_smoothFillTransform.anchorMax.x > _slider.value / _slider.maxValue)
        {
           _smoothFillTransform.anchorMax = new Vector2(_smoothFillTransform.anchorMax.x - 0.001f, _smoothFillTransform.anchorMax.y);
        } else if (_slider.value == _slider.maxValue && _smoothFillTransform.anchorMax.x < 1)
            _smoothFillTransform.anchorMax = new Vector2(1f, _smoothFillTransform.anchorMax.y);
    }
}
