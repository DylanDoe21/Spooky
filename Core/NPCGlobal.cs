using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

using Spooky.Content.Biomes;
using Spooky.Content.Buffs.Debuff;
using Spooky.Content.Items.Catacomb.Key;
using Spooky.Content.Items.SpookyBiome.Misc;
using Spooky.Content.Items.SpookyHell.Misc;
using Spooky.Content.Projectiles.SpookyBiome;
using Spooky.Content.NPCs.Boss.Orroboro.Phase2;

namespace Spooky.Core
{
    public class NPCGlobal : GlobalNPC
    {
        public override void ModifyGlobalLoot(GlobalLoot globalLoot) 
        {
            //make hardmode enemies drop the biome keys, 1/2500 chance like vanilla
            globalLoot.Add(ItemDropRule.ByCondition(new SpookyKeyCondition(), ModContent.ItemType<SpookyBiomeKey>(), 2500));
            globalLoot.Add(ItemDropRule.ByCondition(new SpookyHellKeyCondition(), ModContent.ItemType<SpookyHellKey>(), 2500));

            //catacomb keys
            globalLoot.Add(ItemDropRule.ByCondition(new CatacombKey1Condition(), ModContent.ItemType<CatacombKey1>(), 1));
            globalLoot.Add(ItemDropRule.ByCondition(new CatacombKey2Condition(), ModContent.ItemType<CatacombKey2>(), 1));
            globalLoot.Add(ItemDropRule.ByCondition(new CatacombKey3Condition(), ModContent.ItemType<CatacombKey3>(), 1));
        }

        public override void OnKill(NPC npc)
        {
            if (npc.HasBuff(ModContent.BuffType<PumpkinWhipDebuff>()))
            {
                Vector2 Speed = new Vector2(3f, 0f).RotatedByRandom(2 * Math.PI);

                for (int numProjectiles = 0; numProjectiles < 3; numProjectiles++)
                {
                    Vector2 speed = Speed.RotatedBy(2 * Math.PI / 2 * (numProjectiles + Main.rand.NextDouble() - 0.5));

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, speed,
                        ModContent.ProjectileType<PumpkinWhipFly>(), 15, 0f, Main.myPlayer, 0, 0);
                    }
                }
            }
        }

        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            if (Main.LocalPlayer.HasBuff(ModContent.BuffType<EntityDebuff>()))
            {
				pool.Clear();
            }
        }
    }

    public class CatacombKey1Condition : IItemDropRuleCondition
	{
		public bool CanDrop(DropAttemptInfo info) 
        {
			if (!info.IsInSimulation) 
            {
				NPC npc = info.npc;

				if (!NPC.downedBoss1 && npc.type == NPCID.EyeofCthulhu)
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
			return "Drops from Eye of Cthulhu";
		}
	}

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

				if (Main.hardMode && !npc.friendly && !npc.boss && info.player.InModBiome<SpookyBiome>())
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
			return "Drops in 'Spooky Forest' in hardmode";
		}
	}

    public class SpookyHellKeyCondition : IItemDropRuleCondition
	{
		public bool CanDrop(DropAttemptInfo info) 
        {
			if (!info.IsInSimulation) 
            {
				NPC npc = info.npc;

				if (Main.hardMode && !npc.friendly && !npc.boss && info.player.InModBiome<SpookyHellBiome>())
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
			return "Drops in 'Valley of Eyes' in hardmode";
		}
	}
}