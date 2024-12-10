using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This Will Auto Add Character Controller To Gameobject If It's Not Already Applied:
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    // Camera:
    public Camera playerCam;

    // Movement Settings:
    public float walkSpeed = 3f;
    public float runSpeed = 5f;
    public float gravity = 10f;

    // Camera Settings:
    public float lookSpeed = 2f;
    public float lookXLimit = 75f;
    public float cameraRotationSmooth = 5f;

    // Run Bobbing Settings:
    public float runBobIntensity = 0.2f; // Intensitas gerakan
    public float runBobFrequency = 10f;   // Frekuensi gerakan
    private Vector3 originalCameraLocalPosition; // Untuk menyimpan posisi awal kamera
    private float bobOffset = 0f;                // Offset untuk sinusoidal bobbing


    // Ground Sounds:
    public AudioClip[] woodFootstepSounds;
    public AudioClip[] tileFootstepSounds;
    public AudioClip[] carpetFootstepSounds;
    public Transform footstepAudioPosition;
    public AudioSource audioSource;

    private bool isWalking = false;
    private bool isFootstepCoroutineRunning = false;
    private AudioClip[] currentFootstepSounds;

    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;
    float rotationY = 0;

    // Camera Zoom Settings:
    public int ZoomFOV = 35;
    public int initialFOV;
    public float cameraZoomSmooth = 1;

    private bool isZoomed = false;

    // Can The Player Move?:
    private bool canMove = true;

    CharacterController characterController;

    void Start()
    {
        // Ensure We Are Using The Character Controller Component:
        characterController = GetComponent<CharacterController>();

        // Lock And Hide Cursor:
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Initialize current footstep sounds to wood sounds by default
        currentFootstepSounds = woodFootstepSounds;

        // Save the original local position of the camera
        originalCameraLocalPosition = playerCam.transform.localPosition;
    }

    void Update()
    {
        // Walking/Running In Action:
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        characterController.Move(moveDirection * Time.deltaTime);

        // Camera Movement In Action:
        if (canMove)
        {
            rotationX -= Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);

            rotationY += Input.GetAxis("Mouse X") * lookSpeed;

            Quaternion targetRotationX = Quaternion.Euler(rotationX, 0, 0);
            Quaternion targetRotationY = Quaternion.Euler(0, rotationY, 0);

            playerCam.transform.localRotation = Quaternion.Slerp(playerCam.transform.localRotation, targetRotationX, Time.deltaTime * cameraRotationSmooth);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotationY, Time.deltaTime * cameraRotationSmooth);
        }

        // Add run bobbing effect when running
        if (characterController.isGrounded)
        {
            if (isRunning && (moveDirection.x != 0 || moveDirection.z != 0))
            {
                // Update bobOffset menggunakan waktu dan frekuensi
                bobOffset += Time.deltaTime * runBobFrequency;

                // Hitung gerakan vertikal dan horizontal dengan fungsi sinus dan kosinus
                float bobVertical = Mathf.Sin(bobOffset) * runBobIntensity; // Gerakan vertikal
                float bobHorizontal = Mathf.Cos(bobOffset) * runBobIntensity * 0.5f; // Gerakan horizontal

                // Terapkan ke posisi lokal kamera
                playerCam.transform.localPosition = originalCameraLocalPosition + new Vector3(bobHorizontal, bobVertical, 0);
            }
            else
            {
                // Reset posisi kamera ke posisi awal saat tidak berlari
                playerCam.transform.localPosition = Vector3.Lerp(
                    playerCam.transform.localPosition,
                    originalCameraLocalPosition,
                    Time.deltaTime * 10f
                );
                bobOffset = 0f;
            }
        }

        // Zooming In Action:
        if (Input.GetButtonDown("Fire2"))
        {
            isZoomed = true;
        }

        if (Input.GetButtonUp("Fire2"))
        {
            isZoomed = false;
        }

        if (isZoomed)
        {
            playerCam.GetComponent<Camera>().fieldOfView = Mathf.Lerp(playerCam.fieldOfView, ZoomFOV, Time.deltaTime * cameraZoomSmooth);
        }
        else
        {
            playerCam.GetComponent<Camera>().fieldOfView = Mathf.Lerp(playerCam.fieldOfView, initialFOV, Time.deltaTime * cameraZoomSmooth);
        }

        // Play footstep sounds when walking
        if ((curSpeedX != 0f || curSpeedY != 0f) && !isWalking && !isFootstepCoroutineRunning)
        {
            isWalking = true;
            StartCoroutine(PlayFootstepSounds(1.3f / (isRunning ? runSpeed : walkSpeed)));
        }
        else if (curSpeedX == 0f && curSpeedY == 0f)
        {
            isWalking = false;
        }
    }

    // Play footstep sounds with a delay based on movement speed
    IEnumerator PlayFootstepSounds(float footstepDelay)
    {
        isFootstepCoroutineRunning = true;

        while (isWalking)
        {
            if (currentFootstepSounds.Length > 0)
            {
                int randomIndex = Random.Range(0, currentFootstepSounds.Length);
                audioSource.transform.position = footstepAudioPosition.position;
                audioSource.clip = currentFootstepSounds[randomIndex];
                audioSource.Play();
                yield return new WaitForSeconds(footstepDelay);
            }
            else
            {
                yield break;
            }
        }

        isFootstepCoroutineRunning = false;
    }

    // Detect ground surface and set the current footstep sounds array accordingly
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wood"))
        {
            currentFootstepSounds = woodFootstepSounds;
        }
        else if (other.CompareTag("Tile"))
        {
            currentFootstepSounds = tileFootstepSounds;
        }
        else if (other.CompareTag("Carpet"))
        {
            currentFootstepSounds = carpetFootstepSounds;
        }
    }
}
