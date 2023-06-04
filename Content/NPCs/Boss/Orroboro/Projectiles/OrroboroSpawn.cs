using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.Chat;
using Terraria.Audio;
using Microsoft.Xna.Framework;

namespace Spooky.Content.NPCs.Boss.Orroboro.Projectiles
{
    public class OrroboroSpawn : ModProjectile
    {
        public override string Texture => "Spooky/Content/Projectiles/Blank";

        public static int CrackTimer = 0;

        public static readonly SoundStyle EggDecaySound = new("Spooky/Content/Sounds/SpookyHell/EggDecay", SoundType.Sound);
        public static readonly SoundStyle EggCrackSound1 = new("Spooky/Content/Sounds/SpookyHell/EggCrack1", SoundType.Sound);
        public static readonly SoundStyle EggCrackSound2 = new("Spooky/Content/Sounds/SpookyHell/EggCrack2", SoundType.Sound);

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
            CrackTimer++;

            if (CrackTimer == 1)
            {
                SoundEngine.PlaySound(EggDecaySound, Projectile.Center);
            }
            
            if (CrackTimer == 100)
            {
                SoundEngine.PlaySound(EggCrackSound1, Projectile.Center);

                for (int numDust = 0; numDust < 50; numDust++)
                {
                    int DustGore = Dust.NewDust(new Vector2(Projectile.Center.X, Projectile.Center.Y), 
                    Projectile.width, Projectile.height, DustID.Blood, 0f, 0f, 100, default, 2f);

                    Main.dust[DustGore].scale *= Main.rand.NextFloat(1.2f, 2f);
                    Main.dust[DustGore].velocity *= Main.rand.NextFloat(3f, 5f);
                    Main.dust[DustGore].noGravity = true;

                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[DustGore].scale = 0.5f;
                        Main.dust[DustGore].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
            }

            if (CrackTimer >= 200)
            {
                SoundEngine.PlaySound(EggCrackSound2, Projectile.Center);

                //spawn orroboro with message
                int Orroboro = NPC.NewNPC(Projectile.GetSource_FromAI(), (int)Projectile.Center.X, (int)Projectile.Center.Y + 65, ModContent.NPCType<OrroHeadP1>(), 0, -1);

                //net update so it doesnt vanish on multiplayer
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.SyncNPC, number: Orroboro);
                }

                //spawn message
                string text = Language.GetTextValue("Mods.Spooky.EventsAndBosses.OrroboroSpawn");

                if (Main.netMode != NetmodeID.Server)
                {
                    Main.NewText(text, 171, 64, 255);
                }
                else
                {
                    ChatHelper.BroadcastChatMessage(NetworkText.FromKey(text), new Color(171, 64, 255));
                }

                //spawn egg gores to make it look like it broke
                Vector2 Position = new((int)Projectile.Center.X, (int)Projectile.Center.Y + 65);

                for (int numGores = 1; numGores <= 5; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server)
                    {
                        Gore.NewGore(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity, ModContent.Find<ModGore>("Spooky/EggShard" + numGores).Type, 1.2f);
                    }
                }

                CrackTimer = 0;
                Projectile.Kill();
            }
        }
    }
}