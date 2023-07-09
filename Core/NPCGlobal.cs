using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using System.Collections.Generic;

using Spooky.Content.Biomes;
using Spooky.Content.Buffs;
using Spooky.Content.Buffs.Debuff;
using Spooky.Content.Items.BossBags.Accessory;
using Spooky.Content.Items.Catacomb.Misc;
using Spooky.Content.Items.Cemetery.Misc;
using Spooky.Content.Items.Pets;
using Spooky.Content.Items.SpookyBiome;
using Spooky.Content.Items.SpookyBiome.Misc;
using Spooky.Content.Items.SpookyHell.Misc;
using Spooky.Content.NPCs.Boss.BigBone;
using Spooky.Content.NPCs.Boss.Daffodil;
using Spooky.Content.NPCs.Boss.Moco;
using Spooky.Content.NPCs.Boss.Orroboro;
using Spooky.Content.NPCs.Boss.RotGourd;
using Spooky.Content.NPCs.Boss.SpookySpirit;
using Spooky.Content.Projectiles.SpookyBiome;

namespace Spooky.Core
{
    public class NPCGlobal : GlobalNPC
    {
        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
			//draw purple aura around any enemy with the egg event enemy buff
			//this will never actually be applied on any enemy in game, besides the egg event enemies
            if (npc.HasBuff(ModContent.BuffType<EggEventEnemyBuff>()))
            {
				Texture2D tex = Terraria.GameContent.TextureAssets.Npc[npc.type].Value;

				Color color = new Color(127 - npc.alpha, 127 - npc.alpha, 127 - npc.alpha, 0).MultiplyRGBA(Color.Purple);

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
			//remove spawnrates during the egg event
			if (player.InModBiome(ModContent.GetInstance<EggEventBiome>()))
            {
				spawnRate = 0;
				maxSpawns = 0;
			}
		}

		public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
		{
            //disable all spawns when any spooky mod boss is alive
            if (NPC.AnyNPCs(ModContent.NPCType<RotGourd>()) || NPC.AnyNPCs(ModContent.NPCType<SpookySpirit>()) || 
			NPC.AnyNPCs(ModContent.NPCType<Moco>()) || NPC.AnyNPCs(ModContent.NPCType<DaffodilEye>()) || NPC.AnyNPCs(ModContent.NPCType<BigBone>()) || 
			NPC.AnyNPCs(ModContent.NPCType<OrroHeadP1>()) || NPC.AnyNPCs(ModContent.NPCType<OrroHead>()) || NPC.AnyNPCs(ModContent.NPCType<BoroHead>()))
            {
                pool.Clear();
            }

            //disable spawns during the entity encounter
            if (spawnInfo.Player.HasBuff(ModContent.BuffType<EntityDebuff>()))
			{
				pool.Clear();
			}

			//remove all hell enemies from the spawn pool while in the eye valley
			if (spawnInfo.Player.InModBiome(ModContent.GetInstance<SpookyHellBiome>()) || spawnInfo.Player.InModBiome(ModContent.GetInstance<EggEventBiome>()))
			{
				pool.Remove(NPCID.Hellbat);
				pool.Remove(NPCID.Lavabat);
                pool.Remove(NPCID.LavaSlime);
                pool.Remove(NPCID.FireImp);
                pool.Remove(NPCID.Demon);
                pool.Remove(NPCID.VoodooDemon);
				pool.Remove(NPCID.RedDevil);
                pool.Remove(NPCID.BoneSerpentHead);
                pool.Remove(NPCID.BoneSerpentBody);
                pool.Remove(NPCID.BoneSerpentTail);
                pool.Remove(NPCID.DemonTaxCollector);
			}
		}

		public override void ModifyShop(NPCShop shop)
		{
			//merchant sells mossy pebbles for the slingshots
			if (shop.NpcType == NPCID.Merchant)
			{
				shop.Add<MossyPebble>();
			}

			//add spooky mod's biome solutions to the steampunker shop
			if (shop.NpcType == NPCID.Steampunker)
			{
				shop.Add<SpookySolution>();
				shop.Add<CemeterySolution>();
			}
		}

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            //list of every orro & boro segment
            int[] OrroBoroSegments = { ModContent.NPCType<OrroHeadP1>(), ModContent.NPCType<OrroHead>(), ModContent.NPCType<BoroHead>(),
			ModContent.NPCType<OrroBodyP1>(), ModContent.NPCType<OrroBody>(), ModContent.NPCType<BoroBodyP1>(), ModContent.NPCType<BoroBody>(),
			ModContent.NPCType<BoroBodyConnect>(), ModContent.NPCType<OrroTail>(), ModContent.NPCType<BoroTailP1>(), ModContent.NPCType<BoroTail>() };

			//give all of orro & boro segments resistance to piercing projectiles
            if (OrroBoroSegments.Contains(npc.type))
			{
                if (projectile.penetrate <= -1 || projectile.penetrate >= 2)
                {
                    float damageDivide = 1.8f;
                    modifiers.FinalDamage /= (int)damageDivide;
                }
            }
        }

        public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
			//moco expert item booger drop
			if (Main.LocalPlayer.GetModPlayer<SpookyPlayer>().MocoNose && Main.LocalPlayer.GetModPlayer<SpookyPlayer>().MocoBoogerCharge < 15 &&
			!Main.LocalPlayer.HasBuff(ModContent.BuffType<BoogerFrenzyCooldown>()))
			{
				if (Main.rand.NextBool(25))
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

        public override void OnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
			//moco expert item booger drop
			//copied from above because npcs getting hit by items and projectiles is handled separately by tmodloader now
			if (Main.LocalPlayer.GetModPlayer<SpookyPlayer>().MocoNose && Main.LocalPlayer.GetModPlayer<SpookyPlayer>().MocoBoogerCharge < 15 &&
			!Main.LocalPlayer.HasBuff(ModContent.BuffType<BoogerFrenzyCooldown>()))
			{
				if (Main.rand.NextBool(25))
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

			//inflict enemies with gourd decay while wearing the rotten gourd armor
			if (player.GetModPlayer<SpookyPlayer>().GourdSet && item.DamageType == DamageClass.Melee)
			{
				if (Main.rand.NextBool(12))
				{
					npc.AddBuff(ModContent.BuffType<GourdDecay>(), 3600);
				}
			}
		}

        public override void ModifyGlobalLoot(GlobalLoot globalLoot) 
        {
			//drop skull goop during a blood moon if you are in the spooky biome
			globalLoot.Add(ItemDropRule.ByCondition(new DropConditions.SkullGoopPetCondition(), ModContent.ItemType<DissolvedBone>(), 100));

            //make enemies drop spooky mod's biome keys, with a 1 in 2500 chance like vanilla's biome keys
            globalLoot.Add(ItemDropRule.ByCondition(new DropConditions.SpookyKeyCondition(), ModContent.ItemType<SpookyBiomeKey>(), 2500));
            globalLoot.Add(ItemDropRule.ByCondition(new DropConditions.SpookyHellKeyCondition(), ModContent.ItemType<SpookyHellKey>(), 2500));

            //make certain bosses drop the catacomb barrier keys
			globalLoot.Add(ItemDropRule.ByCondition(new DropConditions.YellowCatacombKeyCondition(), ModContent.ItemType<CatacombKey1>(), 1));
            globalLoot.Add(ItemDropRule.ByCondition(new DropConditions.RedCatacombKeyCondition(), ModContent.ItemType<CatacombKey2>(), 1));
            globalLoot.Add(ItemDropRule.ByCondition(new DropConditions.OrangeCatacombKeyCondition(), ModContent.ItemType<CatacombKey3>(), 1));
        }
    }
}