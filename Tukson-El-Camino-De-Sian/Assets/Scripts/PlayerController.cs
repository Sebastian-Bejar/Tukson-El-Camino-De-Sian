using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Configuracion de Movimiento")]
    [SerializeField] private float velocidadMovimiento = 8f;
    [SerializeField] private float fuerzaSalto = 12f;
    [SerializeField] private float multiplicadorCorteSalto = 0.5f; // Cuánto frena la subida al soltar el botón

    [Header("Game Feel del Salto")]
    [SerializeField] private float coyoteTime = 0.15f; // Tiempo de gracia al caer (en segundos)
    [SerializeField] private float jumpBufferTime = 0.15f; // Tiempo de espera para ejecutar el salto antes de tocar piso

    [Header("Deteccion de Suelo y Rampas")]
    [SerializeField] private Transform comprobadorSuelo;
    [SerializeField] private float radioComprobacion = 0.08f;
    [SerializeField] private LayerMask capaSuelo;
    [SerializeField] private float longitudRaycastRampa = 0.5f;

    private Rigidbody2D rb;
    private float inputHorizontal;
    private bool enElSuelo;
    private bool mirandoDerecha = true;

    private Vector2 normalSuelo;
    private bool enRampa;
    private bool estabaEnRampa;

    // Timers de Game Feel
    private float coyoteTimeCounter;
    private float jumpBufferCounter;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // 1. Lectura de Input Horizontal
        inputHorizontal = Input.GetAxisRaw("Horizontal");

        // 2. Gestion de Timers para el Jump Buffer y Coyote Time
        if (enElSuelo)
        {
            coyoteTimeCounter = coyoteTime; // Carga completa del tiempo de gracia al estar en piso
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime; // Se consume mientras está en el aire
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime; // Guarda la intención de saltar por X tiempo
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        // 3. Salto Variable (Cortar la altura al soltar la tecla)
        if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * multiplicadorCorteSalto);
            coyoteTimeCounter = 0f; // Evitar dobles saltos raros
        }

        GirarSprite();
    }

    private void FixedUpdate()
    {
        ComprobarSueloYRampas();

        // 1. Movimiento Horizontal & Rampas
        if (enElSuelo && enRampa && jumpBufferCounter <= 0)
        {
            Vector2 direccionRampa = Vector2.Perpendicular(normalSuelo).normalized;
            Vector2 velocidadRampa = -inputHorizontal * velocidadMovimiento * direccionRampa;
            rb.linearVelocity = velocidadRampa;
            estabaEnRampa = true;
        }
        else
        {
            if (estabaEnRampa && !enElSuelo && jumpBufferCounter <= 0 && rb.linearVelocity.y > 0)
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

        // 2. Ejecutar Salto con Coyote Time & Jump Buffer
        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);

            // Consumir los timers para evitar saltos infinitos
            jumpBufferCounter = 0f;
            coyoteTimeCounter = 0f;
            estabaEnRampa = false;
        }
    }

    private void ComprobarSueloYRampas()
    {
        enElSuelo = Physics2D.OverlapCircle(comprobadorSuelo.position, radioComprobacion, capaSuelo);

        RaycastHit2D hit = Physics2D.Raycast(comprobadorSuelo.position, Vector2.down, longitudRaycastRampa, capaSuelo);

        if (hit)
        {
            normalSuelo = hit.normal;
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