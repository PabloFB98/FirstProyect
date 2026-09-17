using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Jetpack : MonoBehaviour
{
    #region Properties

    // Energía actual del jetpack.
    // Mathf.Clamp evita que sea menor que 0 o mayor que la energía máxima.
    public float Energy
    {
        get => _energy;
        set => _energy = Mathf.Clamp(value, 0, _maxEnergy);
    }

    // Indica si el jetpack está actualmente funcionando.
    public bool Flying { get; private set; }

    #endregion

    #region Fields

    private Rigidbody2D _targetRB;
    private AudioManager _audioManager;

    [SerializeField] private float _energy;
    [SerializeField] private float _maxEnergy;
    [SerializeField] private float _energyFlyingRatio;
    [SerializeField] private float _energyRegenerationRatio;

    [SerializeField] private float _flyForce;

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        // Guardamos una referencia al Rigidbody2D para no buscarlo continuamente.
        _targetRB = GetComponent<Rigidbody2D>();

        // Buscamos el gestor de audio de la escena.
        _audioManager = FindFirstObjectByType<AudioManager>();
    }

    private void Start()
    {
        // El jetpack comienza con la energía al máximo.
        Energy = _maxEnergy;
    }

    private void FixedUpdate()
    {
        // La física del jetpack se ejecuta en FixedUpdate.
        if (Flying)
            DoFly();

        // Regeneramos energía cuando el personaje está prácticamente quieto
        // verticalmente.
        if (Mathf.Abs(_targetRB.velocity.y) < 0.1f)
            Regenerate();
    }

    #endregion

    #region Public Methods

    public void FlyUp()
    {
        // Solo iniciamos el sonido una vez, al comenzar el vuelo.
        if (!Flying)
        {
            Flying = true;

            if (_audioManager != null)
                _audioManager.StartJetpack();
        }
    }

    public void StopFlying()
    {
        // Solo hacemos el trabajo necesario si realmente estaba volando.
        if (Flying)
        {
            Flying = false;

            if (_audioManager != null)
                _audioManager.StopJetpack();
        }
    }

    // Recupera energía de forma gradual.
    public void Regenerate()
    {
        Energy += _energyRegenerationRatio;
    }

    // Añade una cantidad concreta de energía.
    public void AddEnergy(float energy)
    {
        Energy += energy;
    }

    #endregion

    #region Private Methods

    private void DoFly()
    {
        if (Energy > 0)
        {
            // Aplicamos una fuerza vertical para impulsar al jugador.
            _targetRB.AddForce(Vector2.up * _flyForce);

            // Consumimos energía mientras el jetpack está funcionando.
            Energy -= _energyFlyingRatio;
        }
        else
        {
            // Sin energía, el vuelo se detiene automáticamente.
            StopFlying();
        }
    }

    #endregion
}
