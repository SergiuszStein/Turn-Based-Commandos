using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    private bool _isItThisPlayersTurn;
    public int _playerIndex;
    [SerializeField] private PlayerManager _playerManager;
    [SerializeField] private UI _uI;

    [Header("Player Movement")]
    [SerializeField] private Collider _collider;
    [SerializeField, Range(1, 10000)] private float _playerSpeed;
    [SerializeField] public Rigidbody _rigidbody;
    [SerializeField, Range(1, 10000)] public float _jumpForce;

    private Vector3 _wasdInputVector3;
    private Vector3 _3PcameraForward;
    private Vector3 _3PcameraRight;
    private Vector3 _movementDirection;
    private Vector3 _lastMovementDirection = Vector3.forward;
    private bool _isTouchingBounce;

    [Header("3rd Person Camera")] [Range(1, 1000)]
    public float _3pCameraSensitivity;

    [Range(1, 10000)] public float _3pCameraZoomSensitivity;
    [SerializeField] private GameObject _camera;
    [SerializeField] private GameObject _3pCameraHorizontalAnchor;
    [SerializeField] private GameObject _3pCameraVerticalAnchor;
    [SerializeField] private GameObject _3pCameraZoomAnchor;
    [SerializeField] private GameObject _3pCameraAimAnchor;
    [SerializeField] private GameObject _crosshair;
    private Vector3 _mouseInputVector;
    private float _3pCameraVerticalRotation;
    private float _3pCameraHorizontalRotation;
    private float _3pCameraZoom = -5;

    [Header("3D Model")]
    [SerializeField] private GameObject _playerModel;
    [SerializeField] private GameObject _shotgunModel;
    [SerializeField] public string _playerColor;
    
    [Header("Health")]
    [SerializeField] private GameObject _healthBarAnchor;
    [SerializeField] private Image _healthBar;
    [SerializeField] private GameObject _blood;
    [SerializeField] private float _fullHealth = 100;
    [SerializeField] private float _currentHealth;
    public bool _isAlive = true;
    
    private int _healthPackHeal;

    [Header("Combat")]
    [SerializeField] private GameObject _shotgunBlast;
    [SerializeField] private GameObject _shotgunFrontAnchor;
    [SerializeField] private GameObject _shotgunBullet;
    [SerializeField] private GameObject _boomModel;
    [SerializeField, Range(0, 10)] private float _boomRadius;
    [SerializeField, Range(0, 100)] private int _clogDamage;
    private bool _isAiming = false;
    private int _shotgunAmmoMax = 4;
    private int _shotgunAmmoCurrent;
    
    [Header("Physics")]
    [SerializeField] private PhysicMaterial _friction;
    [SerializeField] private PhysicMaterial _noFriction;
    
    private void Awake()
    {
        _currentHealth = _fullHealth;
        _shotgunAmmoCurrent = _shotgunAmmoMax;
    }

    private void Update()
    {
        if (!_isItThisPlayersTurn || !_isAlive)
        {
            Rotate_healthBarAnchor();
            
            return;
        }
        
        //Player movement
        Update_wasdInputVector3();
        Update_movementDirection();
        Move_Player();
        Jump_Player();
        DieIfFellFromMap();

        //Aiming
        Check_ifIsAiming();

        //3rd Person Camera
        Update_mouseInputVector3();
        Update_3pCameraVerticalRotation();
        Rotate_3pCameraHorizontalAnchor();
        Rotate_3pCameraVerticalAnchor();
        Rotate_3pCameraAimAnchor();
        Zoom_3pCamera();
        Move_camera();

        //3D model
        Rotate_playerModel();
        Rotate_shotgunModel();
        
        //Combat
        ShootShotgun();
    }

    public void ActivatePlayer(bool isItThatPlayersTurn, bool playerTurn)
    {
        _isItThisPlayersTurn = isItThatPlayersTurn;
        
        _healthBarAnchor.gameObject.SetActive(!isItThatPlayersTurn);
        
        Update_HealthBar();

        _isAiming = false;

        _lastMovementDirection = _playerModel.transform.forward;
        _3pCameraHorizontalAnchor.transform.rotation = _playerModel.transform.rotation;

        _crosshair.SetActive(false);
        
        _rigidbody.velocity = Vector3.zero;

        if (playerTurn)
        {
            _rigidbody.isKinematic = !isItThatPlayersTurn;
            _rigidbody.useGravity = isItThatPlayersTurn;

            if (isItThatPlayersTurn)
            {
                _collider.material = _noFriction;
            }
            else
            {
                _collider.material = _friction;
            }
        }
        else
        {
            _rigidbody.isKinematic = false;
            _rigidbody.useGravity = true;
            
            _collider.material = _friction;
        }

        if (isItThatPlayersTurn)
        {
            _uI.Update_AmmoDisplay(_shotgunAmmoCurrent);
        }
    }

    //
    //PLAYER MOVEMENT CODE
    //

    //Updates vector of wasd input
    private void Update_wasdInputVector3()
    {
        _wasdInputVector3 = Vector3.Normalize(
            new Vector3(
                Input.GetAxisRaw("Horizontal"),
                0,
                Input.GetAxisRaw("Vertical")
            )
        );
    }

    //Creates movement direction from wasd input and camera horizontal anchor y rotation
    private void Update_movementDirection()
    {
        if (_isAiming)
        {
            _movementDirection = (_playerModel.transform.forward * _wasdInputVector3.z) + (_playerModel.transform.right * _wasdInputVector3.x);
        }
        else
        {
            _movementDirection = (_3pCameraHorizontalAnchor.transform.forward * _wasdInputVector3.z) + (_3pCameraHorizontalAnchor.transform.right * _wasdInputVector3.x);
        }
    }

    //Moves player in direction of wasd vector and modifies it by his speed
    private void Move_Player()
    {
        _rigidbody.velocity = new Vector3(
            _movementDirection.x * (Time.deltaTime * _playerSpeed),
            _rigidbody.velocity.y,
            _movementDirection.z * (Time.deltaTime * _playerSpeed)
        );
    }
    
    //Checks if grounded
    private bool Check_ifGrounded()
    {
        return Physics.Raycast((transform.position + new Vector3(0, 0.1f, 0)), -transform.up, 0.5f, LayerMask.GetMask("Ground"));
    }
    
    private bool Check_ifOnBarrel()
    {
        return Physics.Raycast((transform.position + new Vector3(0, 0.1f, 0)), -transform.up, 0.5f, LayerMask.GetMask("GasBarrel"));
    }
    
    //Makes player character go up when you press space
    public void Jump_Player()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (Check_ifGrounded() || Check_ifOnBarrel())
            {
                _rigidbody.AddForce(new Vector3(0, _jumpForce, 0));
            }
        }
    }

    //Kill player if he falls off the map
    private void DieIfFellFromMap()
    {
        if (transform.position.y <= -5)
        {
            _playerManager.NextTurn();
            
            LmaoDed();
        }
    }

    //
    //AIMING CODE
    //

    private void Check_ifIsAiming()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            _isAiming = true;

            _crosshair.SetActive(true);
        }

        if (Input.GetButtonUp("Fire2"))
        {
            _isAiming = false;

            _lastMovementDirection = _playerModel.transform.forward;
            _3pCameraHorizontalAnchor.transform.rotation = _playerModel.transform.rotation;

            _crosshair.SetActive(false);
        }
    }

    //
    //3RD PERSON CAMERA CODE
    //

    //Updates vector of mouse input
    private void Update_mouseInputVector3()
    {
        _mouseInputVector = 10 * new Vector3(
            Input.GetAxis("Mouse X"),
            Input.GetAxis("Mouse Y"),
            0
        );
    }

    //Updates vertical rotation of the vertical anchor along with mouse movement taking into account camera sensitivity and clamps it between a set amount of degrees
    private void Update_3pCameraVerticalRotation()
    {
        _3pCameraVerticalRotation = Mathf.Clamp(
            (_3pCameraVerticalRotation + ((_mouseInputVector.y * _3pCameraSensitivity * Time.deltaTime) * -1)),
            -45,
            90
        );
    }

    //Rotates horizontal anchor along with mouse movement taking into account camera sensitivity
    private void Rotate_3pCameraHorizontalAnchor()
    {
        _3pCameraHorizontalAnchor.transform.Rotate(
            0,
            (_mouseInputVector.x * _3pCameraSensitivity * Time.deltaTime),
            0,
            Space.Self
        );
    }

    //Rotates vertical anchor using camera vertical rotation
    private void Rotate_3pCameraVerticalAnchor()
    {
        _3pCameraVerticalAnchor.transform.localEulerAngles = new Vector3(
            _3pCameraVerticalRotation,
            0,
            0
        );
    }

    //Rotates anchor for aiming
    private void Rotate_3pCameraAimAnchor()
    {
        _3pCameraAimAnchor.transform.localEulerAngles = new Vector3(_3pCameraVerticalRotation, 0, 0);
    }

    //Makes the zoom anchor go closer or further away from player model and controls that movement using scroll wheel
    private void Zoom_3pCamera()
    {
        _3pCameraZoom = Mathf.Clamp(
            _3pCameraZoom + (Input.GetAxisRaw("Mouse ScrollWheel") * _3pCameraZoomSensitivity * Time.deltaTime),
            -10,
            -2.5f
        );

        _3pCameraZoomAnchor.transform.localPosition = new Vector3(
            0,
            0,
            _3pCameraZoom
        );
    }


    //Moves camera to the correct place depending on if you are aiming
    private void Move_camera()
    {
        if (_isAiming)
        {
            _camera.transform.position = Vector3.Lerp(
                _camera.transform.position,
                _3pCameraAimAnchor.transform.position,
                Time.deltaTime * 25);

            _camera.transform.rotation = Quaternion.Lerp(
                _camera.transform.rotation,
                _3pCameraAimAnchor.transform.rotation,
                Time.deltaTime * 25
            );
        }
        else
        {
            _camera.transform.position = Vector3.Lerp(
                _camera.transform.position,
                _3pCameraZoomAnchor.transform.position,
                Time.deltaTime * 25);

            _camera.transform.rotation = Quaternion.Lerp(
                _camera.transform.rotation,
                _3pCameraZoomAnchor.transform.rotation,
                Time.deltaTime * 25
            );
        }
    }

    //
    //3D MODEL CODE
    //

    //Rotates player character towards the last movement direction
    private void Rotate_playerModel()
    {
        if (_isAiming)
        {
            _playerModel.transform.Rotate(
                0,
                (_mouseInputVector.x * (_3pCameraSensitivity * Time.deltaTime)),
                0,
                Space.Self
            );
        }
        else
        {
            if (_movementDirection != Vector3.zero)
            {
                _lastMovementDirection = _movementDirection;
            }

            _playerModel.transform.rotation = Quaternion.Lerp(
                _playerModel.transform.rotation,
                Quaternion.LookRotation(_lastMovementDirection),
                Time.deltaTime * 25
            );
        }
    }

    //Rotates the shotgun model to be facing where player is aiming
    private void Rotate_shotgunModel()
    {
        if (_isAiming)
        {
            _shotgunModel.transform.localEulerAngles = new Vector3(
                _3pCameraVerticalRotation,
                0,
                0
            );
        }
        else
        {
            _shotgunModel.transform.localEulerAngles = Vector3.zero;
        }
    }

    //
    //COMBAT FUNCTIONS CODE
    //

    //Lowers health when takes damage
    public void Damage(int damage)
    {
        _currentHealth -= damage;

        Update_HealthBar();

        Instantiate(_blood, transform.position, Quaternion.identity);

        if (_currentHealth <= 0)
        {
            LmaoDed();
        }

        if (_isItThisPlayersTurn)
        {
            _playerManager.NextTurn();
        }
    }
    
    public void Heal(int healing)
    {
        _currentHealth += healing;

        Update_HealthBar();

        if (_currentHealth >= 10)
        {
            _currentHealth = 100;
        }
    }
    
    public void Bounce(Vector3 origin, float force)
    {
        if (origin.y < 0)
        {
            origin.y *= -1;
        }
        
        _rigidbody.isKinematic = false;
        _rigidbody.useGravity = true;
        
        _rigidbody.AddForce((origin * force), ForceMode.Impulse);
    }

    //Removes player from players list and deletes him
    public void LmaoDed()
    {
        _playerManager.PlayerDied(_playerIndex, this.gameObject);
    }

    //Rotates health bar to face camera
    private void Rotate_healthBarAnchor()
    {
        _healthBarAnchor.transform.rotation = Quaternion.LookRotation(
            Vector3.Normalize(
                (_camera.transform.position - _healthBarAnchor.transform.position)
            )
        );
    }

    //Updates the health bar above head or on screen depending on if this player is active
    private void Update_HealthBar()
    {
        if (_isItThisPlayersTurn)
        {
            _uI.Update_MainHealthBar(_currentHealth, _fullHealth);
        }
        else
        {
            _healthBar.fillAmount = _currentHealth / _fullHealth;
        }
    }
    
    //Shoots shotgun using
    private void ShootShotgun()
    {
        if (_isAiming)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                if (_shotgunAmmoCurrent > 0)
                {
                    Collider[] _overlapSphere = Physics.OverlapSphere(_shotgunFrontAnchor.transform.position, 0.1f);
                    
                    if (_overlapSphere.Length != 0)
                    {
                        Collider[] _hitColliders = Physics.OverlapSphere(_shotgunFrontAnchor.transform.position, _boomRadius);

                        for (int i = 0; i < _hitColliders.Length; i++)
                        {
                            GameObject _hit = _hitColliders[i].gameObject;
            
                            if (_hit.layer == LayerMask.NameToLayer("Player"))
                            {
                                _hit.GetComponent<Player>().Damage(_clogDamage);
                            }
                            else if (_hit.layer == LayerMask.NameToLayer("GasBarrel"))
                            {
                                _hit.GetComponent<GasBarrel>().Damage();
                            }
                        }

                        Instantiate(_boomModel, transform.position, Quaternion.identity);
                        
                        _shotgunAmmoCurrent = 0;
                        _uI.Update_AmmoDisplay(_shotgunAmmoCurrent);
                        
                        _playerManager.NextTurn();
                    }
                    else
                    {
                        Instantiate(
                            _shotgunBullet,
                            _shotgunFrontAnchor.transform.position,
                            _camera.transform.rotation
                        );

                        Instantiate(
                            _shotgunBlast,
                            _shotgunFrontAnchor.transform.position,
                            _camera.transform.rotation
                        );
                                                    
                        _shotgunAmmoCurrent--;
                        _uI.Update_AmmoDisplay(_shotgunAmmoCurrent);
                                                    
                        _playerManager.NextTurn();
                    }
                }
            }
        }
    }

    public void Arm(int ammo)
    {
        _shotgunAmmoCurrent += ammo;

        if (_shotgunAmmoCurrent > _shotgunAmmoMax)
        {
            _shotgunAmmoCurrent = _shotgunAmmoMax;
        }
        
        _uI.Update_AmmoDisplay(_shotgunAmmoCurrent);
    }
}