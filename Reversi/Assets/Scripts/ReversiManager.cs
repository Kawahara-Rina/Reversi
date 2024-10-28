using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReversiManager : MonoBehaviour
{
    // 定数定義
    private const int MAX_SQUARE = 8;  // 盤面のマス数
    private const int NONE = 0;        // マスの状態　空 
    private const int BLACK = 1;       // マスの状態　黒のコマ
    private const int WHITE = -1;      // マスの状態　白のコマ

    // 変数定義
    //[SerializeField] private float radius; // コマの半径

    // デバッグ用
    [SerializeField]private GameObject none;
    [SerializeField]private GameObject black;
    [SerializeField]private GameObject white;
    [SerializeField]private GameObject parent;
    [SerializeField]private GameObject select;
    [SerializeField] private Text turnText;

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

    // カーソルの位置
    private int selectPosX,selectPosY;

    // 黒・白のターン
    private int turn;

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
    }

    // 駒を描画する処理
    private void DrawPiece()
    {
        // オセロのマス分ループし、描画
        for (int i = NONE; i < MAX_SQUARE; i++)
        {
            for (int j = NONE; j < MAX_SQUARE; j++)
            {
                var obj = none;

                // 色別に画像変える
                switch (board[i, j])
                {
                    case NONE:
                        obj = none;
                        break;

                    case BLACK:
                        obj = black;
                        break;

                    case WHITE:
                        obj = white;
                        break;
                }

                // 何もないところ以外は描画
                if (obj != none)
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
    // 上方向にコマが置けるかどうかの判定処理
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
                    // 2の延長線上に置かれている自分と同じ色のコマの最短の位置
                    var myColPos = 0;

                    // 3.2の延長線上に自分と同じ色の石が置かれていること
                    for (int i = _posY; i > 0; i--)
                    {
                        // 自分と同じ色のコマを見つけた場合
                        if (board[_posX, i] == myCol)
                        {
                            // 位置を格納し、ループから抜ける
                            myColPos = i;
                            break;
                        }
                    }

                    // 自分と同じ色のコマの最短の位置までコマをひっくり返す
                    for (int i = _posY; i > myColPos; i--)
                    {
                        board[_posX, i] = myCol;
                    }
                    return true;
                }
            }
        }

        // 上の条件を満たさない場合はfalseを返す(コマを置けない)
        return false;
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
        // 選択しているマス移動
        ChangeSelectPos();

        // コマを配置
        PlaceThePiece();

        // テキストを表示
        DrawText();
    }
}
