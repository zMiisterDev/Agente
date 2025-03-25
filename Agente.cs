using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class AgenteML : Agent
{
    public GameObject balaPrefab;
    public Transform puntoDisparo;
    public float velocidadMovimiento = 5f;
    public float fuerzaSalto = 5f;
    public float cadenciaDisparo = 1f;

    private Rigidbody2D rb;
    private bool puedeSaltar = true;
    private bool dobleSaltoDisponible = true;
    private float siguienteDisparo = 0f;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(rb.velocity);
        sensor.AddObservation(puedeSaltar);
        sensor.AddObservation(dobleSaltoDisponible);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float mover = actions.ContinuousActions[0];

        // Movimiento horizontal
        rb.velocity = new Vector2(mover * velocidadMovimiento, rb.velocity.y);
        
        // Salto
        if (actions.DiscreteActions[0] == 1 && (puedeSaltar || dobleSaltoDisponible))
        {
            rb.velocity = new Vector2(rb.velocity.x, fuerzaSalto);
            if (puedeSaltar) puedeSaltar = false;
            else dobleSaltoDisponible = false;
        }

        // Disparo
        if (actions.DiscreteActions[1] == 1 && Time.time >= siguienteDisparo)
        {
            Disparar();
            siguienteDisparo = Time.time + cadenciaDisparo;
        }
    }

    private void Disparar()
    {
        GameObject bala = Instantiate(balaPrefab, puntoDisparo.position, Quaternion.identity);
        bala.GetComponent<Rigidbody2D>().velocity = new Vector2(transform.localScale.x * 10, 0);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Suelo"))
        {
            puedeSaltar = true;
            dobleSaltoDisponible = true;
        }
    }
}
