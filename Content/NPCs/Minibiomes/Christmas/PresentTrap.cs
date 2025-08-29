using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Microsoft.Xna.Framework;
using System.IO;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.NPCs.Minibiomes.Christmas.Projectiles;

namespace Spooky.Content.NPCs.Minibiomes.Christmas
{
	public class PresentTrapBlue : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 2;
            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
		}

		public override void SetDefaults()
		{
            NPC.lifeMax = 60;
            NPC.damage = 30;
			NPC.defense = 0;
			NPC.width = 46;
			NPC.height = 46;
            NPC.npcSlots = 0.5f;
            NPC.noGravity = false;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.aiStyle = -1;
		}
        
        public override void FindFrame(int frameHeight)
		{
            if (NPC.ai[0] == 0)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
            else
            {
               NPC.frame.Y = 1 * frameHeight; 
            }
		}

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
		{
			return false;
		}

        public override bool PreAI()
        {
            foreach (var player in Main.ActivePlayers)
            {
                if (NPC.Distance(player.Center) <= 150f)
                {
                    if (NPC.ai[1] == 0)
                    {
                        if (Main.netMode != NetmodeID.Server) 
                        {
                            Gore.NewGorePerfect(NPC.GetSource_FromAI(), NPC.position, new Vector2(Main.rand.Next(-2, 3), -3), ModContent.Find<ModGore>("Spooky/PresentTrapBlueLidGore").Type);
                        }

                        NPC.ai[1] = 1;
                    }
                }
            }

            return true;
        }

        public override void AI()
        {
            foreach (var player in Main.ActivePlayers)
            {
                if (NPC.Distance(player.Center) <= 150f)
                {
                    if (NPC.ai[0] == 0)
                    {
                        switch (Main.rand.Next(4))
                        {
                            //knives
                            case 0:
                            {
                                for (int numProjs = 0; numProjs <= 2; numProjs++)
                                {
                                    Vector2 PosToShootTo = new Vector2(NPC.Center.X, NPC.Center.Y - 30);

                                    Vector2 ShootSpeed = PosToShootTo - NPC.Center;
                                    ShootSpeed.Normalize();
                                    ShootSpeed *= 5f;

                                    Vector2 velocity = ShootSpeed.RotatedByRandom(MathHelper.ToRadians(65));

                                    NPCGlobalHelper.ShootHostileProjectile(NPC, NPC.Center - new Vector2(0, 15), velocity, ModContent.ProjectileType<KnifeCleaver>(), NPC.damage, 4.5f);
                                }

                                break;
                            }
                            //pipe bombs
                            case 1:
                            {
                                for (int numProjs = 0; numProjs <= 2; numProjs++)
                                {
                                    Vector2 PosToShootTo = new Vector2(NPC.Center.X, NPC.Center.Y - 30);

                                    Vector2 ShootSpeed = PosToShootTo - NPC.Center;
                                    ShootSpeed.Normalize();
                                    ShootSpeed *= 10f;

                                    Vector2 velocity = ShootSpeed.RotatedByRandom(MathHelper.ToRadians(32));

                                    NPCGlobalHelper.ShootHostileProjectile(NPC, NPC.Center - new Vector2(0, 15), velocity, ModContent.ProjectileType<PipeBomb>(), NPC.damage, 4.5f);
                                }

                                break;
                            }
                            //dumbbell
                            case 2:
                            {
                                Vector2 PosToShootTo = new Vector2(NPC.Center.X, NPC.Center.Y - 30);

                                Vector2 ShootSpeed = PosToShootTo - NPC.Center;
                                ShootSpeed.Normalize();
                                ShootSpeed *= 20f;

                                Vector2 velocity = ShootSpeed.RotatedByRandom(MathHelper.ToRadians(12));

                                NPCGlobalHelper.ShootHostileProjectile(NPC, NPC.Center - new Vector2(0, 15), velocity, ModContent.ProjectileType<Dumbbell>(), NPC.damage, 4.5f);

                                break;
                            }
                            //dirty needle
                            case 3:
                            {
                                for (int numProjs = 0; numProjs <= 1; numProjs++)
                                {
                                    Vector2 PosToShootTo = new Vector2(NPC.Center.X, NPC.Center.Y - 30);

                                    Vector2 ShootSpeed = PosToShootTo - NPC.Center;
                                    ShootSpeed.Normalize();
                                    ShootSpeed *= 6f;

                                    Vector2 velocity = ShootSpeed.RotatedByRandom(MathHelper.ToRadians(35));

                                    NPCGlobalHelper.ShootHostileProjectile(NPC, NPC.Center - new Vector2(0, 15), velocity, ModContent.ProjectileType<DirtyNeedle>(), NPC.damage, 4.5f);
                                }

                                break;
                            }
                        }

                        NPC.ai[0] = 1;
                    }
                }
            }

            if (NPC.ai[0] > 0)
            {
                NPC.alpha += 2;
                if (NPC.alpha >= 255)
                {
                    NPC.active = false;
                }
            }
        }
	}

    public class PresentTrapGreen : PresentTrapBlue
	{
        public override bool PreAI()
        {
            foreach (var player in Main.ActivePlayers)
            {
                if (NPC.Distance(player.Center) <= 150f)
                {
                    if (NPC.ai[1] == 0)
                    {
                        if (Main.netMode != NetmodeID.Server) 
                        {
                            Gore.NewGorePerfect(NPC.GetSource_FromAI(), NPC.position, new Vector2(Main.rand.Next(-2, 3), -3), ModContent.Find<ModGore>("Spooky/PresentTrapGreenLidGore").Type);
                        }

                        NPC.ai[1] = 1;
                    }
                }
            }

            return true;
        }
    }

    public class PresentTrapRed : PresentTrapBlue
	{
        public override bool PreAI()
        {
            foreach (var player in Main.ActivePlayers)
            {
                if (NPC.Distance(player.Center) <= 150f)
                {
                    if (NPC.ai[1] == 0)
                    {
                        if (Main.netMode != NetmodeID.Server) 
                        {
                            Gore.NewGorePerfect(NPC.GetSource_FromAI(), NPC.position, new Vector2(Main.rand.Next(-2, 3), -3), ModContent.Find<ModGore>("Spooky/PresentTrapRedLidGore").Type);
                        }

                        NPC.ai[1] = 1;
                    }
                }
            }

            return true;
        }
    }
}