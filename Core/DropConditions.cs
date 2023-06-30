using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.GameContent.ItemDropRules;

using Spooky.Content.Biomes;
using Spooky.Content.NPCs.Boss.Daffodil;
using Spooky.Content.NPCs.Boss.Moco;
using Spooky.Content.NPCs.Boss.Orroboro;
using Spooky.Content.NPCs.Boss.SpookySpirit;
using System;

namespace Spooky.Core
{
    public class DropConditions
    {
        public class SkullGoopPetCondition : IItemDropRuleCondition
        {
            public bool CanDrop(DropAttemptInfo info)
            {
                if (!info.IsInSimulation) 
                {
                    if (Main.bloodMoon && info.player.InModBiome<SpookyBiome>())
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

        public class YellowCatacombKeyCondition : IItemDropRuleCondition
        {
            public bool CanDrop(DropAttemptInfo info) 
            {
                if (!info.IsInSimulation) 
                {
                    NPC npc = info.npc;

                    if (!Flags.downedSpookySpirit && npc.type == ModContent.NPCType<SpookySpirit>())
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

        public class RedCatacombKeyCondition : IItemDropRuleCondition
        {
            public bool CanDrop(DropAttemptInfo info) 
            {
                if (!info.IsInSimulation) 
                {
                    NPC npc = info.npc;

                    if (!Flags.downedDaffodil && npc.type == ModContent.NPCType<DaffodilEye>())
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
                return false;
            }

            public string GetConditionDescription() 
            {
                return null;
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
                return false;
            }

            public string GetConditionDescription() 
            {
                return null;
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
                return false;
            }

            public string GetConditionDescription() 
            {
                return null;
            }
        }

        public class PostRotGourdCondition : IItemDropRuleCondition
        {
            public bool CanDrop(DropAttemptInfo info)
            {
                if (!info.IsInSimulation)
                {
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
                return null;
            }
        }

        public class PostSpookySpiritCondition : IItemDropRuleCondition
        {
            public bool CanDrop(DropAttemptInfo info)
            {
                if (!info.IsInSimulation)
                {
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
                return null;
            }
        }

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
                return true;
            }
            
            public string GetConditionDescription()
            {
                return Description.Value;
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