using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalInputs : NetworkBehaviour
{
    private bool _jumpButton;
    private bool _isFirePressed;

    NetworkInputData _data;

    private Vector2 _mouseDir;

    [SerializeField] private Transform _weapon;
    private Camera _myCam;
    [SerializeField] private LayerMask _mouseMask;


    public static LocalInputs Instance { get; private set; }

    private void Start()
    {
        _myCam = Camera.main;
    }

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            Instance = this;
            _data = new NetworkInputData();
        }
        else
        {
            enabled = false;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space))
        {
            _jumpButton = true;
        }

        if (Input.GetMouseButtonDown(0))
        {
            _isFirePressed = true;
        }

        SetMouseDir();


        //_jumpButton = _jumpButton | Input.GetKeyDown(KeyCode.W);
        //_fireButton = _fireButton | Input.GetKeyDown(KeyCode.Space);
    }

    public NetworkInputData UpdateInputs()
    {
        _data.xAxi = Input.GetAxis("Horizontal");

        _data.buttons.Set(PlayerButtons.Jump, _jumpButton);
        _jumpButton = false;

        _data.isFirePressed = _isFirePressed;
        _isFirePressed = false;

        _data.direction = _mouseDir;

        return _data;
    }

    private void SetMouseDir()
    {
        if (!_myCam) return;

        if (!Physics.Raycast(_myCam.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo
                , Mathf.Infinity, _mouseMask)) return;

        var mousePos = hitInfo.point;

        mousePos.z = 0;

        _mouseDir = mousePos - _weapon.transform.position;
    }
}
