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
        //stuff for entity drops
        //spooky forest chest key condition
        public class SpookyForestEntityCondition : IItemDropRuleCondition
        {
            public bool CanDrop(DropAttemptInfo info) 
            {
                if (!info.IsInSimulation) 
                {
                    NPC npc = info.npc;

                    if (npc.value > 0 && info.player.InModBiome<SpookyBiome>() && !Flags.encounteredMan)
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

        //cemetery condition
        public class SwampyCemeteryEntityCondition : IItemDropRuleCondition
        {
            public bool CanDrop(DropAttemptInfo info) 
            {
                if (!info.IsInSimulation) 
                {
                    NPC npc = info.npc;

                    if (npc.value > 0 && info.player.InModBiome<CemeteryBiome>() && !Flags.encounteredHorse)
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

        //catacomb condition
        public class CatacombEntityCondition : IItemDropRuleCondition
        {
            public bool CanDrop(DropAttemptInfo info) 
            {
                if (!info.IsInSimulation) 
                {
                    NPC npc = info.npc;

                    if (npc.value > 0 && (info.player.InModBiome<CatacombBiome>() || info.player.InModBiome<CatacombBiome2>()) && !Flags.encounteredBaby)
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

        //eye valley condition
        public class EyeValleyEntityCondition : IItemDropRuleCondition
        {
            public bool CanDrop(DropAttemptInfo info) 
            {
                if (!info.IsInSimulation) 
                {
                    NPC npc = info.npc;

                    if (npc.value > 0 && info.player.InModBiome<SpookyHellBiome>() && !Flags.encounteredFlesh)
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

        //drop from daffodil if she hasnt been defeated yet
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

        //TODO: probably will change this because golem dropping the key is a bit lame
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
                return true;
            }
            
            public string GetConditionDescription()
            {
                return Description.Value;
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