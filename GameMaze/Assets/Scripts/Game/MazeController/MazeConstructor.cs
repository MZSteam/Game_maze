using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MazeConstructor : MonoBehaviour
{
    public bool showDebug;

    private MazeDataGenerator dataGenerator;
    [SerializeField] private Camera camera;
    [SerializeField] private GameObject player;
    private bool CameraPos = false;
    private int sizeR = 0;
    private int sizeC = 0;
    public Tilemap tilemap;
    public Tilemap tilemapWall;
    public TileBase floorTile;
    public TileBase wallTile;
    public TileBase wallSideTile;
    public TileBase wallCornerTile;
    public TileBase wallDownTile;

    public int[,] data
    {
        get; private set;
    }

    void Awake()
    {
        dataGenerator = new MazeDataGenerator();
        data = new int[,]
        {
            {1, 1, 1},
            {1, 0, 1},
            {1, 1, 1}
        };
    }
    public void GenerateNewMaze(int sizeRows, int sizeCols)
    {
        if(sizeRows % 2 == 0 && sizeCols % 2 == 0) 
        {
            Debug.LogError("Odd numbers work better for dungeon size.");
        }

        sizeR = sizeRows;
        sizeC = sizeCols;
        data = dataGenerator.FromDimensions(sizeRows, sizeCols);//�������� ��������� ���������
        data = dataGenerator.mazeOutputGenerator(data, sizeRows, sizeCols);//���������� ��������� ������� �� ���������
    }
    private void OnGUI()
    {
        if(!showDebug)
        {
            return;
        }

        int[,] maze = data;
        int rMax = maze.GetUpperBound(0);
        int cMax = maze.GetUpperBound(1);

        string msg = "";

        for(int i = rMax; i >= 0; i--)
        {
            for(int j = 0; j <= cMax; j++)
            {
                if (maze[i,j] == 0)
                {
                    msg += "....";
                }
                else
                {
                    msg += "==";
                }
            }
            msg += "\n";
        }
        GUI.Label(new Rect(20,20,500,500), msg);
    }
    private void Start()//�������� ���������
    {

        int[,] maze = data;
        int rMax = maze.GetUpperBound(0);
        int cMax = maze.GetUpperBound(1);

        for (int i = 0; i < maze.GetLength(0); i++)
        {
            for (int j = 0; j < maze.GetLength(1); j++)
            {
                tilemap.SetTile(new Vector3Int(i, j, 0), floorTile);//�������� ���� �� ��� ��������� �����
                if (maze[i, j] == 0)
                {
                    if (!CameraPos)
                    {
                        if (sizeR / 2 < i && sizeC / 2 < j)
                        {
                            if (j - 2 < sizeC / 2) //����������� ������ � ������ �� ��������� ������� �������
                            {
                                Vector3 pos = new Vector3(i, j, 0f);
                                Vector3Int cellPosition = tilemap.WorldToCell(pos);
                                Vector3 cellCenterPosition = tilemap.GetCellCenterWorld(cellPosition);
                                player.transform.position = cellCenterPosition;//���� ����������� �� ������� �� �������� ����� �� tilemap, ��� �� �� ��������� � ������
                                camera.transform.position = new Vector3(cellCenterPosition.x, cellCenterPosition.y, -10f);//������ �������� ����� �� �������������� ��� � �����
                                CameraPos = !CameraPos;
                            }
                        }
                    }
                }
                else if (maze[i, j] == 1)
                {
                    try//����������, ��� �� �� �������� ������ ����� �������� �������� ��������� �������� < 0 ��� ������ ������������� ������� ���������
                    {
                        if (maze[i + 1, j] == 1 && maze[i - 1, j] == 1 && maze[i, j - 1] == 1 && maze[i, j + 1] != 1)
                        {
                            tilemapWall.SetTile(new Vector3Int(i, j, 0), wallCornerTile);//�������� ������ ������� ����� ���� 3 �������, ���� ����� � ���
                        }
                        else if(maze[i + 1, j] == 0 && maze[i - 1, j] == 0 && maze[i, j - 1] == 0 && maze[i, j + 1] == 1)
                        {
                            tilemapWall.SetTile(new Vector3Int(i, j, 0), wallDownTile);//�������� ������ ��������� ���� ������ ������������� ������ ���� ���� ���� ������
                        }
                        else if (maze[i + 1, j] == 1 && maze[i - 1, j] == 1)
                        {
                            tilemapWall.SetTile(new Vector3Int(i, j, 0), wallTile);// ������� ������ �������� ������ ���� ���� ����� � ������ �����
                        }
                        else if (maze[i, j - 1] == 1 && maze[i, j + 1] == 1)
                        {
                            tilemapWall.SetTile(new Vector3Int(i, j, 0), wallSideTile);//����� ������� �������� ������ ���� ���� ������ � ���� � ������
                        }
                        else
                        {
                            tilemapWall.SetTile(new Vector3Int(i, j, 0), wallTile);// �� ���� ���� ������� ������������ ����������� �����
                        }
                    }catch (System.IndexOutOfRangeException e) 
                    {
                        tilemapWall.SetTile(new Vector3Int(i, j, 0), wallTile);
                    }        
                }
            }
        }
    }
}
