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
        data = dataGenerator.FromDimensions(sizeRows, sizeCols);//¬ызывает генератор лабиринта
        data = dataGenerator.mazeOutputGenerator(data, sizeRows, sizeCols);//¬ызываетс€ генератор выходов из лабиринта
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
    private void Start()//—оздание лаберинта
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
                                Vector3 pos = new Vector3(i, j, 0f);
                                Vector3Int cellPosition = tilemap.WorldToCell(pos);
                                Vector3 cellCenterPosition = tilemap.GetCellCenterWorld(cellPosition);
                                player.transform.position = cellCenterPosition;//грой становитьс€ на позицию по середине тайла на tilemap, что бы не по€вл€лс€ в стенке
                                camera.transform.position = new Vector3(cellCenterPosition.x, cellCenterPosition.y, -10f);//камера получает такое же местоположение как и герой
                                CameraPos = !CameraPos;
                            }
                        }
                    }
                }
                else if (maze[i, j] == 1)
                {
                    try//исключени€, что бы не вызывать ошибку когда пытаетс€ получить значаени€ элемента < 0 или больше максимального размера лаберинта
                    {
                        if (maze[i + 1, j] == 1 && maze[i - 1, j] == 1 && maze[i, j - 1] == 1 && maze[i, j + 1] != 1)
                        {
                            tilemapWall.SetTile(new Vector3Int(i, j, 0), wallCornerTile);//—оздание стенки угловой когда есть 3 стороны, лево право и низ
                        }
                        else if(maze[i + 1, j] == 0 && maze[i - 1, j] == 0 && maze[i, j - 1] == 0 && maze[i, j + 1] == 1)
                        {
                            tilemapWall.SetTile(new Vector3Int(i, j, 0), wallDownTile);//—оздание стенки окончани€ есть стенка заканчиваетс€ только есть есть блок сверху
                        }
                        else if (maze[i + 1, j] == 1 && maze[i - 1, j] == 1)
                        {
                            tilemapWall.SetTile(new Vector3Int(i, j, 0), wallTile);// стенука пр€ма€ создаЄтс€ только если есть слева и справо стена
                        }
                        else if (maze[i, j - 1] == 1 && maze[i, j + 1] == 1)
                        {
                            tilemapWall.SetTile(new Vector3Int(i, j, 0), wallSideTile);//стена бокова€ создаЄтс€ только если есть стенки с низу и сверху
                        }
                        else
                        {
                            tilemapWall.SetTile(new Vector3Int(i, j, 0), wallTile);// во всех иных случа€х используетс€ стандартна€ стена
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
