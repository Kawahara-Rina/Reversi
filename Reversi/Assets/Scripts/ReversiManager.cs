using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReversiManager : MonoBehaviour
{
    // �萔��`
    private const int MAX_SQUARE = 8;          // �Ֆʂ̃}�X��
    private const int NONE = 0;                // �}�X�̏�ԁ@�� 
    private const int BLACK = 1;               // �}�X�̏�ԁ@���̃R�}
    private const int WHITE = -1;              // �}�X�̏�ԁ@���̃R�}
    private const float CELL_SIZE = 1.0f;      // 1�R�}�̑傫��

    public Material lineMaterial; // ���`��p�̃}�e���A�����A�^�b�`

    // �ϐ���`
    //[SerializeField] private float radius; // �R�}�̔��a

    // �f�o�b�O�p
    [SerializeField]private GameObject black;  // �\������R�}�u���v
    [SerializeField]private GameObject white;  // �\������R�}�u���v
    [SerializeField]private GameObject parent; // �R�}�𐶐�����̈�
    [SerializeField]private GameObject select; // �I�𒆂̃}�X
    [SerializeField] private Text turnText;    // �^�[����\������e�L�X�g
    [SerializeField] private Text scoreText;   // �e�F�̃R�}����\������e�L�X�g

    // �I�Z���̔Ֆʂ��`
    // �I�Z���̔Ֆʂ�8x8
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

    // LineRenderer�擾�p
    //LineRenderer renderer;

    // �J�[�\���̈ʒu
    private int selectPosX,selectPosY;

    // ���E���̃^�[��
    private int turn;
    // ���E���̃R�}��
    private int scoreBk,scoreWh;

    // ����������
    private void Init()
    {
        // �R�}��`��
        DrawPiece();

        // �I�𒆂̃}�X�̃|�W�V����������
        selectPosX = 0;
        selectPosY = 0;

        // �^�[��������
        turn = BLACK;
        // �X�R�A������
        scoreBk = 2;
        scoreWh = 2;

        // �w�i�`��

        OnRenderObject();


    }

    private void OnRenderObject()
    {
        if (lineMaterial == null)
        {
            Debug.LogWarning("Line Material is not assigned.");
            return;
        }

        // �`��̊�_�𒆉��ɒ���
        float offset = -CELL_SIZE / 2;

        //DrawGreenBackground(offset);
        DrawLines(offset);
    }

    // �΂̎l�p�`��`�悷�郁�\�b�h
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

                // �F�ʂɉ摜�ς���
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
        select.transform.position = new Vector2(selectPosX, -selectPosY);
    }

    // �R�}��z�u���鏈��
    private void PlaceThePiece()
    {

        // �X�y�[�X�L�[�����ŃR�}��u��
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // ���̃}�X�ɒu���邩�ǂ����̔���
            // ������̒T���̂�
            if (SelectCheck(selectPosX, selectPosY))
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

                // �^�[����ύX
                turn *= -1;
            }
        }
    }

    /// <summary>
    // �㉺�����ɃR�}���u���邩�ǂ����̔��菈��
    /// </summary>
    /// <param name="_posX">�R�}��u���ʒu��x����</param>
    /// <param name="_posY">�R�}��u���ʒu��y����</param>
    /// <returns></returns>
    private bool SelectCheck(int _posX,int _posY)
    {
        // �����̐F
        var myCol = turn;
        // ����̐F
        var enCol = turn * -1;
        // �R�}��u���邩�ǂ����̃t���O
        var isPlaced = false;
        // �T�������������ɒu����Ă��鎩���Ɠ����F�̃R�}�̍ŒZ�̈ʒu
        var myColPos = 0;

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

        // �����𖞂����Ă���ꍇ��true��Ԃ�(�R�}��u����)
        if (isPlaced)
        {
            return true;
        }
        else
        {
            // ��̏����𖞂����Ȃ��ꍇ��false��Ԃ�(�R�}��u���Ȃ�)
            return false;
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
        if (Input.GetMouseButtonDown(0))
        {
            var mPos= Input.mousePosition;
            Debug.Log("x:" + mPos.x + "    y:" + mPos.y);
        }

        // �I�����Ă���}�X�ړ�
        ChangeSelectPos();

        // �R�}��z�u
        PlaceThePiece();

        // �e�L�X�g��\��
        DrawText();
    }
}
