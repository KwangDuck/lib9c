using System;
using System.Collections.Generic;
using Nekoyume.Model.State;
using Nekoyume.TableData;

namespace Nekoyume.Model
{
    [Serializable]
    public class WorldInformation : IState
    {
        [Serializable]
        public struct World : IState
        {
            public readonly int Id;
            public readonly string Name;
            public readonly int StageBegin;
            public readonly int StageEnd;
            public readonly long UnlockedBlockIndex;
            public readonly long StageClearedBlockIndex;
            public readonly int StageClearedId;

            public bool IsUnlocked => UnlockedBlockIndex != -1;
            public bool IsStageCleared => StageClearedBlockIndex != -1;

            public World(
                WorldSheet.Row worldRow,
                long unlockedBlockIndex = -1,
                long stageClearedBlockIndex = -1,
                int stageClearedId = -1)
            {
                Id = worldRow.Id;
                Name = worldRow.Name;
                StageBegin = worldRow.StageBegin;
                StageEnd = worldRow.StageEnd;
                UnlockedBlockIndex = unlockedBlockIndex;
                StageClearedBlockIndex = stageClearedBlockIndex;
                StageClearedId = stageClearedId;
            }

            public World(World world, long unlockedBlockIndex = -1)
            {
                Id = world.Id;
                Name = world.Name;
                StageBegin = world.StageBegin;
                StageEnd = world.StageEnd;
                UnlockedBlockIndex = unlockedBlockIndex;
                StageClearedBlockIndex = world.StageClearedBlockIndex;
                StageClearedId = world.StageClearedId;
            }

            public World(World world, long stageClearedBlockIndex, int stageClearedId)
            {
                Id = world.Id;
                Name = world.Name;
                StageBegin = world.StageBegin;
                StageEnd = world.StageEnd;
                UnlockedBlockIndex = world.UnlockedBlockIndex;
                StageClearedBlockIndex = stageClearedBlockIndex;
                StageClearedId = stageClearedId;
            }

            public bool ContainsStageId(int stageId)
            {
                return stageId >= StageBegin &&
                       stageId <= StageEnd;
            }

            public bool IsPlayable(int stageId)
            {
                return stageId <= GetNextStageIdForPlay();
            }

            public int GetNextStageIdForPlay()
            {
                if (!IsUnlocked)
                    return -1;

                return GetNextStageId();
            }

            public int GetNextStageId()
            {
                return IsStageCleared ? Math.Min(StageEnd, StageClearedId + 1) : StageBegin;
            }
        }

        private Dictionary<int, World> _worlds;

        public WorldInformation(
            long blockIndex,
            WorldSheet worldSheet,
            bool openAllOfWorldsAndStages = false)
        {
            if (worldSheet is null)
            {
                return;
            }

            var orderedSheet = worldSheet.OrderedList;
            _worlds = new Dictionary<int, World>();

            if (openAllOfWorldsAndStages)
            {
                foreach (var row in orderedSheet)
                {
                    _worlds.Add(row.Id, new World(row, blockIndex, blockIndex, row.StageEnd));
                }
            }
            else
            {
                var isFirst = true;
                foreach (var row in orderedSheet)
                {
                    var worldId = row.Id;
                    if (isFirst)
                    {
                        isFirst = false;
                        _worlds.Add(worldId, new World(row, blockIndex));
                    }
                    else
                    {
                        _worlds.Add(worldId, new World(row));
                    }
                }
            }
        }

        public WorldInformation(long blockIndex, WorldSheet worldSheet, int clearStageId = 0)
        {
            if (worldSheet is null)
            {
                return;
            }

            var orderedSheet = worldSheet.OrderedList;
            _worlds = new Dictionary<int, World>();

            if (clearStageId > 0)
            {
                foreach (var row in orderedSheet)
                {
                    if (row.StageBegin > clearStageId)
                    {
                        _worlds.Add(row.Id, new World(row));
                    }
                    else if (row.StageEnd > clearStageId)
                    {
                        _worlds.Add(row.Id, new World(row, blockIndex, blockIndex, clearStageId));
                    }
                    else
                    {
                        _worlds.Add(row.Id, new World(row, blockIndex, blockIndex, row.StageEnd));
                    }
                }
            }
            else
            {
                var isFirst = true;
                foreach (var row in orderedSheet)
                {
                    var worldId = row.Id;
                    if (isFirst)
                    {
                        isFirst = false;
                        _worlds.Add(worldId, new World(row, blockIndex));
                    }
                    else
                    {
                        _worlds.Add(worldId, new World(row));
                    }
                }
            }
        }

        public bool IsStageCleared(int stageId)
        {
            return false;
        }
    }
}
