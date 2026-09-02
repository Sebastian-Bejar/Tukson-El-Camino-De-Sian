using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Configuracion de Movimiento")]
    [SerializeField] private float velocidadMovimiento = 8f;
    [SerializeField] private float fuerzaSalto = 12f;

    [Header("Deteccion de Suelo y Rampas")]
    [SerializeField] private Transform comprobadorSuelo;
    [SerializeField] private float radioComprobacion = 0.08f;
    [SerializeField] private LayerMask capaSuelo;
    [SerializeField] private float longitudRaycastRampa = 0.5f;

    private Rigidbody2D rb;
    private float inputHorizontal;
    private bool enElSuelo;
    private bool deseoSaltar;
    private bool mirandoDerecha = true;

    private Vector2 normalSuelo;
    private bool enRampa;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // 1. Leer entrada horizontal
        inputHorizontal = Input.GetAxisRaw("Horizontal");

        // 2. Capturar intención de salto
        if (Input.GetButtonDown("Jump"))
        {
            deseoSaltar = true;
        }

        GirarSprite();
    }

    private bool estabaEnRampa;

    private void FixedUpdate()
    {
        ComprobarSueloYRampas();

        // 1. Movimiento en Rampa vs Suelo Plano / Aire
        if (enElSuelo && enRampa && !deseoSaltar)
        {
            // Moverse alineado a la rampa
            Vector2 direccionRampa = Vector2.Perpendicular(normalSuelo).normalized;
            Vector2 velocidadRampa = -inputHorizontal * velocidadMovimiento * direccionRampa;
            rb.linearVelocity = velocidadRampa;
            estabaEnRampa = true;
        }
        else
        {
            // SI VENÍAMOS DE LA RAMPA Y SALIMOS VOLANDO SIN SALTAR:
            // Cortamos el impulso en Y para que caiga de inmediato sin despegar.
            if (estabaEnRampa && !enElSuelo && !deseoSaltar && rb.linearVelocity.y > 0)
            {
                rb.linearVelocity = new Vector2(inputHorizontal * velocidadMovimiento, 0f);
            }
            else
            {
                rb.linearVelocity = new Vector2(inputHorizontal * velocidadMovimiento, rb.linearVelocity.y);
            }

            if (enElSuelo)
            {
                estabaEnRampa = false;
            }
        }

        // 2. Salto limpio
        if (deseoSaltar)
        {
            if (enElSuelo)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);
                estabaEnRampa = false;
            }
            deseoSaltar = false;
        }
    }

    private void ComprobarSueloYRampas()
    {
        // Detección de suelo por círculo
        enElSuelo = Physics2D.OverlapCircle(comprobadorSuelo.position, radioComprobacion, capaSuelo);

        // Raycast hacia abajo para detectar el ángulo de la rampa
        RaycastHit2D hit = Physics2D.Raycast(comprobadorSuelo.position, Vector2.down, longitudRaycastRampa, capaSuelo);

        if (hit)
        {
            normalSuelo = hit.normal;
            // Si la normal no apunta 100% hacia arriba (Vector2.up), estamos en una rampa
            enRampa = Vector2.Angle(normalSuelo, Vector2.up) > 0.1f;
        }
        else
        {
            enRampa = false;
        }
    }

    private void GirarSprite()
    {
        if (inputHorizontal > 0 && !mirandoDerecha)
        {
            Girar();
        }
        else if (inputHorizontal < 0 && mirandoDerecha)
        {
            Girar();
        }
    }

    private void Girar()
    {
        mirandoDerecha = !mirandoDerecha;
        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;
    }

    private void OnDrawGizmosSelected()
    {
        if (comprobadorSuelo != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(comprobadorSuelo.position, radioComprobacion);
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(comprobadorSuelo.position, Vector2.down * longitudRaycastRampa);
        }
    }
}