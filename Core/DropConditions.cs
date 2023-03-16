using Terraria;
using Terraria.ID;
using Terraria.GameContent.ItemDropRules;

using Spooky.Content.Biomes;

namespace Spooky.Core
{
    public class DropConditions
    {
        public class CatacombKey2Condition : IItemDropRuleCondition
        {
            public bool CanDrop(DropAttemptInfo info) 
            {
                if (!info.IsInSimulation) 
                {
                    NPC npc = info.npc;

                    if (!Main.hardMode && npc.type == NPCID.WallofFlesh)
                    {
                        return true;
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
                return "Drops from Wall of Flesh";
            }
        }

        public class CatacombKey3Condition : IItemDropRuleCondition
        {
            public bool CanDrop(DropAttemptInfo info) 
            {
                if (!info.IsInSimulation) 
                {
                    NPC npc = info.npc;

                    if (!NPC.downedGolemBoss && npc.type == NPCID.Golem)
                    {
                        return true;
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
                return "Drops from Golem";
            }
        }

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
                return true;
            }

            public string GetConditionDescription() 
            {
                return "Drops in the Spooky Forest in hardmode";
            }
        }

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
                return true;
            }

            public string GetConditionDescription() 
            {
                return "Drops in the Valley of Eyes in hardmode";
            }
        }

        public class PostRotGourdCondition : IItemDropRuleCondition
        {
            public bool CanDrop(DropAttemptInfo info)
            {
                if (!info.IsInSimulation)
                {
                    NPC npc = info.npc;

                    if (Flags.downedRotGourd)
                    {
                        return true;
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
                return "Drops after Rot Gourd is defeated";
            }
        }

        public class PostSpookySpiritCondition : IItemDropRuleCondition
        {
            public bool CanDrop(DropAttemptInfo info)
            {
                if (!info.IsInSimulation)
                {
                    NPC npc = info.npc;

                    if (Flags.downedSpookySpirit)
                    {
                        return true;
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
                return "Drops after the Spooky Spirit is defeated";
            }
        }

        public class PostOrroboroCondition : IItemDropRuleCondition
        {
            public bool CanDrop(DropAttemptInfo info) 
            {
                if (!info.IsInSimulation) 
                {
                    NPC npc = info.npc;

                    if (Flags.downedOrroboro)
                    {
                        return true;
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
                return "Drops after Orro & Boro have been defeated";
            }
        }
    }
}