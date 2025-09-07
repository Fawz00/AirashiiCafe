using System;
using Core.Events;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraController : Singleton<CameraController>
{
    public Camera mainCamera;

    // For now, this class is a placeholder for future camera control logic.
    public override void Awake()
    {
        base.Awake();
        if (mainCamera == null)
        {
            mainCamera = GetComponentInChildren<Camera>();
        }
    }
}
