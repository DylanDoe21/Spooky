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
using System.Collections.Generic;

using Spooky.Core;

namespace Spooky.Content.NPCs.SpiderCave.SpiderWar
{
    public class TrumpetSpider : ModNPC  
    {
        float SaveRotation;

        public static readonly SoundStyle TrumpetSound = new("Spooky/Content/Sounds/TrumpetSpider", SoundType.Sound);

        public override void SetStaticDefaults()
        {
			Main.npcFrameCount[NPC.type] = 8;
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 1000;
            NPC.damage = 35;
            NPC.defense = 0;
            NPC.width = 62;
            NPC.height = 52;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0.5f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpiderCaveBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
                new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.TrumpetSpider"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpiderCaveBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;

            if (NPC.ai[0] > 1 && NPC.ai[0] < 350)
            {
                if (NPC.frameCounter > 6)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y >= frameHeight * 8)
                {
                    NPC.frame.Y = 4 * frameHeight;
                }
            }
            else
            {
                if (NPC.frameCounter > 4)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y >= frameHeight * 4)
                {
                    NPC.frame.Y = 0 * frameHeight;
                }
            }
        }

        public override bool CheckActive()
		{
			return !SpiderWarWorld.SpiderWarActive;
		}

        public override void AI()
        {
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            float RotateDirection = NPC.velocity.ToRotation();
            float RotateSpeed = 0.1f;

            NPC.rotation = NPC.rotation.AngleTowards(RotateDirection, RotateSpeed);

            if (NPC.ai[0] == 0)
            {
                Vector2 desiredVelocity = NPC.DirectionTo(player.Center) * 7;
                NPC.velocity = Vector2.Lerp(NPC.velocity, desiredVelocity, 1f / 20);

                SaveRotation = NPC.rotation;

                if (NPC.Distance(player.Center) <= 100f)
                {
                    NPC.ai[0]++;
                }
            }
            else
            {
                NPC.ai[0]++;
                if (NPC.ai[0] == 2)
                {
                    SoundEngine.PlaySound(TrumpetSound, NPC.Center);

                    NPC.velocity = Vector2.Zero;
                }

                if (NPC.ai[0] == 350)
                {
                    if (SpiderWarWorld.SpiderWarActive)
                    {
                        SpiderWarWorld.SpiderWarPoints++;
                    }
                }

                if (NPC.ai[0] > 350)
                {
                    Vector2 desiredVelocity = NPC.DirectionTo(player.Center) * -32;
                    NPC.velocity = Vector2.Lerp(NPC.velocity, desiredVelocity, 1f / 20);

                    NPC.EncourageDespawn(60);
                }
                else
                {
                    NPC.rotation = SaveRotation;
                }
            }
        }
    }
}