﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Mechanics
{
    public class LevelGenerator : MonoBehaviour
    {
        public uint mapSize = 100;
        public uint splitCount = 5;

        public float splitRatio = 0.25f;
        private float _splitRatio;

        public uint minRoomSize = 6;
        public MobSpawner mobSpawner;

        public Vector2 StartPoint
        {
            get
            {
                var index = Random.Range(0, _rooms.Count);
                return _rooms[index].center;
            }
        }

        public TilemapVisualizer tilemap;

        private readonly List<Rect> _rooms = new List<Rect>();
        private readonly HashSet<Vector2Int> _paths = new HashSet<Vector2Int>();
        private readonly HashSet<Vector2Int> _decorations = new HashSet<Vector2Int>();
        private readonly HashSet<Vector2Int> _trees = new HashSet<Vector2Int>();

        public void Generate()
        {
            Clear();

            var rect = new Rect(0, 0, mapSize, mapSize);
            GenerateLawns(rect, splitCount);
            GeneratePaths();
            GenerateDecorations(_rooms);
            GenerateDecorations(_paths);
            GenerateTrees();
            mobSpawner.Spawn(_rooms);

            tilemap.PaintWalls();
            tilemap.PaintLawns(_rooms);
            tilemap.PaintPaths(_paths);
            tilemap.PaintForest(_trees);

            tilemap.Cut(_rooms);
            tilemap.Cut(_paths);

            tilemap.PaintDecorations(_decorations, _paths);
        }

        private void GenerateTrees()
        {
            for (var y = 0; y < mapSize; y += 4)
            for (var x = 0; x < mapSize; x += Random.Range(2, 6))
                _trees.Add(new Vector2Int(x, y + Random.Range(-2, 2)));
        }

        private void Start()
        {
            _splitRatio = 1 - splitRatio;
        }

        private void GenerateDecorations(IEnumerable<Rect> lawns)
        {
            foreach (var rect in lawns)
            {
                var minDecorationsCount = rect.width;
                var maxDecorationsCount = rect.width + rect.height;
                var decorationsCount = Random.Range(minDecorationsCount, maxDecorationsCount);
                for (var i = 0; i < decorationsCount; ++i)
                {
                    var x = Random.Range((int) rect.x, (int) rect.xMax);
                    var y = Random.Range((int) rect.y, (int) rect.yMax);
                    _decorations.Add(new Vector2Int(x, y));
                }
            }
        }

        private void GenerateDecorations(ICollection<Vector2Int> paths)
        {
            var minDecorationsCount = paths.Count / 6;
            var maxDecorationsCount = paths.Count / 5;
            var decorationsCount = Random.Range(minDecorationsCount, maxDecorationsCount);
            for (var i = 0; i < decorationsCount; ++i)
            {
                var index = Random.Range(0, paths.Count);
                var point = paths.ElementAt(index);
                _decorations.Add(point);
            }
        }

        private void Clear()
        {
            mobSpawner.Clear();
            _rooms.Clear();
            _paths.Clear();
            _decorations.Clear();
            tilemap.Clear();
        }

        private void GenerateLawns(Rect rect, uint parts)
        {
            if (parts == 0)
            {
                _rooms.Add(ReshapeRect(rect));
                return;
            }

            bool splitByVertical;
            if (rect.width / rect.height >= 1.25)
                splitByVertical = true;
            else if (rect.height / rect.width >= 1.25)
                splitByVertical = false;
            else
                splitByVertical = Random.Range(0f, 1f) > 0.5f;

            var splitByHorizontal = !splitByVertical;

            var width1 = splitByVertical
                ? Random.Range(rect.width * splitRatio, rect.width * _splitRatio)
                : rect.width;
            var height1 = splitByHorizontal
                ? Random.Range(rect.height * splitRatio, rect.height * _splitRatio)
                : rect.height;

            // ReSharper disable TailRecursiveCall
            var x1 = rect.x;
            var y1 = rect.y;
            var rect1 = new Rect(x1, y1, width1, height1);
            GenerateLawns(rect1, parts - 1);

            var width2 = splitByVertical ? rect.width - rect1.width : rect.width;
            var height2 = splitByHorizontal ? rect.height - rect1.height : rect.height;
            var x2 = splitByVertical ? x1 + width1 : x1;
            var y2 = splitByHorizontal ? y1 + height1 : y1;
            var rect2 = new Rect(x2, y2, width2, height2);
            GenerateLawns(rect2, parts - 1);
        }

        private Rect ReshapeRect(Rect rect)
        {
            var minLength = Math.Min(rect.width, rect.height);
            var width = Random.Range(minLength * splitRatio, rect.width);
            if (width < minRoomSize)
                width = minRoomSize;
            var height = Random.Range(minLength * splitRatio, rect.height);
            if (height < minRoomSize)
                height = minRoomSize;
            var x = Random.Range(rect.x, rect.xMax - width);
            var y = Random.Range(rect.y, rect.yMax - height);

            return new Rect(x, y, width, height);
        }

        private void GeneratePaths()
        {
            for (var i = 0; i < _rooms.Count - 1; ++i)
            {
                var room = _rooms[i].center;
                var nextRoom = _rooms[i + 1].center;
                var position = room;

                while ((int) position.x != (int) nextRoom.x)
                {
                    if (nextRoom.x > position.x)
                        position += Vector2.right;
                    else if (nextRoom.x < position.x)
                        position += Vector2.left;

                    for (var j = 0; j < 2; ++j)
                    {
                        var y = (int) position.y + j;
                        _paths.Add(new Vector2Int((int) position.x, y));
                    }
                }

                while ((int) position.y != (int) nextRoom.y)
                {
                    if (nextRoom.y > position.y)
                        position += Vector2.up;
                    else if (nextRoom.y < position.y)
                        position += Vector2.down;

                    for (var j = 0; j < 2; ++j)
                    {
                        var x = (int) position.x + j;
                        _paths.Add(new Vector2Int(x, (int) position.y));
                    }
                }
            }
        }
    }
}