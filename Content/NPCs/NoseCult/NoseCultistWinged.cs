using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Content.Dusts;
using Spooky.Content.NPCs.NoseCult.Projectiles;

namespace Spooky.Content.NPCs.NoseCult
{
	public class NoseCultistWinged : ModNPC
	{
        Vector2 SavePosition;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 9;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 230;
            NPC.damage = 32;
            NPC.defense = 5;
            NPC.width = 78;
			NPC.height = 72;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.25f;
            NPC.HitSound = SoundID.NPCHit48 with { Pitch = -0.3f };
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.NoseTempleBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.NoseCultistWinged"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.NoseTempleBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;

            if (NPC.ai[1] < 240)
            {
                if (NPC.frameCounter > 2)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y >= frameHeight * 5)
                {
                    NPC.frame.Y = 0 * frameHeight;
                }
            }
            else
            {
                if (NPC.frame.Y < frameHeight * 5)
                {
                    NPC.frame.Y = 5 * frameHeight;
                }

                if (NPC.frameCounter > 8)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y >= frameHeight * 9)
                {
                    NPC.ai[1] = 0;
                    NPC.ai[2] = 0;
                    NPC.ai[3] = 0;

                    NPC.frame.Y = 0 * frameHeight;
                }
            }
        }

        public override void AI()
		{
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            NPC Parent = Main.npc[(int)NPC.ai[0]];
            
            NPC.spriteDirection = NPC.direction;

            NPC.ai[1]++;

            if (NPC.ai[1] == 5)
            {
                SavePosition = new Vector2(Parent.Center.X + Main.rand.Next(-300, 300), Parent.Center.Y - Main.rand.Next(10, 150));
            }

            if (NPC.ai[1] > 5)
            {
                Vector2 GoTo = SavePosition;

                float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 6, 12);
                NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
            }

            if (NPC.ai[1] >= 240)
            {
                if (NPC.frame.Y == 6 * NPC.height && NPC.ai[3] == 0)
                {
                    Vector2 ShootSpeed = player.Center - NPC.Center;
                    ShootSpeed.Normalize();
                    ShootSpeed *= 12f;

                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, ShootSpeed, ModContent.ProjectileType<NoseCultistWingedSnot>(), NPC.damage / 4, 0f, Main.myPlayer);

                    NPC.ai[3]++;

                    NPC.netUpdate = true;
                }
            }
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 6; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/NoseCultistWingedGore" + numGores).Type);
                    }
                }
            }
        }
    }
}