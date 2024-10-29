using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReversiManager : MonoBehaviour
{
    // 定数定義
    private const int MAX_SQUARE = 8;          // 盤面のマス数
    private const int NONE = 0;                // マスの状態　空 
    private const int BLACK = 1;               // マスの状態　黒のコマ
    private const int WHITE = -1;              // マスの状態　白のコマ
    private const float CELL_SIZE = 1.0f;      // 1コマの大きさ

    public Material lineMaterial; // 線描画用のマテリアルをアタッチ

    // 変数定義
    //[SerializeField] private float radius; // コマの半径

    // デバッグ用
    [SerializeField]private GameObject black;  // 表示するコマ「黒」
    [SerializeField]private GameObject white;  // 表示するコマ「白」
    [SerializeField]private GameObject parent; // コマを生成する領域
    [SerializeField]private GameObject select; // 選択中のマス
    [SerializeField] private Text turnText;    // ターンを表示するテキスト
    [SerializeField] private Text scoreText;   // 各色のコマ数を表示するテキスト

    // オセロの盤面を定義
    // オセロの盤面は8x8
    //private int[,] board = new int[MAX_SQUARE,MAX_SQUARE];
    private int[,] board =
    {
        { NONE, NONE, NONE, NONE, NONE, NONE, NONE, NONE },
        { NONE, NONE, NONE, NONE, NONE, NONE, NONE, NONE },
        { NONE, NONE, NONE, NONE, NONE, NONE, NONE, NONE },
        { NONE, NONE, NONE, WHITE, BLACK, NONE, NONE, NONE },
        { NONE, NONE, NONE, BLACK, WHITE, NONE, NONE, NONE },
        { NONE, NONE, NONE, NONE, NONE, NONE, NONE, NONE },
        { NONE, NONE, NONE, NONE, NONE, NONE, NONE, NONE },
        { NONE, NONE, NONE, NONE, NONE, NONE, NONE, NONE },
    };

    // LineRenderer取得用
    //LineRenderer renderer;

    // カーソルの位置
    private int selectPosX,selectPosY;

    // 黒・白のターン
    private int turn;
    // 黒・白のコマ数
    private int scoreBk,scoreWh;

    // 初期化処理
    private void Init()
    {
        // コマを描画
        DrawPiece();

        // 選択中のマスのポジション初期化
        selectPosX = 0;
        selectPosY = 0;

        // ターン初期化
        turn = BLACK;
        // スコア初期化
        scoreBk = 2;
        scoreWh = 2;

        // 背景描画

        OnRenderObject();


    }

    private void OnRenderObject()
    {
        if (lineMaterial == null)
        {
            Debug.LogWarning("Line Material is not assigned.");
            return;
        }

        // 描画の基準点を中央に調整
        float offset = -CELL_SIZE / 2;

        //DrawGreenBackground(offset);
        DrawLines(offset);
    }

    // 緑の四角形を描画するメソッド
    private void DrawGreenBackground(float offset)
    {
        lineMaterial.SetPass(0);
        GL.Begin(GL.QUADS);
        GL.Color(Color.green);

        GL.Vertex3(offset, offset, 0);
        GL.Vertex3(offset + MAX_SQUARE * CELL_SIZE, offset, 0);
        GL.Vertex3(offset + MAX_SQUARE * CELL_SIZE, offset - MAX_SQUARE * CELL_SIZE, 0);
        GL.Vertex3(offset, offset - MAX_SQUARE * CELL_SIZE, 0);

        GL.End();
    }


    // 枠線を描画する処理
    private void DrawLines(float offset)
    {
        

        // マテリアルのセット
        lineMaterial.SetPass(0);
        GL.Begin(GL.LINES);
        GL.Color(Color.black);

        
        // 縦線を描画
        for (int i = 0; i <= MAX_SQUARE; i++)
        {
            float x = offset + i * CELL_SIZE;
            GL.Vertex3(x,-offset, 0);
            GL.Vertex3(x, -offset - MAX_SQUARE * CELL_SIZE, 0);
        }

        // 横線を描画
        for (int j = 0; j <= MAX_SQUARE; j++)
        {
            float y = -offset - j * CELL_SIZE;
            GL.Vertex3(offset, y, 0);
            GL.Vertex3(offset + MAX_SQUARE * CELL_SIZE, y, 0);
        }

        GL.End();
    }
    // テキストを表示する処理
    private void DrawText()
{
    // ターンの描画
    var player = "";
    if (turn == BLACK)
    {
        player = "黒";
    }
    else
    {
        player = "白";
    }
    // ターンを表示
    turnText.text = "ターン：" + player;

    // スコアの描画
    scoreText.text = "黒：" + scoreBk + "\n白：" + scoreWh;

}

    // 駒を描画する処理
    private void DrawPiece()
    {
        // スコアのリセット
        scoreBk = 0;
        scoreWh = 0;

        // オセロのマス分ループし、描画
        for (int i = NONE; i < MAX_SQUARE; i++)
        {
            for (int j = NONE; j < MAX_SQUARE; j++)
            {
                GameObject obj = null;

                // 色別に画像変える
                switch (board[i, j])
                {
                    case NONE:
                        obj = null;
                        break;

                    case BLACK:
                        obj = black;
                        // スコアを加算
                        scoreBk++;
                        break;

                    case WHITE:
                        obj = white;
                        // スコアを加算
                        scoreWh++;
                        break;
                }

                // 何もないところ以外は描画
                if (obj != null)
                {
                    // コマ生成
                    var piece = Instantiate(obj);
                    piece.transform.SetParent(parent.transform, false);

                    piece.transform.position = new Vector2(i, -j);


                    //Debug.Log("["+i+","+j+"]   " +board[i, j]);

                }
            }
        }
    }

    // 選択中のマスの位置を変更する処理
    private void ChangeSelectPos()
    {
        // 選択しているマス移動
        if (Input.GetKeyDown(KeyCode.A))
        {
            // 端ではなければ移動可能
            if (selectPosX > 0) {
                selectPosX -= 1;
            }
            else {
                selectPosX = MAX_SQUARE - 1;
            }
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            // 端ではなければ移動可能
            if (selectPosX < MAX_SQUARE - 1) {
                selectPosX += 1;
            }
            else {
                selectPosX = 0;
            }
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            // 端ではなければ移動可能
            if (selectPosY > 0) {
                selectPosY -= 1;
            }
            else {
                selectPosY = MAX_SQUARE - 1;
            }
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            // 端ではなければ移動可能
            if (selectPosY < MAX_SQUARE - 1) {
                selectPosY += 1;
            }
            else {
                selectPosY = 0;
            }
        }
        //選択中のマス
        select.transform.position = new Vector2(selectPosX, -selectPosY);
    }

    // コマを配置する処理
    private void PlaceThePiece()
    {

        // スペースキー押下でコマを置く
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // そのマスに置けるかどうかの判定
            // 上方向の探索のみ
            if (SelectCheck(selectPosX, selectPosY))
            {
                // 選択中のマスにコマを置く
                if (turn == BLACK)
                {
                    board[selectPosX, selectPosY] = BLACK;
                }
                else
                {
                    board[selectPosX, selectPosY] = WHITE;
                }

                // コマを消去
                for (int i = NONE; i < parent.transform.childCount; i++)
                {
                    Destroy(parent.transform.GetChild(i).gameObject);
                }

                // コマを再描画
                DrawPiece();

                // ターンを変更
                turn *= -1;
            }
        }
    }

    /// <summary>
    // 上下方向にコマが置けるかどうかの判定処理
    /// </summary>
    /// <param name="_posX">コマを置く位置のx成分</param>
    /// <param name="_posY">コマを置く位置のy成分</param>
    /// <returns></returns>
    private bool SelectCheck(int _posX,int _posY)
    {
        // 自分の色
        var myCol = turn;
        // 相手の色
        var enCol = turn * -1;
        // コマを置けるかどうかのフラグ
        var isPlaced = false;
        // 探索したい方向に置かれている自分と同じ色のコマの最短の位置
        var myColPos = 0;

        #region 上方向の探索
        // 上方向の探索
        // 検索範囲の除外
        // 石を置いたときに挟むことが出来ない上2マスの場所
        if (_posY >= 2)
        {
            // 1.石を置こうとするマスに石が置かれていないこと
            if (board[_posX, _posY] == NONE)
            {
                // 2.石を置こうとするマスの1マス隣(上)に自分と異なる色の石があること
                if (board[_posX, _posY - 1] == enCol)
                {
                    // 3.2の延長線上に自分と同じ色の石が置かれていること
                    for (int i = _posY; i > 0; i--)
                    {
                        // 自分と同じ色のコマを見つけた場合
                        if (board[_posX, i] == myCol)
                        {
                            // コマを置けるため、フラグを立てる
                            isPlaced = true;

                            // 位置を格納し、ループから抜ける
                            myColPos = i;
                            break;
                        }
                    }

                    // コマを置くことができれば、ひっくり返す
                    if (isPlaced)
                    {
                        // 自分と同じ色のコマの最短の位置までコマをひっくり返す
                        for (int i = _posY; i > myColPos; i--)
                        {
                            board[_posX, i] = myCol;
                        }
                    }
                    
                }
            }
        }
        #endregion
        
        #region 下方向の探索
        // 下方向の探索
        // 検索範囲の除外
        // 石を置いたときに挟むことが出来ない下2マスの場所
        if (_posY <= 5)
        {
            // 1.石を置こうとするマスに石が置かれていないこと
            if (board[_posX, _posY] == NONE)
            {
                // 2.石を置こうとするマスの1マス隣(下)に自分と異なる色の石があること
                if (board[_posX, _posY + 1] == enCol)
                {
                    // 3.2の延長線上に自分と同じ色の石が置かれていること
                    for (int i = _posY; i < MAX_SQUARE; i++)
                    {
                        // 自分と同じ色のコマを見つけた場合
                        if (board[_posX, i] == myCol)
                        {
                            // コマを置けるため、フラグを立てる
                            isPlaced = true;

                            // 位置を格納し、ループから抜ける
                            myColPos = i;

                            break;
                        }
                    }

                    // コマを置くことができれば、ひっくり返す
                    if (isPlaced)
                    {
                        // 自分と同じ色のコマの最短の位置までコマをひっくり返す
                        for (int i = _posY; i < myColPos; i++)
                        {
                            board[_posX, i] = myCol;
                        }
                    }
                }
            }
        }
        #endregion

        // 条件を満たしている場合はtrueを返す(コマを置ける)
        if (isPlaced)
        {
            return true;
        }
        else
        {
            // 上の条件を満たさない場合はfalseを返す(コマを置けない)
            return false;
        }

        
    }

    // Start is called before the first frame update
    void Start()
    {
        // 初期化処理
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var mPos= Input.mousePosition;
            Debug.Log("x:" + mPos.x + "    y:" + mPos.y);
        }

        // 選択しているマス移動
        ChangeSelectPos();

        // コマを配置
        PlaceThePiece();

        // テキストを表示
        DrawText();
    }
}
