using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Content.Dusts;
using Spooky.Content.NPCs.PandoraBox;

namespace Spooky.Content.NPCs.PandoraBox.Projectiles
{
    public class PandoraEnemySpawn : ModProjectile
    {
        public override string Texture => "Spooky/Content/Projectiles/Blank";

        int EnemyType;

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 2000;
            Projectile.penetrate = 1;
            Projectile.alpha = 255;
        }

        public override void AI()
        {
            Projectile.velocity.Y += 0.1f;

            Vector2 position = Projectile.Center + Vector2.Normalize(Projectile.velocity);

            int newDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.HallowSpray, 0f, 0f, 0, default, 1.2f);
			Main.dust[newDust].position = position;
			Main.dust[newDust].fadeIn = 0.5f;
			Main.dust[newDust].noGravity = true;

            Projectile.ai[1]++;

            if (Projectile.ai[1] >= 60)
            {
                //spawn enemy depending on what ai[0] is set to
                switch ((int)Projectile.ai[0])
                {
                    //Bobbert
                    case 0:
                    {
                        EnemyType = NPC.NewNPC(Projectile.GetSource_Death(), (int)Projectile.Center.X, (int)Projectile.Center.Y, ModContent.NPCType<Bobbert>());

                        break;
                    }

                    //Stitch
                    case 1:
                    {
                        EnemyType = NPC.NewNPC(Projectile.GetSource_Death(), (int)Projectile.Center.X, (int)Projectile.Center.Y, ModContent.NPCType<Stitch>());

                        break;
                    }

                    //Sheldon
                    case 2:
                    {
                        EnemyType = NPC.NewNPC(Projectile.GetSource_Death(), (int)Projectile.Center.X, (int)Projectile.Center.Y, ModContent.NPCType<Sheldon>());
                        
                        break;
                    }

                    //Chester
                    case 3:
                    {
                        EnemyType = NPC.NewNPC(Projectile.GetSource_Death(), (int)Projectile.Center.X, (int)Projectile.Center.Y, ModContent.NPCType<Chester>());
                        
                        break;
                    }
                }

                NetMessage.SendData(MessageID.SyncNPC, number: EnemyType);

                Projectile.Kill();
            }
        }

        public override void Kill(int timeLeft)
		{
            if (!PandoraBoxWorld.SpawnedEnemySpawners)
			{
				PandoraBoxWorld.SpawnedEnemySpawners = true;

				if (Main.netMode == NetmodeID.Server)
				{
					NetMessage.SendData(MessageID.WorldData);
				}
			}
        }
    }
}