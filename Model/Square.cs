using System;
using System.Collections.Generic;
using System.Linq;

namespace DerivcoTestTask.Model
{
    public class Square
    {
        public int X { get; set; }
        public int Y { get; set; }

        private int _surfaceArea = 1;
        public int SurfaceArea
        {
            get
            {
                if (RelatedSquares.Any())
                {
                    return Math.Max(RelatedSquares.Max(x => x._surfaceArea), _surfaceArea);
                }
                return _surfaceArea;
            }
            private set => _surfaceArea = value;
        }

        public SquareType SquareType { get; set; }
        public List<Square> RelatedSquares { get; private set; }
        internal bool IsRelated { get; set; }
        internal bool IsMain { get; set; }

        public Square()
        {
            RelatedSquares = new List<Square>();
        }

        public Square(int x, int y, SquareType squareType) : this()
        {
            X = x;
            Y = y;
            SquareType = squareType;
        }

        public Square(int x, int y, int surfaceArea, SquareType squareType = SquareType.Water) : this(x, y, squareType, surfaceArea)
        { }

        public Square(int x, int y, SquareType squareType, int surfaceArea) : this(x, y, squareType)
        {
            SurfaceArea = surfaceArea;
        }

        internal void AddRelatedSquare(Square square)
        {
            _surfaceArea++;
            RelatedSquares.Add(square);
        }

        public override string ToString()
        {
            return $"X={X}; Y={Y}; Square Type={SquareType}; SurfaceArea={SurfaceArea}";
        }

        public string ToPrettyString()
        {
            return $"The lake with surface area of {SurfaceArea} square meters is located at ({X}, {Y}).";
        }

        public override bool Equals(object obj)
        {
            if (obj is Square square)
            {
                return square.SquareType.Equals(SquareType)
                    && square.SurfaceArea.Equals(SurfaceArea)
                    && square.X.Equals(X)
                    && square.Y.Equals(Y);
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}