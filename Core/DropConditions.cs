using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;

using Spooky.Content.Biomes;
using Spooky.Content.NPCs.Boss.Orroboro;

namespace Spooky.Core
{
    public class DropConditions
    {
        public class RedCatacombKeyCondition : IItemDropRuleCondition
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
                return "Mods.Spooky.DropConditions.RedCatacombKeyCondition";
            }
        }

        public class OrangeCatacombKeyCondition : IItemDropRuleCondition
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
                return "Mods.Spooky.DropConditions.OrangeCatacombKeyCondition";
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
                return "Mods.Spooky.DropConditions.SpookyKeyCondition";
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
                return "Mods.Spooky.DropConditions.SpookyHellKeyCondition";
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

            //this will never show up in ui
            public bool CanShowItemDropInUI()
            {
                return false;
            }

            public string GetConditionDescription()
            {
                return "";
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

            //this will never show up in ui
            public bool CanShowItemDropInUI()
            {
                return false;
            }

            public string GetConditionDescription()
            {
                return "";
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
                return "Mods.Spooky.DropConditions.PostOrroboroCondition";
            }
        }

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
                return "";
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
                return "";
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
                return "";
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
                return "";
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
                return "";
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
                return "";
            }
        }
    }
}