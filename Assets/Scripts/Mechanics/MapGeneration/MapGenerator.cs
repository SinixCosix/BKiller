﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Mechanics.MapGeneration
{
    public class MapGenerator : MonoBehaviour
    {
        public static MapGenerator Instance;

        public Vector2 StartPoint { get; private set; }
        public TilemapPainter painter;
        [NonSerialized] public List<Rect> Rooms = new List<Rect>();
        public readonly HashSet<Vector2Int> PointsOfPoints = new HashSet<Vector2Int>();

        public readonly HashSet<Vector2Int> Paths = new HashSet<Vector2Int>();
        public readonly HashSet<Vector2Int> Decorations = new HashSet<Vector2Int>();
        public readonly HashSet<Vector2Int> Forest = new HashSet<Vector2Int>();

        public MapGenerator()
        {
            Instance = this;
        }

        public void Generate()
        {
            RoomsGenerator.Generate();
            PathsGenerator.Generate();
            DecorationsGenerator.Generate();
            ForestGenerator.Generate();

            painter.PaintWalls();
            painter.PaintRooms(PointsOfPoints);
            painter.PaintPaths(Paths);
            painter.PaintForest(Forest);
            painter.PaintDecorations(Decorations, Paths);

            painter.Cut(PointsOfPoints);
            painter.Cut(Paths);
        }

        public void Clear()
        {
            Rooms.Clear();
            PointsOfPoints.Clear();
            Paths.Clear();
            Decorations.Clear();
            Forest.Clear();
            
            painter.Clear();
            
        }

        public void SelectStartPoint()
        {
            var index = Random.Range(0, Rooms.Count);
            StartPoint = Rooms[index].center;
        }
    }
}