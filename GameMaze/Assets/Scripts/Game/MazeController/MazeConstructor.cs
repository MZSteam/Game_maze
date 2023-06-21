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
        data = dataGenerator.FromDimensions(sizeRows, sizeCols);//Вызывает генератор лабиринта
        data = dataGenerator.mazeOutputGenerator(data, sizeRows, sizeCols);//Вызывается генератор выходов из лабиринта
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
    private void Start()//Создание лаберинта
    {

        int[,] maze = data;
        int rMax = maze.GetUpperBound(0);
        int cMax = maze.GetUpperBound(1);

        for (int i = 0; i < maze.GetLength(0); i++)
        {
            for (int j = 0; j < maze.GetLength(1); j++)
            {
                tilemap.SetTile(new Vector3Int(i, j, 0), floorTile);//создание пола на все возможные блоки
                if (maze[i, j] == 0)
                {
                    if (!CameraPos)
                    {
                        if (sizeR / 2 < i && sizeC / 2 < j)
                        {
                            if (j - 2 < sizeC / 2) //перемещение камеры и игрока на начальную игровую позицию
                            {
                                placementOfEnemiesAndThePlayer(maze, i, j);
                            }
                        }
                    }
                }
                else if (maze[i, j] == 1)
                {
                    try//исключения, что бы не вызывать ошибку когда пытается получить значаения элемента < 0 или больше максимального размера лаберинта
                    {
                        if (maze[i + 1, j] == 1 && maze[i - 1, j] == 1 && maze[i, j - 1] == 1 && maze[i, j + 1] != 1)
                        {
                            tilemapWall.SetTile(new Vector3Int(i, j, 0), wallCornerTile);//Создание стенки угловой когда есть 3 стороны, лево право и низ
                        }
                        else if(maze[i + 1, j] == 0 && maze[i - 1, j] == 0 && maze[i, j - 1] == 0 && maze[i, j + 1] == 1)
                        {
                            tilemapWall.SetTile(new Vector3Int(i, j, 0), wallDownTile);//Создание стенки окончания есть стенка заканчивается только есть есть блок сверху
                        }
                        else if (maze[i - 1, j] == 1 && maze[i + 1, j] == 0 && maze[i, j + 1] == 0 && maze[i, j - 1] == 1)
                        {
                            tilemapWall.SetTile(new Vector3Int(i, j, 0), wallLeftDownTile);//Созаднаие боковой стены слева в низ
                        }
                        else if (maze[i - 1, j] == 0 && maze[i + 1, j] == 1 && maze[i, j + 1] == 0 && maze[i, j - 1] == 1)
                        {
                            tilemapWall.SetTile(new Vector3Int(i, j, 0), wallRightDownTile);//Созаднаие боковой стены справа в низ
                        }
                        else if (maze[i - 1, j] == 1 && maze[i + 1, j] == 0 && maze[i, j + 1] == 1 && maze[i, j - 1] == 0)
                        {
                            tilemapWall.SetTile(new Vector3Int(i, j, 0), wallLeftUpTile); //Созаднаие боковой стены слева в верх
                        }
                        else if (maze[i - 1, j] == 0 && maze[i + 1, j] == 1 && maze[i, j + 1] == 1 && maze[i, j - 1] == 0)
                        {
                            tilemapWall.SetTile(new Vector3Int(i, j, 0), wallRightUpTile); //Созаднаие боковой стены справа в верх
                        }
                        else if (maze[i + 1, j] == 1 && maze[i - 1, j] == 1)
                        {
                            tilemapWall.SetTile(new Vector3Int(i, j, 0), wallTile);// стенука прямая создаётся только если есть слева и справо стена
                        }
                        else if (maze[i, j - 1] == 1 && maze[i, j + 1] == 1)
                        {
                            tilemapWall.SetTile(new Vector3Int(i, j, 0), wallSideTile);//стена боковая создаётся только если есть стенки с низу и сверху
                        }
                        else
                        {
                            tilemapWall.SetTile(new Vector3Int(i, j, 0), wallTile);// во всех иных случаях используется стандартная стена
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
        player.transform.position = cellCenterPosition;//грой становиться на позицию по середине тайла на tilemap, что бы не появлялся в стенке
        camera.transform.position = new Vector3(cellCenterPosition.x, cellCenterPosition.y, -10f);//камера получает такое же местоположение как и герой
        CameraPos = !CameraPos;
        for(int g = 0; g < 5; g++)//создание 5 врягов или другуго-го заддоного количества.
        {
            Enim = true;// добро циклу
            while (Enim)
            {
                int b = 0;
                int x = Random.Range(5, sizeR - 5);
                int y = Random.Range(5, sizeR - 5);
                if (Mathf.Abs((x + y) - (i + j)) > sizeC / 4 && x !=0 && y != 0)//Проверка растояния между врагом и игроком
                {
                    if (maze[x, y] == 0)//проверка на то что враг заспавниться не в стенке
                    {
                        if(n > 0)//если хотя бы 1 враг есть на карте
                        {
                            for (int f = 0; f < n; f++)//Перечисление всех врагов из массива
                            {
                                if (Mathf.Abs((en[f, 0] + en[f, 1]) - (x + y)) > 15)//Провека каждого врага и растояния между новым врагом
                                {
                                    b += 1;
                                    if(b >= n)//Проверка на то что растояние между врагами достаточное
                                    {
                                        Vector3 posE = new Vector3(x, y, 0f);
                                        Vector3Int cellPositionE = tilemap.WorldToCell(posE);
                                        Vector3 cellCenterPositionE = tilemap.GetCellCenterWorld(cellPositionE);
                                        Instantiate(enemies, cellCenterPositionE, Quaternion.identity);//спав врага
                                        en[n, 0] = x;
                                        en[n, 1] = y;
                                        n += 1;
                                        Enim = false;// для выхода из цикла
                                    }
                                }
                            }
                        }
                        else//Спав самого первого врага
                        {
                            Vector3 posE = new Vector3(x, y, 0f);
                            Vector3Int cellPositionE = tilemap.WorldToCell(posE);
                            Vector3 cellCenterPositionE = tilemap.GetCellCenterWorld(cellPositionE);
                            Instantiate(enemies, cellCenterPositionE, Quaternion.identity);
                            en[n, 0] = x;
                            en[n, 1] = y;
                            n += 1;
                            Enim = false;// для выхода из цикла
                        }
                    }
                }
            }
        }
    }
}
