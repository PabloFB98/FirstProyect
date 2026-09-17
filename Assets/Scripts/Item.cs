using UnityEngine;

public class Item : MonoBehaviour
{
    // Tipos de objetos que pueden aparecer durante el juego.
    public enum ItemType
    {
        Damage, // Hace daño al jugador.
        Health, // Recupera vida.
        Fuel    // Recupera energía del jetpack.
    }

    // Tipo de este objeto concreto.
    public ItemType itemType;

    // Tiempo que permanece en el suelo antes de desaparecer.
    public float groundDestroyTime = 2f;

    [Header("Partículas")]
    // Particle System que se muestra cuando el objeto impacta con el jugador.
    [SerializeField] private ParticleSystem _impactParticles;

    // Evita programar varias veces la destrucción al tocar el suelo.
    private bool _hasTouchedGround = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // ---------------------------------------------------------
        // COLISIÓN CON EL SUELO
        // ---------------------------------------------------------
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            if (!_hasTouchedGround)
            {
                _hasTouchedGround = true;

                // El objeto no desaparece inmediatamente:
                // esperamos el tiempo configurado en groundDestroyTime.
                Invoke(nameof(DestroyItem), groundDestroyTime);
            }
        }

        // ---------------------------------------------------------
        // COLISIÓN CON EL JUGADOR
        // ---------------------------------------------------------
        if (collision.gameObject.CompareTag("Player"))
        {
            Health health = collision.gameObject.GetComponent<Health>();

            if (health != null)
            {
                // OBJETO DE DAÑO
                if (itemType == ItemType.Damage)
                {
                    health.TakeDamage(0.5f);

                    Player player = collision.gameObject.GetComponent<Player>();

                    if (player != null)
                    {
                        // Mantenemos los sonidos tal y como funcionan actualmente.
                        player.PlayHitSound();
                        player.PlayRockBreakSound();
                    }
                }
                // OBJETO DE VIDA
                else if (itemType == ItemType.Health)
                {
                    health.Heal(1f);
                }
                // OBJETO DE COMBUSTIBLE
                else if (itemType == ItemType.Fuel)
                {
                    Jetpack jetpack = collision.gameObject.GetComponent<Jetpack>();

                    if (jetpack != null)
                        jetpack.AddEnergy(25f);
                }
            }

            // ---------------------------------------------------------
            // PARTÍCULAS DEL IMPACTO
            // ---------------------------------------------------------
            if (_impactParticles != null)
            {
                // Creamos una copia de las partículas en la posición
                // del objeto antes de destruir el Item.
                ParticleSystem particles = Instantiate(
                    _impactParticles,
                    transform.position,
                    Quaternion.identity
                );

                // La copia deja de ser hija del Item.
                // Así puede continuar reproduciéndose aunque destruyamos
                // el objeto que ha provocado el impacto.
                particles.transform.SetParent(null);

                particles.Play();

                // Eliminamos la copia cuando haya terminado.
                Destroy(
                    particles.gameObject,
                    particles.main.duration +
                    particles.main.startLifetime.constantMax +
                    0.2f
                );
            }

            // El objeto se elimina después de recogerlo o impactar.
            DestroyItem();
        }
    }

    // Destruye este objeto de la escena.
    private void DestroyItem()
    {
        Destroy(gameObject);
    }
}
