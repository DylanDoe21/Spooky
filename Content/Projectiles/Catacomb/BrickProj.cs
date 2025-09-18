using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System;

using Spooky.Core;
using Spooky.Content.NPCs.Boss.Daffodil;

namespace Spooky.Content.Projectiles.Catacomb
{
    public class BrickProj : ModProjectile
    {
        public static readonly SoundStyle BonkSound = new("Spooky/Content/Sounds/BrickBonk", SoundType.Sound) { PitchVariance = 0.6f };

        public override void SetDefaults()
        {
            Projectile.width = 44;
            Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.netImportant = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
        }

        public override void AI()
        {
            Projectile.rotation += 0.25f * (float)Projectile.direction;

            Projectile.ai[0]++;
            if (Projectile.ai[0] >= 20)
            {
                Projectile.velocity.Y = Projectile.velocity.Y + 0.5f;
            }

            foreach (var npc in Main.ActiveNPCs)
			{
                if (npc != null && npc.type == ModContent.NPCType<DaffodilBody>() && npc.Distance(Projectile.Center) <= 100f)
                {
                    SoundEngine.PlaySound(BonkSound, Projectile.Center);

                    if (Main.netMode != NetmodeID.SinglePlayer)
                    {
                        ModPacket packet = Mod.GetPacket();
                        packet.Write((byte)SpookyMessageType.SpawnDaffodil);
                        packet.Send();
                    }
                    else
                    {
                        Flags.SpawnDaffodil = true;
                    }

                    Projectile.Kill();

                    break;
                }
            }
        }

        public override void OnKill(int timeLeft)
		{
            for (int numGores = 1; numGores <= 2; numGores++)
            {
                if (Main.netMode != NetmodeID.Server) 
                {
                    Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ModContent.Find<ModGore>("Spooky/BrickGore" + numGores).Type);
                }
            }
        }
    }
}