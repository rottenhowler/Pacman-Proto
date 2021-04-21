
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Maze : ISerializationCallbackReceiver {
    [System.Flags]
    public enum Passage : short {
        None  = 0b0000,
        
        Right = 0b0001,
        Top   = 0b0010,
    }

    public int width;
    public int height;
    public Passage[,] cells;

    public List<Passage> serializedCells;

    public Maze(int width, int height) {
        this.width = width;
        this.height = height;

        cells = new Passage[width, height];
    }

    public void OnBeforeSerialize() {
        serializedCells = new List<Passage>(width * height);
        for (int x = 0; x < width; x++) {
            for (int y=0; y < height; y++) {
                serializedCells.Add(cells[x, y]);
            }
        }
    }

    public void OnAfterDeserialize() {
        cells = new Passage[width, height];
        if (serializedCells is null)
            return;

        int i = 0;
        for (int x = 0; x < width; x++) {
            for (int y=0; y < height; y++) {
                if (i >= serializedCells.Count)
                    break;
                cells[x, y] = serializedCells[i++];
            }

            if (i >= serializedCells.Count)
                break;
        }
        serializedCells = null;
    }
}