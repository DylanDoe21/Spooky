using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;
using Spooky.Content.NPCs.Quest.Projectiles;

namespace Spooky.Content.NPCs.Quest
{
	public class EyeWizardOrb : ModNPC
	{
		private static Asset<Texture2D> NPCTexture;

		public override void SetStaticDefaults()
        {
			NPCID.Sets.CantTakeLunchMoney[Type] = true;
        }

		public override void SetDefaults()
		{
            NPC.lifeMax = 250;
            NPC.damage = 0;
			NPC.defense = 0;
			NPC.width = 28;
			NPC.height = 30;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0f;
			NPC.noTileCollide = true;
			NPC.noGravity = true;
			NPC.HitSound = SoundID.Tink with { Volume = 0.75f, Pitch = 1.25f };
			NPC.DeathSound = SoundID.Shatter;
			NPC.aiStyle = -1;
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
			NPCTexture ??= ModContent.Request<Texture2D>(Texture);

            float fade = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.4f / 2.4f * 6f)) / 2f + 0.5f;

            var effects = NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Vector2 drawPosition = new Vector2(NPC.Center.X, NPC.Center.Y) - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4);
			Color color = new Color(127 - NPC.alpha, 127 - NPC.alpha, 127 - NPC.alpha, 0).MultiplyRGBA(Color.White);

            for (int repeats = 0; repeats < 4; repeats++)
            {
                Vector2 afterImagePosition = new Vector2(NPC.Center.X, NPC.Center.Y) + NPC.rotation.ToRotationVector2() - screenPos + new Vector2(0, NPC.gfxOffY + 4) - NPC.velocity * repeats;
                Main.spriteBatch.Draw(NPCTexture.Value, afterImagePosition, NPC.frame, color * fade, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale * 1.2f, effects, 0f);
            }

            Main.spriteBatch.Draw(NPCTexture.Value, drawPosition, NPC.frame, color, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale * 1.2f, effects, 0f);

            return true;
		}

		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }

		public override void AI()
		{
			NPC Parent = Main.npc[(int)NPC.ai[0]];

			Parent.direction = NPC.Center.X < Parent.Center.X ? -1 : 1;

			NPC.rotation += 0.05f * (float)Parent.direction;

			if (NPC.ai[1] == 0)
			{
				NPC.ai[3] = NPC.position.Y;
				NPC.ai[1]++;
			}

			NPC.ai[2]++;
			NPC.position.Y = NPC.ai[3] + (float)Math.Sin(NPC.ai[2] / 30) * 30;
		}

		public override void HitEffect(NPC.HitInfo hit)
        {
			//spawn gores here
            if (NPC.life <= 0) 
            {
				SpookyPlayer.ScreenShakeAmount = 5;

				NPC Parent = Main.npc[(int)NPC.ai[0]];

				for (int numProjectiles = 0; numProjectiles < 6; numProjectiles++)
			    {
                    Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, new Vector2(Main.rand.Next(-12, 13), Main.rand.Next(-7, -3)), ModContent.ProjectileType<EyeOrbEnergy>(), 0, 0, Main.myPlayer, Parent.whoAmI);
                }

				for (int numGores = 1; numGores <= 4; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, new Vector2(Main.rand.Next(-12, 12), Main.rand.Next(-12, -5)), ModContent.Find<ModGore>("Spooky/EyeWizardOrbGore" + numGores).Type);
                    }
                }
			}
		}
	}
}