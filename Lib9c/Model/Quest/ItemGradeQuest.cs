using System;
using System.Collections.Generic;
using System.Globalization;
using Nekoyume.Model.Item;
using Nekoyume.TableData;

namespace Nekoyume.Model.Quest
{
    [Serializable]
    public class ItemGradeQuest : Quest
    {
        public readonly int Grade;
        public readonly List<int> ItemIds = new List<int>();
        public ItemGradeQuest(ItemGradeQuestSheet.Row data, QuestReward reward) 
            : base(data, reward)
        {
            Grade = data.Grade;
        }

        public override QuestType QuestType => QuestType.Obtain;

        public override void Check()
        {
            if (Complete)
                return;

            Complete = _current >= Goal;
        }

        // FIXME: 이 메서드 구현은 중복된 코드가 다른 데서도 많이 있는 듯.
        public override string GetProgressText() =>
            string.Format(
                CultureInfo.InvariantCulture,
                GoalFormat,
                Math.Min(Goal, _current),
                Goal
            );

        public void Update(ItemUsable itemUsable)
        {
            if (Complete)
                return;

            if (!ItemIds.Contains(itemUsable.Id))
            {
                _current++;
                ItemIds.Add(itemUsable.Id);
                ItemIds.Sort();
            }
            Check();
        }

        protected override string TypeId => "itemGradeQuest";
    }
}
