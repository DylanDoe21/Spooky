using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

using Spooky.Content.Dusts;
using Spooky.Content.Items.Minibiomes.Vegetable;

namespace Spooky.Content.NPCs.Minibiomes.Vegetable
{
    public class RollingPepper : ModNPC  
    {
        private static Asset<Texture2D> NPCTexture;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;

			NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Position = new Vector2(0f, -10f),
                PortraitPositionXOverride = 0f,
                PortraitPositionYOverride = -10f
            };
        }

        public override void SetDefaults()
		{
            NPC.lifeMax = 80;
            NPC.damage = 25;
            NPC.defense = 0;
            NPC.width = 22;
			NPC.height = 22;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0f;
			NPC.noTileCollide = false;
			NPC.noGravity = false;
            NPC.behindTiles = true;
			NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.VegetableBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.RollingPepper"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.VegetableBiome>().ModBiomeBestiaryInfoElement)
			});
        }

        public override void FindFrame(int frameHeight)
        {   
            //running animation
            NPC.frameCounter++;
            if (NPC.frameCounter > 7)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 4)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPCTexture ??= ModContent.Request<Texture2D>(Texture);

            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

			spriteBatch.Draw(NPCTexture.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
			spriteBatch.Draw(NPCTexture.Value, NPC.Center - screenPos, NPC.frame, Color.Red * NPC.ai[2], NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

			return false;
        }

		public override void AI()
		{
			NPC.TargetClosest();
			Player player = Main.player[NPC.target];

			//dont charge at the player if this npc doesnt have line of sight
			bool HasLineOfSight = Collision.CanHitLine(player.position, player.width, player.height, NPC.position, NPC.width, NPC.height);

			if (HasLineOfSight || NPC.ai[0] > 10)
			{
				NPC.ai[0]++;
			}

			if (Collision.SolidTiles(NPC.position, NPC.width, NPC.height) && NPC.ai[0] < 130)
			{
				NPC.noGravity = true;
			}
			else
			{
				NPC.noGravity = false;
			}

			if (NPC.ai[0] < 10)
			{
				NPC.ai[2] = 0;

				if (Collision.SolidTiles(NPC.position, NPC.width, NPC.height))
				{
					NPC.velocity = Vector2.Zero;
				}
			}

			if (NPC.ai[0] > 10)
			{
				NPC.ai[1] += 0.005f;

				if (NPC.ai[2] < 1)
				{
					NPC.ai[2] += 0.01f;
				}
				
				if (NPC.ai[2] > 0.5f)
				{
					if (Main.rand.NextBool(10))
					{
						Color[] colors = new Color[] { Color.Gray, Color.DarkGray };

						int DustEffect = Dust.NewDust(NPC.position, NPC.width, 3, ModContent.DustType<SmokeEffect>(), 0f, 0f, 100, Main.rand.Next(colors) * 0.5f, 0.2f);
						Main.dust[DustEffect].velocity.X = 0;
						Main.dust[DustEffect].velocity.Y = -2;
						Main.dust[DustEffect].alpha = 100;
					}
				}

				NPC.rotation += NPC.ai[1] * (float)NPC.direction;
			}

			if (NPC.ai[0] == 130)
			{
				NPC.knockBackResist = 0.25f;

				NPC.noGravity = true;

				Vector2 ChargeDirection = new Vector2(player.Center.X, NPC.Center.Y - 250) - NPC.Center;
				ChargeDirection.Normalize();
				ChargeDirection *= 15;
				NPC.velocity = ChargeDirection;
			}

			if (NPC.ai[0] > 130)
			{
				if (Collision.SolidTiles(NPC.position, NPC.width, NPC.height))
				{
					NPC.knockBackResist = 0f;

					NPC.velocity = Vector2.Zero;
					NPC.ai[0] = 0;
					NPC.ai[1] = 0;
					NPC.ai[2] = 0;
				}
			}
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<PlantMulch>(), 3, 1, 2));
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0)
            {
                for (int numGores = 1; numGores <= 4; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/RollingPepperGore" + numGores).Type);
                    }
                }
            }
        }
    }
}