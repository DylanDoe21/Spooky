using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;

using Spooky.Content.Buffs;
using Spooky.Content.Events;
using Spooky.Content.NPCs.EggEvent.Projectiles;

namespace Spooky.Content.NPCs.EggEvent
{
    public class Capillary : ModNPC  
    {
        public static List<int> BuffableNPCs = new List<int>() 
        {
            ModContent.NPCType<Crux>(),
            ModContent.NPCType<Distended>(),
            ModContent.NPCType<DistendedBrute>(),
            ModContent.NPCType<Vesicator>(),
            ModContent.NPCType<Vigilante>(),
            ModContent.NPCType<Visitant>()
        };

        public static readonly SoundStyle HitSound = new("Spooky/Content/Sounds/SpookyHell/EnemyHit", SoundType.Sound);
        public static readonly SoundStyle DeathSound = new("Spooky/Content/Sounds/SpookyHell/EnemyDeath", SoundType.Sound);
        public static readonly SoundStyle ScreechSound = new("Spooky/Content/Sounds/SpookyHell/CapillaryScreech", SoundType.Sound);

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Capillary");
            Main.npcFrameCount[NPC.type] = 5;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = Main.hardMode ? 250 : 120;
            NPC.damage = Main.hardMode ? 75 : 50;
            NPC.defense = Main.hardMode ? 40 : 20;
            NPC.width = 50;
            NPC.height = 94;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 5, 0);
            NPC.noTileCollide = false;
            NPC.noGravity = false;
            NPC.HitSound = HitSound;
			NPC.DeathSound = DeathSound;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpookyHellBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Normally hiding in the ground, the eye valley's living capillaries emerge when the egg is threatened, trapping intruders and supporting their allies."),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyHellBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/EggEvent/CapillaryGlow").Value;

            Main.EntitySpriteDraw(tex, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), 
            NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            Player player = spawnInfo.Player;

            if (player.InModBiome(ModContent.GetInstance<Biomes.EggEventBiome>()) && EggEventWorld.EggEventProgress >= 90)
            {
                return 12f;
            }

            return 0f;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 1;

            if (NPC.frameCounter > 10)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0.0;
            }
            if (NPC.frame.Y >= frameHeight * 5)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
        }

        public override void AI()
        {
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);

            NPC.localAI[0]++;

            if (NPC.localAI[0] >= 600)
            {
                SoundEngine.PlaySound(ScreechSound, NPC.Center);

                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC buffTarget = Main.npc[i];
                    if (buffTarget.active)
                    {
                        if (BuffableNPCs.Contains(buffTarget.type) && Vector2.Distance(NPC.Center, buffTarget.Center) < 600 && buffTarget.type != NPC.type)
                        {
                            buffTarget.AddBuff(ModContent.BuffType<EggEventEnemyBuff>(), 300);
                        }
                    }
                }

                NPC.localAI[0] = 0;
            }
        }
    }
}