using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// アイテムにカーソルが乗った際に説明を表示するスクリプト
/// </summary>
public class FollowCursor : MonoBehaviour
{

    #region 変数宣言
    [SerializeField] private RectTransform panel; // 表示するパネルのRectTransform
    [SerializeField] private Canvas canvas;       // 対象のキャンバス

    [SerializeField] private float defaultDistanceFromCamera = 10f; // カメラからの距離
    [SerializeField] private Vector3 panelOffset;  // パネルの表示位置オフセット
    #endregion

    #region Unityイベント
    private void Start()
    {
        panel = GetComponent<RectTransform>();
    }

    private void Update()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = defaultDistanceFromCamera;

        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        panel.position = worldPosition + panelOffset;
    }
    #endregion
}
