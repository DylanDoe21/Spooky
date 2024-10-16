using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.NPCs.NoseCult.Projectiles;

namespace Spooky.Content.NPCs.NoseCult
{
	public class SnotMonster : ModNPC
	{
		public override void SetStaticDefaults()
		{	
			Main.npcFrameCount[NPC.type] = 3;
			NPCID.Sets.CantTakeLunchMoney[Type] = true;
		}

		public override void SetDefaults()
		{
			NPC.lifeMax = 100;
            NPC.damage = 25;
			NPC.defense = 0;
			NPC.width = 38;
			NPC.height = 48;
            NPC.npcSlots = 0f;
			NPC.knockBackResist = 0f;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
			NPC.HitSound = SoundID.Item177;
			NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = 0;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.NoseTempleBiome>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.SnotMonster"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.NoseTempleBiome>().ModBiomeBestiaryInfoElement)
			});
		}

		public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter > (12 - (int)NPC.ai[1]))
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 3)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
        }

        public override void AI()
        {
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

			NPC.spriteDirection = NPC.direction;

            NPC.ai[0]++;

            if (NPC.ai[0] >= 300)
            {
                NPC.ai[1] += 0.2f;

                if (NPC.ai[1] >= 20)
                {
                    for (int numProjectiles = 0; numProjectiles < 10; numProjectiles++)
                    {
                        NPCGlobalHelper.ShootHostileProjectile(NPC, new Vector2(NPC.Center.X, NPC.Center.Y - 5), 
                        new Vector2(Main.rand.Next(-4, 5), Main.rand.Next(-4, 0)), ModContent.ProjectileType<SnotMonsterBooger>(), NPC.damage, 2f);
                    }

                    player.ApplyDamageToNPC(NPC, NPC.lifeMax * 2, 0, 0, false);
                }
            }
		}

		public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int numDusts = 0; numDusts < 20; numDusts++)
                {
                    int newDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.KryptonMoss, 0f, -2f, 0, default, 1.5f);
                    Main.dust[newDust].position.X += Main.rand.Next(-50, 51) * 0.05f - 1.5f;
                    Main.dust[newDust].position.Y += Main.rand.Next(-50, 51) * 0.05f - 1.5f;
                    Main.dust[newDust].noGravity = true;
                    
                    if (Main.dust[newDust].position != NPC.Center)
                    {
                        Main.dust[newDust].velocity = NPC.DirectionTo(Main.dust[newDust].position) * 2f;
                    }
                }
            }
        }
    }
}