using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("Rocas azules")]
    [SerializeField] private GameObject _rocaAzul1;
    [SerializeField] private GameObject _rocaAzul2;

    [Header("Rocas de lava")]
    [SerializeField] private GameObject _rocaLava1;
    [SerializeField] private GameObject _rocaLava2;

    [Header("Rocas marrones")]
    [SerializeField] private GameObject _rocaMarron1;
    [SerializeField] private GameObject _rocaMarron2;

    [Header("Granizo")]
    [SerializeField] private GameObject _granizo1;
    [SerializeField] private GameObject _granizo2;

    [Header("Asteroides")]
    [SerializeField] private GameObject _asteroide1;
    [SerializeField] private GameObject _asteroide2;

    [Header("Objetos especiales")]
    [SerializeField] private GameObject _vida;
    [SerializeField] private GameObject _fuel;

    [Header("Configuración")]
    [SerializeField] private float _tiempoAparicion = 2f;
    [SerializeField] private float _minX = -6f;
    [SerializeField] private float _maxX = 6f;

    // Cuenta el tiempo restante hasta la siguiente aparición.
    private float _temporizador;

    private void Update()
    {
        // Restamos el tiempo transcurrido desde el último frame.
        _temporizador -= Time.deltaTime;

        // Cuando llega a cero, generamos un objeto.
        if (_temporizador <= 0f)
        {
            AparecerObjeto();

            // Calculamos el tiempo que debe pasar hasta el siguiente objeto.
            _temporizador = ObtenerTiempoAparicion();
        }
    }

    // Genera un objeto en una posición horizontal aleatoria.
    private void AparecerObjeto()
    {
        // La altura del spawner determina qué tipo de objeto puede aparecer.
        float altura = transform.position.y;
        GameObject prefabObjeto = ObtenerObjetoPorAltura(altura);

        // Si no hay ningún prefab asignado, no intentamos crear nada.
        if (prefabObjeto == null)
            return;

        // Elegimos una posición X aleatoria dentro del rango configurado.
        float posicionX = transform.position.x + Random.Range(_minX, _maxX);

        Vector3 posicionAparicion = new Vector3(
            posicionX,
            transform.position.y,
            transform.position.z
        );

        // Creamos una instancia del prefab seleccionado.
        Instantiate(prefabObjeto, posicionAparicion, Quaternion.identity);
    }

    // Determina cada cuánto aparecen objetos según la altura.
    // A mayor altura, reducimos el intervalo y aumentamos la dificultad.
    private float ObtenerTiempoAparicion()
    {
        float altura = transform.position.y;

        if (altura <= 112f)
            return 2f;

        if (altura <= 274f)
            return 1.7f;

        if (altura <= 516f)
            return 1.4f;

        return 1.1f;
    }

    // Selecciona el tipo de objeto según la altura actual.
    private GameObject ObtenerObjetoPorAltura(float altura)
    {
        // ---------------------------------------------------------
        // OBJETOS ESPECIALES
        // ---------------------------------------------------------
        // Tenemos un 10 % de probabilidad de vida y otro 10 % de fuel.
        float probabilidadEspecial = Random.Range(0f, 100f);

        if (probabilidadEspecial < 10f)
            return _vida;

        if (probabilidadEspecial < 20f)
            return _fuel;

        // ---------------------------------------------------------
        // 0 - 112: ROCAS AZULES
        // ---------------------------------------------------------
        if (altura <= 112f)
        {
            return Random.Range(0, 2) == 0
                ? _rocaAzul1
                : _rocaAzul2;
        }

        // ---------------------------------------------------------
        // 113 - 130: ROCAS DE LAVA
        // ---------------------------------------------------------
        if (altura <= 130f)
        {
            return Random.Range(0, 2) == 0
                ? _rocaLava1
                : _rocaLava2;
        }

        // ---------------------------------------------------------
        // 131 - 274: ROCAS MARRONES
        // ---------------------------------------------------------
        if (altura <= 274f)
        {
            return Random.Range(0, 2) == 0
                ? _rocaMarron1
                : _rocaMarron2;
        }

        // ---------------------------------------------------------
        // 275 - 516: GRANIZO
        // ---------------------------------------------------------
        if (altura <= 516f)
        {
            return Random.Range(0, 2) == 0
                ? _granizo1
                : _granizo2;
        }

        // ---------------------------------------------------------
        // 517 - 860: ASTEROIDES
        // ---------------------------------------------------------
        return Random.Range(0, 2) == 0
            ? _asteroide1
            : _asteroide2;
    }
}
