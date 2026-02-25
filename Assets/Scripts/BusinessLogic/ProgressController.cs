using UnityEngine;
using UnityEngine.UI;

public class ProgressController : MonoBehaviour
{
    [SerializeField]
    private Slider _slider;

    public void UpdateProgress(float progress)
    {
        _slider.value = progress;
    }
}
