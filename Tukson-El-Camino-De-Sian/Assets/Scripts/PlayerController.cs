using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Congiguracion de Movimiento")]
    [SerializeField] private float velocidadMovimiento = 8f;
    [SerializeField] private float FuezaSalto = 12f;

    [Header("Deteccion de Suelo")]
    [SerializeField] private Transform comprobadorSuelo;
    [SerializeField] private float radioComprobacion = 0.2f;
    [SerializeField] private LayerMask capaSuelo;

    private Rigidbody2D rb;
    private float inputHorizontal;
    private bool enElSuelo;
    private bool mirandoDerecha = true;

    private void Awake()
    {
        //Guardamos la referencia del Player en el Rigidbody2D
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        //1. Leer las teclas (A/D | Flechas | Stick del Joystick)
        inputHorizontal = Input.GetAxisRaw("Horizontal");

        //2. Comprobamos si estamos pisando el suelo mediante un circulo de deteccion
        enElSuelo = Physics2D.OverlapCircle(comprobadorSuelo.position, radioComprobacion, capaSuelo);
        
        //3. Salto (Espacio / Boton "A" del Mando)
        if (Input.GetButtonDown("Jump") && enElSuelo)
        {
            Saltar();
        }
        GirarSprite();

    }

    private void FixedUpdate()
    {
        //Aplicamos la velocidad horizontal usando las fisicas de Unity
        rb.linearVelocity = new Vector2(inputHorizontal * velocidadMovimiento, rb.linearVelocity.y);
    }

    private void Saltar()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, FuezaSalto);
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
        //Dibuja en el editor el circulo rojo para ver donde detecta el suelo
        if (comprobadorSuelo != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(comprobadorSuelo.position, radioComprobacion);
        }
    }













}














