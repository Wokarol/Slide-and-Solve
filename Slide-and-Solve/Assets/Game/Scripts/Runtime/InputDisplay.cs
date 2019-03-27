using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputDisplay : MonoBehaviour
{
    [SerializeField]
    Image
        _up = null,
        _down = null,
        _left = null,
        _right = null,
        _space = null;
    [SerializeField] Color _defaultColor = Color.white;
    [SerializeField] Color _pressedColor = Color.white;
    [SerializeField] float _speed = 1;

    bool targetUpState, targetDownState, targetLeftState, targetRightState, _targetSpaceState;

    private void Start() {
        _up.color = _defaultColor;
        _down.color = _defaultColor;
        _left.color = _defaultColor;
        _right.color = _defaultColor;
        _space.color = _defaultColor;
    }

    private void Update() {
        HandleInput(ref targetUpState, _up, KeyCode.UpArrow);
        HandleInput(ref targetDownState, _down, KeyCode.DownArrow);
        HandleInput(ref targetLeftState, _left, KeyCode.LeftArrow);
        HandleInput(ref targetRightState, _right, KeyCode.RightArrow);
        HandleInput(ref _targetSpaceState, _space, KeyCode.Space);
    }

    private void HandleInput(ref bool targetState, Image image, KeyCode key) {
        targetState = Input.GetKey(key);
        image.color = image.color.MoveTowards(targetState ? _pressedColor : _defaultColor, Time.deltaTime * _speed);
    }
}
