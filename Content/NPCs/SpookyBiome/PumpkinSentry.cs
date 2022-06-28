using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

using Spooky.Content.NPCs.SpookyBiome.Projectiles;

namespace Spooky.Content.NPCs.SpookyBiome
{
    public class PumpkinSentry : ModNPC  
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pumpkin Spitter");
            Main.npcFrameCount[NPC.type] = 6;
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 65;
            NPC.damage = 45;
            NPC.defense = 20;
            NPC.width = 56;
            NPC.height = 72;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.noTileCollide = false;
            NPC.noGravity = false;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpookyBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new MoonLordPortraitBackgroundProviderBestiaryInfoElement(), //Plain black background
				new FlavorTextBestiaryInfoElement("Immobile pumpkin creatures that grow out of the ground. They attack by spitting harmful pumpkin mush by opening their pumpkin like mouth.")
			});
		}

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            Player player = spawnInfo.Player;

			if (!spawnInfo.Invasion && Main.invasionType == 0 && !Main.pumpkinMoon && !Main.snowMoon && !Main.eclipse &&
            !(player.ZoneTowerSolar || player.ZoneTowerVortex || player.ZoneTowerNebula || player.ZoneTowerStardust))
            {
                //spawn on the surface during the night
                if (player.InModBiome(ModContent.GetInstance<Biomes.SpookyBiome>()) && !Main.dayTime)
                {
                    return 10f;
                }
            }

            return 0f;
        }

        public override void FindFrame(int frameHeight)
        {
            //idle animation
            if (NPC.ai[0] < 400)
            {
                NPC.frameCounter += 1;

                if (NPC.frameCounter > 10)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0.0;
                }
                if (NPC.frame.Y >= frameHeight * 4)
                {
                    NPC.frame.Y = 0 * frameHeight;
                }
            }

            //open mouth for spit attack
            if (NPC.ai[0] >= 400)
            {
                NPC.frameCounter += 1;

                if (NPC.frameCounter > 10)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0.0;
                }
                if (NPC.frame.Y >= frameHeight * 6)
                {
                    NPC.frame.Y = 5 * frameHeight;
                }
            }
        }

        public override void AI()
        {
            NPC.TargetClosest(true);

            int Damage = Main.expertMode ? 25 : 35;

            NPC.ai[0]++;
            if (NPC.ai[0] > 400 && NPC.ai[0] <= 500)
            {
                if (Main.rand.Next(8) == 0)
                {
                    SoundEngine.PlaySound(SoundID.NPCDeath13, NPC.Center);

                    float Spread = Main.rand.Next(-250, 250) * 0.01f;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, 0 + Spread, -10,
                        ModContent.ProjectileType<PumpkinSpit>(), Damage, 1, Main.myPlayer, 0, 0);
                    }
                }
            }
            
            if (NPC.ai[0] >= 500)
            {
                NPC.ai[0] = 0;
            }
        }

        public override bool CheckDead()
        {
            for (int numDust = 0; numDust < 20; numDust++)
            {
                int DustGore = Dust.NewDust(new Vector2(NPC.Center.X, NPC.Center.Y), NPC.width / 2, NPC.height / 2, 288, 0f, 0f, 100, default, 2f);

                Main.dust[DustGore].velocity *= 3f;
                Main.dust[DustGore].noGravity = true;

                if (Main.rand.Next(2) == 0)
                {
                    Main.dust[DustGore].scale = 0.5f;
                    Main.dust[DustGore].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }

            return true;
        }
    }
}