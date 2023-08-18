using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.Chat;
using Terraria.Audio;
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
            Projectile.timeLeft = 3600; 
            Projectile.alpha = 255;
        }

        public override bool CanHitPlayer(Player target)
        {
            return false;
        }

        public override void AI()
        {
            Spooky.MistGhostSpawnX = (int)Projectile.Center.X;
            Spooky.MistGhostSpawnY = (int)Projectile.Center.Y;

            Projectile.ai[0]++;

            if (Projectile.ai[0] >= 1)
            {
                if (Main.netMode != NetmodeID.SinglePlayer) 
				{
                    ModPacket packet = Mod.GetPacket();
					packet.Write((byte)SpookyMessageType.SpawnSpookySpirit);
					packet.Send();
				}
				else 
				{
                    NPC.NewNPC(Projectile.GetSource_FromAI(), (int)Projectile.Center.X, (int)Projectile.Center.Y, ModContent.NPCType<MistGhost>());
				}

                for (int numDusts = 0; numDusts < 30; numDusts++)
                {
                    int dustGore = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<GlowyDust>(), 0f, -2f, 0, default, 0.35f);
                    Main.dust[dustGore].color = Color.OrangeRed;
                    Main.dust[dustGore].velocity.X *= Main.rand.NextFloat(-5f, 5f);
                    Main.dust[dustGore].velocity.Y *= Main.rand.NextFloat(-3f, 3f);
                    Main.dust[dustGore].noGravity = true;
                }

                Projectile.netUpdate = true;

                Projectile.Kill();
            }
        }
    }
}