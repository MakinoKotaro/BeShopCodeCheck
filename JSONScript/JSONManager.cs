using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// プレイヤーデータをJSONで保存・読み込みするマネージャー
/// </summary>
public class JSONManager : MonoBehaviour
{
    #region 変数宣言

    [SerializeField] private PlayerInfo playerInfo;              // ゲーム中のプレイヤーデータ
    [SerializeField] private PlayerInfo savedPlayerInfo;         // 保存されたプレイヤーデータ（中断データなど）

    private string datapath;                                     // データ保存先のパス
    private bool isInitialized = false;                          // 初期化が完了したかどうかのフラグ
    private bool clickContinue = false;                          // 続きからプレイを選んだかのフラグ

    #endregion

    #region プロパティ

    /// <summary>プレイヤーデータの取得・設定</summary>
    public PlayerInfo PlayerInfo { get => playerInfo; set => playerInfo = value; }

    /// <summary>データパスの取得・設定</summary>
    public string Datapath { get => datapath; set => datapath = value; }

    #endregion

    #region Unityイベント

    /// <summary>
    /// データパスの初期設定とJSONファイルの初回コピー処理
    /// </summary>
    private void Awake()
    {
#if UNITY_EDITOR
        // エディタ用のパス設定
        datapath = Application.dataPath + "/JSON/PlayerInfo.json";
#endif

#if UNITY_STANDALONE
        // スタンドアロンビルド用の初期読み込みパス
        datapath = Path.Combine(Application.streamingAssetsPath, "JSON/PlayerInfo.json");
#endif

        // 実際の保存先をpersistentDataPathに設定
        string sourcePath = Path.Combine(Application.streamingAssetsPath, "JSON/PlayerInfo.json");
        datapath = Path.Combine(Application.persistentDataPath, "PlayerInfo.json");

        // ファイルが存在しなければストリーミングアセットからコピー
        if (!File.Exists(datapath))
        {
            File.Copy(sourcePath, datapath);
            Debug.Log("Copied JSON file to persistentDataPath: " + datapath);
        }
    }

    /// <summary>
    /// 起動後にデータ変更通知を登録
    /// </summary>
    private void Start()
    {
        if (playerInfo != null)
        {
            // データ変更時にSaveを呼ぶリスナー登録
            playerInfo.AddListener(OnPlayerInfoChanged);
        }
        else
        {
            Debug.Log("PlayerInfo is null");
        }
    }

    /// <summary>
    /// デバッグ用：Jキーで現在のサブスペルを出力
    /// </summary>
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            Debug.Log(playerInfo.NowSubSpell);
        }
    }

    #endregion

    #region データ保存/読み込み処理

    /// <summary>
    /// PlayerInfoに変更があった場合の処理
    /// </summary>
    private void OnPlayerInfoChanged()
    {
        Debug.Log("PlayerInfoが変更されました!");
        SavePlayerInfo(playerInfo);
    }

    /// <summary>
    /// PlayerInfoをJSON形式でファイルに保存する
    /// </summary>
    public void SavePlayerInfo(PlayerInfo player)
    {
        // JSONへ変換
        string jsonstr = JsonUtility.ToJson(player);

        // ファイルに書き込み
        StreamWriter writer = new StreamWriter(datapath, false);
        writer.WriteLine(jsonstr);
        writer.Flush();
        writer.Close();

        Debug.Log("Saved Player Info: " + jsonstr);
    }

    /// <summary>
    /// 現在のプレイヤーデータを保存用の変数にコピーする
    /// </summary>
    public void CopyPlayerInfoToSavedPlayerInfo()
    {
        // 元データがnullならエラー
        if (playerInfo == null)
        {
            Debug.LogError("PlayerInfo is null. Cannot copy data.");
            return;
        }

        // 保存先が未作成ならインスタンス生成
        if (savedPlayerInfo == null)
        {
            Debug.LogWarning("SavedPlayerInfo is null. Creating a new instance.");
            savedPlayerInfo = ScriptableObject.CreateInstance<PlayerInfo>();
        }

        // JSON文字列として保存し、上書きコピー
        string jsonstr = JsonUtility.ToJson(playerInfo);
        JsonUtility.FromJsonOverwrite(jsonstr, savedPlayerInfo);

        Debug.Log("Copied PlayerInfo to SavedPlayerInfo: " + JsonUtility.ToJson(savedPlayerInfo));

        // ファイルに保存
        SaveSavedPlayerInfo();
    }

    /// <summary>
    /// savedPlayerInfo をファイルに保存する
    /// </summary>
    private void SaveSavedPlayerInfo()
    {
        Debug.Log("Saving SavedPlayerInfo...");

        // nullなら新規作成
        if (savedPlayerInfo == null)
        {
            Debug.LogWarning("SavedPlayerInfo is null. Creating a new instance.");
            savedPlayerInfo = ScriptableObject.CreateInstance<PlayerInfo>();
        }

        // ファイルパス作成と保存処理
        string savedPlayerInfoPath = Path.Combine(Application.persistentDataPath, "SavedPlayerInfo.json");
        string jsonstr = JsonUtility.ToJson(savedPlayerInfo);

        try
        {
            File.WriteAllText(savedPlayerInfoPath, jsonstr);
            Debug.Log("Saved SavedPlayerInfo to " + savedPlayerInfoPath);
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to save SavedPlayerInfo: " + ex.Message);
        }
    }

    /// <summary>
    /// 続きからプレイを選択した場合、保存データを読み込む
    /// </summary>
    public void LoadSavedPlayerInfo()
    {
        // 続きからプレイでなければ処理しない
        if (!clickContinue) return;

        Debug.Log("continue");

        string savedPlayerInfoPath = Path.Combine(Application.persistentDataPath, "SavedPlayerInfo.json");

        // ファイルがあれば読み込み処理
        if (File.Exists(savedPlayerInfoPath))
        {
            string jsonstr = File.ReadAllText(savedPlayerInfoPath);

            if (savedPlayerInfo == null)
            {
                savedPlayerInfo = ScriptableObject.CreateInstance<PlayerInfo>();
            }

            // 保存されたJSONを上書き
            JsonUtility.FromJsonOverwrite(jsonstr, savedPlayerInfo);
            Debug.Log("Loaded SavedPlayerInfo: " + jsonstr);

            // 現在のプレイヤーデータに反映
            CopySavedPlayerInfoToPlayerInfo();
        }
        else
        {
            Debug.LogWarning("SavedPlayerInfo.json not found. Creating new instance.");
            savedPlayerInfo = ScriptableObject.CreateInstance<PlayerInfo>();
        }
    }

    /// <summary>
    /// 保存データを現在のプレイヤーデータに反映
    /// </summary>
    private void CopySavedPlayerInfoToPlayerInfo()
    {
        // 保存データが null の場合は中断
        if (savedPlayerInfo == null)
        {
            Debug.LogError("SavedPlayerInfo is null. Cannot copy data.");
            return;
        }

        // プレイヤーデータが null の場合は新規作成
        if (playerInfo == null)
        {
            Debug.LogWarning("PlayerInfo is null. Creating a new instance.");
            playerInfo = ScriptableObject.CreateInstance<PlayerInfo>();
        }

        // JSON経由でコピー
        string jsonstr = JsonUtility.ToJson(savedPlayerInfo);
        JsonUtility.FromJsonOverwrite(jsonstr, playerInfo);

        Debug.Log("Copied SavedPlayerInfo to PlayerInfo: " + JsonUtility.ToJson(playerInfo));
    }

    #endregion

    #region ボタン処理

    /// <summary>
    /// 続きからプレイを選択した時に呼ばれる
    /// </summary>
    public void OnClickContinue()
    {
        clickContinue = true;
    }

    /// <summary>
    /// 新規ゲーム開始時に呼ばれる
    /// </summary>
    public void OnClicknNewGame()
    {
        clickContinue = false;
    }

    #endregion
}
