using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ReversiManager : MonoBehaviour
{
    // 定数定義
    private const int MAX_SQUARE = 8;               // 盤面のマス数
    private const int NONE  =  0;                   // マスの状態　空 
    private const int BLACK =  1;                   // マスの状態　黒のコマ
    private const int WHITE = -1;                   // マスの状態　白のコマ
    private const float CELL_SIZE = 1.0f;           // 1コマの大きさ

    // 使用するオブジェクト、テキスト、マテリアル等
    [SerializeField] private GameObject black;      // 表示するコマ「黒」
    [SerializeField] private GameObject white;      // 表示するコマ「白」
    [SerializeField] private GameObject parent;     // コマを生成する領域
    [SerializeField] private GameObject select;     // 選択中のマス
    [SerializeField] private GameObject resultPanel;// リザルト時のパネル
    [SerializeField] private GameObject skipText;   // スキップ時のテキスト
    [SerializeField] private Text turnText;         // ターンを表示するテキスト
    [SerializeField] private Text scoreText;        // 各色のコマ数を表示するテキスト
    [SerializeField] private Text resultText;       // リザルト時のテキスト
    [SerializeField] private Material lineMaterial; // 線描画用のマテリアル

    // オセロの盤面を定義
    // オセロの盤面は8x8
    private int[,] board =
    {
        { NONE, NONE, NONE, NONE, NONE, NONE, NONE, NONE },
        { NONE, NONE, NONE, NONE, NONE, NONE, NONE, NONE },
        { NONE, NONE, NONE, NONE, NONE, NONE, NONE, NONE },
        { NONE, NONE, NONE, WHITE, BLACK, NONE, NONE, NONE },
        { NONE, NONE, NONE, BLACK, WHITE, NONE, NONE, NONE },
        { NONE, NONE, NONE, NONE, NONE, NONE, NONE, NONE },
        { NONE, NONE, NONE, NONE, NONE, NONE, NONE, NONE },
        { NONE, NONE, NONE, NONE, NONE, NONE, NONE, NONE },};

    // カーソルの位置
    private int selectPosX,selectPosY;
    // 置く場所を選択したかどうかのフラグ
    private bool isSelect;

    // 黒・白のターン
    private int turn;
    // 黒・白のコマ数
    private int scoreBk,scoreWh;

    // ターン開始時フラグ
    private bool turnStart;
    // ゲームエンドフラグ
    private bool isGameEnd;

    // リトライボタン押下時の処理
    public void TapRetryButton()
    {
        // ゲームシーンを呼び出す
        SceneManager.LoadScene("GameScene");
    }

    // 初期化処理
    private void Init()
    {
        // コマを描画
        DrawPiece();

        // 盤面描画
        OnRenderObject();

        // 各変数の初期化
        // 選択中のマスのポジション初期化
        selectPosX = 0;
        selectPosY = 0;
        isSelect = false;

        // ターン初期化
        turn = BLACK;
        turnStart = true;

        // スコア初期化
        scoreBk = 2;
        scoreWh = 2;

        // ゲームエンドフラグ初期化
        isGameEnd = false;
    }

    // オブジェクトをレンダリングする関数
    // カメラがシーンをレンダリングした後に呼び出される
    private void OnRenderObject()
    {
        // 描画の基準点を中央に調整
        float offset = -CELL_SIZE / 2;

        // 盤面の描画(緑の四角形、枠線)
        //DrawGreenBackground(offset);
        DrawLines(offset);
    }

    // 緑の四角形を描画する処理
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

                // 色別に画像を変える
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
        // マウス座標と、ワールド座標
        Vector3 mPos,wPos;

        // 画面タップ時
        if (Input.GetMouseButtonDown(0))
        {
            // タップされた座標を取得、ワールド座標に変換する
            mPos = Input.mousePosition;
            wPos = Camera.main.ScreenToWorldPoint(new Vector3(mPos.x, mPos.y, 10f));

            // タップされた場所が盤面内なら
            if ((wPos.x >= -CELL_SIZE / 2 && wPos.x <= MAX_SQUARE - (CELL_SIZE / 2))&&
                (wPos.y <=  CELL_SIZE / 2 && wPos.y >= -(MAX_SQUARE - (CELL_SIZE / 2)))
                )
            {
                // タップ位置を基に、盤面のマスのサイズに補正する
                int gridX = Mathf.FloorToInt(wPos.x / CELL_SIZE + (CELL_SIZE / 2));
                int gridY = Mathf.FloorToInt(wPos.y / CELL_SIZE + (CELL_SIZE / 2));

                // 補正した座標をワールド座標に格納
                wPos.x = gridX * CELL_SIZE;
                wPos.y = gridY * CELL_SIZE;

                // ワールド座標を選択中の座標に格納 
                select.transform.position = wPos;

                // ワールド座標の絶対値を選択中の座標に格納
                selectPosX = Mathf.Abs(Mathf.FloorToInt(wPos.x));
                selectPosY = Mathf.Abs(Mathf.FloorToInt(wPos.y));

                // 選択したフラグを立てる
                isSelect = true;
            }
        }

        #region キーボード用移動操作
        /*
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
        select.transform.position = new Vector2(selectPosX,selectPosY);
        */
        #endregion
    }

    // コマを配置する処理
    private void PlaceThePiece()
    {
        // マスが選択(タップ)された場合
        if (isSelect)
        {
            // そのマスに置けるかどうかの判定
            if (SelectCheck(selectPosX, selectPosY,true))
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

                // ターンを変更、フラグを初期化
                turn *= -1;
                turnStart = true;
                isSelect = false;
            }
        }
    }

    ///<summary>
    // 選択した場所にコマが置けるかどうかの判定処理・ひっくり返す処理
    /// </summary>
    /// <param name="_posX">コマを置く位置のx成分</param>
    /// <param name="_posY">コマを置く位置のy成分</param>
    /// <param name="_isFlip">コマをひっくり返すかどうかのフラグ</param>
    /// <returns>true:コマを置ける　false:コマを置けない</returns>
    private bool SelectCheck(int _posX, int _posY, bool _isFlip)
    {
        // 自分の色
        var myCol = turn;
        // 相手の色
        var enCol = turn * -1;
        // コマを置けるかどうかのフラグ
        var isPlaced = false;
        // 探索したい方向に置かれている自分と同じ色のコマの最短の位置
        //var myColPos = 0;

        // ひっくり返すことができるコマの位置を格納するリスト
        List<(int, int)> flipPos = new List<(int, int)>();

        // 1.石を置こうとするマスに石が置かれていないこと
        if (board[_posX, _posY] != NONE)
        {
            // この条件を満たしていない場合は探索処理終了
            return false;
        }

        // 各方向ごとの探索処理を行うための関数
        // _x:探索方向のx成分(左右)
        // _t:探索方向のy成分(上下)
        // _isFlip:コマをひっくり返すかどうかのフラグ
        void SearchDirection(int _x, int _y)
        {
            // ひっくり返す事ができるコマの位置を格納する一時リスト
            var temp = new List<(int, int)>();

            // 探索する位置を格納
            var x = _posX + _x;
            var y = _posY + _y;

            // 盤面内で探索を行う
            while (x >= 0 && x < MAX_SQUARE && y >= 0 && y < MAX_SQUARE)
            {
                // 2.石を置こうとするマスの1マス隣に自分と異なる色の石があること
                if (board[x, y] == enCol)
                {
                    // 一時リストに追加
                    // この時、コマが挟まれているかどうかはわからないため、一時リストに追加しておく
                    temp.Add((x, y));
                }
                // 3.2の延長線上に自分と同じ色の石が置かれていること
                else if (board[x, y] == myCol)
                {
                    // 自分と同じ色のコマを見つけた場合、ひっくり返せることが確定するため
                    // 一時リストに格納したコマの位置をひっくり返す事ができるリストに追加する
                    flipPos.AddRange(temp);

                    break;
                }
                // 上の条件を満たさない場合はループを抜ける(コマを置けない)
                else
                {
                    break;
                }

                // 次に探索する位置に変更
                x += _x;
                y += _y;
            }
        }

        // 上下左右、斜め上下左右方向の探索
        if (_posY >= 2) SearchDirection( 0, -1);   // 上
        if (_posY <= 5) SearchDirection( 0,  1);   // 下
        if (_posX >= 2) SearchDirection(-1,  0);   // 左
        if (_posX <= 5) SearchDirection( 1,  0);   // 右
        if (_posX >= 2 && _posY >= 2) SearchDirection(-1, -1);   // 左斜め上
        if (_posX >= 2 && _posY <= 5) SearchDirection(-1,  1);   // 左斜め下
        if (_posX <= 5 && _posY >= 2) SearchDirection( 1, -1);   // 右斜め上
        if (_posX <= 5 && _posY <= 5) SearchDirection( 1,  1);   // 右斜め下

        // コマを置ける場合は、リスト内の位置をもとに該当するコマをひっくり返す
        if (flipPos.Count > 0)
        {
            // コマをひっくり返すフラグがtrueの場合はひっくり返す
            if (_isFlip)
            {
                foreach (var pos in flipPos)
                {
                    board[pos.Item1, pos.Item2] = myCol;
                }
            }
            // コマを置けるため、フラグを成立
            isPlaced = true;
        }

        // コマを置けるかどうかのフラグを返す
        // true:コマを置ける　　false:コマを置けない
        return isPlaced;

        #region 8方向探索の元の考え方(関数化前)
        /*
        #region 右斜め上方向の探索
        // 右斜め上方向の探索
        // 検索範囲の除外
        // 石を置いたときに挟むことが出来ない上2マス・右2マスの場所
        if (_posY >= 2 && _posX <= 5)
        {
            // 1.石を置こうとするマスに石が置かれていないこと
            if (board[_posX, _posY] == NONE)
            {
                // 2.石を置こうとするマスの1マス右上に自分と異なる色の石があること
                if (board[_posX + 1, _posY - 1] == enCol)
                {
                    // 3.2の延長線上に自分と同じ色の石が置かれていること
                    // TODO　多方向ひっくり返せるようにする
                    for (int i = 0; i < MAX_SQUARE; i++)
                    {
                        // 範囲外参照を防ぐ条件チェック 
                        if (_posX + i >= MAX_SQUARE || _posY - i < 0)
                        {
                            break;
                        }

                        // 自分と同じ色のコマを見つけた場合
                        if (board[_posX+i, _posY-i] == myCol)
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
                        for (int i = 0; i < myColPos; i++)
                        {
                            board[_posX+i, _posY-i] = myCol;
                        }
                    }

                }
            }
        }
        #endregion

        #region 右斜め下方向の探索
        // 右斜め下方向の探索
        // 検索範囲の除外
        // 石を置いたときに挟むことが出来ない下2マス・右2マスの場所
        if (_posY <= 5 && _posX <= 5)
        {
            // 1.石を置こうとするマスに石が置かれていないこと
            if (board[_posX, _posY] == NONE)
            {
                // 2.石を置こうとするマスの1マス右下に自分と異なる色の石があること
                if (board[_posX + 1, _posY + 1] == enCol)
                {
                    // 3.2の延長線上に自分と同じ色の石が置かれていること
                    // TODO　多方向ひっくり返せるようにする
                    for (int i = 0; i < MAX_SQUARE; i++)
                    {
                        // 範囲外参照を防ぐ条件チェック 
                        if (_posX + i >= MAX_SQUARE || _posY + i >= MAX_SQUARE)
                        {
                            break;
                        }

                        // 自分と同じ色のコマを見つけた場合
                        if (board[_posX + i, _posY + i] == myCol)
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
                        for (int i = 0; i < myColPos; i++)
                        {
                            board[_posX + i, _posY + i] = myCol;
                        }
                    }

                }
            }
        }
        #endregion

        #region 左斜め上方向の探索
        // 左斜め上方向の探索
        // 検索範囲の除外
        // 石を置いたときに挟むことが出来ない上2マス・左2マスの場所
        if (_posY >= 2 && _posX >= 2)
        {
            // 1.石を置こうとするマスに石が置かれていないこと
            if (board[_posX, _posY] == NONE)
            {
                // 2.石を置こうとするマスの1マス左上に自分と異なる色の石があること
                if (board[_posX - 1, _posY - 1] == enCol)
                {
                    // 3.2の延長線上に自分と同じ色の石が置かれていること
                    // TODO　多方向ひっくり返せるようにする
                    for (int i = 0; i < MAX_SQUARE; i++)
                    {
                        // 範囲外参照を防ぐ条件チェック 
                        if (_posX - i < 0 || _posY - i < 0)
                        {
                            break;
                        }

                        // 自分と同じ色のコマを見つけた場合
                        if (board[_posX - i, _posY - i] == myCol)
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
                        for (int i = 0; i < myColPos; i++)
                        {
                            board[_posX - i, _posY - i] = myCol;
                        }
                    }

                }
            }
        }
        #endregion

        #region 左斜め下方向の探索
        // 左斜め下方向の探索
        // 検索範囲の除外
        // 石を置いたときに挟むことが出来ない上2マス・左2マスの場所
        if (_posY <= 5 && _posX >= 2)
        {
            // 1.石を置こうとするマスに石が置かれていないこと
            if (board[_posX, _posY] == NONE)
            {
                // 2.石を置こうとするマスの1マス左上に自分と異なる色の石があること
                if (board[_posX - 1, _posY + 1] == enCol)
                {
                    // 3.2の延長線上に自分と同じ色の石が置かれていること
                    // TODO　多方向ひっくり返せるようにする
                    for (int i = 0; i < MAX_SQUARE; i++)
                    {
                        // 範囲外参照を防ぐ条件チェック 
                        if (_posX - i < 0 || _posY + i >= MAX_SQUARE)
                        {
                            break;
                        }

                        // 自分と同じ色のコマを見つけた場合
                        if (board[_posX - i, _posY + i] == myCol)
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
                        for (int i = 0; i < myColPos; i++)
                        {
                            board[_posX - i, _posY + i] = myCol;
                        }
                    }

                }
            }
        }
        #endregion


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

        #region 左方向の探索
        // 左方向の探索
        // 検索範囲の除外
        // 石を置いたときに挟むことが出来ない左2マスの場所
        if (_posX >= 2)
        {
            // 1.石を置こうとするマスに石が置かれていないこと
            if (board[_posX, _posY] == NONE)
            {
                // 2.石を置こうとするマスの1マス隣(左)に自分と異なる色の石があること
                if (board[_posX - 1, _posY] == enCol)
                {
                    // 3.2の延長線上に自分と同じ色の石が置かれていること
                    for (int i = _posX; i > 0; i--)
                    {
                        // 自分と同じ色のコマを見つけた場合
                        if (board[i, _posY] == myCol)
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
                        for (int i = _posX; i > myColPos; i--)
                        {
                            board[i, _posY] = myCol;
                        }
                    }

                }
            }
        }
        #endregion

        #region 右方向の探索
        // 右方向の探索
        // 検索範囲の除外
        // 石を置いたときに挟むことが出来ない右2マスの場所
        if (_posX <= 5)
        {
            // 1.石を置こうとするマスに石が置かれていないこと
            if (board[_posX, _posY] == NONE)
            {
                // 2.石を置こうとするマスの1マス隣(右)に自分と異なる色の石があること
                if (board[_posX + 1, _posY] == enCol)
                {
                    // 3.2の延長線上に自分と同じ色の石が置かれていること
                    for (int i = _posX; i < MAX_SQUARE; i++)
                    {
                        // 自分と同じ色のコマを見つけた場合
                        if (board[i, _posY] == myCol)
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
                        for (int i = _posX; i < myColPos; i++)
                        {
                            board[i, _posY] = myCol;
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
        // 上の条件を満たさない場合はfalseを返す(コマを置けない)
        else
        {
            return false;
        }
        */
        #endregion

    }

    // ターン開始時にコマを置く場所があるかどうかを判定する処理
    private void TurnControl()
    {
        // ターンをスキップするかどうかのフラグ
        var isSkip = true;

        // コマがすべて黒・白の場合に使用するフラグ
        var isAllBlack=true;
        var isAllWhite=true;
        // マスが全て埋まっている場合に使用するフラグ
        var isAllPlaced = true;

        // ターン開始時一度だけ判定
        if (turnStart)
        {
            // 盤面全体を確認
            for (int i = 0; i < MAX_SQUARE; i++)
            {
                for (int j = 0; j < MAX_SQUARE; j++)
                {
                    // コマが全て黒・白かどうか判定
                    if (board[i, j] == BLACK)
                    {
                        isAllWhite = false;
                    }
                    else if (board[i, j] == WHITE)
                    {
                        isAllBlack = false;
                    }
                    // マスがすべて埋まっているか判定
                    else
                    {
                        isAllPlaced = false;
                    }

                    // 1マスずつ確認
                    if (SelectCheck(i, j,false))
                    {
                        // 置ける場所があればターン継続(ループから抜ける)
                        isSkip = false;

                        // 全て黒・白の判定のためコメントアウト中
                        //break;
                    }

                }

                // 置ける場所があればターン継続(ループから抜ける)
                //if (!isSkip)
                //{
                //    break;
                //}
            }

            // 判定するかどうかのフラグを初期化
            turnStart = false;

            // コマを置ける場所がなくスキップの場合
            if (isSkip)
            {
                // コマが全て黒・白の場合、全てのマスが埋まっている場合はゲーム終了
                if(isAllBlack || isAllWhite || isAllPlaced)
                {
                    isGameEnd = true;
                    return;
                }

                // スキップ時のテキストを表示
                var animator = skipText.GetComponent<Animator>();
                animator.SetTrigger("Skip");

                // ターンを変更
                turn *= -1;

                return;
            }
        }
    }

    // ゲーム終了時の処理
    private void GameEnd()
    {
        if (isGameEnd)
        {
            // リザルトパネル表示
            resultPanel.SetActive(true);

            var resText = "";

            // 勝利プレイヤーの表示
            // 黒のコマが多い場合
            if(scoreBk > scoreWh)
            {
                resText = "黒の勝利";
            }
            // 白のコマが多い場合
            else if(scoreWh > scoreBk)
            {
                resText = "白の勝利";
            }
            // 同点の場合
            else
            {
                resText = "引き分け";
            }

            // リザルトの表示
            resultText.text = resText;
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
        // コマを置ける場所があるかを確認し、ターン制御
        TurnControl();

        // 選択しているマス移動
        ChangeSelectPos();

        // コマを配置
        PlaceThePiece();

        // テキストを表示
        DrawText();

        // ゲーム終了時の処理
        GameEnd();
    }
}
