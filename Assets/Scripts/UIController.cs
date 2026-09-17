using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    #region Fields

    [Header("Referencias")]
    [SerializeField] private Jetpack _jetpack;
    [SerializeField] private Health _health;

    [Header("Barras")]
    [SerializeField] private Slider _energySlider;
    [SerializeField] private Slider _healthSlider;

    [Header("Texto")]
    [SerializeField] private TextMeshProUGUI _textSlider;

    #endregion

    #region Unity Callbacks

    private void Start()
    {
        // La vida máxima de la barra se establece según la vida inicial del jugador.
        _healthSlider.maxValue = _health.CurrentHealth;

        // Comenzamos con la barra de vida llena.
        _healthSlider.value = _health.CurrentHealth;
    }

    private void Update()
    {
        // Actualizamos la barra de energía del jetpack.
        _energySlider.value = _jetpack.Energy;

        // Mostramos la altura actual del jugador como número entero.
        _textSlider.text = ((int)_jetpack.transform.position.y).ToString();

        // Actualizamos la barra de vida.
        _healthSlider.value = _health.CurrentHealth;
    }

    #endregion
}
