using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Content.Dusts;
using Spooky.Content.NPCs.PandoraBox;

namespace Spooky.Content.NPCs.PandoraBox.Projectiles
{
    public class PandoraEnemySpawnChester : ModProjectile
    {
        public override string Texture => "Spooky/Content/Projectiles/Blank";

        int EnemyType;

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 2000;
            Projectile.penetrate = 1;
            Projectile.alpha = 255;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
		{
			return false;
		}

        public override void AI()
        {
            Projectile.velocity.Y += 0.1f;

            Vector2 position = Projectile.Center + Vector2.Normalize(Projectile.velocity);

            int newDust = Dust.NewDust(Projectile.position, Projectile.width / 2, Projectile.height / 2, DustID.HallowSpray, 0f, 0f, 0, default, 1.2f);
			Main.dust[newDust].position = position;
			Main.dust[newDust].fadeIn = 0.5f;
			Main.dust[newDust].noGravity = true;

            Projectile.ai[1]++;

            if (Projectile.ai[1] >= 60)
            {
                //chance to spawn stitch or sheldon, otherwise spawn a bobbert
                if (Main.rand.NextBool(8))
                {
                    switch (Main.rand.Next(2))
                    {
                        //Stitch
                        case 0:
                        {
                            EnemyType = ModContent.NPCType<Stitch>();
                            break;
                        }

                        //Sheldon
                        case 1:
                        {
                            EnemyType = ModContent.NPCType<Sheldon>();
                            break;
                        }
                    }
                }
                else
                {
                    EnemyType = ModContent.NPCType<Bobbert>();
                }

                int NewEnemy = NPC.NewNPC(Projectile.GetSource_Death(), (int)Projectile.Center.X, (int)Projectile.Center.Y, EnemyType);

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NetMessage.SendData(MessageID.SyncNPC, number: NewEnemy);
                }

                PandoraBoxWorld.SpawnedEnemySpawners = true;

                Projectile.Kill();
            }
        }
    }
}