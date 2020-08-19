// Separate class for pathfinding

using System;
using System.Collections.Generic;
using System.Linq;

namespace MySpace
{
    public static class PathFinder
    {
        public static List<GridIndex> FindPath(GridIndex startIndex, GridIndex endIndex, TileMap gridMovements)
        {
            if (gridMovements == null) return (new List<GridIndex>());

            Enums.MoveDirections[] moveDirs = (from Enums.MoveDirections m in Enum.GetValues(typeof(Enums.MoveDirections)) select m).ToArray();
            List<List<GridIndex>> allPaths = new List<List<GridIndex>>();
            List<GridIndex> firstPath = new List<GridIndex>();
            List<GridIndex> reachedIndexes = new List<GridIndex>();
            firstPath.Add(startIndex);
            if (startIndex == endIndex)
                return firstPath;
            reachedIndexes.Add(startIndex);
            allPaths.Add(firstPath);

            bool Working = true;
            int CycleCounter = 0;

            while (Working && CycleCounter < 1000)
            {
                Working = false;
                CycleCounter++;

                #region oldPathfinding
                //List<List<GridIndex>> allPathsTmp = new List<List<GridIndex>>();
                //foreach (List<GridIndex> onePath in allPaths)
                //{
                //    GridIndex lastIndex = onePath[onePath.Count - 1];

                //    foreach (Enums.MoveDirections Dir in Enum.GetValues(typeof(Enums.MoveDirections)))
                //    {
                //        if (gridMovements.GridTileHasDirection(lastIndex, Dir))
                //        {
                //            GridIndex nextIndex = lastIndex.GetAdjacent(Dir);
                //            if (onePath.Contains(nextIndex) == false)
                //            {
                //                List<GridIndex> newPath = new List<GridIndex>(onePath);
                //                newPath.Add(nextIndex);
                //                if (nextIndex == endIndex)
                //                {
                //                    //CleanUpPath(ref newPath);
                //                    return (newPath);
                //                }
                //                Working = true;
                //                allPathsTmp.Add(newPath);
                //            }
                //        }
                //    }
                //}
                #endregion

                int CycleCounter2 = 0;
                int length = allPaths.Count;
                for (int i = 0; i < length; i++)
                {
                    CycleCounter2++;
                    

                    GridIndex lastIndex = allPaths[i][allPaths[i].Count - 1];
                    if (CycleCounter2 > 1000)
                    {
                        UnityEngine.Debug.LogError("Large cycle count! allPaths.Count = " + allPaths.Count + ", i = " + i + ", cycleCounter2 = " + CycleCounter2 + ", length = " + length + ", lastIndex = " + lastIndex.ToString() + ", allPaths[i].count = " + allPaths[i].Count);
                    }

                    int movements = 0;
                    foreach (Enums.MoveDirections Dir in moveDirs)
                    {
                        if (gridMovements.GridTileHasDirection(lastIndex, Dir))
                        {
                            GridIndex nextIndex = lastIndex.GetAdjacent(Dir);
                            //only add the available movements if the list doesnt contain the index that is to this direction already.
                            if (!(allPaths[i].Contains(nextIndex)) && !(reachedIndexes.Contains(nextIndex)))
                                movements |= (byte)Dir;
                        }
                    }

                    //If the path is a dead end, remove it from the list of paths that will continue to be checked.
                    if (movements <= 0)
                    {
                        allPaths.Remove(allPaths[i]);
                        i--;
                        length--;
                        continue;
                    }

                    for(int ii = 0; ii < moveDirs.Length; ii++)
                    {
                        if ((movements & (byte)moveDirs[ii]) != 0)
                        {
                            GridIndex nextIndex = lastIndex.GetAdjacent(moveDirs[ii]);
                            if (allPaths[i].Contains(nextIndex))
                                UnityEngine.Debug.LogError("Path already contains this index!!");
                            if (reachedIndexes.Contains(nextIndex))
                                UnityEngine.Debug.LogError("Somehow going over the same index again");
                            //make sure there's no index out     
                            //of range exception on the next
                            //part of this if statement          Check to see if this is the last movement direction that is valid.
                            if (((ii - 1) < moveDirs.Length) && (movements - (int)moveDirs[ii + 1] > 0))
                            {
                                List<GridIndex> newPath = new List<GridIndex>(allPaths[i]);
                                newPath.Add(nextIndex);
                                if (nextIndex == endIndex)
                                {
                                    //CleanUpPath(ref newPath);
                                    return (newPath);
                                }
                                Working = true;
                                allPaths.Add(newPath);
                                reachedIndexes.Add(nextIndex);
                            }
                            else
                            {
                                allPaths[i].Add(nextIndex);
                                reachedIndexes.Add(nextIndex);
                                if (nextIndex == endIndex)
                                    return allPaths[i];
                                Working = true;
                            }
                        }
                    }
                }

                //allPaths = allPathsTmp;
            }

            UnityEngine.Debug.LogError("Pathfinding failed! Cycle Counter = " + CycleCounter);
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
