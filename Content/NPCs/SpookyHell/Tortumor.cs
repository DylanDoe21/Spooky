using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using Terraria.GameContent.Bestiary;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

using Spooky.Content.NPCs.SpookyHell.Projectiles;

namespace Spooky.Content.NPCs.SpookyHell
{
    public class Tortumor : ModNPC
    {
        public int MoveSpeedX = 0;
		public int MoveSpeedY = 0;

        public static readonly SoundStyle ScreechSound = new SoundStyle("Spooky/Content/Sounds/TumorScreech1", SoundType.Sound);

		public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tortumor");
            Main.npcFrameCount[NPC.type] = 6;
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 75;
            NPC.damage = 45;
            NPC.defense = 20;
            NPC.width = 58;
            NPC.height = 62;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit9;
            NPC.DeathSound = SoundID.NPCDeath22;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Content.Biomes.SpookyHellBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new MoonLordPortraitBackgroundProviderBestiaryInfoElement(), //Plain black background
				new FlavorTextBestiaryInfoElement("Disgusting flesh amalgams born from the living hell. They can attack by using chunks of themselves as projectiles.")
			});
		}

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            Player player = spawnInfo.Player;

			if (Main.LocalPlayer.InModBiome(ModContent.GetInstance<Content.Biomes.SpookyHellBiome>()))
			{
                return 15f;
            }
            return 0f;
        }

        public override void FindFrame(int frameHeight)
        {
            //regular anims
            if (NPC.ai[0] <= 420)
            {
                NPC.frameCounter += 1;

                if (NPC.frameCounter > 7)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0.0;
                }
                if (NPC.frame.Y >= frameHeight * 4)
                {
                    NPC.frame.Y = 0 * frameHeight;
                }
            }

            //screaming animation
            if (NPC.ai[0] == 420)
            {
                NPC.frame.Y = 4 * frameHeight;
            }
            if (NPC.ai[0] > 420)
            {
                NPC.frameCounter += 1;

                if (NPC.frameCounter > 10)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0.0;
                }
                if (NPC.frame.Y >= frameHeight * 6)
                {
                    NPC.frame.Y = 4 * frameHeight;
                }
            }
        }
        
        public override void AI()
		{
			Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);

            NPC.rotation = NPC.velocity.X * 0.04f;

            int Damage = Main.expertMode ? 30 : 40;
			
			NPC.ai[0]++;  

            //dust spawning for when big tumor summons this
            if (NPC.ai[0] == -1)
            {
                for (int numDust = 0; numDust < 20; numDust++)
                {
                    int DustGore = Dust.NewDust(new Vector2(NPC.Center.X, NPC.Center.Y), 
                    NPC.width / 2, NPC.height / 2, 5, 0f, 0f, 100, default(Color), 2f);

                    Main.dust[DustGore].scale *= Main.rand.NextFloat(1f, 2f);
                    Main.dust[DustGore].velocity *= 3f;
                    Main.dust[DustGore].noGravity = true;

                    if (Main.rand.Next(2) == 0)
                    {
                        Main.dust[DustGore].scale = 0.5f;
                        Main.dust[DustGore].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
            }

            if (NPC.ai[0] <= 420)
            {
                int MaxSpeed = 25;

                //flies to players X position
                if (NPC.Center.X >= player.Center.X && MoveSpeedX >= -MaxSpeed - 8) 
                {
                    MoveSpeedX--;
                }
                else if (NPC.Center.X <= player.Center.X && MoveSpeedX <= MaxSpeed + 8)
                {
                    MoveSpeedX++;
                }

                NPC.velocity.X = MoveSpeedX * 0.1f;
                
                //flies to players Y position
                if (NPC.Center.Y >= player.Center.Y - 60f && MoveSpeedY >= -MaxSpeed)
                {
                    MoveSpeedY--;
                }
                else if (NPC.Center.Y <= player.Center.Y - 60f && MoveSpeedY <= MaxSpeed)
                {
                    MoveSpeedY++;
                }

                NPC.velocity.Y = MoveSpeedY * 0.1f;
            }

            if (NPC.ai[0] >= 420)
            {
                if (NPC.ai[0] == 420)
                {
                    SoundEngine.PlaySound(ScreechSound, NPC.Center);
                }

                NPC.velocity *= 0.95f;

                if (NPC.ai[0] == 495)
                {
                    for (int numDust = 0; numDust < 20; numDust++)
                    {
                        int DustGore = Dust.NewDust(new Vector2(NPC.Center.X, NPC.Center.Y), 
                        NPC.width / 2, NPC.height / 2, 5, 0f, 0f, 100, default(Color), 2f);

                        Main.dust[DustGore].scale *= Main.rand.NextFloat(1f, 2f);
                        Main.dust[DustGore].velocity *= 3f;
                        Main.dust[DustGore].noGravity = true;

                        if (Main.rand.Next(2) == 0)
                        {
                            Main.dust[DustGore].scale = 0.5f;
                            Main.dust[DustGore].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                        }
                    }

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 vector = Vector2.UnitY.RotatedByRandom(1.57079637050629f) * new Vector2(5f, 3f);

                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X + Main.rand.Next(-50, 50), NPC.Center.Y + Main.rand.Next(-50, 50), 
                        vector.X, vector.Y, ModContent.ProjectileType<TumorOrb1>(), Damage, 0f, Main.myPlayer, 0f, (float)NPC.whoAmI);

                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X + Main.rand.Next(-50, 50), NPC.Center.Y + Main.rand.Next(-50, 50), 
                        vector.X, vector.Y, ModContent.ProjectileType<TumorOrb2>(), Damage, 0f, Main.myPlayer, 0f, (float)NPC.whoAmI);

                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X + Main.rand.Next(-50, 50), NPC.Center.Y + Main.rand.Next(-50, 50), 
                        vector.X, vector.Y, ModContent.ProjectileType<TumorOrb3>(), Damage, 0f, Main.myPlayer, 0f, (float)NPC.whoAmI);
                    }
                }

                if (NPC.ai[0] >= 510)
                {
                    NPC.ai[0] = 0;
                }
            }
        }

        public override bool CheckDead() 
		{
            Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/TortumorGore1").Type);
            Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/TortumorGore2").Type);
            Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/TortumorGore3").Type);
            Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/TortumorGore4").Type);

            for (int numDust = 0; numDust < 20; numDust++)
            {
                int DustGore = Dust.NewDust(new Vector2(NPC.Center.X, NPC.Center.Y), 
                NPC.width / 2, NPC.height / 2, 5, 0f, 0f, 100, default(Color), 2f);

                Main.dust[DustGore].scale *= Main.rand.NextFloat(1f, 2f);
                Main.dust[DustGore].velocity *= 3f;
                Main.dust[DustGore].noGravity = true;

                if (Main.rand.Next(2) == 0)
                {
                    Main.dust[DustGore].scale = 0.5f;
                    Main.dust[DustGore].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }

            return true;
		}
    }
}