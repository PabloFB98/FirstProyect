using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private Jetpack _jetpack;
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _flyHorizontalSpeed = 6f;

    [Header("Comprobación del suelo")]
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private float _groundCheckRadius = 0.1f;
    [SerializeField] private LayerMask _groundLayer;

    [Header("Llama del jetpack")]
    [SerializeField] private Transform _jetpackFire;

    private Animator _anim;
    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;
    private SpriteRenderer _jetpackFireRenderer;

    private AudioManager _audioManager;

    // Variables utilizadas para controlar la caída y su sonido.
    private bool _wasFalling;
    private float _fallTimer;
    private bool _fallSoundPlaying;

    // Indica que el jugador ha muerto y evita seguir procesando controles.
    private bool _dead;

    // Posición horizontal original de la llama.
    private float _jetpackFireOriginalX;

    private void Awake()
    {
        // Guardamos referencias a los componentes del propio jugador.
        _anim = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        // Buscamos el gestor de audio de la escena.
        _audioManager = FindFirstObjectByType<AudioManager>();

        // Si no se ha asignado la llama desde el Inspector,
        // intentamos encontrarla automáticamente por su nombre.
        if (_jetpackFire == null)
            _jetpackFire = transform.Find("Jetpack Fire");

        if (_jetpackFire != null)
        {
            _jetpackFireOriginalX = _jetpackFire.localPosition.x;
            _jetpackFireRenderer = _jetpackFire.GetComponent<SpriteRenderer>();
        }

        // La llama comienza apagada.
        if (_jetpackFireRenderer != null)
            _jetpackFireRenderer.enabled = false;
    }

    private void Update()
    {
        // Después de morir no necesitamos procesar controles ni animaciones.
        if (_dead)
            return;

        // Comprobamos si el jugador está tocando una superficie del Ground.
        bool grounded = Physics2D.OverlapCircle(
            _groundCheck.position,
            _groundCheckRadius,
            _groundLayer
        );

        // Informamos al Animator del estado actual del jugador.
        _anim.SetBool("Grounded", grounded);

        // ---------------------------------------------------------
        // SALTO
        // ---------------------------------------------------------
        if (grounded && Input.GetKeyDown(KeyCode.W))
        {
            _anim.SetTrigger("Jump");
            _anim.SetBool("Falling", false);
            _audioManager.PlayJump();
        }

        // ---------------------------------------------------------
        // INICIO DEL VUELO
        // ---------------------------------------------------------
        if (!grounded && Input.GetKeyDown(KeyCode.W))
        {
            _jetpack.FlyUp();

            _anim.SetBool("Falling", false);

            StopFallSound();

            _wasFalling = false;
            _fallTimer = 0f;
        }

        // Mientras mantenemos W pulsada, seguimos dando fuerza al jetpack.
        if (Input.GetKey(KeyCode.W) && _jetpack.Flying)
            _jetpack.FlyUp();

        // Actualizamos el estado Flying del Animator.
        _anim.SetBool("Flying", _jetpack.Flying);

        // La llama solo se muestra mientras el jetpack está volando.
        if (_jetpackFireRenderer != null)
            _jetpackFireRenderer.enabled = _jetpack.Flying;

        // ---------------------------------------------------------
        // CAÍDA
        // ---------------------------------------------------------
        if (!grounded && !_jetpack.Flying && _rb.velocity.y < 0)
        {
            _anim.SetBool("Falling", true);

            if (!_wasFalling)
            {
                _fallTimer = 0f;
                _wasFalling = true;
            }

            _fallTimer += Time.deltaTime;

            // Reproducimos el sonido después de 0.3 segundos de caída.
            if (_fallTimer >= 0.3f && !_fallSoundPlaying)
            {
                _audioManager.PlayFall();
                _fallSoundPlaying = true;
            }
        }

        // ---------------------------------------------------------
        // ATERRIZAJE
        // ---------------------------------------------------------
        if (grounded)
        {
            bool landedFromFall = _wasFalling;

            _anim.SetBool("Falling", false);

            StopFallSound();

            if (landedFromFall)
                _audioManager.PlayGround();

            _wasFalling = false;
            _fallTimer = 0f;
        }

        // Al soltar W detenemos el jetpack.
        if (Input.GetKeyUp(KeyCode.W))
            _jetpack.StopFlying();

        // ---------------------------------------------------------
        // MOVIMIENTO HORIZONTAL
        // ---------------------------------------------------------
        float horizontal = Input.GetAxisRaw("Horizontal");

        // El jugador se mueve un poco más rápido mientras vuela.
        float currentSpeed = _jetpack.Flying
            ? _flyHorizontalSpeed
            : _moveSpeed;

        _rb.velocity = new Vector2(
            horizontal * currentSpeed,
            _rb.velocity.y
        );

        // ---------------------------------------------------------
        // DIRECCIÓN DEL PERSONAJE
        // ---------------------------------------------------------
        if (horizontal < 0)
        {
            // Mirando a la izquierda.
            _spriteRenderer.flipX = false;

            if (_jetpackFire != null)
            {
                Vector3 position = _jetpackFire.localPosition;
                position.x = Mathf.Abs(_jetpackFireOriginalX);
                _jetpackFire.localPosition = position;
            }
        }
        else if (horizontal > 0)
        {
            // Mirando a la derecha.
            _spriteRenderer.flipX = true;

            if (_jetpackFire != null)
            {
                Vector3 position = _jetpackFire.localPosition;
                position.x = -Mathf.Abs(_jetpackFireOriginalX);
                _jetpackFire.localPosition = position;
            }
        }

        // La animación de caminar solo se activa en el suelo.
        _anim.SetBool(
            "Walking",
            grounded && horizontal != 0
        );
    }

    // Animation Event del último frame de jump_side.
    // Si W sigue pulsada, iniciamos el vuelo.
    public void FinishJump()
    {
        if (Input.GetKey(KeyCode.W))
            _jetpack.FlyUp();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // La lava provoca la muerte del jugador.
        if (collision.gameObject.CompareTag("Lava"))
        {
            Health health = GetComponent<Health>();

            if (health != null)
                health.TakeLavaDamage();
        }
    }

    // Reproduce el sonido y activa la animación de golpe.
    public void PlayHitSound()
    {
        Health health = GetComponent<Health>();

        // Si el golpe ya ha provocado la muerte, no reproducimos
        // la animación de impacto antes de la animación de muerte.
        if (health != null && health.Dead)
            return;

        _audioManager.PlayHit();
        _anim.SetTrigger("Hit");
    }

    // Reproduce el sonido de rotura de la roca.
    // Se mantiene separado porque Item.cs lo utiliza directamente.
    public void PlayRockBreakSound()
    {
        _audioManager.PlayRockBreak();
    }

    // Activa el estado de muerte del jugador.
    public void Die()
    {
        _dead = true;

        // Detenemos el jetpack y el movimiento.
        _jetpack.StopFlying();
        _rb.velocity = Vector2.zero;

        // El Animator se encarga de pasar a la animación vanish.
        _anim.SetBool("Dead", true);
    }

    // Animation Event del último frame de la animación vanish.
    public void ReturnToMenu()
    {
        StartCoroutine(LoadMenu());
    }

    private System.Collections.IEnumerator LoadMenu()
    {
        // Esperamos un frame antes de cambiar de escena.
        yield return null;

        SceneManager.LoadScene("MainMenu");
    }

    // Detiene el sonido de caída si estaba reproduciéndose.
    private void StopFallSound()
    {
        if (_fallSoundPlaying)
        {
            _audioManager.StopFall();
            _fallSoundPlaying = false;
        }
    }
}
