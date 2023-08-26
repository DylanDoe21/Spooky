using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System;

using Spooky.Content.Dusts;
using Spooky.Content.Items.Catacomb;
using Spooky.Content.Items.Pets;

namespace Spooky.Content.NPCs.PandoraBox.Projectiles
{
    public class PandoraLootSpawner : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 72;
            Projectile.height = 72;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 3600;
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
            int dust = Dust.NewDust(dustPosition, 1, 1, ModContent.DustType<GlowyDust>(), 0f, 0f, 0, default, 0.05f);
            Main.dust[dust].color = Color.Cyan;
            Main.dust[dust].noGravity = true;
            Main.dust[dust].position = dustPosition;
            Main.dust[dust].velocity *= 0.2f;

            Projectile.scale = 1f * (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly * 25));

            Projectile.ai[0]++;

            if (Projectile.ai[0] < 35)
            {
                Projectile.velocity *= 1.03f;
            }

            if (Projectile.ai[0] >= 75)
            {
                SoundEngine.PlaySound(SoundID.AbigailUpgrade, Projectile.Center);

                //drop one of the pandora accessories
                int[] Accessories = new int[] { ModContent.ItemType<PandoraChalice>(), ModContent.ItemType<PandoraCross>(), 
                ModContent.ItemType<PandoraCuffs>(), ModContent.ItemType<PandoraRosary>() };

                int newItem = Item.NewItem(Projectile.GetSource_DropAsItem(), Projectile.Hitbox, Main.rand.Next(Accessories));

                if (Main.netMode == NetmodeID.MultiplayerClient && newItem >= 0)
                {
                    NetMessage.SendData(MessageID.SyncItem, -1, -1, null, newItem, 1f);
                }

                //chance to drop the funny bean
                if (Main.rand.NextBool(20))
                {
                    int FunnyBean = Item.NewItem(Projectile.GetSource_DropAsItem(), Projectile.Hitbox, ModContent.ItemType<PandoraBean>());

                    if (Main.netMode == NetmodeID.MultiplayerClient && FunnyBean >= 0)
                    {
                        NetMessage.SendData(MessageID.SyncItem, -1, -1, null, FunnyBean, 1f);
                    }
                }

                for (int numDusts = 0; numDusts < 30; numDusts++)
                {
                    int dustGore = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<GlowyDust>(), 0f, -2f, 0, default, 0.35f);
                    Main.dust[dustGore].color = Color.Cyan;
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