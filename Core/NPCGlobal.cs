using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;

using Spooky.Content.Buffs;
using Spooky.Content.Buffs.Debuff;
using Spooky.Content.Items.BossSummon;
using Spooky.Content.Items.Catacomb.Misc;
using Spooky.Content.Items.Cemetery.Misc;
using Spooky.Content.Items.SpiderCave.Misc;
using Spooky.Content.Items.SpookyBiome.Misc;
using Spooky.Content.Items.SpookyHell.Misc;
using Spooky.Content.NPCs.Boss.Orroboro;
using Spooky.Content.NPCs.Catacomb.Layer1;
using Spooky.Content.Projectiles.SpiderCave;

namespace Spooky.Core
{
    public class NPCGlobal : GlobalNPC
	{
		public override bool InstancePerEntity => true;

		public int KeybrandDefenseStacks = 0;
		public bool InitializedKeybrandDefense = false;

		public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            //draw white aura on an enemy with the bee damage buff
            if (npc.HasBuff(ModContent.BuffType<BeeDamageBuff>()))
            {
                Vector2 frameOrigin = npc.frame.Size() / 2f;
                Vector2 drawPos = npc.position - Main.screenPosition + frameOrigin + new Vector2(0f, npc.gfxOffY + 4);

                float fade = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.4f / 2.4f * 6f)) / 2f + 0.5f;

                float time = Main.GlobalTimeWrappedHourly;

                time %= 4f;
                time /= 2f;

                time = time * 0.5f + 0.5f;

                var effects = npc.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

                Color color = new Color(125 - npc.alpha, 125 - npc.alpha, 125 - npc.alpha, 0).MultiplyRGBA(Color.White) * 0.5f;

                for (float i = 0f; i < 1f; i += 0.25f)
                {
                    float radians = (i + (fade / 2)) * MathHelper.TwoPi;
                    spriteBatch.Draw(Terraria.GameContent.TextureAssets.Npc[npc.type].Value, drawPos + new Vector2(0f, 1f).RotatedBy(radians) * time, npc.frame, color, npc.rotation, frameOrigin, npc.scale * 1.2f, effects, 0);
                }
			}

            //draw a gold aura on enemies being healed
            if (npc.HasBuff(ModContent.BuffType<BeeHealingBuff>()))
            {
                Vector2 frameOrigin = npc.frame.Size() / 2f;
                Vector2 drawPos = npc.position - Main.screenPosition + frameOrigin + new Vector2(0f, npc.gfxOffY + 4);

                float fade = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.4f / 2.4f * 6f)) / 2f + 0.5f;

                float time = Main.GlobalTimeWrappedHourly;

                time %= 4f;
                time /= 2f;

                time = time * 0.5f + 0.5f;

                var effects = npc.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

                Color color = new Color(125 - npc.alpha, 125 - npc.alpha, 125 - npc.alpha, 0).MultiplyRGBA(Color.Gold) * 0.5f;
                
                for (float i = 0f; i < 1f; i += 0.25f)
                {
                    float radians = (i + (fade / 2)) * MathHelper.TwoPi;
                    spriteBatch.Draw(Terraria.GameContent.TextureAssets.Npc[npc.type].Value, drawPos + new Vector2(0f, 1f).RotatedBy(radians) * time, npc.frame, color, npc.rotation, frameOrigin, npc.scale * 1.2f, effects, 0);
                }
			}

			return true;
        }

		public override void ModifyShop(NPCShop shop)
		{
			//add spooky mod's biome solutions to the steampunker shop
			if (shop.NpcType == NPCID.Steampunker)
			{
				shop.Add<SpookySolution>();
				shop.Add<CemeterySolution>();
				shop.Add<SpookyHellSolution>();
			}
		}

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            //list of every orro & boro segment
            int[] OrroBoroSegments = { ModContent.NPCType<OrroHeadP1>(), ModContent.NPCType<OrroHead>(), ModContent.NPCType<BoroHead>(),
			ModContent.NPCType<OrroBodyP1>(), ModContent.NPCType<OrroBody>(), ModContent.NPCType<BoroBodyP1>(), ModContent.NPCType<BoroBody>(),
			ModContent.NPCType<BoroBodyConnect>(), ModContent.NPCType<OrroTail>(), ModContent.NPCType<BoroTailP1>(), ModContent.NPCType<BoroTail>() };

			//give all orro & boro segments resistance to piercing projectiles because terraria worm moment
            if (OrroBoroSegments.Contains(npc.type))
			{
                if (projectile.penetrate <= -1 || projectile.penetrate >= 2)
                {
                    modifiers.FinalDamage /= 1.5f;
                }
            }

            //enemies inflicted with the pheromone stinger debuff take increased damage from all spider related minions
            if (npc.HasBuff(ModContent.BuffType<PheromoneWhipDebuff>()))
            {
                int[] SpiderMinionProjectiles = { ProjectileID.SpiderHiver, ProjectileID.BabySpider, ProjectileID.VenomSpider, ProjectileID.JumperSpider, ProjectileID.DangerousSpider,
                ModContent.ProjectileType<SpiderBabyGreen>(), ModContent.ProjectileType<SpiderBabyPurple>(), ModContent.ProjectileType<SpiderBabyRed>(),
                ModContent.ProjectileType<OrbWeaverSentrySmallSpike>(), ModContent.ProjectileType<OrbWeaverSentryBigSpike>() };

                if (SpiderMinionProjectiles.Contains(projectile.type))
                {
                    modifiers.FinalDamage *= 1.4f;
                }
            }

            //enemies inflicted with the hunter mark debuff, they take more damage from ranged weapons
            if (npc.HasBuff(ModContent.BuffType<HunterScarfMark>()) && modifiers.DamageType == DamageClass.Ranged)
            {
                modifiers.FinalDamage *= 1.2f;
            }
        }

		public override void OnKill(NPC npc)
		{
			//summon zomboid necromancer souls from undead catacomb enemies if they are killed nearby one
			int[] UndeadCatacombEnemies = new int[] { ModContent.NPCType<BoneStackerMoving1>(), ModContent.NPCType<BoneStackerMoving2>(), ModContent.NPCType<BoneStackerMoving3>(),
			ModContent.NPCType<RollingSkull1>(), ModContent.NPCType<RollingSkull2>(), ModContent.NPCType<RollingSkull3>(), ModContent.NPCType<RollingSkull4>(),
			ModContent.NPCType<Skeletoid1>(), ModContent.NPCType<Skeletoid2>(), ModContent.NPCType<Skeletoid3>(), ModContent.NPCType<Skeletoid4>(), ModContent.NPCType<SkeletoidBig>() };

			if (UndeadCatacombEnemies.Contains(npc.type))
			{
				for (int i = 0; i < Main.maxNPCs; i++)
				{
					NPC target = Main.npc[i];
					if (target.type == ModContent.NPCType<ZomboidNecromancer>() && npc.Distance(target.Center) <= 500f)
					{
						int Soul = NPC.NewNPC(npc.GetSource_Death(), (int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<ZomboidNecromancerSoul>(), ai0: target.whoAmI);

						if (Main.netMode != NetmodeID.MultiplayerClient)
						{
							NetMessage.SendData(MessageID.SyncNPC, number: Soul);
						}
					}
				}
			}

			base.OnKill(npc);
		}

		public override void ModifyGlobalLoot(GlobalLoot globalLoot) 
        {
			//entity drop
			globalLoot.Add(ItemDropRule.ByCondition(new DropConditions.CatacombEntityCondition(), ModContent.ItemType<BabyRattle>(), 450));

            //make enemies drop spooky mod's biome keys, with a 1 in 2500 chance like vanilla's biome keys
            globalLoot.Add(ItemDropRule.ByCondition(new DropConditions.SpookyKeyCondition(), ModContent.ItemType<SpookyBiomeKey>(), 2500));
			globalLoot.Add(ItemDropRule.ByCondition(new DropConditions.CemeteryKeyCondition(), ModContent.ItemType<CemeteryKey>(), 2500));
			globalLoot.Add(ItemDropRule.ByCondition(new DropConditions.SpiderKeyCondition(), ModContent.ItemType<SpiderKey>(), 2500));
            globalLoot.Add(ItemDropRule.ByCondition(new DropConditions.SpookyHellKeyCondition(), ModContent.ItemType<SpookyHellKey>(), 2500));

            //make certain bosses drop the catacomb barrier keys
			globalLoot.Add(ItemDropRule.ByCondition(new DropConditions.YellowCatacombKeyCondition(), ModContent.ItemType<CatacombKey1>(), 1));
            globalLoot.Add(ItemDropRule.ByCondition(new DropConditions.RedCatacombKeyCondition(), ModContent.ItemType<CatacombKey2>(), 1));
            globalLoot.Add(ItemDropRule.ByCondition(new DropConditions.OrangeCatacombKeyCondition(), ModContent.ItemType<CatacombKey3>(), 1));

			//eye valley enemies should not drop living flame blocks
            globalLoot.RemoveWhere(rule => rule is ItemDropWithConditionRule drop && drop.itemId == ItemID.LivingFireBlock);
			//re-add living fire blocks dropping with a custom condition that excludes the valley of eyes
            globalLoot.Add(ItemDropRule.ByCondition(new DropConditions.UnderworldDropCondition(), ItemID.LivingFireBlock, 50, 20, 50));

			//eye valley enemies should not drop cascade yoyo
			globalLoot.RemoveWhere(rule => rule is ItemDropWithConditionRule drop && drop.itemId == ItemID.Cascade);
			//re-add the cascade dropping with a custom condition that excludes the valley of eyes
            globalLoot.Add(ItemDropRule.ByCondition(new DropConditions.UnderworldCascadeDropCondition(), ItemID.Cascade, 400));

			//eye valley enemies should not drop hel-fire yoyo
			globalLoot.RemoveWhere(rule => rule is ItemDropWithConditionRule drop && drop.itemId == ItemID.HelFire);
			//re-add the hel-fire dropping with a custom condition that excludes the valley of eyes
            globalLoot.Add(ItemDropRule.ByCondition(new DropConditions.UnderworldDropCondition(), ItemID.HelFire, 400));
        }
    }
}