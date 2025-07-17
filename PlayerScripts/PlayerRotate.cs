using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotate : MonoBehaviour
{
    #region 変数宣言
    [SerializeField] private Transform player; // プレイヤーのTransform
    [SerializeField] private CinemachineFreeLook freeLookCamera; // Cinemachineカメラ

    private const float rotationSpeed = 5f;
    #endregion

    #region Unityイベント
    private void Update()
    {
        if (freeLookCamera == null || player == null) return;

        Vector3 cameraForward = freeLookCamera.transform.forward;
        cameraForward.y = 0f;
        cameraForward.Normalize();

        if (cameraForward != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(cameraForward);
            player.rotation = Quaternion.Slerp(player.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }
    #endregion
}
