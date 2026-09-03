using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Configuracion de Movimiento")]
    [SerializeField] private float velocidadMovimiento = 8f;
    [SerializeField] private float fuerzaSalto = 10f;
    [SerializeField] private float multiplicadorCorteSalto = 0.5f; 

    [Header("Game Feel del Salto")]
    [SerializeField] private float coyoteTime = 0.15f; 
    [SerializeField] private float jumpBufferTime = 0.15f;

    [Header("Sistema de Ataque")]
    [SerializeField] private Transform AttackPoint;
    [SerializeField] private float radioAtaque = 0.5f;
    [SerializeField] private LayerMask capaEnemigos;
    [SerializeField] private int danoAtaque = 10;
    [SerializeField] private float tiempoEntreAtaques = 0.35f;
    private float tiempoSiguienteAtaque = 0f;

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

    
    private float coyoteTimeCounter;
    private float jumpBufferCounter;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
       
        inputHorizontal = Input.GetAxisRaw("Horizontal");

       
        if (enElSuelo)
        {
            coyoteTimeCounter = coyoteTime; 
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime; 
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        
        if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * multiplicadorCorteSalto);
            coyoteTimeCounter = 0f; 
        }

        if (Time.time >= tiempoSiguienteAtaque)
        {
            if (Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.J))
            {
                Atacar();
                tiempoSiguienteAtaque = Time.time + tiempoEntreAtaques;
            }
        }


        GirarSprite();
    }

    private void FixedUpdate()
    {
        ComprobarSueloYRampas();

        
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

       
        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);

            
            jumpBufferCounter = 0f;
            coyoteTimeCounter = 0f;
            estabaEnRampa = false;
        }
    }

    private void Atacar()
    {
        Collider2D[] enemigosGolpeados = Physics2D.OverlapCircleAll(AttackPoint.position, radioAtaque, capaEnemigos);
        
        foreach (Collider2D enemigo in enemigosGolpeados)
        {
            Debug.Log("Gpolpeaste a: " + enemigo.name + "inflingiendo " + danoAtaque + " de daño.");
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

        if (AttackPoint  != null)
        {
            Gizmos.color= Color.yellow;
            Gizmos.DrawWireSphere(AttackPoint.position, radioAtaque);
        }
    }
}