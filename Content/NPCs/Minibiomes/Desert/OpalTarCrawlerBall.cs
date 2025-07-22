using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.NPCs.Minibiomes.Desert
{
    public class OpalTarCrawlerBall : ModNPC  
    {
		private static Asset<Texture2D> NPCTexture;

		public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 10;
            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
        }

        public override void SetDefaults()
		{
			NPC.lifeMax = 1200;
            NPC.damage = 55;
			NPC.defense = 15;
			NPC.width = 72;
			NPC.height = 72;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 5, 0);
			NPC.noGravity = false;
			NPC.noTileCollide = false;
			NPC.HitSound = SoundID.Item95 with { Volume = 0.8f, Pitch = 1f };
			NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = 0;
        }

		public override void FindFrame(int frameHeight)
		{
			if (NPC.ai[0] == 1)
			{
				NPC.frameCounter++;
				if (NPC.frameCounter > 5)
				{
					NPC.frame.Y = NPC.frame.Y + frameHeight;
					NPC.frameCounter = 0;
				}
				if (NPC.frame.Y >= frameHeight * 10)
				{
					NPC.frame.Y = 9 * frameHeight;
				}
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			NPCTexture ??= ModContent.Request<Texture2D>(Texture);

			Main.EntitySpriteDraw(NPCTexture.Value, NPC.Center - screenPos, NPC.frame, NPC.GetNPCColorTintedByBuffs(NPC.GetAlpha(drawColor)), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);

			return false;
		}

		public override void ModifyHoverBoundingBox(ref Rectangle boundingBox)
		{
			boundingBox = new Rectangle((int)NPC.position.X - (NPC.width / 3), (int)NPC.position.Y, NPC.width, NPC.height);
		}

		public override void AI()
        {
			NPC.rotation += (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) * 0.01f;

			foreach (Player player in Main.ActivePlayers)
			{
				if (!player.dead && player.Distance(NPC.Center) < 200f && NPC.ai[0] <= 0)
				{
					NPC.velocity = new Vector2(Main.rand.Next(-10, 11), -12);

					NPC.ai[0] = 1;
				}
			}

			if (NPC.ai[0] == 1)
            {
				NPC.ai[1]++;
				if (NPC.ai[1] > 75)
				{
					int Ant = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y + (NPC.height / 2), ModContent.NPCType<OpalTarCrawler>());
					Main.npc[Ant].rotation = NPC.rotation - MathHelper.TwoPi;

					NPC.active = false;
				}
            }
        }

		public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0) 
            {
                NPC BestiaryParent = new();

                BestiaryParent.SetDefaults(ModContent.NPCType<OpalTarCrawler>());
                Main.BestiaryTracker.Kills.RegisterKill(BestiaryParent);

                for (int numDusts = 0; numDusts < 25; numDusts++)
                {                                                                                  
                    int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Asphalt, 0f, -2f, 0, default, 1f);
                    Main.dust[dust].position.X += Main.rand.Next(-25, 25) * 0.05f - 1.5f;
                    Main.dust[dust].position.Y += Main.rand.Next(-25, 25) * 0.05f - 1.5f;
                }

                for (int numGores = 1; numGores <= 2; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/OpalTarCrawlerBallGore" + numGores).Type);
                    }
                }

				for (int numGores = 1; numGores <= 6; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/OpalTarCrawlerBallGore3").Type);
                    }
                }
            }
        }
    }
}