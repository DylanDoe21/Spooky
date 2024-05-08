using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.Chat;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Content.Dusts;

namespace Spooky.Content.NPCs.Boss.SpookySpirit.Projectiles
{
    public class SpookySpiritSpawn : ModProjectile
    {
        public override string Texture => "Spooky/Content/Projectiles/Blank";

        public static readonly SoundStyle SpawnSound = new("Spooky/Content/Sounds/SpookySpirit/SpookySpiritSummon", SoundType.Sound);

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
            //make a trail of dust
            Vector2 dustPosition = Projectile.Center;
            dustPosition -= Projectile.velocity * 0.25f;
            int dust = Dust.NewDust(dustPosition, 1, 1, ModContent.DustType<GlowyDust>(), 0f, 0f, 0, default, 0.25f);
            Main.dust[dust].color = Color.BlueViolet;
            Main.dust[dust].noGravity = true;
            Main.dust[dust].position = dustPosition;
            Main.dust[dust].velocity *= 0.2f;

            Projectile.velocity *= 1.03f;

            Spooky.SpookySpiritSpawnX = (int)Projectile.Center.X;
            Spooky.SpookySpiritSpawnY = (int)Projectile.Center.Y;

            Projectile.ai[0]++;

            if (Projectile.ai[0] == 1)
            {
                SoundEngine.PlaySound(SoundID.DD2_DarkMageAttack, Projectile.Center);
                SoundEngine.PlaySound(SpawnSound, Projectile.Center);

                Projectile.netUpdate = true;
            }

            if (Projectile.ai[0] >= 85)
            {
                //spawn message
                string text = Language.GetTextValue("Mods.Spooky.EventsAndBosses.SpookySpiritSpawn");

                if (Main.netMode == NetmodeID.Server) 
                {
                    ChatHelper.BroadcastChatMessage(NetworkText.FromKey(text), new Color(171, 64, 255));

					ModPacket packet = Mod.GetPacket();
					packet.Write((byte)SpookyMessageType.SpawnSpookySpirit);
					packet.Send();
				}
				else if (Main.netMode == NetmodeID.SinglePlayer) 
                {
                    Main.NewText(text, 171, 64, 255);

					NPC.NewNPC(Projectile.GetSource_FromAI(), (int)Projectile.Center.X, (int)Projectile.Center.Y, ModContent.NPCType<SpookySpirit>());
                }

                for (int numDusts = 0; numDusts < 30; numDusts++)
                {
                    int dustGore = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<GlowyDust>(), 0f, -2f, 0, default, 0.35f);
                    Main.dust[dustGore].color = Color.BlueViolet;
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