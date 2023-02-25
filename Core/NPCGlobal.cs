using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

using Spooky.Content.Biomes;
using Spooky.Content.Buffs;
using Spooky.Content.Buffs.Debuff;
using Spooky.Content.Items.BossBags.Accessory;
using Spooky.Content.Items.Catacomb.Key;
using Spooky.Content.Items.SpookyBiome.Misc;
using Spooky.Content.Items.SpookyHell.Misc;
using Spooky.Content.NPCs.Boss.BigBone;
using Spooky.Content.NPCs.Boss.Moco;
using Spooky.Content.NPCs.Boss.RotGourd;

namespace Spooky.Core
{
    public class NPCGlobal : GlobalNPC
    {
        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
			//draw red aura around any enemy with the egg event enemy buff
			//this will never actually be applied on any enemy in game, besides egg event enemies
            if (npc.HasBuff(ModContent.BuffType<EggEventEnemyBuff>()))
            {
				Texture2D tex = Terraria.GameContent.TextureAssets.Npc[npc.type].Value;

				Color color = new Color(127 - npc.alpha, 127 - npc.alpha, 127 - npc.alpha, 0).MultiplyRGBA(Color.Red);

				var effects = npc.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

				for (int numEffect = 0; numEffect < 5; numEffect++)
				{
					Color newColor = color;
					newColor = npc.GetAlpha(newColor);
					newColor *= 1f;
					Vector2 vector = new Vector2(npc.Center.X, npc.Center.Y) + (npc.rotation).ToRotationVector2() - Main.screenPosition + new Vector2(0, npc.gfxOffY + 2);
					Main.EntitySpriteDraw(tex, vector, npc.frame, newColor, npc.rotation, npc.frame.Size() / 2f, npc.scale * 1.25f, effects, 0);
				}
			}

			return true;
        }

        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
		{
			//increase spawnrates and max spawns during the egg event
			if (player.InModBiome(ModContent.GetInstance<EggEventBiome>()))
            {
				spawnRate = (int)(spawnRate / 2f);
				maxSpawns = (int)(maxSpawns * 2f);
			}
		}

		public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
		{
			//disable spawns during the entity encounter
			if (Main.LocalPlayer.HasBuff(ModContent.BuffType<EntityDebuff>()))
			{
				pool.Clear();
			}

			//disable spawns when any spooky mod boss is alive
			if (NPC.AnyNPCs(ModContent.NPCType<RotGourd>()) || NPC.AnyNPCs(ModContent.NPCType<Moco>()) || NPC.AnyNPCs(ModContent.NPCType<BigBone>()))
			{
				pool.Clear();
			}
		}

		public override void SetupShop(int type, Chest shop, ref int nextSlot)
		{
			//steampunker sells spooky clentaminator solution
			if (type == NPCID.Steampunker)
			{
				shop.item[nextSlot].SetDefaults(ModContent.ItemType<SpookySolution>());
				nextSlot++;
			}
		}

        public override void OnHitByProjectile(NPC npc, Projectile projectile, int damage, float knockback, bool crit)
        {
			if (Main.LocalPlayer.GetModPlayer<SpookyPlayer>().MocoNose && Main.LocalPlayer.GetModPlayer<SpookyPlayer>().MocoBoogerCharge < 15 &&
			!Main.LocalPlayer.HasBuff(ModContent.BuffType<BoogerFrenzyCooldown>()))
			{
				if (Main.rand.Next(25) == 0)
				{
					int itemType = ModContent.ItemType<MocoNoseBooger>();
					int newItem = Item.NewItem(npc.GetSource_OnHit(npc), npc.Hitbox, itemType);
					Main.item[newItem].noGrabDelay = 0;

					if (Main.netMode == NetmodeID.MultiplayerClient && newItem >= 0)
					{
						NetMessage.SendData(MessageID.SyncItem, -1, -1, null, newItem, 1f);
					}
				}
			}
		}

        public override void OnHitByItem(NPC npc, Player player, Item item, int damage, float knockback, bool crit)
        {
			if (Main.LocalPlayer.GetModPlayer<SpookyPlayer>().MocoNose && Main.LocalPlayer.GetModPlayer<SpookyPlayer>().MocoBoogerCharge < 15 &&
			!Main.LocalPlayer.HasBuff(ModContent.BuffType<BoogerFrenzyCooldown>()))
			{
				if (Main.rand.Next(25) == 0)
				{
					int itemType = ModContent.ItemType<MocoNoseBooger>();
					int newItem = Item.NewItem(npc.GetSource_OnHit(npc), npc.Hitbox, itemType);
					Main.item[newItem].noGrabDelay = 0;

					if (Main.netMode == NetmodeID.MultiplayerClient && newItem >= 0)
					{
						NetMessage.SendData(MessageID.SyncItem, -1, -1, null, newItem, 1f);
					}
				}
			}
		}

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
    }

    public class CatacombKey1Condition : IItemDropRuleCondition
	{
		public bool CanDrop(DropAttemptInfo info) 
        {
			if (!info.IsInSimulation) 
            {
				NPC npc = info.npc;

				if (!Flags.CatacombKey1 && (npc.type == NPCID.BrainofCthulhu || 
				((npc.type == NPCID.EaterofWorldsHead || npc.type == NPCID.EaterofWorldsBody || npc.type == NPCID.EaterofWorldsTail) && npc.boss)))
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

				if (!Flags.CatacombKey2 && npc.type == NPCID.WallofFlesh)
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

				if (!Flags.CatacombKey3 && npc.type == NPCID.Golem)
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