using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace _2048_Solver
{
    public class Grid
    {
        public static readonly Direction[] AllDirections = { Direction.Up, Direction.Right, Direction.Down, Direction.Left };

        private int[,] cells;

        public Grid()
        {
            cells = new int[4, 4];
        }

        public int? this[int row, int col]
        {
            get
            {
                int value = cells[row, col];
                return value != 0 ? value : (int?)null;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            
            for (int row = 0; row <= 3; row++)
            {
                sb.AppendLine("+----+----+----+----+");
                for (int col = 0; col <= 3; col++)
                {
                    sb.Append("|");
                    sb.Append((this[row, col]?.ToString() ?? "").PadLeft(4));
                    if (col == 3)
                    {
                        sb.AppendLine("|");
                    }
                }
                if (row == 3)
                {
                    sb.AppendLine("+----+----+----+----+");
                }
            }

            return sb.ToString();
        }

        public bool Contains(int value) => AllCells.Any(c => c.Value == value);

        public IEnumerable<int> AllValues => AllCells.Select(c => c.Value);

        private IEnumerable<(int Row, int Col)> EmptyCells =>
            AllCells.Where(c => c.Value == 0).Select(c => (c.Row, c.Col));

        private IEnumerable<(int Row, int Col, int Value)> PopulatedCells =>
            AllCells.Where(c => c.Value != 0);

        private IEnumerable<(int Row, int Col, int Value)> AllCells
        {
            get
            {
                for (int row = 0; row <= 3; row++)
                {
                    for (int col = 0; col <= 3; col++)
                    {
                        yield return (row, col, cells[row, col]);
                    }
                }
            }
        }

        public void AddNewNumberInEmptyCell()
        {
            var cell = RandomUtility.PickOne(EmptyCells.ToList());
            int number = RandomUtility.Between(0.0, 1.0) < 0.9 ? 2 : 4;
            cells[cell.Row, cell.Col] = number;
        }

        public Grid Clone()
        {
            Grid clone = new Grid();

            foreach (var c in AllCells)
            {
                clone.cells[c.Row, c.Col] = c.Value;
            }

            return clone;
        }

        private bool RowIsEmpty(int row)
        {
            return Enumerable.Range(0, 4).All(col => cells[row, col] == 0);
        }

        private bool ColumnIsEmpty(int col)
        {
            return Enumerable.Range(0, 4).All(row => cells[row, col] == 0);
        }

        public bool Contains2048 => this.AllValues.Any(v => v >= 2048);

        public bool TryShift(Direction direction, out int points)
        {
            switch (direction)
            {
                case Direction.Left:
                    return TryShiftLeft(out points);
                case Direction.Right:
                    return TryShiftRight(out points);
                case Direction.Up:
                    return TryShiftUp(out points);
                case Direction.Down:
                    return TryShiftDown(out points);
                default:
                    throw new Exception($"Cannot shift in direction: {direction}");
            }
        }

        public bool TryRandomShift(out Direction direction, out int points)
        {
            List<Direction> remainingDirections = new List<Direction>(Grid.AllDirections);
            while (remainingDirections.Count > 0)
            {
                direction = RandomUtility.PickOneAndRemove(remainingDirections);
                if (this.TryShift(direction, out points))
                    return true;
            }

            direction = Direction.None;
            points = 0;
            return false;
        } 

        private bool TryShiftLeft(out int points)
        {
            bool allowed = false;
            points = 0;

            for (int row = 0; row <= 3; row++)
            {
                bool keepShifting = !RowIsEmpty(row);
                while (keepShifting)
                {
                    keepShifting = false;
                    for (int col = 1; col <= 3; col++)
                    {
                        if (cells[row, col] != 0)
                        {
                            if (cells[row, col] > 0 && cells[row, col] == cells[row, col - 1])
                            {
                                int newValue = cells[row, col] * 2;
                                cells[row, col - 1] = 0 - newValue;
                                cells[row, col] = 0;
                                points += newValue;
                                allowed = true;
                                keepShifting = true;
                            }
                            else if (cells[row, col - 1] == 0)
                            {
                                cells[row, col - 1] = cells[row, col];
                                cells[row, col] = 0;
                                allowed = true;
                                keepShifting = true;
                            }
                        }
                    }
                }
            }

            foreach (var cell in PopulatedCells)
            {
                if (cell.Value < 0)
                {
                    int value = Math.Abs(cell.Value);
                    cells[cell.Row, cell.Col] = value;
                    points += value;
                }
            }

            return allowed;
        }

        private bool TryShiftRight(out int points)
        {
            bool allowed = false;
            points = 0;

            for (int row = 0; row <= 3; row++)
            {
                bool keepShifting = !RowIsEmpty(row);
                while (keepShifting)
                {
                    keepShifting = false;
                    for (int col = 2; col >= 0; col--)
                    {
                        if (cells[row, col] != 0)
                        {
                            if (cells[row, col] > 0 && cells[row, col] == cells[row, col + 1])
                            {
                                int newValue = cells[row, col] * 2;
                                cells[row, col + 1] = 0 - newValue;
                                cells[row, col] = 0;
                                points += newValue;
                                allowed = true;
                                keepShifting = true;
                            }
                            else if (cells[row, col + 1] == 0)
                            {
                                cells[row, col + 1] = cells[row, col];
                                cells[row, col] = 0;
                                allowed = true;
                                keepShifting = true;
                            }
                        }
                    }
                }
            }

            foreach (var cell in PopulatedCells)
            {
                if (cell.Value < 0)
                {
                    int value = Math.Abs(cell.Value);
                    cells[cell.Row, cell.Col] = value;
                    points += value;
                }
            }

            return allowed;
        }

        private bool TryShiftUp(out int points)
        {
            bool allowed = false;
            points = 0;

            for (int col = 0; col <= 3; col++)
            {
                bool keepShifting = !ColumnIsEmpty(col);
                while (keepShifting)
                {
                    keepShifting = false;
                    for (int row = 1; row <= 3; row++)
                    {
                        if (cells[row, col] != 0)
                        {
                            if (cells[row, col] > 0 && cells[row, col] == cells[row - 1, col])
                            {
                                int newValue = cells[row, col] * 2;
                                cells[row - 1, col] = 0 - newValue;
                                cells[row, col] = 0;
                                allowed = true;
                                keepShifting = true;
                            }
                            else if (cells[row - 1, col] == 0)
                            {
                                cells[row - 1, col] = cells[row, col];
                                cells[row, col] = 0;
                                allowed = true;
                                keepShifting = true;
                            }
                        }
                    }
                }
            }

            foreach (var cell in PopulatedCells)
            {
                if (cell.Value < 0)
                {
                    int value = Math.Abs(cell.Value);
                    cells[cell.Row, cell.Col] = value;
                    points += value;
                }
            }

            return allowed;
        }

        private bool TryShiftDown(out int points)
        {
            bool allowed = false;
            points = 0;

            for (int col = 0; col <= 3; col++)
            {
                bool keepShifting = !ColumnIsEmpty(col);
                while (keepShifting)
                {
                    keepShifting = false;
                    for (int row = 2; row >= 0; row--)
                    {
                        if (cells[row, col] != 0)
                        {
                            if (cells[row, col] > 0 && cells[row, col] == cells[row + 1, col])
                            {
                                int newValue = cells[row, col] * 2;
                                cells[row + 1, col] = 0 - newValue;
                                cells[row, col] = 0;
                                allowed = true;
                                keepShifting = true;
                            }
                            else if (cells[row + 1, col] == 0)
                            {
                                cells[row + 1, col] = cells[row, col];
                                cells[row, col] = 0;
                                allowed = true;
                                keepShifting = true;
                            }
                        }
                    }
                }
            }

            foreach (var cell in PopulatedCells)
            {
                if (cell.Value < 0)
                {
                    int value = Math.Abs(cell.Value);
                    cells[cell.Row, cell.Col] = value;
                    points += value;
                }
            }

            return allowed;
        }
    }
}