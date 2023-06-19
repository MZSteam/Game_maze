using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeDataGenerator
{
    public float placementThreshold;

    public MazeDataGenerator()
    {
        placementThreshold = .1f;
    }
    public int[,] FromDimensions(int sizeRows, int sizeCols)
    {
        int[,] maze = new int[sizeRows, sizeCols];

        for (int i = 0; i < sizeRows; i++)
        {
            for (int j = 0; j < sizeCols; j++)
            {
                maze[i, j] = 1;
            }
        }

        System.Random rand = new System.Random();
        int r = rand.Next(sizeRows);
        int c = rand.Next(sizeCols);
        maze[r, c] = 0;

        GenerateMazeRecursive(maze, r, c);

        return maze;
    }
    private void GenerateMazeRecursive(int[,] maze, int r, int c)
    {
        List<int> directions = new List<int> { 1, 2, 3, 4 };
        directions = ShuffleList(directions);

        foreach (int direction in directions)
        {
            switch (direction)
            {
                case 1: // Up
                    if (r - 2 <= 0)
                        continue;
                    if (maze[r - 2, c] != 0)
                    {
                        maze[r - 2, c] = 0;
                        maze[r - 1, c] = 0;
                        GenerateMazeRecursive(maze, r - 2, c);
                    }
                    break;
                case 2: // Right
                    if (c + 2 >= maze.GetLength(1) - 1)
                        continue;
                    if (maze[r, c + 2] != 0)
                    {
                        maze[r, c + 2] = 0;
                        maze[r, c + 1] = 0;
                        GenerateMazeRecursive(maze, r, c + 2);
                    }
                    break;
                case 3: // Down
                    if (r + 2 >= maze.GetLength(0) - 1)
                        continue;
                    if (maze[r + 2, c] != 0)
                    {
                        maze[r + 2, c] = 0;
                        maze[r + 1, c] = 0;
                        GenerateMazeRecursive(maze, r + 2, c);
                    }
                    break;
                case 4: // Left
                    if (c - 2 <= 0)
                        continue;
                    if (maze[r, c - 2] != 0)
                    {
                        maze[r, c - 2] = 0;
                        maze[r, c - 1] = 0;
                        GenerateMazeRecursive(maze, r, c - 2);
                    }
                    break;
            }
        }
    }
    private List<int> ShuffleList(List<int> list)
    {
        List<int> shuffledList = new List<int>();
        System.Random rand = new System.Random();
        while (list.Count > 0)
        {
            int index = rand.Next(list.Count);
            shuffledList.Add(list[index]);
            list.RemoveAt(index);
        }
        return shuffledList;
    }
}
