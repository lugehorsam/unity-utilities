﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{

    /// <summary>
    /// Arbitrary grid layout. Bottom left is row 0, col 0
    /// </summary>
    public class GridLayout<T> : Layout<T> where T : IGridMember<T>, ILayoutMember
    {
        public event Action<int> OnIndexTouch = delegate { };
        protected override string Name { get { return "GridLayout";  } }

        public float CellHeight
        {
            get { return _cellHeight; }
        }

        public float CellWidth
        {
            get { return _cellWidth; }
        } 

        private readonly Grid<T> _grid;
        private readonly float _cellWidth;
        private readonly float _cellHeight;        
        private readonly RectTransform _rectTransform;
        private readonly List<GridCell> _cellOutlines;
        
        public GridLayout(Grid<T> grid, float cellWidth, float cellHeight) : base(grid)
        {
            _grid = grid;
            _rectTransform = GameObject.AddComponent<RectTransform>();
            _cellWidth = cellWidth;
            _cellHeight = cellHeight;
            _cellOutlines = CreateCellOutlines();
            ShowOutline(true);
            DoLayout();
        }

        List<GridCell> CreateCellOutlines()
        {
            var lineSquares = new List<GridCell>();
            for (int i = 0; i < _grid.Rows * _grid.Columns; i++)
            {
                Vector2[] squarePoints = GetSquarePoints(i);
                var gridCell = new GridCell
                (
                    squarePoints[0],
                    squarePoints[1],
                    squarePoints[2],
                    squarePoints[3],
                    new Vector3(_cellWidth, _cellHeight)
                );
                
                gridCell.LineWidth = .025f;
                gridCell.Transform.SetParent(Transform);
                lineSquares.Add
                (
                    gridCell    
                );
                
                gridCell.TouchDispatcher.OnTouch += (cell, gesture) => OnIndexTouch(i);
            }

            return lineSquares;
        }
                
        protected override Vector2 GetIdealLocalPosition(T element)
        {
            return new Vector2(element.Column * _cellWidth, element.Row * _cellHeight);
        }

        public void ShowOutline(bool show)
        {
            foreach (var outline in _cellOutlines)
            {
                outline.GameObject.SetActive(show);
            }
        }

        Vector2[] GetSquarePoints(int gridIndex)
        {
            var offsetCombinations = new Vector2
            (
                _grid.ColumnOfIndex(gridIndex) * _cellWidth,
                _grid.RowOfIndex(gridIndex) * _cellHeight
            ).GetOffsetCombinations(_cellWidth, _cellHeight);
            
            return offsetCombinations;
        }
    }   
}
