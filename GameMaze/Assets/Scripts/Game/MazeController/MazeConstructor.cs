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
        data = dataGenerator.FromDimensions(sizeRows, sizeCols);
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
    private void Start()
    {

        int[,] maze = data;
        int rMax = maze.GetUpperBound(0);
        int cMax = maze.GetUpperBound(1);

        for (int i = 0; i < maze.GetLength(0); i++)
        {
            for (int j = 0; j < maze.GetLength(1); j++)
            {
                tilemap.SetTile(new Vector3Int(i, j, 0), floorTile);
                if (maze[i, j] == 0)
                {
                    if (!CameraPos)
                    {
                        if (sizeR / 2 < i && sizeC / 2 < j)
                        {
                            if (j - 2 < sizeC / 2) 
                            {
                                camera.transform.position = new Vector3Int(i, j, -10);
                                player.transform.position = new Vector3Int(i, j, 0);
                                CameraPos = !CameraPos;
                            }
                        }
                    }
                }
                else if (maze[i, j] == 1)
                {
                    try
                    {
                        if (maze[i + 1, j] == 1 && maze[i - 1, j] == 1)
                        {
                            tilemapWall.SetTile(new Vector3Int(i, j, 0), wallTile);
                        }
                        else if (maze[i, j - 1] == 1 && maze[i, j + 1] == 1)
                        {
                            tilemapWall.SetTile(new Vector3Int(i, j, 0), wallSideTile);
                        }
                        else
                        {
                            tilemapWall.SetTile(new Vector3Int(i, j, 0), wallCornerTile);
                        }
                    }catch (System.IndexOutOfRangeException e) 
                    { 
                    
                    }        
                }
            }
        }
    }
}
