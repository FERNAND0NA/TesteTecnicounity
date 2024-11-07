using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MoveControl : MonoBehaviour
{
    public CharacterController _characterController;
    public Animator _anim;

    [Header("Motion Settings")]
    [SerializeField] float _moveSpeed = 5f;         // Velocidade de movimento do jogador
    [SerializeField] float _rotationSpeed = 720f;   // Velocidade de rota��o
    [SerializeField] float _gravityValue = -9.81f;  // Valor da gravidade

    private Vector3 moveDirection;        // Dire��o do movimento
    private Vector3 _playerVelocity;      // Velocidade do jogador para c�lculo de gravidade
    private float inputX;                 // Entrada no eixo horizontal
    private float inputY;                 // Entrada no eixo vertical
    private bool _isPunching;             // Para verificar se o soco est� sendo realizado
    private bool _isGrounded;             // Verifica se o personagem est� no ch�o

    void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _anim = GetComponent<Animator>();
    }

    void Update()
    {
        ProcessInput();        // Processa a entrada de movimenta��o
        Move();                // Aplica o movimento
        ApplyGravity();        // Aplica a gravidade
        DetectPunchInput();    // Detecta o toque ou pressionamento da tecla "E" para o soco
    }

    void ProcessInput()
    {
        // Captura o input para movimenta��o
        inputX = Input.GetAxis("Horizontal");
        inputY = Input.GetAxis("Vertical");

        // Normaliza a dire��o do movimento
        moveDirection = new Vector3(inputX, 0, inputY).normalized;

        // Atualiza a vari�vel Speed no Animator para a Blend Tree 1D
        float speed = moveDirection.magnitude * _moveSpeed;  // Calcula a velocidade total
        _anim.SetFloat("Speed", speed);  // Atualiza o par�metro "Speed" no Animator
    }

    void DetectPunchInput()
    {
        // Detecta o toque na tela ou o pressionamento da tecla "E"
        if (Touchscreen.current.primaryTouch.press.isPressed || Keyboard.current.eKey.isPressed)
        {
            OnPunch(); // Chama o m�todo de soco
        }
    }

    void OnPunch()
    {
        if (!_isPunching) // Previne soco cont�nuo
        {
            _isPunching = true;
            _anim.SetTrigger("Punch"); // Aciona o trigger de soco
            Invoke("ResetPunch", 0.5f); // Reseta o estado de soco ap�s 0.5s (tempo da anima��o)
        }
    }

    void ResetPunch()
    {
        _isPunching = false; // Reseta a vari�vel de soco
    }

    void ApplyGravity()
    {
        // Aplica a gravidade se o personagem n�o estiver no ch�o
        if (!_isGrounded)
        {
            _playerVelocity.y += _gravityValue * Time.deltaTime;
        }
        else if (_isGrounded && _playerVelocity.y < 0)
        {
            // Se est� no ch�o e a velocidade vertical � menor que zero, reseta a velocidade vertical
            _playerVelocity.y = 0f;
        }

        // Aplica a movimenta��o final considerando a velocidade vertical
        _characterController.Move(_playerVelocity * Time.deltaTime);
    }

    public void Move()
    {
        if (moveDirection.magnitude > 0)
        {
            // Aplica a rota��o suave na dire��o do movimento
            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, _rotationSpeed * Time.deltaTime);

            // Movimento baseado no CharacterController
            _characterController.Move(moveDirection * _moveSpeed * Time.deltaTime);
        }
    }

    // Verifica se o personagem est� no ch�o
    void CheckGroundStatus()
    {
        float groundCheckDistance = 0.1f;
        _isGrounded = _characterController.isGrounded || Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);
    }
}
