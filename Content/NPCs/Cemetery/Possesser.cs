using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

using Spooky.Content.Buffs.Debuff;
using Spooky.Content.Items.Food;

namespace Spooky.Content.NPCs.Cemetery
{
    public class Possesser : ModNPC  
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;

            var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Position = new Vector2(0f, -10f),
                PortraitPositionXOverride = 0f,
                PortraitPositionYOverride = -30f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
        }
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 25;
            NPC.damage = 15;
            NPC.width = 34;
			NPC.height = 42;
            NPC.npcSlots = 1f;
            NPC.value = Item.buyPrice(0, 0, 0, 50);
            NPC.HitSound = SoundID.NPCHit54;
			NPC.DeathSound = SoundID.NPCHit36;
            NPC.aiStyle = 14;
			AIType = NPCID.GiantBat;
            AnimationType = NPCID.GiantBat;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.CemeteryBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.Possesser"),
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CemeteryBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            Player player = spawnInfo.Player;

			if (!spawnInfo.Invasion && Main.invasionType == 0 && !Main.pumpkinMoon && !Main.snowMoon && !Main.eclipse &&
            !(player.ZoneTowerSolar || player.ZoneTowerVortex || player.ZoneTowerNebula || player.ZoneTowerStardust))
            {
                if (player.InModBiome(ModContent.GetInstance<Biomes.CemeteryBiome>()) && !Main.dayTime)
                {
                    return 4.5f;
                }
            }

            return 0f;
        }

        public override void AI()
		{
			NPC.spriteDirection = NPC.direction;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            for (int numDusts = 0; numDusts < 20; numDusts++)
            {
                int GhostDust = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 229, 0f, 0f, 100, default(Color), 2f);
                Main.dust[GhostDust].velocity *= 3f;
                Main.dust[GhostDust].noGravity = true;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[GhostDust].scale = 0.5f;
                    Main.dust[GhostDust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }

            target.AddBuff(ModContent.BuffType<Possessed>(), 360, false);
            NPC.active = false;
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            //dont run on multiplayer
			if (Main.netMode == NetmodeID.Server) 
            {
				return;
			}

			if (NPC.life <= 0) 
            {
                for (int numDust = 0; numDust < 35; numDust++)
                {                                                                                  
                    int DustGore = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.HallowSpray, 0f, -2f, 0, default(Color), 1.5f);
                    Main.dust[DustGore].noGravity = true;
                    Main.dust[DustGore].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                    Main.dust[DustGore].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;

                    if (Main.dust[DustGore].position != NPC.Center)
                    {
                        Main.dust[DustGore].velocity = NPC.DirectionTo(Main.dust[DustGore].position) * Main.rand.NextFloat(1f, 2f);
                    }
                }
            }
        }
    }
}