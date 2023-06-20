using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MazeConstructor))]

public class GameController : MonoBehaviour
{
    private MazeConstructor generator;
    [SerializeField] private int SizeMaze;// ������ ���������, ����� ��������� ����� ��������� unity

    void Start()
    {
        generator = GetComponent<MazeConstructor>();
        generator.GenerateNewMaze(SizeMaze, SizeMaze);// ����� ������ ���������
    }
}
