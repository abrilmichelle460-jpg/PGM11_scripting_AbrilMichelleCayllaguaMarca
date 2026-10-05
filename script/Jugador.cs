using System.Collections.Generic;
using UnityEngine;
using TMPro;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(Animator))]
public class Jugador : MonoBehaviour
{
    // Los identificadores coinciden con los parámetros configurados en PjController.
    private static readonly int IdentificadorVelocidad = Animator.StringToHash("Velocidad");
    private static readonly int IdentificadorVelocidadVertical = Animator.StringToHash("VelocidadVertical");
    private static readonly int IdentificadorEstaEnPiso = Animator.StringToHash("estaEnPiso");

    [Header("Movimiento")]
    [SerializeField] private float velocidadMovimiento = 3f;
    [SerializeField] private float fuerzaSalto = 7f;

    [Header("Interfaz")]
    [SerializeField] private TMP_Text textoContadorAbejas;

    private int cantidadAbejas;

    private readonly HashSet<Collider2D> superficiesDeSuelo = new();
    private Rigidbody2D cuerpoRigido;
    private SpriteRenderer renderizadorSprite;
    private Animator animador;
    private float direccionHorizontal;
    private bool estaEnSuelo;
    private bool saltoSolicitado;

    private void Awake()
    {
        cuerpoRigido = GetComponent<Rigidbody2D>();
        renderizadorSprite = GetComponent<SpriteRenderer>();
        animador = GetComponent<Animator>();
        cantidadAbejas = 0;
        ActualizarTextoContador();
    }

    private void Update()
    {
        LeerMovimientoHorizontal();
        ActualizarOrientacion();
        ActualizarAnimaciones();

        if (Input.GetKeyDown(KeyCode.Space) && estaEnSuelo)
        {
            saltoSolicitado = true;
        }
    }

    private void FixedUpdate()
    {
        cuerpoRigido.linearVelocity = new Vector2(
            direccionHorizontal * velocidadMovimiento,
            cuerpoRigido.linearVelocity.y
        );

        if (!saltoSolicitado)
        {
            return;
        }

        cuerpoRigido.linearVelocity = new Vector2(
            cuerpoRigido.linearVelocity.x,
            fuerzaSalto
        );
        saltoSolicitado = false;
    }

    private void LeerMovimientoHorizontal()
    {
        bool moverIzquierda = Input.GetKey(KeyCode.A);
        bool moverDerecha = Input.GetKey(KeyCode.D);

        if (moverIzquierda == moverDerecha)
        {
            direccionHorizontal = 0f;
            return;
        }

        direccionHorizontal = moverIzquierda ? -1f : 1f;
    }

    private void ActualizarOrientacion()
    {
        if (renderizadorSprite == null || direccionHorizontal == 0f)
        {
            return;
        }

        renderizadorSprite.flipX = direccionHorizontal < 0f;
    }

    // Sincroniza el Animator con la velocidad y el contacto real con el suelo.
    private void ActualizarAnimaciones()
    {
        animador.SetFloat(IdentificadorVelocidad, Mathf.Abs(cuerpoRigido.linearVelocity.x));
        animador.SetFloat(IdentificadorVelocidadVertical, cuerpoRigido.linearVelocity.y);
        animador.SetBool(IdentificadorEstaEnPiso, estaEnSuelo);
    }

    private void OnCollisionEnter2D(Collision2D colision)
    {
        RegistrarContactoConSuelo(colision);
    }

    private void OnCollisionStay2D(Collision2D colision)
    {
        RegistrarContactoConSuelo(colision);
    }

    private void OnCollisionExit2D(Collision2D colision)
    {
        superficiesDeSuelo.Remove(colision.collider);
        ActualizarEstadoDeSuelo();
    }

    private void RegistrarContactoConSuelo(Collision2D colision)
    {
        bool contactoInferior = false;

        foreach (ContactPoint2D contacto in colision.contacts)
        {
            if (contacto.normal.y > 0.5f)
            {
                contactoInferior = true;
                break;
            }
        }

        if (contactoInferior)
        {
            superficiesDeSuelo.Add(colision.collider);
        }
        else
        {
            superficiesDeSuelo.Remove(colision.collider);
        }

        ActualizarEstadoDeSuelo();
    }

    private void ActualizarEstadoDeSuelo()
    {
        estaEnSuelo = superficiesDeSuelo.Count > 0;

        if (!estaEnSuelo)
        {
            saltoSolicitado = false;
        }
    }

    // Mantiene sincronizado el valor visible con la cantidad recogida.
    private void ActualizarTextoContador()
    {
        if (textoContadorAbejas != null)
        {
            textoContadorAbejas.text = cantidadAbejas.ToString();
        }
    }

    // Retira la abeja de la escena cuando el jugador entra en su área de recolección.
    private void OnTriggerEnter2D(Collider2D otroColisionador)
    {
        if (!otroColisionador.CompareTag("abejita"))
        {
            return;
        }

        cantidadAbejas++;
        ActualizarTextoContador();
        Destroy(otroColisionador.gameObject);
    }

}
