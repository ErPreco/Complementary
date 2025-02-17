using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour {
    public static Player Instance { get; private set; }

    [SerializeField] private Vector3 playerCenter;
    [SerializeField] private float playerRadius = 0.5f;
    [SerializeField] private float skinWidth = 0.05f;
    [Space]
    [SerializeField] private float movementSpeed = 8;
    [SerializeField] private float jumpHeight = 3;
    [SerializeField] private float coyoteJumpTime = 0.05f;
    [SerializeField] private float jumpBufferTime = 0.1f;
    [SerializeField] private float gravityScale = 3;
    [SerializeField] private LayerMask collisionableLayerMask;
    [SerializeField] private LayerMask platformLayerMask;
    [SerializeField] private LayerMask endLayerMask;
    [SerializeField] private LayerMask hazardLayerMask;
    [Space]
    [SerializeField] private GameObject playerVisualGameObject;
    [SerializeField] private ParticleSystem playerDeathParticles;

    private float playerHeight;
    private Vector3 gravity;
    private float yVelocity;
    private float coyoteJumpTimer;
    private float jumpBufferTimer;
    private bool stopUpdate;

    private void Awake() {
        if (Instance != null) {
            Debug.LogError("A Player instance was already created.");
        }
        Instance = this;

        playerHeight = playerCenter.y * 2;
    }

    private void Start() {
        GameInput.Instance.OnJumpAction += GameInput_OnJumpAction;
        PlatformsParent.Instance.OnPlatformsSwitched += Platforms_OnPlatformsSwitched;
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;

        gravity = Physics.gravity * gravityScale;
        stopUpdate = true;

        transform.position += Vector3.up * skinWidth;
    }

    private void Update() {
        if (stopUpdate) return;

        HandleJump();
    }

    private void FixedUpdate() {
        if (stopUpdate) return;

        HandleMovement();
    }

    private void GameInput_OnJumpAction(object sender, EventArgs e) {
        // player has jumpBufferTime seconds to touch the ground and actually jump
        jumpBufferTimer = jumpBufferTime;
    }
    
    private void Platforms_OnPlatformsSwitched(object sender, EventArgs e) {
        if (CheckPlatformOverlapping(transform.position)) {
            // player is stuck into a platform
            for (int attempt = 1; attempt <= 2; attempt++) {
                // tries to find a close point outside the platform where to move the player
                List<(DistancePoint distancePoint, bool isRayInOut)> raycastHitDistancePointWithDirectionList = new List<(DistancePoint, bool)>();
                for (int theta = 0; theta < 360; theta += 45) {
                    // draws some rays towards the player to find a spot because if the ray starts inside a collider it does not detect the object
                    Vector3 playerCenterInWorldSpace = transform.position + playerCenter;
                    float angleRad = theta * Mathf.PI / 180;

                    // rays from outside to player center
                    Vector3 rayOrigin = playerCenterInWorldSpace + new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad), 0) * attempt;
                    Vector3 rayDir = playerCenterInWorldSpace - rayOrigin;
                    if (Physics.Raycast(rayOrigin, rayDir, out RaycastHit raycastHit, rayDir.magnitude, platformLayerMask)) {
                        // the ray started outside the platform because it hit something
                        raycastHitDistancePointWithDirectionList.Add((new DistancePoint(raycastHit.point, playerCenterInWorldSpace), false));
                    }

                    // rays from player center to outside
                    rayOrigin = playerCenterInWorldSpace;
                    rayDir = new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad), 0) * attempt;
                    if (Physics.Raycast(rayOrigin, rayDir, out raycastHit, rayDir.magnitude, platformLayerMask)) {
                        // the ray started outside the platform because it hit something
                        raycastHitDistancePointWithDirectionList.Add((new DistancePoint(raycastHit.point, playerCenterInWorldSpace), true));
                    }
                    Debug.DrawLine(rayOrigin, rayOrigin + rayDir, Color.red);
                }

                // finds the closest spot outside the platform where the player does not overlap any other collider
                raycastHitDistancePointWithDirectionList = raycastHitDistancePointWithDirectionList.OrderBy(tuple => tuple.distancePoint.GetDistance()).ToList();
                foreach ((DistancePoint distancePoint, bool isRayInOut) in raycastHitDistancePointWithDirectionList) {
                    Vector3 movementVector = GetPlayerMovementWithHitPoint(distancePoint.GetPoint1(), isRayInOut);
                    if (!CheckPlatformOverlapping(transform.position + movementVector)) {
                        transform.position += movementVector;
                        return;
                    }
                }
            }

            // no spot was found
            GameManager.Instance.GameOver();
        }
    }

    private void GameManager_OnStateChanged(object sender, EventArgs e) {
        if (GameManager.Instance.IsGamePlaying()) {
            stopUpdate = false;
        } else if (GameManager.Instance.IsGameOver()) {
            stopUpdate = true;
            Kill();
        } else if (GameManager.Instance.IsGameFinished()) {
            stopUpdate = true;
        }
    }

    /// <summary>
    /// Checks if the player overlaps a platform collider.
    /// </summary>
    /// <param name="playerPosition">The position of the Player transform.</param>
    /// <returns>Whether the player collider overlaps a platform collider or not.</returns>
    private bool CheckPlatformOverlapping(Vector3 playerPosition) {
        Vector3 zOffset = Vector3.forward * -5;   // offsets the capsule because it does not detect collisions if they are overlapped
        Vector3 p1 = playerPosition + Vector3.up * (playerRadius + skinWidth) + zOffset;
        Vector3 p2 = playerPosition + Vector3.up * (skinWidth + playerHeight - playerRadius) + zOffset;
        return Physics.CapsuleCast(p1, p2, playerRadius, Vector3.forward, -zOffset.z, platformLayerMask);
    }

    /// <summary>
    /// Returns the vector for which the player needs to move to be close to the given point.
    /// </summary>
    /// <param name="point">The point of the RaycastHit.</param>
    /// <param name="isRayInOut">Whether the ray came from player center to outside or vice versa.</param>
    /// <returns>The vector of the movement.</returns>
    private Vector3 GetPlayerMovementWithHitPoint(Vector3 point, bool isRayInOut) {
        Vector3 playerCenterInWorldSpace = transform.position + playerCenter;
        Vector3 movementVector = point - playerCenterInWorldSpace;
        float movementThreshold = 0.01f;

        float yOffset = 0;
        float xOffset = 0;
        if (isRayInOut) {
            // player center is off the collider
            if (movementVector.y > movementThreshold) {
                // upward movement
                yOffset = -(playerCenter.y + skinWidth);
            } else if (movementVector.y < -movementThreshold) {
                // downward movement
                yOffset = playerCenter.y + skinWidth;
            }

            if (movementVector.x > movementThreshold) {
                // movement to the right
                xOffset = -(playerRadius + skinWidth);
            } else if (movementVector.x < -movementThreshold) {
                // movement to the left
                xOffset = playerRadius + skinWidth;
            }
        } else {
            // player center is inside the collider
            if (movementVector.y > movementThreshold) {
                // upward movement
                yOffset = playerCenter.y + skinWidth;
            } else if (movementVector.y < -movementThreshold) {
                // downward movement
                yOffset = -(playerCenter.y + skinWidth);
            }

            if (movementVector.x > movementThreshold) {
                // movement to the right
                xOffset = playerRadius + skinWidth;
            } else if (movementVector.x < -movementThreshold) {
                // movement to the left
                xOffset = -(playerRadius + skinWidth);
            }
        }
        movementVector.y += yOffset;
        movementVector.x += xOffset;

        return movementVector;
    }

    /// <summary>
    /// Handles the movement.
    /// </summary>
    private void HandleMovement() {
        Vector3 boxCastCenter = transform.position + Vector3.up * playerCenter.y;
        Vector3 halfExtension = new Vector3(playerRadius, playerCenter.y, playerRadius);
        
        HandleHorizontalMovement(boxCastCenter, halfExtension);
        HandleVerticalMovement(boxCastCenter, halfExtension);
    }

    /// <summary>
    /// Handles the horizontal component of the movement.
    /// </summary>
    private void HandleHorizontalMovement(Vector3 boxCastCenter, Vector3 halfExtension) {
        Vector3 movementDirection = Vector3.right * GameInput.Instance.GetMovementDirection();
        float movementMagnitude = movementSpeed * Time.fixedDeltaTime;

        bool isHazardHit = BoxCastHit(boxCastCenter, halfExtension, movementDirection, movementMagnitude, hazardLayerMask);
        if (isHazardHit) {
            // player hit a hazard
            GameManager.Instance.GameOver();
            return;
        }

        bool canMove = !BoxCastHit(boxCastCenter, halfExtension, movementDirection, movementMagnitude, collisionableLayerMask);
        if (canMove) {
            transform.position += movementDirection * movementMagnitude;
        }

        float rotateSpeed = 10;
        transform.forward = Vector3.Slerp(transform.forward, movementDirection, Time.fixedDeltaTime * rotateSpeed);
    }

    /// <summary>
    /// Handles the vertical component of the movement.
    /// </summary>
    private void HandleVerticalMovement(Vector3 boxCastCenter, Vector3 halfExtension) {
        // sets the sign based on the previous velocity direction
        Vector3 movementDirection = yVelocity > 0 ? Vector3.up : Vector3.down;
        // from physics we have delta_v = 0.5 * g * t^2 + v_0 * t
        float movementMagnitude = (yVelocity > 0 ? 1 : -1) * (0.5f * gravity.y * Time.fixedDeltaTime + yVelocity) * Time.fixedDeltaTime;
        
        bool isHazardHit = BoxCastHit(boxCastCenter, halfExtension, movementDirection, movementMagnitude + skinWidth, hazardLayerMask);
        if (isHazardHit) {
            // player hit a hazard
            GameManager.Instance.GameOver();
            return;
        }
        
        bool canMove = !BoxCastHit(boxCastCenter, halfExtension, movementDirection, movementMagnitude + skinWidth, collisionableLayerMask, out RaycastHit raycastHit);
        if (canMove) {
            yVelocity += gravity.y * Time.fixedDeltaTime;
            transform.position += movementDirection * movementMagnitude;
        } else {
            yVelocity = 0;
            float yPlayerPositionCloseToHitPoint = raycastHit.point.y + (movementDirection.y < 0 ? skinWidth : -(playerHeight + skinWidth));
            Vector3 playerPositionCloseToHitPoint = new Vector3(transform.position.x, yPlayerPositionCloseToHitPoint, transform.position.z);
            transform.position = playerPositionCloseToHitPoint;
        }

        if (IsOnObjectWithLayerMask(endLayerMask)) {
            GameManager.Instance.GameFinished();
        }
    }

    /// <summary>
    /// Returns whether a Physics.BoxCast with the given properties collides or not.
    /// </summary>
    /// <param name="boxCastCenter">The center of the box.</param>
    /// <param name="halfExtension">The half extension of the box.</param>
    /// <param name="direction">The direction of the cast.</param>
    /// <param name="magnitude">The magnitude of the cast.</param>
    /// <param name="layerMask">The layer mask of the collision.</param>
    /// <returns>Whether a Physics.BoxCast with the given properties collides or not.</returns>
    private bool BoxCastHit(Vector3 boxCastCenter, Vector3 halfExtension, Vector3 direction, float magnitude, LayerMask layerMask) {
        return Physics.BoxCast(boxCastCenter, halfExtension, direction, Quaternion.identity, magnitude, layerMask);
    }

    /// <summary>
    /// Returns whether a Physics.BoxCast with the given properties collides or not.
    /// </summary>
    /// <param name="boxCastCenter">The center of the box.</param>
    /// <param name="halfExtension">The half extension of the box.</param>
    /// <param name="direction">The direction of the cast.</param>
    /// <param name="magnitude">The magnitude of the cast.</param>
    /// <param name="layerMask">The layer mask of the collision.</param>
    /// <param name="raycastHit">The RaycastHit returned by the Physics.BoxCast.</param>
    /// <returns>Whether a Physics.BoxCast with the given properties collides or not.</returns>
    private bool BoxCastHit(Vector3 boxCastCenter, Vector3 halfExtension, Vector3 direction, float magnitude, LayerMask layerMask, out RaycastHit raycastHit) {
        return Physics.BoxCast(boxCastCenter, halfExtension, direction, out raycastHit, Quaternion.identity, magnitude, layerMask);
    }

    /// <summary>
    /// Handles the jump.
    /// </summary>
    private void HandleJump() {
        if (IsOnObjectWithLayerMask(platformLayerMask)) {
            coyoteJumpTimer = coyoteJumpTime;
        } else {
            coyoteJumpTimer -= Time.deltaTime;
        }
        jumpBufferTimer -= Time.deltaTime;

        if (coyoteJumpTimer > 0 && jumpBufferTimer > 0) {
            // player wanted to jump less than jumpBufferTime seconds ago and
            // touched the ground less than coyoteJumpTime seconds ago
            yVelocity = Mathf.Sqrt(jumpHeight * -2 * gravity.y);

            jumpBufferTimer = 0;
        }
    }

    /// <summary>
    /// Checks if the player is on an object labeled with the given LayerMask.
    /// </summary>
    /// <param name="layerMask">The LayerMask of the object.</param>
    /// <returns>Whether the player is on the object or not.</returns>
    private bool IsOnObjectWithLayerMask(LayerMask layerMask) {
        Vector3 halfExtents = new Vector3(playerRadius, (skinWidth + 0.01f) * 2, playerRadius);
        return Physics.CheckBox(transform.position, halfExtents, Quaternion.identity, layerMask);
    }

    /// <summary>
    /// Kills the player.
    /// </summary>
    private void Kill() {
        Destroy(playerVisualGameObject);
        playerDeathParticles.Play();
    }

    private void OnDestroy() {
        GameInput.Instance.OnJumpAction -= GameInput_OnJumpAction;
        PlatformsParent.Instance.OnPlatformsSwitched -= Platforms_OnPlatformsSwitched;
        GameManager.Instance.OnStateChanged -= GameManager_OnStateChanged;
    }
}
