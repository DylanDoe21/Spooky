using Terraria;
using Terraria.ModLoader;
using Terraria.Achievements;
using Terraria.GameContent.Achievements;
using System;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.NPCs.Boss.BigBone;
using Spooky.Content.NPCs.Boss.SpookFishron;
using Spooky.Content.NPCs.Boss.RotGourd;

namespace Spooky.Content.Achievements
{
	public class CustomConditionTracker : ConditionFloatTracker
	{
		private Dictionary<AchievementCondition, float> weightedConditions;

		public CustomConditionTracker(Dictionary<AchievementCondition, float> weightedConditions)
		{
			this.weightedConditions = weightedConditions;
			foreach ((AchievementCondition condition, float weight) in weightedConditions)
			{
				_maxValue += weight;
				condition.OnComplete += OnConditionCompleted;
			}
		}

		private void OnConditionCompleted(AchievementCondition condition)
		{
			SetValue(Math.Min(_value + weightedConditions[condition], _maxValue));
		}

		protected override void Load()
		{
			foreach ((AchievementCondition condition, float weight) in weightedConditions)
			{
				if (condition.IsCompleted)
					_value += weight;
			}
		}
	}
}