using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.GameContent.ItemDropRules;
using System.Collections.Generic;

using Spooky.Content.Biomes;
using Spooky.Content.NPCs.Boss.Daffodil;
using Spooky.Content.NPCs.Boss.Orroboro;
using Spooky.Content.NPCs.Boss.SpookySpirit;

namespace Spooky.Core
{
    public class DropConditions
    {
        //underworld condition workaround so eye valley enemies dont drop living fire blocks or the hel-fire
        public class UnderworldDropCondition : IItemDropRuleCondition
        {
            public bool CanDrop(DropAttemptInfo info)
            {
                if (!info.IsInSimulation) 
                {
                    if (info.player.ZoneUnderworldHeight && !info.player.InModBiome<SpookyHellBiome>() && Main.hardMode)
                    {
                        return true;
                    }
                }
                
                return false;
            }

            public bool CanShowItemDropInUI() 
            {
                return false;
            }

            public string GetConditionDescription() 
            {
                return null;
            }
        }
    
        public class UnderworldCascadeDropCondition : IItemDropRuleCondition
        {
            public bool CanDrop(DropAttemptInfo info)
            {
                if (!info.IsInSimulation) 
                {
                    if (info.player.ZoneUnderworldHeight && !info.player.InModBiome<SpookyHellBiome>() && NPC.downedBoss3 && !Main.hardMode)
                    {
                        return true;
                    }
                }
                
                return false;
            }

            public bool CanShowItemDropInUI() 
            {
                return false;
            }

            public string GetConditionDescription() 
            {
                return null;
            }
        }

        //drop from spooky spirit if it hasnt been defeated yet
        public class YellowCatacombKeyCondition : IItemDropRuleCondition
        {
            public bool CanDrop(DropAttemptInfo info) 
            {
                if (!info.IsInSimulation) 
                {
                    NPC npc = info.npc;

                    if (!Flags.CatacombKey1 && npc.type == ModContent.NPCType<SpookySpirit>())
                    {
                        return true;
                    }
                }
                
                return false;
            }

            public bool CanShowItemDropInUI() 
            {
                return false;
            }

            public string GetConditionDescription() 
            {
                return null;
            }
        }

        //drop from daffodil if she hasnt been defeated yet
        public class RedCatacombKeyCondition : IItemDropRuleCondition
        {
            public bool CanDrop(DropAttemptInfo info) 
            {
                if (!info.IsInSimulation) 
                {
                    NPC npc = info.npc;

                    if (!Flags.CatacombKey2 && npc.type == ModContent.NPCType<DaffodilEye>())
                    {
                        return true;
                    }
                }
                
                return false;
            }

            public bool CanShowItemDropInUI() 
            {
                return false;
            }

            public string GetConditionDescription() 
            {
                return null;
            }
        }

        //TODO: probably will change this because golem dropping the key is a bit lame
        public class OrangeCatacombKeyCondition : IItemDropRuleCondition
        {
            public bool CanDrop(DropAttemptInfo info) 
            {
                if (!info.IsInSimulation) 
                {
                    NPC npc = info.npc;

                    if (!Flags.CatacombKey3 && npc.type == NPCID.Golem)
                    {
                        return true;
                    }
                }
                
                return false;
            }

            public bool CanShowItemDropInUI() 
            {
                return false;
            }

            public string GetConditionDescription() 
            {
                return null;
            }
        }

        //spooky forest chest key condition
        public class SpookyKeyCondition : IItemDropRuleCondition
        {
            public bool CanDrop(DropAttemptInfo info) 
            {
                if (!info.IsInSimulation) 
                {
                    NPC npc = info.npc;

                    if (Main.hardMode && npc.value > 0 && info.player.InModBiome<SpookyBiome>())
                    {
                        return true;
                    }
                }
                
                return false;
            }

            public bool CanShowItemDropInUI() 
            {
                return false;
            }

            public string GetConditionDescription() 
            {
                return null;
            }
        }

        //swampy cemetery chest key condition
        public class CemeteryKeyCondition : IItemDropRuleCondition
        {
            public bool CanDrop(DropAttemptInfo info) 
            {
                if (!info.IsInSimulation) 
                {
                    NPC npc = info.npc;

                    if (Main.hardMode && npc.value > 0 && info.player.InModBiome<CemeteryBiome>())
                    {
                        return true;
                    }
                }
                
                return false;
            }

            public bool CanShowItemDropInUI() 
            {
                return false;
            }

            public string GetConditionDescription() 
            {
                return null;
            }
        }

        //spider grotto chest key condition
        public class SpiderKeyCondition : IItemDropRuleCondition
        {
            public bool CanDrop(DropAttemptInfo info) 
            {
                if (!info.IsInSimulation) 
                {
                    NPC npc = info.npc;

                    if (Main.hardMode && npc.value > 0 && info.player.InModBiome<SpiderCaveBiome>())
                    {
                        return true;
                    }
                }
                
                return false;
            }

            public bool CanShowItemDropInUI() 
            {
                return false;
            }

            public string GetConditionDescription() 
            {
                return null;
            }
        }

        //valley of eyes chest key condition
        public class SpookyHellKeyCondition : IItemDropRuleCondition
        {
            public bool CanDrop(DropAttemptInfo info) 
            {
                if (!info.IsInSimulation) 
                {
                    NPC npc = info.npc;

                    if (Main.hardMode && npc.value > 0 && info.player.InModBiome<SpookyHellBiome>())
                    {
                        return true;
                    }
                }
                
                return false;
            }

            public bool CanShowItemDropInUI() 
            {
                return false;
            }

            public string GetConditionDescription() 
            {
                return null;
            }
        }

        //drops after moco has been defeated
        public class PostMocoCondition : IItemDropRuleCondition
        {
            private static LocalizedText Description = Language.GetOrRegister("Mods.Spooky.DropConditions.PostMocoCondition");

            public bool CanDrop(DropAttemptInfo info) 
            {
                if (!info.IsInSimulation) 
                {
                    NPC npc = info.npc;

                    if (Flags.downedMoco)
                    {
                        return true;
                    }
                }
                
                return false;
            }

            public bool CanShowItemDropInUI() 
            {
                return Flags.downedMoco;
            }

            public string GetConditionDescription() 
            {
                return Description.Value;
            }
        }

        //drops after orro & boro have been defeated
        public class PostOrroboroCondition : IItemDropRuleCondition
        {
            private static LocalizedText Description = Language.GetOrRegister("Mods.Spooky.DropConditions.PostOrroboroCondition");

            public bool CanDrop(DropAttemptInfo info) 
            {
                if (!info.IsInSimulation) 
                {
                    if (Flags.downedOrroboro)
                    {
                        return true;
                    }
                }
                
                return false;
            }

            public bool CanShowItemDropInUI() 
            {
                return Flags.downedOrroboro;
            }
            
            public string GetConditionDescription()
            {
                return Description.Value;
            }
        }

        //special condition for the spider war, if the current spider war wave is below 10 then have a 10% chance to drop items, and at wave 10 guarantee drops
        public class SpiderWarItemDropCondition : IItemDropRuleCondition
        {
            public bool CanDrop(DropAttemptInfo info)
            {
                if (!info.IsInSimulation) 
                {
                    if (SpiderWarWorld.SpiderWarActive)
                    {
                        if (SpiderWarWorld.SpiderWarWave >= 10)
                        {
                            return true;
                        }
                        else
                        {
                            return Main.rand.NextBool(10);
                        }
                    }
                }
                
                return false;
            }

            public bool CanShowItemDropInUI() 
            {
                return true;
            }

            public string GetConditionDescription() 
            {
                return null;
            }
        }

        public class OneFromOptionsForSpiderWar : IItemDropRule
		{
			public List<IItemDropRuleChainAttempt> ChainedRules { get; }

			private int chanceDenominator;
			private int chanceNumerator;
			private (int itemID, int minAmount, int maxAmount)[] itemData;

			public OneFromOptionsForSpiderWar(int chanceDenominator = 1, int chanceNumerator = 1, params (int itemID, int minAmount, int maxAmount)[] itemData)
			{
				this.chanceDenominator = chanceDenominator;
				this.chanceNumerator = chanceNumerator;
				this.itemData = itemData;

				ChainedRules = new List<IItemDropRuleChainAttempt>();
			}

			public bool CanDrop(DropAttemptInfo info)
			{
				if (!info.IsInSimulation)
				{
					if (SpiderWarWorld.SpiderWarActive)
					{
						if (SpiderWarWorld.SpiderWarWave >= 10)
						{
							return true;
						}
						else
						{
							return Main.rand.NextBool(10);
						}
					}
				}

				return false;
			}

			public void ReportDroprates(List<DropRateInfo> drops, DropRateInfoChainFeed ratesInfo)
			{
				float num = chanceNumerator / (float)chanceDenominator;
				float num2 = num * ratesInfo.parentDroprateChance;
				float dropRate = 1f / itemData.Length * num2;
				for (int i = 0; i < itemData.Length; i++)
				{
					drops.Add(new DropRateInfo(itemData[i].itemID, itemData[i].minAmount, itemData[i].maxAmount, dropRate, ratesInfo.conditions));
				}
				Chains.ReportDroprates(ChainedRules, num, drops, ratesInfo);
			}

			public ItemDropAttemptResult TryDroppingItem(DropAttemptInfo info)
			{
				ItemDropAttemptResult result;
				if (info.rng.Next(chanceDenominator) < chanceNumerator)
				{
					int i = info.rng.Next(itemData.Length);
					CommonCode.DropItem(info, itemData[i].itemID, Main.rand.Next(itemData[i].minAmount, itemData[i].maxAmount + 1));
					result = default(ItemDropAttemptResult);
					result.State = ItemDropAttemptResultState.Success;
					return result;
				}
				result = default(ItemDropAttemptResult);
				result.State = ItemDropAttemptResultState.FailedRandomRoll;
				return result;
			}
		}


        //all the conditions below this point exist because with orro & boro, only the last worm alive should drop items
        //item drop conditions was basically the only way i could get it working (at least that i could think of)

        //for orro's non expert drops
        public class ShouldOrroDropLoot : IItemDropRuleCondition
        {
            public bool CanDrop(DropAttemptInfo info)
            {
                if (!info.IsInSimulation)
                {
                    if (!NPC.AnyNPCs(ModContent.NPCType<BoroHead>()) && !Main.expertMode && !Main.masterMode)
                    {
                        return true;
                    }
                }

                return false;
            }

            public bool CanShowItemDropInUI()
            {
                return !Main.expertMode && !Main.masterMode;
            }

            public string GetConditionDescription()
            {
                return null;
            }
        }

        //for orro's expert drops
        public class ShouldOrroDropLootExpert : IItemDropRuleCondition
        {
            public bool CanDrop(DropAttemptInfo info)
            {
                if (!info.IsInSimulation)
                {
                    if (!NPC.AnyNPCs(ModContent.NPCType<BoroHead>()) && Main.expertMode)
                    {
                        return true;
                    }
                }

                return false;
            }

            public bool CanShowItemDropInUI()
            {
                return Main.expertMode;
            }

            public string GetConditionDescription()
            {
                return null;
            }
        }

        //for orro's master drops
        public class ShouldOrroDropLootMaster : IItemDropRuleCondition
        {
            public bool CanDrop(DropAttemptInfo info)
            {
                if (!info.IsInSimulation)
                {
                    if (!NPC.AnyNPCs(ModContent.NPCType<BoroHead>()) && Main.masterMode)
                    {
                        return true;
                    }
                }

                return false;
            }

            public bool CanShowItemDropInUI()
            {
                return Main.masterMode;
            }

            public string GetConditionDescription()
            {
                return null;
            }
        }

        //for boro's non expert drops
        public class ShouldBoroDropLoot : IItemDropRuleCondition
        {
            public bool CanDrop(DropAttemptInfo info)
            {
                if (!info.IsInSimulation)
                {
                    if (!NPC.AnyNPCs(ModContent.NPCType<OrroHead>()) && !Main.expertMode && !Main.masterMode)
                    {
                        return true;
                    }
                }

                return false;
            }

            public bool CanShowItemDropInUI()
            {
                return !Main.expertMode && !Main.masterMode;
            }

            public string GetConditionDescription()
            {
                return null;
            }
        }

        //for boro's expert drops
        public class ShouldBoroDropLootExpert : IItemDropRuleCondition
        {
            public bool CanDrop(DropAttemptInfo info)
            {
                if (!info.IsInSimulation)
                {
                    if (!NPC.AnyNPCs(ModContent.NPCType<OrroHead>()) && Main.expertMode)
                    {
                        return true;
                    }
                }

                return false;
            }

            public bool CanShowItemDropInUI()
            {
                return Main.expertMode;
            }

            public string GetConditionDescription()
            {
                return null;
            }
        }

        //for boro's master drops
        public class ShouldBoroDropLootMaster : IItemDropRuleCondition
        {
            public bool CanDrop(DropAttemptInfo info)
            {
                if (!info.IsInSimulation)
                {
                    if (!NPC.AnyNPCs(ModContent.NPCType<OrroHead>()) && Main.masterMode)
                    {
                        return true;
                    }
                }

                return false;
            }

            public bool CanShowItemDropInUI()
            {
                return Main.masterMode;
            }

            public string GetConditionDescription()
            {
                return null;
            }
        }
    }
}