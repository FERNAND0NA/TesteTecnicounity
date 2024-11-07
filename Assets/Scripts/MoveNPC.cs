using System;
using UnityEngine;
using UnityEngine.AI;

public class MoveNPC : MonoBehaviour
{
    private NavMeshAgent _agent;
    [SerializeField] private Transform _player;
    private Animator _anim;

    private CharacterController _characterController;

    [SerializeField] private bool _estaFugindo;

    [SerializeField] private float _distanciaFuga = 10f;
    private float _distanciaPlayer;

    [SerializeField] private float _distanciaDestinoAleatorio = 20f;
    [SerializeField] private float _tempoEspera = 3f;
    private float _tempoAteProximoDestino;

    // Variações de velocidade
    private float VelocidadeIdle = 0f;
    [SerializeField] public float VelocidadeWalk = 3.5f;
    [SerializeField] public float VelocidadeRun = 10.5f;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _anim = GetComponent<Animator>();
        _characterController = Camera.main.GetComponent<CharacterController>();

        if (_characterController != null)
        {
            _player = _characterController.transform;
        }
        else
        {
            Debug.LogWarning("CharacterController não encontrado na câmera principal.");
        }

        DefinirNovoDestinoAleatorio();
    }

    void Update()
    {
        _distanciaPlayer = Vector3.Distance(transform.position, _player.position);

        if (_distanciaPlayer < _distanciaFuga)
        {
            Fugir();
        }
        else
        {
            Patrulha();
        }

        Animacao();
    }

    void Fugir()
    {
        _estaFugindo = true;

        Vector3 direcaoFuga = (transform.position - _player.position).normalized;
        Vector3 destinoFuga = transform.position + direcaoFuga * _distanciaFuga;

        _agent.SetDestination(destinoFuga);
        _agent.speed = VelocidadeRun; // Define a velocidade de fuga
    }

    void Patrulha()
    {
        _estaFugindo = false;

        if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
        {
            _tempoAteProximoDestino -= Time.deltaTime;
            if (_tempoAteProximoDestino <= 0f)
            {
                DefinirNovoDestinoAleatorio();
                _tempoAteProximoDestino = _tempoEspera;
            }
        }
        else
        {
            _agent.speed = VelocidadeWalk; // Define a velocidade de patrulha
        }
    }

    void DefinirNovoDestinoAleatorio()
    {
        Vector3 pontoAleatorio = transform.position + UnityEngine.Random.insideUnitSphere * _distanciaDestinoAleatorio;
        NavMeshHit hit;

        if (NavMesh.SamplePosition(pontoAleatorio, out hit, _distanciaDestinoAleatorio, NavMesh.AllAreas))
        {
            _agent.SetDestination(hit.position);
        }
    }

    void Animacao()
{
    if (_anim != null)
    {
        float velocidade = _agent.velocity.magnitude;

        // Define o parâmetro "Speed" para controlar a Blend Tree
        _anim.SetFloat("Speed", velocidade);
    }
}
}
