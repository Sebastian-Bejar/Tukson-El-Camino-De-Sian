using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instancia { get; private set;}
    [Header("UI de Vida")]
    [SerializeField] private Slider BarraDeVida;

    private void Awake()
    {
        if (Instancia == null)
        {
            Instancia = this;
        }
        else 
        {
            Destroy(gameObject);
        }
    }

    public void InicializarBarraVida(int VidaMaxima)
    {
        if(BarraDeVida != null)
        {
            BarraDeVida.maxValue = VidaMaxima;
            BarraDeVida.value = VidaMaxima;
        }
    }

    public void ActualizarVidaUI(int VidaActual)
    {
        if (BarraDeVida != null)
        {
            BarraDeVida.value = VidaActual;
        }
    }

}

