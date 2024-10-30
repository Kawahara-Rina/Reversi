using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ReversiManager : MonoBehaviour
{
    // �萔��`
    private const int MAX_SQUARE = 8;               // �Ֆʂ̃}�X��
    private const int NONE  =  0;                   // �}�X�̏�ԁ@�� 
    private const int BLACK =  1;                   // �}�X�̏�ԁ@���̃R�}
    private const int WHITE = -1;                   // �}�X�̏�ԁ@���̃R�}
    private const float CELL_SIZE = 1.0f;           // 1�R�}�̑傫��

    // �g�p����I�u�W�F�N�g�A�e�L�X�g�A�}�e���A����
    [SerializeField] private GameObject black;      // �\������R�}�u���v
    [SerializeField] private GameObject white;      // �\������R�}�u���v
    [SerializeField] private GameObject parent;     // �R�}�𐶐�����̈�
    [SerializeField] private GameObject select;     // �I�𒆂̃}�X
    [SerializeField] private GameObject resultPanel;// ���U���g���̃p�l��
    [SerializeField] private GameObject skipText;   // �X�L�b�v���̃e�L�X�g
    [SerializeField] private Text turnText;         // �^�[����\������e�L�X�g
    [SerializeField] private Text scoreText;        // �e�F�̃R�}����\������e�L�X�g
    [SerializeField] private Text resultText;       // ���U���g���̃e�L�X�g
    [SerializeField] private Material lineMaterial; // ���`��p�̃}�e���A��

    // �I�Z���̔Ֆʂ��`
    // �I�Z���̔Ֆʂ�8x8
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

    // �J�[�\���̈ʒu
    private int selectPosX,selectPosY;
    // �u���ꏊ��I���������ǂ����̃t���O
    private bool isSelect;

    // ���E���̃^�[��
    private int turn;
    // ���E���̃R�}��
    private int scoreBk,scoreWh;

    // �^�[���J�n���t���O
    private bool turnStart;
    // �Q�[���G���h�t���O
    private bool isGameEnd;

    // ���g���C�{�^���������̏���
    public void TapRetryButton()
    {
        // �Q�[���V�[�����Ăяo��
        SceneManager.LoadScene("GameScene");
    }

    // ����������
    private void Init()
    {
        // �R�}��`��
        DrawPiece();

        // �Ֆʕ`��
        OnRenderObject();

        // �e�ϐ��̏�����
        // �I�𒆂̃}�X�̃|�W�V����������
        selectPosX = 0;
        selectPosY = 0;
        isSelect = false;

        // �^�[��������
        turn = BLACK;
        turnStart = true;

        // �X�R�A������
        scoreBk = 2;
        scoreWh = 2;

        // �Q�[���G���h�t���O������
        isGameEnd = false;
    }

    // �I�u�W�F�N�g�������_�����O����֐�
    // �J�������V�[���������_�����O������ɌĂяo�����
    private void OnRenderObject()
    {
        // �`��̊�_�𒆉��ɒ���
        float offset = -CELL_SIZE / 2;

        // �Ֆʂ̕`��(�΂̎l�p�`�A�g��)
        //DrawGreenBackground(offset);
        DrawLines(offset);
    }

    // �΂̎l�p�`��`�悷�鏈��
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

    // �g����`�悷�鏈��
    private void DrawLines(float offset)
    {
        // �}�e���A���̃Z�b�g
        lineMaterial.SetPass(0);
        GL.Begin(GL.LINES);
        GL.Color(Color.black);
        
        // �c����`��
        for (int i = 0; i <= MAX_SQUARE; i++)
        {
            float x = offset + i * CELL_SIZE;
            GL.Vertex3(x,-offset, 0);
            GL.Vertex3(x, -offset - MAX_SQUARE * CELL_SIZE, 0);
        }

        // ������`��
        for (int j = 0; j <= MAX_SQUARE; j++)
        {
            float y = -offset - j * CELL_SIZE;
            GL.Vertex3(offset, y, 0);
            GL.Vertex3(offset + MAX_SQUARE * CELL_SIZE, y, 0);
        }

        GL.End();
    }

    // �e�L�X�g��\�����鏈��
    private void DrawText()
    {
        // �^�[���̕`��
        var player = "";

        if (turn == BLACK)
        {
            player = "��";
        }
        else
        {
            player = "��";
        }

        // �^�[����\��
        turnText.text = "�^�[���F" + player;

        // �X�R�A�̕`��
        scoreText.text = "���F" + scoreBk + "\n���F" + scoreWh;

    }

    // ���`�悷�鏈��
    private void DrawPiece()
    {
        // �X�R�A�̃��Z�b�g
        scoreBk = 0;
        scoreWh = 0;

        // �I�Z���̃}�X�����[�v���A�`��
        for (int i = NONE; i < MAX_SQUARE; i++)
        {
            for (int j = NONE; j < MAX_SQUARE; j++)
            {
                GameObject obj = null;

                // �F�ʂɉ摜��ς���
                switch (board[i, j])
                {
                    case NONE:
                        obj = null;
                        break;

                    case BLACK:
                        obj = black;
                        // �X�R�A�����Z
                        scoreBk++;
                        break;

                    case WHITE:
                        obj = white;
                        // �X�R�A�����Z
                        scoreWh++;
                        break;
                }

                // �����Ȃ��Ƃ���ȊO�͕`��
                if (obj != null)
                {
                    // �R�}����
                    var piece = Instantiate(obj);
                    piece.transform.SetParent(parent.transform, false);

                    piece.transform.position = new Vector2(i, -j);

                    //Debug.Log("["+i+","+j+"]   " +board[i, j]);

                }
            }
        }
    }

    // �I�𒆂̃}�X�̈ʒu��ύX���鏈��
    private void ChangeSelectPos()
    {
        // �}�E�X���W�ƁA���[���h���W
        Vector3 mPos,wPos;

        // ��ʃ^�b�v��
        if (Input.GetMouseButtonDown(0))
        {
            // �^�b�v���ꂽ���W���擾�A���[���h���W�ɕϊ�����
            mPos = Input.mousePosition;
            wPos = Camera.main.ScreenToWorldPoint(new Vector3(mPos.x, mPos.y, 10f));

            // �^�b�v���ꂽ�ꏊ���Ֆʓ��Ȃ�
            if ((wPos.x >= -CELL_SIZE / 2 && wPos.x <= MAX_SQUARE - (CELL_SIZE / 2))&&
                (wPos.y <=  CELL_SIZE / 2 && wPos.y >= -(MAX_SQUARE - (CELL_SIZE / 2)))
                )
            {
                // �^�b�v�ʒu����ɁA�Ֆʂ̃}�X�̃T�C�Y�ɕ␳����
                int gridX = Mathf.FloorToInt(wPos.x / CELL_SIZE + (CELL_SIZE / 2));
                int gridY = Mathf.FloorToInt(wPos.y / CELL_SIZE + (CELL_SIZE / 2));

                // �␳�������W�����[���h���W�Ɋi�[
                wPos.x = gridX * CELL_SIZE;
                wPos.y = gridY * CELL_SIZE;

                // ���[���h���W��I�𒆂̍��W�Ɋi�[ 
                select.transform.position = wPos;

                // ���[���h���W�̐�Βl��I�𒆂̍��W�Ɋi�[
                selectPosX = Mathf.Abs(Mathf.FloorToInt(wPos.x));
                selectPosY = Mathf.Abs(Mathf.FloorToInt(wPos.y));

                // �I�������t���O�𗧂Ă�
                isSelect = true;
            }
        }

        #region �L�[�{�[�h�p�ړ�����
        /*
        // �I�����Ă���}�X�ړ�
        if (Input.GetKeyDown(KeyCode.A))
        {
            // �[�ł͂Ȃ���Έړ��\
            if (selectPosX > 0) {
                selectPosX -= 1;
            }
            else {
                selectPosX = MAX_SQUARE - 1;
            }
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            // �[�ł͂Ȃ���Έړ��\
            if (selectPosX < MAX_SQUARE - 1) {
                selectPosX += 1;
            }
            else {
                selectPosX = 0;
            }
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            // �[�ł͂Ȃ���Έړ��\
            if (selectPosY > 0) {
                selectPosY -= 1;
            }
            else {
                selectPosY = MAX_SQUARE - 1;
            }
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            // �[�ł͂Ȃ���Έړ��\
            if (selectPosY < MAX_SQUARE - 1) {
                selectPosY += 1;
            }
            else {
                selectPosY = 0;
            }
        }
        //�I�𒆂̃}�X
        select.transform.position = new Vector2(selectPosX,selectPosY);
        */
        #endregion
    }

    // �R�}��z�u���鏈��
    private void PlaceThePiece()
    {
        // �}�X���I��(�^�b�v)���ꂽ�ꍇ
        if (isSelect)
        {
            // ���̃}�X�ɒu���邩�ǂ����̔���
            if (SelectCheck(selectPosX, selectPosY,true))
            {
                // �I�𒆂̃}�X�ɃR�}��u��
                if (turn == BLACK)
                {
                    board[selectPosX, selectPosY] = BLACK;
                }
                else
                {
                    board[selectPosX, selectPosY] = WHITE;
                }

                // �R�}������
                for (int i = NONE; i < parent.transform.childCount; i++)
                {
                    Destroy(parent.transform.GetChild(i).gameObject);
                }

                // �R�}���ĕ`��
                DrawPiece();

                // �^�[����ύX�A�t���O��������
                turn *= -1;
                turnStart = true;
                isSelect = false;
            }
        }
    }

    ///<summary>
    // �I�������ꏊ�ɃR�}���u���邩�ǂ����̔��菈���E�Ђ�����Ԃ�����
    /// </summary>
    /// <param name="_posX">�R�}��u���ʒu��x����</param>
    /// <param name="_posY">�R�}��u���ʒu��y����</param>
    /// <param name="_isFlip">�R�}���Ђ�����Ԃ����ǂ����̃t���O</param>
    /// <returns>true:�R�}��u����@false:�R�}��u���Ȃ�</returns>
    private bool SelectCheck(int _posX, int _posY, bool _isFlip)
    {
        // �����̐F
        var myCol = turn;
        // ����̐F
        var enCol = turn * -1;
        // �R�}��u���邩�ǂ����̃t���O
        var isPlaced = false;
        // �T�������������ɒu����Ă��鎩���Ɠ����F�̃R�}�̍ŒZ�̈ʒu
        //var myColPos = 0;

        // �Ђ�����Ԃ����Ƃ��ł���R�}�̈ʒu���i�[���郊�X�g
        List<(int, int)> flipPos = new List<(int, int)>();

        // 1.�΂�u�����Ƃ���}�X�ɐ΂��u����Ă��Ȃ�����
        if (board[_posX, _posY] != NONE)
        {
            // ���̏����𖞂����Ă��Ȃ��ꍇ�͒T�������I��
            return false;
        }

        // �e�������Ƃ̒T���������s�����߂̊֐�
        // _x:�T��������x����(���E)
        // _t:�T��������y����(�㉺)
        // _isFlip:�R�}���Ђ�����Ԃ����ǂ����̃t���O
        void SearchDirection(int _x, int _y)
        {
            // �Ђ�����Ԃ������ł���R�}�̈ʒu���i�[����ꎞ���X�g
            var temp = new List<(int, int)>();

            // �T������ʒu���i�[
            var x = _posX + _x;
            var y = _posY + _y;

            // �Ֆʓ��ŒT�����s��
            while (x >= 0 && x < MAX_SQUARE && y >= 0 && y < MAX_SQUARE)
            {
                // 2.�΂�u�����Ƃ���}�X��1�}�X�ׂɎ����ƈقȂ�F�̐΂����邱��
                if (board[x, y] == enCol)
                {
                    // �ꎞ���X�g�ɒǉ�
                    // ���̎��A�R�}�����܂�Ă��邩�ǂ����͂킩��Ȃ����߁A�ꎞ���X�g�ɒǉ����Ă���
                    temp.Add((x, y));
                }
                // 3.2�̉�������Ɏ����Ɠ����F�̐΂��u����Ă��邱��
                else if (board[x, y] == myCol)
                {
                    // �����Ɠ����F�̃R�}���������ꍇ�A�Ђ�����Ԃ��邱�Ƃ��m�肷�邽��
                    // �ꎞ���X�g�Ɋi�[�����R�}�̈ʒu���Ђ�����Ԃ������ł��郊�X�g�ɒǉ�����
                    flipPos.AddRange(temp);

                    break;
                }
                // ��̏����𖞂����Ȃ��ꍇ�̓��[�v�𔲂���(�R�}��u���Ȃ�)
                else
                {
                    break;
                }

                // ���ɒT������ʒu�ɕύX
                x += _x;
                y += _y;
            }
        }

        // �㉺���E�A�΂ߏ㉺���E�����̒T��
        if (_posY >= 2) SearchDirection( 0, -1);   // ��
        if (_posY <= 5) SearchDirection( 0,  1);   // ��
        if (_posX >= 2) SearchDirection(-1,  0);   // ��
        if (_posX <= 5) SearchDirection( 1,  0);   // �E
        if (_posX >= 2 && _posY >= 2) SearchDirection(-1, -1);   // ���΂ߏ�
        if (_posX >= 2 && _posY <= 5) SearchDirection(-1,  1);   // ���΂߉�
        if (_posX <= 5 && _posY >= 2) SearchDirection( 1, -1);   // �E�΂ߏ�
        if (_posX <= 5 && _posY <= 5) SearchDirection( 1,  1);   // �E�΂߉�

        // �R�}��u����ꍇ�́A���X�g���̈ʒu�����ƂɊY������R�}���Ђ�����Ԃ�
        if (flipPos.Count > 0)
        {
            // �R�}���Ђ�����Ԃ��t���O��true�̏ꍇ�͂Ђ�����Ԃ�
            if (_isFlip)
            {
                foreach (var pos in flipPos)
                {
                    board[pos.Item1, pos.Item2] = myCol;
                }
            }
            // �R�}��u���邽�߁A�t���O�𐬗�
            isPlaced = true;
        }

        // �R�}��u���邩�ǂ����̃t���O��Ԃ�
        // true:�R�}��u����@�@false:�R�}��u���Ȃ�
        return isPlaced;

        #region 8�����T���̌��̍l����(�֐����O)
        /*
        #region �E�΂ߏ�����̒T��
        // �E�΂ߏ�����̒T��
        // �����͈͂̏��O
        // �΂�u�����Ƃ��ɋ��ނ��Ƃ��o���Ȃ���2�}�X�E�E2�}�X�̏ꏊ
        if (_posY >= 2 && _posX <= 5)
        {
            // 1.�΂�u�����Ƃ���}�X�ɐ΂��u����Ă��Ȃ�����
            if (board[_posX, _posY] == NONE)
            {
                // 2.�΂�u�����Ƃ���}�X��1�}�X�E��Ɏ����ƈقȂ�F�̐΂����邱��
                if (board[_posX + 1, _posY - 1] == enCol)
                {
                    // 3.2�̉�������Ɏ����Ɠ����F�̐΂��u����Ă��邱��
                    // TODO�@�������Ђ�����Ԃ���悤�ɂ���
                    for (int i = 0; i < MAX_SQUARE; i++)
                    {
                        // �͈͊O�Q�Ƃ�h�������`�F�b�N 
                        if (_posX + i >= MAX_SQUARE || _posY - i < 0)
                        {
                            break;
                        }

                        // �����Ɠ����F�̃R�}���������ꍇ
                        if (board[_posX+i, _posY-i] == myCol)
                        {
                            // �R�}��u���邽�߁A�t���O�𗧂Ă�
                            isPlaced = true;

                            // �ʒu���i�[���A���[�v���甲����
                            myColPos = i;
                            break;
                        }
                    }

                    // �R�}��u�����Ƃ��ł���΁A�Ђ�����Ԃ�
                    if (isPlaced)
                    {
                        // �����Ɠ����F�̃R�}�̍ŒZ�̈ʒu�܂ŃR�}���Ђ�����Ԃ�
                        for (int i = 0; i < myColPos; i++)
                        {
                            board[_posX+i, _posY-i] = myCol;
                        }
                    }

                }
            }
        }
        #endregion

        #region �E�΂߉������̒T��
        // �E�΂߉������̒T��
        // �����͈͂̏��O
        // �΂�u�����Ƃ��ɋ��ނ��Ƃ��o���Ȃ���2�}�X�E�E2�}�X�̏ꏊ
        if (_posY <= 5 && _posX <= 5)
        {
            // 1.�΂�u�����Ƃ���}�X�ɐ΂��u����Ă��Ȃ�����
            if (board[_posX, _posY] == NONE)
            {
                // 2.�΂�u�����Ƃ���}�X��1�}�X�E���Ɏ����ƈقȂ�F�̐΂����邱��
                if (board[_posX + 1, _posY + 1] == enCol)
                {
                    // 3.2�̉�������Ɏ����Ɠ����F�̐΂��u����Ă��邱��
                    // TODO�@�������Ђ�����Ԃ���悤�ɂ���
                    for (int i = 0; i < MAX_SQUARE; i++)
                    {
                        // �͈͊O�Q�Ƃ�h�������`�F�b�N 
                        if (_posX + i >= MAX_SQUARE || _posY + i >= MAX_SQUARE)
                        {
                            break;
                        }

                        // �����Ɠ����F�̃R�}���������ꍇ
                        if (board[_posX + i, _posY + i] == myCol)
                        {
                            // �R�}��u���邽�߁A�t���O�𗧂Ă�
                            isPlaced = true;

                            // �ʒu���i�[���A���[�v���甲����
                            myColPos = i;
                            break;
                        }
                    }

                    // �R�}��u�����Ƃ��ł���΁A�Ђ�����Ԃ�
                    if (isPlaced)
                    {
                        // �����Ɠ����F�̃R�}�̍ŒZ�̈ʒu�܂ŃR�}���Ђ�����Ԃ�
                        for (int i = 0; i < myColPos; i++)
                        {
                            board[_posX + i, _posY + i] = myCol;
                        }
                    }

                }
            }
        }
        #endregion

        #region ���΂ߏ�����̒T��
        // ���΂ߏ�����̒T��
        // �����͈͂̏��O
        // �΂�u�����Ƃ��ɋ��ނ��Ƃ��o���Ȃ���2�}�X�E��2�}�X�̏ꏊ
        if (_posY >= 2 && _posX >= 2)
        {
            // 1.�΂�u�����Ƃ���}�X�ɐ΂��u����Ă��Ȃ�����
            if (board[_posX, _posY] == NONE)
            {
                // 2.�΂�u�����Ƃ���}�X��1�}�X����Ɏ����ƈقȂ�F�̐΂����邱��
                if (board[_posX - 1, _posY - 1] == enCol)
                {
                    // 3.2�̉�������Ɏ����Ɠ����F�̐΂��u����Ă��邱��
                    // TODO�@�������Ђ�����Ԃ���悤�ɂ���
                    for (int i = 0; i < MAX_SQUARE; i++)
                    {
                        // �͈͊O�Q�Ƃ�h�������`�F�b�N 
                        if (_posX - i < 0 || _posY - i < 0)
                        {
                            break;
                        }

                        // �����Ɠ����F�̃R�}���������ꍇ
                        if (board[_posX - i, _posY - i] == myCol)
                        {
                            // �R�}��u���邽�߁A�t���O�𗧂Ă�
                            isPlaced = true;

                            // �ʒu���i�[���A���[�v���甲����
                            myColPos = i;
                            break;
                        }
                    }

                    // �R�}��u�����Ƃ��ł���΁A�Ђ�����Ԃ�
                    if (isPlaced)
                    {
                        // �����Ɠ����F�̃R�}�̍ŒZ�̈ʒu�܂ŃR�}���Ђ�����Ԃ�
                        for (int i = 0; i < myColPos; i++)
                        {
                            board[_posX - i, _posY - i] = myCol;
                        }
                    }

                }
            }
        }
        #endregion

        #region ���΂߉������̒T��
        // ���΂߉������̒T��
        // �����͈͂̏��O
        // �΂�u�����Ƃ��ɋ��ނ��Ƃ��o���Ȃ���2�}�X�E��2�}�X�̏ꏊ
        if (_posY <= 5 && _posX >= 2)
        {
            // 1.�΂�u�����Ƃ���}�X�ɐ΂��u����Ă��Ȃ�����
            if (board[_posX, _posY] == NONE)
            {
                // 2.�΂�u�����Ƃ���}�X��1�}�X����Ɏ����ƈقȂ�F�̐΂����邱��
                if (board[_posX - 1, _posY + 1] == enCol)
                {
                    // 3.2�̉�������Ɏ����Ɠ����F�̐΂��u����Ă��邱��
                    // TODO�@�������Ђ�����Ԃ���悤�ɂ���
                    for (int i = 0; i < MAX_SQUARE; i++)
                    {
                        // �͈͊O�Q�Ƃ�h�������`�F�b�N 
                        if (_posX - i < 0 || _posY + i >= MAX_SQUARE)
                        {
                            break;
                        }

                        // �����Ɠ����F�̃R�}���������ꍇ
                        if (board[_posX - i, _posY + i] == myCol)
                        {
                            // �R�}��u���邽�߁A�t���O�𗧂Ă�
                            isPlaced = true;

                            // �ʒu���i�[���A���[�v���甲����
                            myColPos = i;
                            break;
                        }
                    }

                    // �R�}��u�����Ƃ��ł���΁A�Ђ�����Ԃ�
                    if (isPlaced)
                    {
                        // �����Ɠ����F�̃R�}�̍ŒZ�̈ʒu�܂ŃR�}���Ђ�����Ԃ�
                        for (int i = 0; i < myColPos; i++)
                        {
                            board[_posX - i, _posY + i] = myCol;
                        }
                    }

                }
            }
        }
        #endregion


        #region ������̒T��
        // ������̒T��
        // �����͈͂̏��O
        // �΂�u�����Ƃ��ɋ��ނ��Ƃ��o���Ȃ���2�}�X�̏ꏊ
        if (_posY >= 2)
        {
            // 1.�΂�u�����Ƃ���}�X�ɐ΂��u����Ă��Ȃ�����
            if (board[_posX, _posY] == NONE)
            {
                // 2.�΂�u�����Ƃ���}�X��1�}�X��(��)�Ɏ����ƈقȂ�F�̐΂����邱��
                if (board[_posX, _posY - 1] == enCol)
                {
                    // 3.2�̉�������Ɏ����Ɠ����F�̐΂��u����Ă��邱��
                    for (int i = _posY; i > 0; i--)
                    {
                        // �����Ɠ����F�̃R�}���������ꍇ
                        if (board[_posX, i] == myCol)
                        {
                            // �R�}��u���邽�߁A�t���O�𗧂Ă�
                            isPlaced = true;

                            // �ʒu���i�[���A���[�v���甲����
                            myColPos = i;
                            break;
                        }
                    }

                    // �R�}��u�����Ƃ��ł���΁A�Ђ�����Ԃ�
                    if (isPlaced)
                    {
                        // �����Ɠ����F�̃R�}�̍ŒZ�̈ʒu�܂ŃR�}���Ђ�����Ԃ�
                        for (int i = _posY; i > myColPos; i--)
                        {
                            board[_posX, i] = myCol;
                        }
                    }
                    
                }
            }
        }
        #endregion
        
        #region �������̒T��
        // �������̒T��
        // �����͈͂̏��O
        // �΂�u�����Ƃ��ɋ��ނ��Ƃ��o���Ȃ���2�}�X�̏ꏊ
        if (_posY <= 5)
        {
            // 1.�΂�u�����Ƃ���}�X�ɐ΂��u����Ă��Ȃ�����
            if (board[_posX, _posY] == NONE)
            {
                // 2.�΂�u�����Ƃ���}�X��1�}�X��(��)�Ɏ����ƈقȂ�F�̐΂����邱��
                if (board[_posX, _posY + 1] == enCol)
                {
                    // 3.2�̉�������Ɏ����Ɠ����F�̐΂��u����Ă��邱��
                    for (int i = _posY; i < MAX_SQUARE; i++)
                    {
                        // �����Ɠ����F�̃R�}���������ꍇ
                        if (board[_posX, i] == myCol)
                        {
                            // �R�}��u���邽�߁A�t���O�𗧂Ă�
                            isPlaced = true;

                            // �ʒu���i�[���A���[�v���甲����
                            myColPos = i;

                            break;
                        }
                    }

                    // �R�}��u�����Ƃ��ł���΁A�Ђ�����Ԃ�
                    if (isPlaced)
                    {
                        // �����Ɠ����F�̃R�}�̍ŒZ�̈ʒu�܂ŃR�}���Ђ�����Ԃ�
                        for (int i = _posY; i < myColPos; i++)
                        {
                            board[_posX, i] = myCol;
                        }
                    }
                }
            }
        }
        #endregion

        #region �������̒T��
        // �������̒T��
        // �����͈͂̏��O
        // �΂�u�����Ƃ��ɋ��ނ��Ƃ��o���Ȃ���2�}�X�̏ꏊ
        if (_posX >= 2)
        {
            // 1.�΂�u�����Ƃ���}�X�ɐ΂��u����Ă��Ȃ�����
            if (board[_posX, _posY] == NONE)
            {
                // 2.�΂�u�����Ƃ���}�X��1�}�X��(��)�Ɏ����ƈقȂ�F�̐΂����邱��
                if (board[_posX - 1, _posY] == enCol)
                {
                    // 3.2�̉�������Ɏ����Ɠ����F�̐΂��u����Ă��邱��
                    for (int i = _posX; i > 0; i--)
                    {
                        // �����Ɠ����F�̃R�}���������ꍇ
                        if (board[i, _posY] == myCol)
                        {
                            // �R�}��u���邽�߁A�t���O�𗧂Ă�
                            isPlaced = true;

                            // �ʒu���i�[���A���[�v���甲����
                            myColPos = i;
                            break;
                        }
                    }

                    // �R�}��u�����Ƃ��ł���΁A�Ђ�����Ԃ�
                    if (isPlaced)
                    {
                        // �����Ɠ����F�̃R�}�̍ŒZ�̈ʒu�܂ŃR�}���Ђ�����Ԃ�
                        for (int i = _posX; i > myColPos; i--)
                        {
                            board[i, _posY] = myCol;
                        }
                    }

                }
            }
        }
        #endregion

        #region �E�����̒T��
        // �E�����̒T��
        // �����͈͂̏��O
        // �΂�u�����Ƃ��ɋ��ނ��Ƃ��o���Ȃ��E2�}�X�̏ꏊ
        if (_posX <= 5)
        {
            // 1.�΂�u�����Ƃ���}�X�ɐ΂��u����Ă��Ȃ�����
            if (board[_posX, _posY] == NONE)
            {
                // 2.�΂�u�����Ƃ���}�X��1�}�X��(�E)�Ɏ����ƈقȂ�F�̐΂����邱��
                if (board[_posX + 1, _posY] == enCol)
                {
                    // 3.2�̉�������Ɏ����Ɠ����F�̐΂��u����Ă��邱��
                    for (int i = _posX; i < MAX_SQUARE; i++)
                    {
                        // �����Ɠ����F�̃R�}���������ꍇ
                        if (board[i, _posY] == myCol)
                        {
                            // �R�}��u���邽�߁A�t���O�𗧂Ă�
                            isPlaced = true;

                            // �ʒu���i�[���A���[�v���甲����
                            myColPos = i;

                            break;
                        }
                    }

                    // �R�}��u�����Ƃ��ł���΁A�Ђ�����Ԃ�
                    if (isPlaced)
                    {
                        // �����Ɠ����F�̃R�}�̍ŒZ�̈ʒu�܂ŃR�}���Ђ�����Ԃ�
                        for (int i = _posX; i < myColPos; i++)
                        {
                            board[i, _posY] = myCol;
                        }
                    }
                }
            }
        }
        #endregion


        // �����𖞂����Ă���ꍇ��true��Ԃ�(�R�}��u����)
        if (isPlaced)
        {
            return true;
        }
        // ��̏����𖞂����Ȃ��ꍇ��false��Ԃ�(�R�}��u���Ȃ�)
        else
        {
            return false;
        }
        */
        #endregion

    }

    // �^�[���J�n���ɃR�}��u���ꏊ�����邩�ǂ����𔻒肷�鏈��
    private void TurnControl()
    {
        // �^�[�����X�L�b�v���邩�ǂ����̃t���O
        var isSkip = true;

        // �R�}�����ׂč��E���̏ꍇ�Ɏg�p����t���O
        var isAllBlack=true;
        var isAllWhite=true;
        // �}�X���S�Ė��܂��Ă���ꍇ�Ɏg�p����t���O
        var isAllPlaced = true;

        // �^�[���J�n����x��������
        if (turnStart)
        {
            // �ՖʑS�̂��m�F
            for (int i = 0; i < MAX_SQUARE; i++)
            {
                for (int j = 0; j < MAX_SQUARE; j++)
                {
                    // �R�}���S�č��E�����ǂ�������
                    if (board[i, j] == BLACK)
                    {
                        isAllWhite = false;
                    }
                    else if (board[i, j] == WHITE)
                    {
                        isAllBlack = false;
                    }
                    // �}�X�����ׂĖ��܂��Ă��邩����
                    else
                    {
                        isAllPlaced = false;
                    }

                    // 1�}�X���m�F
                    if (SelectCheck(i, j,false))
                    {
                        // �u����ꏊ������΃^�[���p��(���[�v���甲����)
                        isSkip = false;

                        // �S�č��E���̔���̂��߃R�����g�A�E�g��
                        //break;
                    }

                }

                // �u����ꏊ������΃^�[���p��(���[�v���甲����)
                //if (!isSkip)
                //{
                //    break;
                //}
            }

            // ���肷�邩�ǂ����̃t���O��������
            turnStart = false;

            // �R�}��u����ꏊ���Ȃ��X�L�b�v�̏ꍇ
            if (isSkip)
            {
                // �R�}���S�č��E���̏ꍇ�A�S�Ẵ}�X�����܂��Ă���ꍇ�̓Q�[���I��
                if(isAllBlack || isAllWhite || isAllPlaced)
                {
                    isGameEnd = true;
                    return;
                }

                // �X�L�b�v���̃e�L�X�g��\��
                var animator = skipText.GetComponent<Animator>();
                animator.SetTrigger("Skip");

                // �^�[����ύX
                turn *= -1;

                return;
            }
        }
    }

    // �Q�[���I�����̏���
    private void GameEnd()
    {
        if (isGameEnd)
        {
            // ���U���g�p�l���\��
            resultPanel.SetActive(true);

            var resText = "";

            // �����v���C���[�̕\��
            // ���̃R�}�������ꍇ
            if(scoreBk > scoreWh)
            {
                resText = "���̏���";
            }
            // ���̃R�}�������ꍇ
            else if(scoreWh > scoreBk)
            {
                resText = "���̏���";
            }
            // ���_�̏ꍇ
            else
            {
                resText = "��������";
            }

            // ���U���g�̕\��
            resultText.text = resText;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // ����������
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        // �R�}��u����ꏊ�����邩���m�F���A�^�[������
        TurnControl();

        // �I�����Ă���}�X�ړ�
        ChangeSelectPos();

        // �R�}��z�u
        PlaceThePiece();

        // �e�L�X�g��\��
        DrawText();

        // �Q�[���I�����̏���
        GameEnd();
    }
}
