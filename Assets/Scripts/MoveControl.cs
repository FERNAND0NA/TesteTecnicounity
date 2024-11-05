using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class MoveControl : MonoBehaviour
{
    public CharacterController _characterController;
    public Animator _anim;

    [Header("Motion Settings")]
    [SerializeField] float _moveSpeed = 5f;         // Velocidade de movimento do jogador
    [SerializeField] float _rotationSpeed = 720f;   // Velocidade de rotação
    [SerializeField] float _jumpHeight = 1.5f;      // Altura do salto
    [SerializeField] float _gravityValue = -9.81f;  // Valor da gravidade

    private Vector3 moveDirection;        // Direção do movimento
    private Vector3 _playerVelocity;      // Velocidade do jogador para cálculo do pulo e gravidade
    private float inputX;                 // Entrada no eixo horizontal
    private float inputY;                 // Entrada no eixo vertical
    [SerializeField] bool _isGrounded;                     // Verifica se o personagem está no chão
    [SerializeField] bool _jumpInput;                      // Controla o estado de pulo
    private float _gravityMultiplier = 0; // Multiplicador de gravidade (ajustável se o personagem estiver no ar)

    void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _anim = GetComponent<Animator>();
    }

    void Update()
    {
        CheckGroundStatus();
        ProcessInput();
        Move();
        ApplyGravity();

        // Debug para verificar o valor de inputX e inputY
        Debug.Log("inputX: " + inputX + ", inputY: " + inputY);
    }

    void ProcessInput()
    {
        // Captura o input para movimentação
        inputX = Input.GetAxis("Horizontal");
        inputY = Input.GetAxis("Vertical");

        // Normaliza a direção do movimento
        moveDirection = new Vector3(inputX, 0, inputY).normalized;

        // Atualiza as variáveis no Animator para o Blend Tree
        _anim.SetFloat("InputX", inputX);
        _anim.SetFloat("InputY", inputY);
        _anim.SetBool("GroundCheck", _isGrounded);

        // Processa o input de pulo
        if (_isGrounded && _jumpInput)
        {
            _playerVelocity.y = Mathf.Sqrt(_jumpHeight * -2f * _gravityValue);
            _jumpInput = false; // Reseta o input de pulo após saltar
        }
    }

    void Move()
    {
        if (moveDirection.magnitude > 0)
        {
            // Aplica a rotação suave na direção do movimento
            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, _rotationSpeed * Time.deltaTime);

            // Movimento baseado no CharacterController
            _characterController.Move(moveDirection * _moveSpeed * Time.deltaTime);
        }
    }

    void ApplyGravity()
    {
        // Aplica o multiplicador de gravidade se o personagem não estiver no chão
        if (!_isGrounded)
        {
            _playerVelocity.y += _gravityValue * _gravityMultiplier * Time.deltaTime;
        }
        else if (_isGrounded && _playerVelocity.y < 0)
        {
            // Se está no chão e a velocidade vertical é menor que zero, reseta a velocidade vertical
            _playerVelocity.y = 0f;
        }

        // Aplica a movimentação final considerando a velocidade vertical
        _characterController.Move(_playerVelocity * Time.deltaTime);
    }

    void CheckGroundStatus()
    {
        // Distância mínima para considerar que o personagem está no chão
        float groundCheckDistance = 0.1f;

        // Usamos Raycast para verificar o contato com o chão
        _isGrounded = _characterController.isGrounded ||
                      Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);

        // Define o multiplicador de gravidade baseado no status do chão
        _gravityMultiplier = _isGrounded ? 1f : 2f;

        // Se estiver no chão e a velocidade vertical é menor que zero, reseta a velocidade vertical
        if (_isGrounded && _playerVelocity.y < 0)
        {
            _playerVelocity.y = 0f;
        }
    }

    // Função para lidar com o input de pulo (Unity New Input System)
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _jumpInput = true;
        }
    }
}