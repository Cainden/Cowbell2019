// Separate class for pathfinding

using System;
using System.Collections.Generic;

namespace MySpace
{
    public static class PathFinder
    {
        public static List<GridIndex> FindPath(GridIndex startIndex, GridIndex endIndex, TileMap gridMovements)
        {
            if (gridMovements == null) return (new List<GridIndex>());

            List<List<GridIndex>> allPaths = new List<List<GridIndex>>();
            List<GridIndex> firstPath = new List<GridIndex>();
            firstPath.Add(startIndex);
            allPaths.Add(firstPath);

            bool Working = true;
            int CycleCounter = 0;

            while (Working)
            {
                Working = false;
                CycleCounter++;

                List<List<GridIndex>> allPathsTmp = new List<List<GridIndex>>();

                foreach (List<GridIndex> onePath in allPaths)
                {
                    GridIndex lastIndex = onePath[onePath.Count - 1];

                    foreach (Enums.MoveDirections Dir in Enum.GetValues(typeof(Enums.MoveDirections)))
                    {
                        if (gridMovements.GridTileHasDirection(lastIndex, Dir))
                        {
                            GridIndex nextIndex = lastIndex.GetAdjacent(Dir);
                            if (onePath.Contains(nextIndex) == false)
                            {
                                List<GridIndex> newPath = new List<GridIndex>(onePath);
                                newPath.Add(nextIndex);
                                if (nextIndex == endIndex)
                                {
                                    CleanUpPath(ref newPath);
                                    return (newPath);
                                }
                                Working = true;
                                allPathsTmp.Add(newPath);
                            }
                        }
                    }
                }

                allPaths = allPathsTmp;
            }

            return (new List<GridIndex>());
        }

        // Cleanup the index path: A row of grid-indizes on same Y/Z-level
        // can be combined to just start/end index ("straight walking line")
        static private void CleanUpPath(ref List<GridIndex> Path)
        {
            if (Path.Count < 3) return;

            List<GridIndex> NewPath = new List<GridIndex>();
            NewPath.Add(Path[0]);

            for (int i = 1; i < (Path.Count - 1); i++)
            {
                if ((Path[i - 1].Y == Path[i].Y) && (Path[i].Y == Path[i + 1].Y) &&
                    (Path[i - 1].Z == Path[i].Z) && (Path[i].Z == Path[i + 1].Z)) continue;

                NewPath.Add(Path[i]);
            }

            NewPath.Add(Path[Path.Count - 1]);
            Path = NewPath;
        }
    }
}
