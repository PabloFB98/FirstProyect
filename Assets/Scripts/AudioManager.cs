using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Sonidos")]
    [SerializeField] private AudioClip _fallSound;
    [SerializeField] private AudioClip _groundSound;
    [SerializeField] private AudioClip _hitSound;
    [SerializeField] private AudioClip _rockBreakSound;
    [SerializeField] private AudioClip _jumpSound;
    [SerializeField] private AudioClip _jetpackSound;

    [Header("Volumen")]
    [SerializeField] private float _fallVolume = 1f;
    [SerializeField] private float _groundVolume = 1f;
    [SerializeField] private float _hitVolume = 1f;
    [SerializeField] private float _rockBreakVolume = 0.2f;
    [SerializeField] private float _jumpVolume = 1f;
    [SerializeField] private float _jetpackVolume = 1f;

    // AudioSource utilizado para reproducir todos los sonidos.
    // Se mantiene esta estructura para no modificar el funcionamiento actual.
    private AudioSource _audioSource;

    private void Awake()
    {
        // Buscamos el AudioSource que ya existe en el objeto.
        _audioSource = GetComponent<AudioSource>();

        // Si no existe, se crea automáticamente.
        // Se mantiene porque forma parte del comportamiento actual.
        if (_audioSource == null)
            _audioSource = gameObject.AddComponent<AudioSource>();
    }

    // Reproduce el sonido de caída.
    // Este sonido utiliza el AudioSource como clip porque después
    // necesitamos poder detenerlo con StopFall().
    public void PlayFall()
    {
        if (_fallSound != null)
        {
            _audioSource.clip = _fallSound;
            _audioSource.loop = false;
            _audioSource.volume = _fallVolume;
            _audioSource.Play();
        }
    }

    // Detiene únicamente el sonido de caída.
    public void StopFall()
    {
        if (_audioSource.isPlaying && _audioSource.clip == _fallSound)
            _audioSource.Stop();
    }

    // Sonido al tocar el suelo.
    public void PlayGround()
    {
        PlaySound(_groundSound, _groundVolume);
    }

    // Sonido al recibir un golpe.
    public void PlayHit()
    {
        PlaySound(_hitSound, _hitVolume);
    }

    // Sonido de rotura/impacto de las rocas.
    public void PlayRockBreak()
    {
        PlaySound(_rockBreakSound, _rockBreakVolume);
    }

    // Sonido del salto.
    public void PlayJump()
    {
        PlaySound(_jumpSound, _jumpVolume);
    }

    // Inicia el sonido continuo del jetpack.
    public void StartJetpack()
    {
        if (_jetpackSound == null)
            return;

        // Evitamos reiniciar el sonido si ya se está reproduciendo.
        if (!_audioSource.isPlaying)
        {
            _audioSource.clip = _jetpackSound;
            _audioSource.loop = true;
            _audioSource.volume = _jetpackVolume;
            _audioSource.Play();
        }
    }

    // Detiene el sonido continuo del jetpack.
    public void StopJetpack()
    {
        if (_audioSource.isPlaying && _audioSource.clip == _jetpackSound)
        {
            _audioSource.Stop();
            _audioSource.loop = false;
        }
    }

    // Método común para reproducir sonidos puntuales.
    private void PlaySound(AudioClip sound, float volume)
    {
        if (sound != null)
            _audioSource.PlayOneShot(sound, volume);
    }
}
