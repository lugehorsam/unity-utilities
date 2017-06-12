﻿using System;
using UnityEngine;

namespace Utilities
{
    [Serializable]
    public class GridMember : IGridMember, IComparable<GridMember>
    {
        public IGrid Grid { get; set; }

        public int Index
        {
            get { return Grid.ToIndex(_row, _column); }
        }

        public int Row
        {
            get { return _row; }
            set { _row = value; }
        }
    
        [SerializeField]
        private int _row;

        public int Column
        {
            get { return _column; }
            set { _column = value; }
        }
    
        [SerializeField]
        private int _column;

        public int CompareTo(GridMember other)
        {
            return Index.CompareTo(other.Index);
        }

        public override string ToString()
        {
            return this.ToString(Row, Column);
        }

        public GridMember(int row, int column)
        {
            _row = row;
            _column = column;
        }

        public GridMember()
        {
        }        
    }
}
