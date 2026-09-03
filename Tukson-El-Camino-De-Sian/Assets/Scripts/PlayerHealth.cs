using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Salud del jugador")]
    [SerializeField] private int vidaMaxima = 100;
    private int vidaActual;

    private void Start()
    {
        vidaActual = vidaMaxima;
        
        if ( HUDManager.Instancia != null )
        {
            HUDManager.Instancia.InicializarBarraVida(vidaMaxima);
        }
    }

    public void RecibirDano(int cantidadDano)
    {
        vidaActual -= cantidadDano;
        vidaActual = Mathf.Clamp(vidaActual, 0, vidaMaxima);

        if (HUDManager.Instancia != null)
        {
            HUDManager.Instancia.ActualizarVidaUI(vidaActual);
        }

        Debug.Log($"Sian Recibio daño. Vida actual: {vidaActual}");

        if (vidaActual <= 0 )
        {
            Morir();
        }
    }

    public void Curar(int Curacion)
    {
        vidaActual += Curacion;
        vidaActual = Mathf.Clamp(vidaActual, 0, vidaMaxima);

        if (HUDManager.Instancia != null )
        {
            HUDManager.Instancia.ActualizarVidaUI(vidaActual);
        }
    }

    private void Morir()
    {
        Debug.Log("Sian murio");

    }

    private void Update()
    {
        //SIMULACION DE RECIBIR DAÑO
        if (Input.GetKeyDown(KeyCode.O))
        {
            RecibirDano(10);
        }
    }












}
