/*
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Spooky.Content.NPCs.SpookyBiome
{
	public class PuttyPumpkinling : ModNPC
	{
		float addedStretch = 0f;
		float landingRecoil = 0f;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Putty Pumpkinling");

			var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                CustomTexturePath = "Spooky/Content/NPCs/SpookyBiome/PuttyPumpkinling"
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
		}

		public override void SetDefaults()
		{
            NPC.lifeMax = 75;
            NPC.damage = 40;
            NPC.defense = 5;
            NPC.width = 18;
            NPC.height = 14;
			NPC.knockBackResist = 0.1f;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = -1;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpookyBiome>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Tiny and squishy little pumpkins that are spawned from the pumpkin putties. They will attempt to swarm foes that are attacking their creator."),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
			Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;

			float stretch = NPC.velocity.Y * 0.1f;

			stretch = Math.Abs(stretch) - addedStretch;
			
			//limit how much he can stretch
			if (stretch > 0.2f)
			{
				stretch = 0.2f;
			}

			//limit how much he can squish
			if (stretch < -0.2f)
			{
				stretch = -0.2f;
			}

			Vector2 scaleStretch = new Vector2(1f - stretch, 1f + stretch);
			
			if (NPC.velocity.Y <= 0)
			{
				scaleStretch = new Vector2(1f + stretch, 1f - stretch);
			}
			if (NPC.velocity.Y > 0)
			{
				scaleStretch = new Vector2(1f - stretch, 1f + stretch);
			}

			var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

			spriteBatch.Draw(tex, NPC.Center + new Vector2(0, NPC.height / 2 + NPC.gfxOffY) - Main.screenPosition, 
			NPC.frame, drawColor, NPC.rotation, new Vector2(NPC.width / 2, NPC.height), scaleStretch, effects, 0f);

			return false;
        }

		public override void AI()
		{
			Player player = Main.player[NPC.target];
			NPC.TargetClosest(true);

			NPC.spriteDirection = NPC.direction;

			if (landingRecoil > 0)
			{
				landingRecoil *= 0.965f;
				landingRecoil -= 0.02f;
			}
			else
			{
				landingRecoil = 0;
			}

			addedStretch = -landingRecoil;

			NPC.ai[0]++;

			Vector2 JumpTo = new(player.Center.X, player.Center.Y - 320);
			
			Vector2 velocity = JumpTo - NPC.Center;

			if (NPC.ai[0] < 30 && NPC.velocity.Y <= 0.1f)
			{
				NPC.velocity.X *= 0;
			}

			//actual jumping
			if (NPC.ai[0] >= 30 && NPC.ai[0] <= 35)
			{
				float speed = MathHelper.Clamp(velocity.Length() / 36, 4, 6);
				velocity.Normalize();
				velocity.Y -= 0.2f;
				velocity.X *= 1.2f;
				NPC.velocity = velocity * speed * 1.1f;
			}

			//fall on the ground
			if (NPC.ai[0] >= 40 && NPC.ai[1] == 0 && NPC.velocity.Y <= 0.1f)
			{
				landingRecoil = 0.5f;

				NPC.velocity.X *= 0;
				
				//complete the jump attack
				NPC.ai[1] = 1; 
			}

			//only loop attack if the jump has been completed
			if (NPC.ai[1] == 1)
			{
				NPC.ai[0] = 0;
				NPC.ai[1] = 0;
				NPC.netUpdate = true;
			}
		}

		public override void HitEffect(int hitDirection, double damage) 
        {
            if (NPC.life <= 0) 
            {
                for (int numDusts = 0; numDusts < 10; numDusts++)
                {
                    int DustGore = Dust.NewDust(NPC.Center, NPC.width / 2, NPC.height / 2, 288, 0f, 0f, 100, default, 2f);
                    Main.dust[DustGore].color = Color.MediumPurple;
					Main.dust[DustGore].scale = 0.5f;
                    Main.dust[DustGore].velocity *= 1.2f;

                    if (Main.rand.Next(2) == 0)
                    {
                        Main.dust[DustGore].scale = 0.5f;
                        Main.dust[DustGore].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
            }
		}
    }
}
*/