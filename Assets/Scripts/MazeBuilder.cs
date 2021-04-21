using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class MazeBuilder {
    public int mazeWidth;
    public int mazeHeight;
    public int wallCount;

    private struct CellPosition : IEquatable<CellPosition> {
        public int x, y;

        public CellPosition(int x, int y) {
            this.x = x;
            this.y = y;
        }

        public bool Equals(CellPosition other) {
            return x == other.x && y == other.y;
        }

        public override string ToString() {
            return "(x = " + x + ", y = " + y + ")";
        }

        public override int GetHashCode() {
            return x * 256 + y;
        }

        public CellPosition Left() {
            return new CellPosition(x - 1, y);
        }
        public CellPosition Right() {
            return new CellPosition(x + 1, y);
        }
        public CellPosition Top() {
            return new CellPosition(x, y+1);
        }
        public CellPosition Bottom() {
            return new CellPosition(x, y-1);
        }

        public List<CellPosition> Neighbours() {
            return new List<CellPosition>() { Left(), Top(), Right(), Bottom() };
        }
    }

    private enum Direction {
        Top,
        Rigth,
        Bottom,
        Left
    }

    private struct WallPosition {
        public CellPosition cell;
        public CellPosition next;

        public WallPosition(CellPosition cell, CellPosition next) {
            this.cell = cell;
            this.next = next;
        }
    }

    public Maze Build() {
        Maze maze = new Maze(mazeWidth, mazeHeight);

        // Build a full spanning tree maze first
        System.Random random = new System.Random();

        ISet<CellPosition> visitedCells = new HashSet<CellPosition>();
        List<WallPosition> walls = new List<WallPosition>();

        CellPosition p = new CellPosition(random.Next() % mazeWidth, random.Next() % mazeHeight);
        foreach (CellPosition neighbour in p.Neighbours()) {
            if (!IsValidCellPosition(maze, neighbour))
                continue;
            walls.Add(new WallPosition(p, neighbour));
        }
        visitedCells.Add(p);

        while (walls.Count > 0) {
            int wallIdx = random.Next() % walls.Count;
            WallPosition wall = walls[wallIdx];
            walls.RemoveAt(wallIdx);

            if (visitedCells.Contains(wall.next)) {
                continue;
            }

            if (wall.cell.x == wall.next.x) {
                maze.cells[wall.cell.x, Math.Min(wall.cell.y, wall.next.y)] |= Maze.Passage.Top;
            } else {
                maze.cells[Math.Min(wall.cell.x, wall.next.x), wall.cell.y] |= Maze.Passage.Right;
            }
            
            p = wall.next;
            foreach (CellPosition neighbour in p.Neighbours()) {
                if (!IsValidCellPosition(maze, neighbour))
                    continue;
                walls.Add(new WallPosition(p, neighbour));
            }
            visitedCells.Add(p);
        }

        // Then reduce number of walls to wallCount
        List<WallPosition> leftoverWalls = new List<WallPosition>();
        for (int x=0; x < maze.width; x++) {
            for (int y=0; y < maze.height; y++) {
                if (x < maze.width - 1 && ((maze.cells[x, y] & Maze.Passage.Right) == Maze.Passage.None)) {
                    leftoverWalls.Add(new WallPosition(new CellPosition(x, y), new CellPosition(x+1, y)));
                }
                if (y < maze.height -1 && ((maze.cells[x, y] & Maze.Passage.Top) == Maze.Passage.None)) {
                    leftoverWalls.Add(new WallPosition(new CellPosition(x, y), new CellPosition(x, y+1)));
                }
            }
        }
        while (leftoverWalls.Count > wallCount) {
            int wallIdx = random.Next() % leftoverWalls.Count;
            WallPosition wall = leftoverWalls[wallIdx];
            
            if (wall.cell.x == wall.next.x) {
                maze.cells[wall.cell.x, Math.Min(wall.cell.y, wall.next.y)] |= Maze.Passage.Top;
            } else {
                maze.cells[Math.Min(wall.cell.x, wall.next.x), wall.cell.y] |= Maze.Passage.Right;
            }

            leftoverWalls.RemoveAt(wallIdx);
        }
        
        return maze;
    }

    private bool IsValidCellPosition(Maze maze, CellPosition cell) {
        return cell.x >= 0 && cell.x < maze.width && cell.y >= 0 && cell.y < maze.height;
    }
}