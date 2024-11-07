using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MoveControl : MonoBehaviour
{
    public CharacterController _characterController;
    public Animator _anim;

    [Header("Motion Settings")]
    [SerializeField] float _moveSpeed = 5f;         // Velocidade de movimento do jogador
    [SerializeField] float _rotationSpeed = 720f;   // Velocidade de rotação
    [SerializeField] float _gravityValue = -9.81f;  // Valor da gravidade

    private Vector3 moveDirection;        // Direção do movimento
    private Vector3 _playerVelocity;      // Velocidade do jogador para cálculo de gravidade
    private float inputX;                 // Entrada no eixo horizontal
    private float inputY;                 // Entrada no eixo vertical
    private bool _isPunching;             // Para verificar se o soco está sendo realizado
    private bool _isGrounded;             // Verifica se o personagem está no chão

    void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _anim = GetComponent<Animator>();
    }

    void Update()
    {
        ProcessInput();        // Processa a entrada de movimentação
        Move();                // Aplica o movimento
        ApplyGravity();        // Aplica a gravidade
        DetectPunchInput();    // Detecta o toque ou pressionamento da tecla "E" para o soco
    }

    void ProcessInput()
    {
        // Captura o input para movimentação
        inputX = Input.GetAxis("Horizontal");
        inputY = Input.GetAxis("Vertical");

        // Normaliza a direção do movimento
        moveDirection = new Vector3(inputX, 0, inputY).normalized;

        // Atualiza a variável Speed no Animator para a Blend Tree 1D
        float speed = moveDirection.magnitude * _moveSpeed;  // Calcula a velocidade total
        _anim.SetFloat("Speed", speed);  // Atualiza o parâmetro "Speed" no Animator
    }

    void DetectPunchInput()
    {
        // Detecta o toque na tela ou o pressionamento da tecla "E"
        if (Touchscreen.current.primaryTouch.press.isPressed || Keyboard.current.eKey.isPressed)
        {
            OnPunch(); // Chama o método de soco
        }
    }

    void OnPunch()
    {
        if (!_isPunching) // Previne soco contínuo
        {
            _isPunching = true;
            _anim.SetTrigger("Punch"); // Aciona o trigger de soco
            Invoke("ResetPunch", 0.5f); // Reseta o estado de soco após 0.5s (tempo da animação)
        }
    }

    void ResetPunch()
    {
        _isPunching = false; // Reseta a variável de soco
    }

    void ApplyGravity()
    {
        // Aplica a gravidade se o personagem não estiver no chão
        if (!_isGrounded)
        {
            _playerVelocity.y += _gravityValue * Time.deltaTime;
        }
        else if (_isGrounded && _playerVelocity.y < 0)
        {
            // Se está no chão e a velocidade vertical é menor que zero, reseta a velocidade vertical
            _playerVelocity.y = 0f;
        }

        // Aplica a movimentação final considerando a velocidade vertical
        _characterController.Move(_playerVelocity * Time.deltaTime);
    }

    public void Move()
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

    // Verifica se o personagem está no chão
    void CheckGroundStatus()
    {
        float groundCheckDistance = 0.1f;
        _isGrounded = _characterController.isGrounded || Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);
    }
}
