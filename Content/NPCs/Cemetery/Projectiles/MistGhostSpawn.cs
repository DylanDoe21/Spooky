using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Content.Dusts;

namespace Spooky.Content.NPCs.Cemetery.Projectiles
{
    public class MistGhostSpawn : ModProjectile
    {
        public override string Texture => "Spooky/Content/Projectiles/Blank";

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 300;
            Projectile.alpha = 255;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            Spooky.MistGhostSpawnX = (int)Projectile.Center.X;
            Spooky.MistGhostSpawnY = (int)Projectile.Center.Y;

			int[] GhostTypes = new int[] { ModContent.NPCType<MistGhost>(), ModContent.NPCType<MistGhostFaces>(), ModContent.NPCType<MistGhostWiggle>() };

			if (Projectile.ai[0] == 0)
			{
				if (Main.netMode != NetmodeID.SinglePlayer)
				{
					ModPacket packet = Mod.GetPacket();
					packet.Write((byte)SpookyMessageType.SpawnMistGhost);
					packet.Send();
				}
				else
				{
					for (int numNPCs = 0; numNPCs < 3; numNPCs++)
					{
						int NewNPC = NPC.NewNPC(Projectile.GetSource_FromAI(), (int)Projectile.Center.X, (int)Projectile.Center.Y, Main.rand.Next(GhostTypes));
						Main.npc[NewNPC].velocity.X = Main.rand.Next(-10, 11);
						Main.npc[NewNPC].velocity.Y = Main.rand.Next(-10, -5);
					}
				}

				for (int numDusts = 0; numDusts < 30; numDusts++)
				{
					int dustGore = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<GlowyDust>(), 0f, -2f, 0, default, 0.35f);
					Main.dust[dustGore].color = Color.OrangeRed;
					Main.dust[dustGore].velocity.X *= Main.rand.NextFloat(-5f, 5f);
					Main.dust[dustGore].velocity.Y *= Main.rand.NextFloat(-3f, 3f);
					Main.dust[dustGore].noGravity = true;
				}

				Projectile.ai[0]++;

				Projectile.netUpdate = true;
			}
			else
			{
				if (NPC.AnyNPCs(ModContent.NPCType<MistGhost>()) || NPC.AnyNPCs(ModContent.NPCType<MistGhostFaces>()) || NPC.AnyNPCs(ModContent.NPCType<MistGhostWiggle>()))
				{
					Projectile.timeLeft = 25;
				}
				else
				{
					WorldGen.KillTile((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16, fail: false);
				}
			}
        }
	}
}