using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float _maxHealth = 5f;

    // Vida actual del jugador.
    private float _currentHealth;

    // Permite consultar la vida actual desde otros scripts,
    // pero solo Health puede modificarla directamente.
    public float CurrentHealth
    {
        get { return _currentHealth; }
    }

    // Indica si el jugador ha muerto.
    // El private set evita que otros scripts cambien este estado directamente.
    public bool Dead
    {
        get;
        private set;
    }

    private void Awake()
    {
        // Al comenzar el juego, la vida actual empieza al máximo.
        _currentHealth = _maxHealth;
    }

    // Resta una cantidad de vida al jugador.
    public void TakeDamage(float damage)
    {
        // Si ya ha muerto, ignoramos cualquier daño posterior.
        if (Dead)
            return;

        _currentHealth -= damage;

        // Evitamos que la vida pueda quedar por debajo de cero.
        _currentHealth = Mathf.Max(_currentHealth, 0f);

        // Si la vida llega a cero, el jugador muere.
        if (_currentHealth <= 0f)
        {
            Die();
        }
    }

    // Recupera una cantidad de vida sin superar la vida máxima.
    public void Heal(float amount)
    {
        // Un jugador muerto no puede recuperar vida.
        if (Dead)
            return;

        _currentHealth += amount;

        // Evitamos superar el máximo de vida.
        _currentHealth = Mathf.Min(_currentHealth, _maxHealth);
    }

    // Daño normal utilizado por el juego.
    public void TakeNormalDamage()
    {
        TakeDamage(0.5f);
    }

    // La lava utiliza una cantidad de daño suficientemente alta
    // para provocar la muerte inmediatamente.
    public void TakeLavaDamage()
    {
        TakeDamage(10f);
    }

    // Cambia el estado del jugador a muerto y avisa al Player
    // para que active la animación y el resto del comportamiento de muerte.
    private void Die()
    {
        Dead = true;

        Player player = GetComponent<Player>();

        if (player != null)
        {
            player.Die();
        }
    }
}
