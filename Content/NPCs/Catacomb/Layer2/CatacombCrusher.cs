using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Items.Food;
using Spooky.Content.NPCs.Catacomb.Layer2.Projectiles;

namespace Spooky.Content.NPCs.Catacomb.Layer2
{
    public class CatacombCrusher1 : ModNPC  
    {
        bool hasCollidedWithFloor = false;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                CustomTexturePath = "Spooky/Content/NPCs/Catacomb/Layer2/CatacombCrusher1"
            };

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Frostburn] = true;
        }

        public override void SetDefaults()
		{
            NPC.lifeMax = 500;
            NPC.damage = 60;
            NPC.defense = 25;
            NPC.width = 88;
            NPC.height = 78;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.value = Item.buyPrice(0, 0, 2, 0);
            NPC.HitSound = SoundID.DD2_SkeletonHurt;
            NPC.alpha = 255;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.CatacombBiome2>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.CatacombCrusher1"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CatacombBiome2>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            SoundEngine.PlaySound(SoundID.DD2_OgreAttack with { Pitch = SoundID.DD2_OgreAttack.Pitch * 0.5f }, NPC.Center);
        }

		public override void AI()
		{
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            if (NPC.alpha > 0)
            {
                NPC.alpha -= 5;
            }

            NPC.ai[0]++;

            if (NPC.ai[0] < 60)
            {
                NPC.velocity *= 0;
            }

            if (NPC.ai[0] == 60)
            {
                NPC.velocity.Y = 35;
            }

            //slam down
            if (NPC.ai[0] > 60 && !hasCollidedWithFloor && IsColliding())
            {
				//use screenshake with a value depending on how far away the player is
				if (player.Distance(NPC.Center) <= 550f)
				{
					SoundEngine.PlaySound(SoundID.Item70, NPC.Center);
					SpookyPlayer.ScreenShakeAmount = 8;
				}
				if (player.Distance(NPC.Center) >= 550f && player.Distance(NPC.Center) <= 1050f)
				{
					SoundEngine.PlaySound(SoundID.Item70 with { Volume = 0.5f }, NPC.Center);
					SpookyPlayer.ScreenShakeAmount = 4;
				}

				NPC.velocity *= 0;
				hasCollidedWithFloor = true;
            }

            if (hasCollidedWithFloor)
            {
				if (NPC.ai[1] == 0)
				{
					NPC.velocity.Y = -7;

					NPC.ai[1] = 1;
				}
				else
				{
					NPC.ai[1]++;

					if (NPC.ai[1] > 5 && IsColliding())
					{
						hasCollidedWithFloor = false;

						NPC.ai[0] = Main.rand.Next(-120, 1);
						NPC.ai[1] = 0;
					}
				}
            }
        }

		public bool IsColliding()
		{
			int minTilePosX = (int)(NPC.position.X / 16) - 1;
			int maxTilePosX = (int)((NPC.position.X + NPC.width) / 16) + 1;
			int minTilePosY = (int)(NPC.position.Y / 16) - 1;
			int maxTilePosY = (int)((NPC.position.Y + NPC.height) / 16) + 1;
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

			for (int i = minTilePosX; i < maxTilePosX; ++i)
			{
				for (int j = minTilePosY; j < maxTilePosY; ++j)
				{
					if (Main.tile[i, j] != null && Main.tile[i, j].HasTile && !Main.tile[i, j].IsActuated &&
					Main.tileSolid[(int)Main.tile[i, j].TileType] && !TileID.Sets.Platforms[(int)Main.tile[i, j].TileType])
					{
						Vector2 vector2;
						vector2.X = (float)(i * 16);
						vector2.Y = (float)(j * 16);
						if (NPC.position.X + NPC.width > vector2.X && NPC.position.X < vector2.X + 16.0 &&
						(NPC.position.Y + NPC.height > (double)vector2.Y && NPC.position.Y < vector2.Y + 16.0))
						{
							return true;
						}
					}
				}
			}

			return false;
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CandyCorn>(), 100));
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
			if (NPC.life <= 0) 
            {
                Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<CatacombCrusherDeath1>(), NPC.damage / 2, 0, NPC.target);
            }
        }
    }

    public class CatacombCrusher2 : CatacombCrusher1  
    {
        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                CustomTexturePath = "Spooky/Content/NPCs/Catacomb/Layer2/CatacombCrusher2"
            };

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Frostburn] = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.CatacombCrusher2"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CatacombBiome2>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void HitEffect(NPC.HitInfo hit) 
        {
			if (NPC.life <= 0) 
            {
                Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<CatacombCrusherDeath2>(), NPC.damage / 2, 0, NPC.target);
            }
        }
    }

    public class CatacombCrusher3 : CatacombCrusher1  
    {
        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                CustomTexturePath = "Spooky/Content/NPCs/Catacomb/Layer2/CatacombCrusher3"
            };

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Frostburn] = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.CatacombCrusher3"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CatacombBiome2>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void HitEffect(NPC.HitInfo hit) 
        {
			if (NPC.life <= 0) 
            {
                Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<CatacombCrusherDeath3>(), NPC.damage / 2, 0, NPC.target);
            }
        }
    }

    public class CatacombCrusher4 : CatacombCrusher1  
    {
        public override void SetDefaults()
		{
            NPC.lifeMax = 700;
            NPC.damage = 60;
            NPC.defense = 25;
			NPC.width = 88;
			NPC.height = 78;
			NPC.npcSlots = 1f;
			NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.value = Item.buyPrice(0, 3, 0, 0);
            NPC.HitSound = SoundID.DD2_SkeletonHurt;
            NPC.alpha = 255;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.CatacombBiome2>().Type };
        }

        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                CustomTexturePath = "Spooky/Content/NPCs/Catacomb/Layer2/CatacombCrusher4"
            };

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Frostburn] = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.CatacombCrusher4"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CatacombBiome2>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void HitEffect(NPC.HitInfo hit) 
        {
			if (NPC.life <= 0) 
            {
                Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<CatacombCrusherDeath4>(), NPC.damage / 2, 0, NPC.target);
            }
        }
    }
}