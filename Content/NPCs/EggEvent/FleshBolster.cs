using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.NPCs.EggEvent.Projectiles;
using Spooky.Content.Projectiles.Cemetery;

namespace Spooky.Content.NPCs.EggEvent
{
	public class FleshBolster : ModNPC
	{
		bool HasSpawnedTendrils = false;

		public static readonly SoundStyle DeathSound = new("Spooky/Content/Sounds/EggEvent/BiomassExplode2", SoundType.Sound);

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 5;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
			{
                CustomTexturePath = "Spooky/Content/NPCs/NPCDisplayTextures/FleshBolsterBestiary",
                Position = new Vector2(0f, 0f),
                PortraitPositionXOverride = 0f,
                PortraitPositionYOverride = 0f
            };
		}

		public override void SendExtraAI(BinaryWriter writer)
        {
            //bools
            writer.Write(HasSpawnedTendrils);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //bools
            HasSpawnedTendrils = reader.ReadBoolean();
        }

		public override void SetDefaults()
		{
			NPC.lifeMax = 1500;
            NPC.damage = 45;
			NPC.defense = 20;
			NPC.width = 76;
			NPC.height = 104;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 1, 0, 0);
            NPC.noGravity = false;
			NPC.HitSound = SoundID.Item177;
			NPC.DeathSound = DeathSound;
			NPC.aiStyle = -1;
			SpawnModBiomes = new int[2] { ModContent.GetInstance<Biomes.SpookyHellBiome>().Type, ModContent.GetInstance<Biomes.SpookyHellEventBiome>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
            bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[Type], quickUnlock: true);

			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.FleshBolster"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyHellBiome>().ModBiomeBestiaryInfoElement),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyHellEventBiome>().ModBiomeBestiaryInfoElement)
			});
		}

		public override void FindFrame(int frameHeight)
		{
			NPC.frameCounter++;
            if (NPC.frameCounter > 8)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 5)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
		}

        public override void AI()
		{
            NPC.spriteDirection = NPC.direction;

			if (!HasSpawnedTendrils)
            {
				bool[] spawnedTentacle = new bool[4];
				for (int i = 0; i < Main.maxProjectiles; i++)
				{
					Projectile projectile = Main.projectile[i];
					if (projectile.active && projectile.type == ModContent.ProjectileType<FleshBolsterBuffer>() && projectile.ai[1] >= 0f && projectile.ai[1] < 4f)
					{
						spawnedTentacle[(int)projectile.ai[1]] = true;
					}
				}

				for (int i = 0; i < 4; i++)
				{
					if (!spawnedTentacle[i])
					{
						NPCGlobalHelper.ShootHostileProjectile(NPC, NPC.Center, Vector2.Zero, ModContent.ProjectileType<FleshBolsterBuffer>(), 0, 0f, 
						ai0: Main.rand.Next(120), ai1: i + 3, ai2: NPC.whoAmI);
					}
				}

                HasSpawnedTendrils = true;
                NPC.netUpdate = true;
            }
		}

		public override void HitEffect(NPC.HitInfo hit) 
        {
			if (NPC.life <= 0) 
            {
                //spawn gores
                for (int numGores = 1; numGores <= 7; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, new Vector2(Main.rand.Next(-12, 13), Main.rand.Next(-12, 13)), ModContent.Find<ModGore>("Spooky/FleshBolsterGore" + numGores).Type);
                    }
                }

				//spawn splatter
				int NumProjectiles = Main.rand.Next(15, 25);
				for (int i = 0; i < NumProjectiles; i++)
				{
					NPCGlobalHelper.ShootHostileProjectile(NPC, NPC.Center, new Vector2(Main.rand.Next(-12, 13), Main.rand.Next(-12, -5)), ModContent.ProjectileType<RedSplatter>(), 0, 0f);
				}
			}
		}
    }
}