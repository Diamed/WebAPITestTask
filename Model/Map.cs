using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("UnitTests")]

namespace DerivcoTestTask.Model
{
    public sealed class Map
    {
        /// <summary>
        /// Raw map as it has been received
        /// </summary>
        public string Raw { get; }
        /// <summary>
        /// Processed map with all squares including <see cref="SquareType.Land"/>
        /// </summary>
        public Square[,] Processed { get; private set; }
        /// <summary>
        /// All water squares what have been found
        /// </summary>
        public List<Square> WaterSquares { get; private set; }

        public Map(string rawMap)
        {
            Raw = rawMap;
        }

        /// <summary>
        /// Proceed raw map into processed map.
        /// If there is some error would be returned message.
        /// </summary>
        /// <returns></returns>
        public string Proceed()
        {
            if (string.IsNullOrWhiteSpace(Raw))
            {
                return "Can you try another one map? Looks like this map is empty.";
            }
            return FillProcessedMap() ?? FillWaterSquares();
        }

        private string FillProcessedMap()
        {
            const char LandSymbol = '#';
            const char WaterSymbol = 'O';

            var map = new List<Square[]>();
            var row = new List<Square>();

            foreach (var symbol in Raw)
            {
                var square = new Square
                {
                    X = row.Count,
                    Y = map.Count
                };
                switch (symbol)
                {
                    case LandSymbol:
                        square.SquareType = SquareType.Land;
                        row.Add(square);
                        break;
                    case WaterSymbol:
                        square.SquareType = SquareType.Water;
                        row.Add(square);
                        break;
                    case '\n':
                        map.Add(row.ToArray());
                        row = new List<Square>();
                        break;
                    default:
                        break;
                }
            }
            if (row.Any())
            {
                map.Add(row.ToArray());
            }

            if (map.Any(x => x.Length != map.First().Length))
            {
                return "Unfortunately we can't handle not square maps currently. Please, try to change your map.";
            }
            Processed = map.ToArray().To2D();

            return null;
        }

        private string FillWaterSquares()
        {
            var processed = Processed.Cast<Square>()
                .Where(x => x.SquareType == SquareType.Water)
                .Order();
            WaterSquares = new List<Square>();

            var added = new List<Square>();
            foreach (var item in processed)
            {
                WaterSquares.Add(item);
            }

            return null;
        }

        /// <summary>
        /// Get description of all water places with coordinates and surface areas.
        /// </summary>
        /// <returns></returns>
        public string GetWaterCoordinates()
        {
            if (!WaterSquares.Any())
            {
                return $"Looks like water is ran out on this land. {Environment.NewLine}" +
                    $"Hope we will be able to find another source of water. {Environment.NewLine}" +
                    $"We will not last long without it. Maybe you have some another map?";
            }

            var sb = new StringBuilder(500);
            sb.AppendLine("We found the following water sources :");
            foreach (var item in GetWaterSources())
            {
                sb.AppendLine(item.ToPrettyString());
            }
            sb.AppendLine("Let's go there immediately before water runs out there!");

            return sb.ToString();
        }

        internal IEnumerable<Square> GetWaterSources()
        {
            foreach (var item in WaterSquares.Where(x => !x.IsMain && !x.IsRelated))
            {
                FindRelated(item);
            }
            return WaterSquares.Where(x => x.IsMain);
        }

        /* We begin from start point and spread waves to all sides
         * -2 -2 -2 -2      -2 -2 -2 -2      -2 -2 -2 -2      -2 -2 -2 -2      -2 -2 -2 -2
         * -2 -2 -1 -2  ->  -2 -2 -1 -2  ->  -2 -2 -1 -2  ->  -2 -2 -1 -2  ->  -2 -2  4 -2
         * -2 -1 -1 -2      -2 -1 -1 -2      -2  2 -1 -2      -2  2  3 -2      -2  2  3 -2
         *  0 -1 -2 -2       0  1 -2 -2       0  1 -2 -2       0  1 -2 -2       0  1 -2 -2
         *  
         *  During executing we mark all found squares as related 
         *  to main square (what was used as start point)
         *  If there are another water squares that are not related
         *  to 'lake' then they won't be marked as related and
         *  would be handled during the next calling of this method
         */
        private void FindRelated(Square startPoint)
        {
            const int isNotVisited = -1, doNotVisit = -2;
            bool add = true;
            int mapHeight = Processed.GetLength(0);
            int mapWidth = Processed.GetLength(1);
            int x, y, step = 0;

            // Making "virtual" map for searching
            int[,] map = GenerateMap(mapHeight, mapWidth, isNotVisited, doNotVisit);

            // We will begin searching from this point
            map[startPoint.Y, startPoint.X] = 0;
            startPoint.IsMain = true;

            // While we have anything to mark
            while (add == true)
            {
                add = false;
                for (y = 0; y < mapHeight; y++)
                {
                    for (x = 0; x < mapWidth; x++)
                    {
                        if (map[y, x] == step)
                        {
                            // If square is not main then add it as related to main
                            var item = WaterSquares.First(s => s.Y == y && s.X == x);
                            if (!item.IsMain)
                            {
                                startPoint.AddRelatedSquare(item);
                                item.IsRelated = true;
                            }

                            // Mark surround squares
                            if (y - 1 >= 0 && map[y - 1, x] != doNotVisit && map[y - 1, x] == isNotVisited)
                            {
                                map[y - 1, x] = step + 1;
                            }
                            if (x - 1 >= 0 && map[y, x - 1] != doNotVisit && map[y, x - 1] == isNotVisited)
                            {
                                map[y, x - 1] = step + 1;
                            }
                            if (y + 1 < mapHeight && map[y + 1, x] != doNotVisit && map[y + 1, x] == isNotVisited)
                            {
                                map[y + 1, x] = step + 1;
                            }
                            if (x + 1 < mapWidth && map[y, x + 1] != doNotVisit && map[y, x + 1] == isNotVisited)
                            {
                                map[y, x + 1] = step + 1;
                            }
                        }
                    }
                }
                step++;
                add = step <= mapHeight * mapWidth;
            }
        }

        private int[,] GenerateMap(int height, int width, int isNotVisited, int doNotVisit)
        {
            int[,] map = new int[height, width];
            
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (Processed[y, x].SquareType == SquareType.Water)
                    {
                        map[y, x] = isNotVisited;
                    }
                    else
                    {
                        map[y, x] = doNotVisit;
                    }
                }
            }

            return map;
        }
    }
}