using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Localization;
using Terraria.GameContent.ItemDropRules;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Linq;

using Spooky.Content.Buffs;
using Spooky.Content.Buffs.Debuff;
using Spooky.Content.Buffs.WhipDebuff;
using Spooky.Content.Items.Catacomb.Misc;
using Spooky.Content.Items.Cemetery.Misc;
using Spooky.Content.Items.SpiderCave.Misc;
using Spooky.Content.Items.SpookyBiome.Misc;
using Spooky.Content.Items.SpookyHell.Misc;
using Spooky.Content.NPCs.Boss.Orroboro;
using Spooky.Content.NPCs.Catacomb;
using Spooky.Content.NPCs.Catacomb.Layer1;
using Spooky.Content.NPCs.SpiderCave.SpiderWar;
using Spooky.Content.NPCs.Tameable;
using Spooky.Content.Projectiles.Blooms;
using Spooky.Content.Projectiles.Catacomb;
using Spooky.Content.Projectiles.SpiderCave;
using Spooky.Content.Projectiles.SpookyBiome;
using Spooky.Content.Projectiles.SpookyHell;
using Spooky.Content.Tiles.Cemetery;
using Spooky.Content.Tiles.SpiderCave;
using Spooky.Content.Tiles.SpookyBiome;
using Spooky.Content.Tiles.Water;

namespace Spooky.Core
{
    public class NPCGlobal : GlobalNPC
	{
		public override bool InstancePerEntity => true;

		public int KeybrandDefenseStacks = 0;
		public bool InitializedKeybrandDefense = false;

        public bool HasVeinChainAttached = false;
        public bool HasGooChompterAttached = false;

		public bool NPCTamed = false; //use for all instances of a tameable animal in spooky mod

		public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
		{
			binaryWriter.Write(NPCTamed);
		}

		public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
		{
			NPCTamed = binaryReader.ReadBoolean();
		}

		public override void SaveData(NPC npc, TagCompound tag)
		{
			tag[nameof(NPCTamed)] = NPCTamed;
		}

		public override void LoadData(NPC npc, TagCompound tag)
		{
			NPCTamed = tag.GetBool(nameof(NPCTamed));
		}

		/*
		public override void Load()
		{
			On_Main.DrawMiscMapIcons += DrawTamedMapIcons;
		}

		public override void Unload()
		{
			On_Main.DrawMiscMapIcons -= DrawTamedMapIcons;
		}

		private static void DrawTamedMapIcons(On_Main.orig_DrawMiscMapIcons orig, Main self, SpriteBatch spriteBatch, Vector2 mapTopLeft, Vector2 mapX2Y2AndOff, Rectangle? mapRect, float mapScale, float drawScale, ref string mouseTextString)
		{
			orig(self, spriteBatch, mapTopLeft, mapX2Y2AndOff, mapRect, mapScale, drawScale, ref mouseTextString);
			DrawTamedMapIcon(self, spriteBatch, mapTopLeft, mapX2Y2AndOff, mapRect, mapScale, drawScale, ref mouseTextString);
		}

		private static void DrawTamedMapIcon(Main self, SpriteBatch spriteBatch, Vector2 mapTopLeft, Vector2 mapX2Y2AndOff, Rectangle? mapRect, float mapScale, float drawScale, ref string mouseTextString)
		{
			if (Main.gameMenu)
			{
				return;
			}

			foreach (NPC npc in Main.ActiveNPCs)
			{
				if (npc.type == ModContent.NPCType<CatacombGuardian>())
				{
					float alphaMult = 1f;
					Vector2 vec = npc.Center / 16f - mapTopLeft;
					vec *= mapScale;
					vec += mapX2Y2AndOff;
					vec = vec.Floor();
					bool draw = true;
					if (mapRect.HasValue)
					{
						Rectangle value2 = mapRect.Value;
						if (!value2.Contains(vec.ToPoint()))
						{
							draw = false;
						}
					}
					if (draw)
					{
						Texture2D texture = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Catacomb/CatacombGuardianMapIcon").Value;

						Rectangle rectangle = texture.Frame();

						spriteBatch.Draw(texture, vec, rectangle, Color.White * alphaMult, 0f, rectangle.Size() / 2f, drawScale, 0, 0f);
						Rectangle rectangle2 = Utils.CenteredRectangle(vec, rectangle.Size() * drawScale);
						if (rectangle2.Contains(Main.MouseScreen.ToPoint()))
						{
							mouseTextString = Language.GetTextValue("Mods.Spooky.NPCs.CatacombGuardian.DisplayName");
						}
					}
				}
				if (npc.GetGlobalNPC<NPCGlobal>().NPCTamed)
				{
					float alphaMult = 1f;
					Vector2 vec = npc.Center / 16f - mapTopLeft;
					vec *= mapScale;
					vec += mapX2Y2AndOff;
					vec = vec.Floor();
					bool draw = true;
					if (mapRect.HasValue)
					{
						Rectangle value2 = mapRect.Value;
						if (!value2.Contains(vec.ToPoint()))
						{
							draw = false;
						}
					}
					if (draw)
					{
						Texture2D texture = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Tameable/TurkeyMapIcon").Value;

						if (npc.type == ModContent.NPCType<Turkey>())
						{
							texture = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Tameable/TurkeyMapIcon").Value;
						}
						else if (npc.type == ModContent.NPCType<LittleDunk>())
						{
							texture = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Tameable/LittleDunkMapIcon").Value;
						}

						Rectangle rectangle = texture.Frame();

						spriteBatch.Draw(texture, vec, rectangle, Color.White * alphaMult, 0f, rectangle.Size() / 2f, drawScale, 0, 0f);
					}
				}
			}
		}
		*/

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

			//add spooky mod's water fountains to the witch doctor shop
			if (shop.NpcType == NPCID.WitchDoctor)
			{
				var SpookFishronDowned = new Condition("Mods.Spooky.Conditions.SpookFishronDowned", () => Flags.downedSpookFishron);

				shop.Add<WaterFountainSpookyItem>();
				shop.Add<WaterFountainRaveItem>();
				shop.Add<WaterFountainEyeValleyItem>();
				shop.Add<WaterFountainTarItem>();
				shop.Add<WaterFountainZombieItem>();
				shop.Add<WaterFountainSpookFishronItem>(SpookFishronDowned);
			}

			if (shop.NpcType == NPCID.Dryad)
			{
				shop.Add(new Item(ModContent.ItemType<SpookyGrassWallItem>())
				{
					shopCustomPrice = Item.buyPrice(copper: 10),
				});
				shop.Add(new Item(ModContent.ItemType<CemeteryGrassWallItem>())
				{
					shopCustomPrice = Item.buyPrice(copper: 10),
				});
				shop.Add(new Item(ModContent.ItemType<DampGrassWallItem>())
				{
					shopCustomPrice = Item.buyPrice(copper: 10),
				});
			}
		}

		public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
		{
			Player player = Main.player[projectile.owner];

			//whip debuff stuff
			if (!projectile.npcProj && !projectile.trap && (projectile.minion || ProjectileID.Sets.MinionShot[projectile.type]))
			{
				//hazmat helmet gives minions 5% chance to critically hit
				if (player.GetModPlayer<SpookyPlayer>().HazmatSet && Main.rand.NextBool(20))
				{
					modifiers.SetCrit();
				}

				//enemies inflicted with the pheromone stinger debuff take increased damage from all spider related minions
				if (npc.HasBuff(ModContent.BuffType<PheromoneWhipDebuff>()))
				{
					int[] SpiderMinionProjectiles = { ProjectileID.SpiderHiver, ProjectileID.SpiderEgg, ProjectileID.BabySpider, ProjectileID.VenomSpider, ProjectileID.JumperSpider, ProjectileID.DangerousSpider,
					ModContent.ProjectileType<SpiderBabyGreen>(), ModContent.ProjectileType<SpiderBabyPurple>(), ModContent.ProjectileType<SpiderBabyRed>(),
					ModContent.ProjectileType<OrbWeaverSentrySmallSpike>(), ModContent.ProjectileType<OrbWeaverSentryBigSpike>() };

					if (SpiderMinionProjectiles.Contains(projectile.type))
					{
						modifiers.FinalDamage *= 1.2f;

						if (Main.rand.NextBool(10))
						{
							modifiers.SetCrit();
						}
					}
				}

				//eggplant whip gives minions 10% chance to critically hit
				if (npc.HasBuff<EggplantWhipDebuff>() && Main.rand.NextBool(10))
				{
					modifiers.SetCrit();
				}

				//generic whip tag damage
				if (npc.HasBuff<ShroomWhipDebuff>())
				{
					modifiers.FlatBonusDamage += ShroomWhipDebuff.tagDamage;
				}
				if (npc.HasBuff<NineTailsDebuff>())
				{
					modifiers.FlatBonusDamage += NineTailsDebuff.tagDamage;
				}
				if (npc.HasBuff<LeechWhipDebuff>())
				{
					modifiers.FlatBonusDamage += LeechWhipDebuff.tagDamage;
				}
				if (npc.HasBuff<SentientLeatherWhipDebuff>())
				{
					modifiers.FlatBonusDamage += SentientLeatherWhipDebuff.tagDamage;
				}
				if (npc.HasBuff<NerveWhipDebuff>())
				{
					modifiers.FlatBonusDamage += NerveWhipDebuff.tagDamage;
				}
				if (npc.HasBuff<TitanoboaWhipDebuff>())
				{
					modifiers.FlatBonusDamage += TitanoboaWhipDebuff.tagDamage;
				}
				if (npc.HasBuff<EelTagDebuff>())
				{
					modifiers.FlatBonusDamage += EelTagDebuff.tagDamage;
				}
			}

			//list of every orro & boro segment
			int[] OrroBoroSegments = { ModContent.NPCType<OrroHeadP1>(), ModContent.NPCType<OrroHead>(), ModContent.NPCType<BoroHead>(),
			ModContent.NPCType<OrroBody>(), ModContent.NPCType<BoroBody>(), ModContent.NPCType<BoroBodyConnect>(), ModContent.NPCType<OrroTail>(), 
			ModContent.NPCType<BoroTail>(), ModContent.NPCType<OrroBodyWings>(), ModContent.NPCType<BoroBodyWings>() };

			//give all orro & boro segments resistance to piercing projectiles because terraria worm moment
			if (OrroBoroSegments.Contains(npc.type))
			{
				if (projectile.penetrate <= -1 || projectile.penetrate >= 2)
				{
					modifiers.FinalDamage /= 1.5f;
				}
			}

			//enemies inflicted with pierced take double damage and are bled for 10 seconds
			if (npc.HasBuff(ModContent.BuffType<PiercedDebuff>()) && projectile.type != ModContent.ProjectileType<SewingNeedle>())
			{
				modifiers.FinalDamage *= 2f;

				npc.AddBuff(ModContent.BuffType<PiercedBleedDebuff>(), 600);

				int buffIndex = npc.FindBuffIndex(ModContent.BuffType<PiercedDebuff>());
				npc.DelBuff(buffIndex);
			}
		}

		public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
		{
			//if the glass eye is staring at an enemy they take 15% more damage
			if (npc.HasBuff(ModContent.BuffType<GlassEyeDebuff>()))
			{
				modifiers.FinalDamage *= 1.15f;
			}
			//dutchman pipe causes enemies to take 10% more damage
			if (npc.HasBuff(ModContent.BuffType<DutchmanPipeDebuff>()))
			{
				modifiers.FinalDamage *= 1.1f;
			}
			//enemies inflicted with the cursed doll effect take 15% more damage from magic attacks
			if (npc.HasBuff(ModContent.BuffType<CursedDollDebuff>()) && modifiers.DamageType == DamageClass.Magic)
			{
				modifiers.FinalDamage *= 1.15f;
			}
			//enemies inflicted with hunter mark take 10% more damage
			if (npc.HasBuff(ModContent.BuffType<HunterScarfMark>()))
			{
				modifiers.FinalDamage *= 1.1f;
			}
			//samsons cuff makes enemies take 18% more damage
			if (npc.HasBuff(ModContent.BuffType<SamsonCuffDebuff>()))
			{
				modifiers.FinalDamage *= 1.18f;
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

			Player player = Main.LocalPlayer;

			//ememies with stomach ache explode into pepto bubbles
			if (npc.HasBuff(ModContent.BuffType<PeptoDebuff>()))
			{
				for (int numProjectiles = 0; numProjectiles < Main.rand.Next(4, 8); numProjectiles++)
				{
					Projectile.NewProjectile(npc.GetSource_Death(), npc.Center.X, npc.Center.Y, Main.rand.NextFloat(-12f, 12f),
					Main.rand.NextFloat(-2f, 0f), ModContent.ProjectileType<PeptoBubble>(), npc.damage, 0, player.whoAmI);
				}
			}

			//spawn souls when you kill an enemy while wearing the skull amulet
			if (player.GetModPlayer<SpookyPlayer>().SkullAmulet && !npc.friendly)
			{
				Projectile.NewProjectile(npc.GetSource_Death(), npc.Center, Vector2.Zero, ModContent.ProjectileType<SkullAmuletSoul>(), 0, 0, player.whoAmI);
			}

			//spawn poppies when you have the poppy bloom
			if (player.GetModPlayer<BloomBuffsPlayer>().CemeteryPoppy && player.ownedProjectileCounts[ModContent.ProjectileType<CemeteryPoppyProj>()] < 2 && !npc.friendly)
			{
				Projectile.NewProjectile(npc.GetSource_Death(), npc.Center, new Vector2(0, -12), ModContent.ProjectileType<CemeteryPoppyProj>(), npc.damage, 0, player.whoAmI);
			}

			//spawn tooth ball if if the enemy has the devil tooth fungus debuff
			if (npc.HasBuff(ModContent.BuffType<DevilToothDebuff>()))
			{
				Vector2 Velocity = new Vector2(0, Main.rand.Next(-12, -6)).RotatedByRandom(MathHelper.ToRadians(40));
				Projectile.NewProjectile(npc.GetSource_Death(), npc.Center, Velocity, ModContent.ProjectileType<BloodToothBall>(), npc.damage, 0, player.whoAmI);
			}

			base.OnKill(npc);
		}

		public override void ModifyGlobalLoot(GlobalLoot globalLoot) 
        {
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

    public static class NPCGlobalHelper
	{
		//utility provided by hallam that shoots a projectile with the correct damage to prevent the issue of terraria over-scaling the damage of hostile projectiles
		public static void ShootHostileProjectile(this Terraria.NPC npc, Vector2 position, Vector2 velocity, int projType, int damage, float knockback, float ai0 = 0, float ai1 = 0, float ai2 = 0, int Frame = 0)
		{
			damage /= 2;

			if (Main.expertMode)
			{
				damage /= Main.masterMode ? 3 : 2;
			}

			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				int NewProj = Projectile.NewProjectile(npc.GetSource_FromAI(), position, velocity, projType, damage, knockback, Main.myPlayer, ai0, ai1, ai2);
				Main.projectile[NewProj].frame = Frame;
			}
		}

		//modified version of NPC.AI_AttemptToFindTeleportSpot from vanilla, but modified a bunch specifically to have a line of sight check to the player
		public static bool TeleportToSpot(NPC npc, Player player, ref Vector2 chosenTile, int targetTileX, int targetTileY, int rangeFromTargetTile, int telefragPreventionDistanceInTiles, bool UseLOSCheck = true)
		{
			int solidTileCheckFluff = 1;
			int PositionX = (int)npc.Center.X / 16;
			int PositionY = (int)npc.Center.Y / 16;
			int Attempts = 0;
			bool CanTeleport = false;
			float VelocityMultiplier = 30f;
			
			if (Math.Abs(PositionX * 16 - targetTileX * 16) + Math.Abs(PositionY * 16 - targetTileY * 16) > 2000)
			{
				Attempts = 100;
				CanTeleport = false;
			}
			while (!CanTeleport && Attempts < 100)
			{
				Attempts++;
				int RandomX = Main.rand.Next(targetTileX - rangeFromTargetTile, targetTileX + rangeFromTargetTile + 1);
				for (int i = Main.rand.Next(targetTileY - rangeFromTargetTile, targetTileY + rangeFromTargetTile + 1); i < targetTileY + rangeFromTargetTile; i++)
				{
					if ((i >= PositionY - 1 && i <= PositionY + 1 && RandomX >= PositionX - 1 && RandomX <= PositionX + 1) || !Main.tile[RandomX, i].HasTile)
					{
						continue;
					}
					bool IsPositionValid = true;
					if (Main.tile[RandomX, i - 1].LiquidType == LiquidID.Lava && Main.tile[RandomX, i - 1].LiquidAmount <= 0)
					{
						IsPositionValid = false;
					}
					bool HasLineOfSight = Collision.CanHitLine(player.position, player.width, player.height, new Vector2(RandomX, i - 1) * 16, 2, 2);
					if (!HasLineOfSight && UseLOSCheck)
					{
						IsPositionValid = false;
					}
					if (!IsPositionValid || !Main.tileSolid[Main.tile[RandomX, i].TileType])
					{
						continue;
					}
					if (Collision.SolidTiles(RandomX - solidTileCheckFluff, RandomX + solidTileCheckFluff, i - 3 - solidTileCheckFluff, i - 1))
					{
						continue;
					}

					//prevent npc from teleporting too close to the player
					Rectangle rectangle = new Rectangle(RandomX * 16, i * 16, 16, 16);
					rectangle.Inflate(telefragPreventionDistanceInTiles * 16, telefragPreventionDistanceInTiles * 16);
					for (int j = 0; j < Main.player.Length; j++)
					{
						Player playerToCheck = Main.player[j];
						if (playerToCheck != null && playerToCheck.active && !playerToCheck.DeadOrGhost)
						{
							Rectangle value = playerToCheck.Hitbox;
							Rectangle value2 = value.Modified((int)(playerToCheck.velocity.X * VelocityMultiplier), (int)(playerToCheck.velocity.Y * VelocityMultiplier), 0, 0);
							Rectangle.Union(ref value2, ref value, out value2);
							if (value2.Intersects(rectangle))
							{
								IsPositionValid = false;
								CanTeleport = false;
								break;
							}
						}
					}

					if (IsPositionValid)
					{
						chosenTile = new Vector2(RandomX, i);
						CanTeleport = true;
					}

					break;
				}
			}

			return CanTeleport;
		}

		//use for when npcs should do special things when colliding with tiles, but dont use NPC.tileCollide
		//also useful for npcs that should only collide with solid tiles excluding platforms, planter boxes, ect
		public static bool IsColliding(NPC npc)
		{
			int minTilePosX = (int)(npc.position.X / 16) - 1;
			int maxTilePosX = (int)((npc.position.X + npc.width) / 16) + 1;
			int minTilePosY = (int)(npc.position.Y / 16) - 1;
			int maxTilePosY = (int)((npc.position.Y + npc.height) / 16) + 1;
			if (minTilePosX < 0)
			{
				minTilePosX = 0;
			}
			if (maxTilePosX > Main.maxTilesX)
			{
				maxTilePosX = Main.maxTilesX;
			}
			if (minTilePosY < 0)
			{
				minTilePosY = 0;
			}
			if (maxTilePosY > Main.maxTilesY)
			{
				maxTilePosY = Main.maxTilesY;
			}

			for (int i = minTilePosX; i < maxTilePosX; i++)
			{
				for (int j = minTilePosY; j < maxTilePosY; j++)
				{
					if (Main.tile[i, j] != null && Main.tile[i, j].HasTile && !Main.tile[i, j].IsActuated &&
					Main.tileSolid[(int)Main.tile[i, j].TileType] && !TileID.Sets.Platforms[(int)Main.tile[i, j].TileType])
					{
						Vector2 vector2;
						vector2.X = (float)(i * 16);
						vector2.Y = (float)(j * 16);
						if (npc.position.X + npc.width > vector2.X && npc.position.X < vector2.X + 16.0 && (npc.position.Y + npc.height > (double)vector2.Y && npc.position.Y < vector2.Y + 16.0))
						{
							return true;
						}
					}
				}
			}

			return false;
		}

		//useful for when npcs should do special things when colliding with the ground by checking at the bottom of the npc
		public static bool IsCollidingWithFloor(NPC npc, bool countPlatforms = false)
		{
			int minTilePosX = (int)(npc.position.X / 16) - 1;
			int maxTilePosX = (int)((npc.position.X + npc.width) / 16) + 1;
			int minTilePosY = (int)(npc.Bottom.Y / 16) - 1;
			int maxTilePosY = (int)(npc.Bottom.Y / 16) + 1;
			if (minTilePosX < 0)
			{
				minTilePosX = 0;
			}
			if (maxTilePosX > Main.maxTilesX)
			{
				maxTilePosX = Main.maxTilesX;
			}
			if (minTilePosY < 0)
			{
				minTilePosY = 0;
			}
			if (maxTilePosY > Main.maxTilesY)
			{
				maxTilePosY = Main.maxTilesY;
			}

			for (int i = minTilePosX; i < maxTilePosX; i++)
			{
				for (int j = minTilePosY; j < maxTilePosY; j++)
				{
					bool ValidTile = Main.tileSolid[(int)Main.tile[i, j].TileType] || Main.tile[i, j].LeftSlope || Main.tile[i, j].RightSlope || Main.tile[i, j].TopSlope || Main.tile[i, j].BottomSlope;
					bool ValidPlatform = countPlatforms && TileID.Sets.Platforms[(int)Main.tile[i, j].TileType] && Main.tileSolidTop[(int)Main.tile[i, j].TileType] && npc.velocity.Y >= 1;

					if (Main.tile[i, j] != null && Main.tile[i, j].HasTile && !Main.tile[i, j].IsActuated && (ValidPlatform || ValidTile))
					{
						Vector2 vector2;
						vector2.X = (float)(i * 16);
						vector2.Y = (float)(j * 16);
						if (npc.position.X + npc.width > vector2.X && npc.position.X < vector2.X + 16.0 && (npc.Bottom.Y + 1 > (double)vector2.Y && npc.Bottom.Y < vector2.Y + 16.0))
						{
							return true;
						}
					}
				}
			}

			return false;
		}

        //check for npcs that arent considered bosses internally or are segments/pieces of bosses
        public static bool IsTechnicallyBoss(this NPC npc)
		{
			int type = npc.type;
			switch (type)
			{
                //eater of worlds segments (because they do not count as bosses individually)
				case NPCID.EaterofWorldsHead:
				case NPCID.EaterofWorldsBody:
				case NPCID.EaterofWorldsTail:
				//skeletron hand and dungeon guardian
				case NPCID.SkeletronHand:
				case NPCID.DungeonGuardian:
				//skeletron prime hands
				case NPCID.PrimeCannon:
				case NPCID.PrimeLaser:
				case NPCID.PrimeSaw:
				case NPCID.PrimeVice:
				//golem pieces
				case NPCID.GolemHead:
				case NPCID.GolemHeadFree:
				case NPCID.GolemFistLeft:
				case NPCID.GolemFistRight:
				//martian saucer
				case NPCID.MartianSaucerCore:
				case NPCID.MartianSaucerCannon:
				case NPCID.MartianSaucerTurret:
				case NPCID.MartianSaucer:
				//flying dutchman
				case NPCID.PirateShip:
                case NPCID.PirateShipCannon:
				//frost moon and pumpkin moon minibosses
				case NPCID.IceQueen:
				case NPCID.SantaNK1:
				case NPCID.Everscream:
				case NPCID.Pumpking:
				case NPCID.MourningWood:
				//old ones army minibosses
				case NPCID.DD2Betsy:
				case NPCID.DD2DarkMageT1:
				case NPCID.DD2DarkMageT3:
				case NPCID.DD2OgreT2:
				case NPCID.DD2OgreT3:
					return true;
			}

			return npc.boss || NPCID.Sets.ShouldBeCountedAsBoss[type];
		}
    }
}