using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Content.Dusts;

namespace Spooky.Content.NPCs.Boss.Moco.Projectiles
{
    public class MocoSpawnerProj : ModProjectile
    {
        public override string Texture => "Spooky/Content/Projectiles/Blank";

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 5; 
            Projectile.alpha = 255;
        }

        public override bool CanHitPlayer(Player target)
        {
            return false;
        }

        public override void AI()
        {
            Spooky.MocoSpawnX = (int)Projectile.Center.X;
            Spooky.MocoSpawnY = (int)Projectile.Center.Y;

            if (Projectile.timeLeft == 2)
            {
                if (!NPC.AnyNPCs(ModContent.NPCType<MocoSpawner>()))
                {
                    if (Main.netMode != NetmodeID.SinglePlayer) 
                    {
                        ModPacket packet = Mod.GetPacket();
                        packet.Write((byte)SpookyMessageType.SpawnMoco);
                        packet.Send();
                    }
                    else 
                    {
                        NPC.NewNPC(Projectile.GetSource_FromAI(), (int)Projectile.Center.X, (int)Projectile.Center.Y, ModContent.NPCType<MocoSpawner>());
                    }
                }

                Projectile.netUpdate = true;
            }
        }
    }
}