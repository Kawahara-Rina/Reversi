using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReversiManager : MonoBehaviour
{
    // �萔��`
    private const int MAX_SQUARE = 8;  // �Ֆʂ̃}�X��
    private const int NONE = 0;        // �}�X�̏�ԁ@�� 
    private const int BLACK = 1;       // �}�X�̏�ԁ@���̃R�}
    private const int WHITE = -1;      // �}�X�̏�ԁ@���̃R�}

    // �ϐ���`
    //[SerializeField] private float radius; // �R�}�̔��a

    // �f�o�b�O�p
    [SerializeField]private GameObject none;
    [SerializeField]private GameObject black;
    [SerializeField]private GameObject white;
    [SerializeField]private GameObject parent;
    [SerializeField]private GameObject select;
    [SerializeField] private Text turnText;

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

    // �J�[�\���̈ʒu
    private int selectPosX,selectPosY;

    // ���E���̃^�[��
    private int turn;

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
    }

    // ���`�悷�鏈��
    private void DrawPiece()
    {
        // �I�Z���̃}�X�����[�v���A�`��
        for (int i = NONE; i < MAX_SQUARE; i++)
        {
            for (int j = NONE; j < MAX_SQUARE; j++)
            {
                var obj = none;

                // �F�ʂɉ摜�ς���
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

                // �����Ȃ��Ƃ���ȊO�͕`��
                if (obj != none)
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
    // ������ɃR�}���u���邩�ǂ����̔��菈��
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
                    // 2�̉�������ɒu����Ă��鎩���Ɠ����F�̃R�}�̍ŒZ�̈ʒu
                    var myColPos = 0;

                    // 3.2�̉�������Ɏ����Ɠ����F�̐΂��u����Ă��邱��
                    for (int i = _posY; i > 0; i--)
                    {
                        // �����Ɠ����F�̃R�}���������ꍇ
                        if (board[_posX, i] == myCol)
                        {
                            // �ʒu���i�[���A���[�v���甲����
                            myColPos = i;
                            break;
                        }
                    }

                    // �����Ɠ����F�̃R�}�̍ŒZ�̈ʒu�܂ŃR�}���Ђ�����Ԃ�
                    for (int i = _posY; i > myColPos; i--)
                    {
                        board[_posX, i] = myCol;
                    }
                    return true;
                }
            }
        }

        // ��̏����𖞂����Ȃ��ꍇ��false��Ԃ�(�R�}��u���Ȃ�)
        return false;
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
        // �I�����Ă���}�X�ړ�
        ChangeSelectPos();

        // �R�}��z�u
        PlaceThePiece();

        // �e�L�X�g��\��
        DrawText();
    }
}
