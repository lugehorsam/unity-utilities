﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilities.Input;

namespace Utilities
{
    public class GridCell : View
    {
        private readonly LineRenderer _lineRenderer;

        public TouchDispatcher<GridCell> TouchDispatcher
        {
            get { return _touchDispatcher; }
        }
        private readonly TouchDispatcher<GridCell> _touchDispatcher;
        
        public float LineWidth
        {
            get { return _lineRenderer.GetWidth(); }
            set { _lineRenderer.SetWidth(value); }
        }

        protected override string Name { get { return "Line Square"; }}
               
        public GridCell(Vector3 vector1, Vector3 vector2, Vector3 vector3, Vector3 vector4, Vector3 size)
        {
            _touchDispatcher = GameObject.AddComponent<GridCellDispatcher>();
            _touchDispatcher.Init(size, this);
            _touchDispatcher.BoxCollider.center = size / 2;
            
            var initialVectors = new []
            {
                vector1, vector2, vector3, vector4
            };
                        
            MathUtils.SortInCycle(initialVectors, CycleDirection.Clockwise);

            var sortedVectors = initialVectors.ToList();
            sortedVectors.Add(initialVectors[0]);
                        
            _lineRenderer = GameObject.AddComponent<LineRenderer>();
            _lineRenderer.useWorldSpace = false;
            _lineRenderer.positionCount = 5;
            
            _lineRenderer.SetPositions
            (
                sortedVectors.ToArray()
            );
        }

        class GridCellDispatcher : TouchDispatcher<GridCell>
        {
            
        }
    }
}