using System.Collections;
using System.Collections.Generic;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MazeConstructor : MonoBehaviour
{
    public bool showDebug;

    private MazeDataGenerator dataGenerator;
    [SerializeField] private Camera camera;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject enemies;
    private bool CameraPos = false;
    private bool Enim = true;
    private int sizeR = 0;
    private int sizeC = 0;
    public Tilemap tilemap;
    public Tilemap tilemapWall;
    public TileBase floorTile;
    public TileBase wallTile;
    public TileBase wallSideTile;
    public TileBase wallCornerTile;
    public TileBase wallDownTile;
    public TileBase wallLeftDownTile;
    public TileBase wallRightDownTile;
    public TileBase wallLeftUpTile;
    public TileBase wallRightUpTile;

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
                                placementOfEnemiesAndThePlayer(maze, i, j);
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
                        else if (maze[i - 1, j] == 1 && maze[i + 1, j] == 0 && maze[i, j + 1] == 0 && maze[i, j - 1] == 1)
                        {
                            tilemapWall.SetTile(new Vector3Int(i, j, 0), wallLeftDownTile);//��������� ������� ����� ����� � ���
                        }
                        else if (maze[i - 1, j] == 0 && maze[i + 1, j] == 1 && maze[i, j + 1] == 0 && maze[i, j - 1] == 1)
                        {
                            tilemapWall.SetTile(new Vector3Int(i, j, 0), wallRightDownTile);//��������� ������� ����� ������ � ���
                        }
                        else if (maze[i - 1, j] == 1 && maze[i + 1, j] == 0 && maze[i, j + 1] == 1 && maze[i, j - 1] == 0)
                        {
                            tilemapWall.SetTile(new Vector3Int(i, j, 0), wallLeftUpTile); //��������� ������� ����� ����� � ����
                        }
                        else if (maze[i - 1, j] == 0 && maze[i + 1, j] == 1 && maze[i, j + 1] == 1 && maze[i, j - 1] == 0)
                        {
                            tilemapWall.SetTile(new Vector3Int(i, j, 0), wallRightUpTile); //��������� ������� ����� ������ � ����
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
    void placementOfEnemiesAndThePlayer(int[,] maze, int i, int j)
    {
        
        int n = 0;
        int[,] en = new int[5, 2];
        Vector3 pos = new Vector3(i, j, 0f);
        Vector3Int cellPosition = tilemap.WorldToCell(pos);
        Vector3 cellCenterPosition = tilemap.GetCellCenterWorld(cellPosition);
        player.transform.position = cellCenterPosition;//���� ����������� �� ������� �� �������� ����� �� tilemap, ��� �� �� ��������� � ������
        camera.transform.position = new Vector3(cellCenterPosition.x, cellCenterPosition.y, -10f);//������ �������� ����� �� �������������� ��� � �����
        CameraPos = !CameraPos;
        for(int g = 0; g < 5; g++)//�������� 5 ������ ��� �������-�� ��������� ����������.
        {
            Enim = true;// ����� �����
            while (Enim)
            {
                int b = 0;
                int x = Random.Range(5, sizeR - 5);
                int y = Random.Range(5, sizeR - 5);
                if (Mathf.Abs((x + y) - (i + j)) > sizeC / 4 && x !=0 && y != 0)//�������� ��������� ����� ������ � �������
                {
                    if (maze[x, y] == 0)//�������� �� �� ��� ���� ������������ �� � ������
                    {
                        if(n > 0)//���� ���� �� 1 ���� ���� �� �����
                        {
                            for (int f = 0; f < n; f++)//������������ ���� ������ �� �������
                            {
                                if (Mathf.Abs((en[f, 0] + en[f, 1]) - (x + y)) > 15)//������� ������� ����� � ��������� ����� ����� ������
                                {
                                    b += 1;
                                    if(b >= n)//�������� �� �� ��� ��������� ����� ������� �����������
                                    {
                                        Vector3 posE = new Vector3(x, y, 0f);
                                        Vector3Int cellPositionE = tilemap.WorldToCell(posE);
                                        Vector3 cellCenterPositionE = tilemap.GetCellCenterWorld(cellPositionE);
                                        Instantiate(enemies, cellCenterPositionE, Quaternion.identity);//���� �����
                                        en[n, 0] = x;
                                        en[n, 1] = y;
                                        n += 1;
                                        Enim = false;// ��� ������ �� �����
                                    }
                                }
                            }
                        }
                        else//���� ������ ������� �����
                        {
                            Vector3 posE = new Vector3(x, y, 0f);
                            Vector3Int cellPositionE = tilemap.WorldToCell(posE);
                            Vector3 cellCenterPositionE = tilemap.GetCellCenterWorld(cellPositionE);
                            Instantiate(enemies, cellCenterPositionE, Quaternion.identity);
                            en[n, 0] = x;
                            en[n, 1] = y;
                            n += 1;
                            Enim = false;// ��� ������ �� �����
                        }
                    }
                }
            }
        }
    }
}
