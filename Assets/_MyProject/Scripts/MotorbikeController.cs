using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class MotorbikeController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 10f;          // Velocità di movimento laterale
    [SerializeField] private float maxLeanAngle = 30f;       // Angolo massimo di inclinazione
    [SerializeField] private float leanSpeed = 5f;           // Velocità di inclinazione
    [SerializeField] private float returnSpeed = 3f;         // Velocità di ritorno alla posizione normale
    [SerializeField] private float roadWidth = 10f;          // Larghezza della strada
    [SerializeField] private float forwardSpeed = 20f;       // Velocità costante in avanti

    [Header("Debug")]
    [SerializeField] private bool showDebugGizmos = true;

    private Rigidbody rb;
    private Vector2 moveInput;
    private float currentLean;
    private Vector3 startingPosition;
    private InputSystem_Actions playerInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        startingPosition = transform.position;

        // Inizializza il sistema di input
        playerInput = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        // Abilita l'action map Player
        playerInput.Player.Enable();

        // Sottoscrivi alla callback del movimento
        playerInput.Player.Move.performed += OnMove;
        playerInput.Player.Move.canceled += OnMove;
    }

    private void OnDisable()
    {
        // Disabilita l'action map e rimuovi i callback
        playerInput.Player.Disable();
        playerInput.Player.Move.performed -= OnMove;
        playerInput.Player.Move.canceled -= OnMove;
    }

    private void FixedUpdate()
    {
        // Calcola il movimento
        Vector3 currentPosition = rb.position;

        // La moto è ruotata di -90 gradi, quindi usiamo -transform.forward per il movimento in avanti
        Vector3 forwardMovement = -transform.forward * forwardSpeed * Time.fixedDeltaTime;
        Vector3 lateralMovement = -transform.right * moveInput.x * moveSpeed * Time.fixedDeltaTime;

        // Calcola la nuova posizione
        Vector3 newPosition = currentPosition + forwardMovement + lateralMovement;

        // Limita il movimento laterale
        newPosition = ClampPositionToRoad(newPosition);

        // Applica il movimento
        rb.MovePosition(newPosition);

        // Gestisce l'inclinazione della moto
        HandleLean();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void HandleLean()
    {
        // Calcola l'angolo di inclinazione target basato sull'input
        float targetLean = moveInput.x * maxLeanAngle;

        // Interpola dolcemente verso l'angolo target
        currentLean = Mathf.Lerp(currentLean, targetLean,
            moveInput.x != 0 ? leanSpeed * Time.fixedDeltaTime : returnSpeed * Time.fixedDeltaTime);

        // Mantiene la Y a -90 e applica l'inclinazione sulla Z
        Quaternion targetRotation = Quaternion.Euler(0, transform.eulerAngles.y, currentLean);
        rb.MoveRotation(targetRotation);
    }

    private Vector3 ClampPositionToRoad(Vector3 position)
    {
        // Calcola i limiti della strada basati sulla posizione iniziale
        float leftLimit = startingPosition.x - roadWidth / 2;
        float rightLimit = startingPosition.x + roadWidth / 2;

        // Limita la posizione della moto
        position.x = Mathf.Clamp(position.x, leftLimit, rightLimit);
        return position;
    }

    private void OnDrawGizmos()
    {
        if (!showDebugGizmos) return;

        // Disegna i limiti della strada
        Gizmos.color = Color.yellow;
        Vector3 leftLimit = startingPosition + Vector3.left * (roadWidth / 2);
        Vector3 rightLimit = startingPosition + Vector3.right * (roadWidth / 2);
        Gizmos.DrawLine(leftLimit, leftLimit + Vector3.forward * 50f);
        Gizmos.DrawLine(rightLimit, rightLimit + Vector3.forward * 50f);
    }
}