using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI の「購入」ボタンにアタッチして使うコントローラ。
/// クリック時に、指定したドロップダウンの購入処理（BuyItem）を呼び出す中継役。
/// </summary>
public class BuyButtonController : MonoBehaviour
{
    #region 変数宣言
    [SerializeField] private GameObject dropdownObject; // 購入先のドロップダウン UI（DropdownController が付いている想定）
    #endregion

    #region カスタムメソッド
    /// <summary>
    /// 「購入」ボタンがクリックされたときに呼ばれる想定のハンドラ。
    /// </summary>
    public void OnClicked()
    {
        DropdownController dropdownController = dropdownObject.GetComponent<DropdownController>(); // ドロップダウンの操作用コンポーネントを取得
        dropdownController.BuyItem();                                                              // 実際の購入処理を呼び出し（実装は DropdownController 側）
    }
    #endregion
}
