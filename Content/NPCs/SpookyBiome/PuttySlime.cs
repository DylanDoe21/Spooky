using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Spooky.Content.NPCs.SpookyBiome
{
	public class PuttySlime1 : ModNPC
	{
		float addedStretch = 0f;
		float landingRecoil = 0f;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Putty Slime");

			var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                CustomTexturePath = "Spooky/Content/NPCs/SpookyBiome/PuttySlime1"
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
		}

		public override void SetDefaults()
		{
            NPC.lifeMax = 30;
            NPC.damage = 15;
            NPC.defense = 5;
            NPC.width = 34;
            NPC.height = 24;
			NPC.knockBackResist = 0.45f;
			NPC.value = Item.buyPrice(0, 0, 0, 25);
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
				new FlavorTextBestiaryInfoElement("Little blobs of putty with spooky faces that hop around the spooky forest, attempting to frighten anyone that comes across them."),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyBiome>().ModBiomeBestiaryInfoElement)
			});
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            Player player = spawnInfo.Player;

			if (!spawnInfo.Invasion && Main.invasionType == 0 && !Main.pumpkinMoon && !Main.snowMoon && !Main.eclipse &&
            !(player.ZoneTowerSolar || player.ZoneTowerVortex || player.ZoneTowerNebula || player.ZoneTowerStardust))
            {
                if (player.InModBiome(ModContent.GetInstance<Biomes.SpookyBiome>()) && Main.dayTime)
                {
                    return 10f;
                }
            }

            return 0f;
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

			Vector2 JumpTo = new(player.Center.X + (NPC.Center.X > player.Center.X ? 150 : -150), player.Center.Y - 400);

			if (NPC.position.X <= player.Center.X + 200 && NPC.position.X >= player.Center.X - 200)
			{
				JumpTo = new(player.Center.X, player.Center.Y - 400);
			}

			Vector2 velocity = JumpTo - NPC.Center;

			if (NPC.ai[0] < 60 && NPC.velocity.Y <= 0.1f)
			{
				NPC.velocity.X *= 0;
			}

			//actual jumping
			if (NPC.ai[0] >= 60 && NPC.ai[0] <= 65)
			{
				float speed = MathHelper.Clamp(velocity.Length() / 36, 3, 5);
				velocity.Normalize();
				velocity.Y -= 0.2f;
				velocity.X *= Main.rand.NextFloat(1.2f, 1.32f);
				NPC.velocity = velocity * speed * 1.1f;
			}

			//fall on the ground
			if (NPC.ai[0] >= 90 && NPC.ai[1] == 0 && NPC.velocity.Y <= 0.1f)
			{
				landingRecoil = 0.5f;

				NPC.velocity.X *= 0;
				
				//complete the slam attack
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

		public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.Gel, 1, 1, 2));
        }

		public override void HitEffect(int hitDirection, double damage) 
        {
            if (NPC.life <= 0) 
            {
                for (int numDusts = 0; numDusts < 20; numDusts++)
                {
                    int DustGore = Dust.NewDust(NPC.Center, NPC.width / 2, NPC.height / 2, 4, 0f, 0f, 100, default, 2f);
                    Main.dust[DustGore].color = Color.LimeGreen;
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

	public class PuttySlime2 : PuttySlime1
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Putty Slime");

			var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                CustomTexturePath = "Spooky/Content/NPCs/SpookyBiome/PuttySlime2"
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
		}

		public override void HitEffect(int hitDirection, double damage) 
        {
            if (NPC.life <= 0) 
            {
                for (int numDusts = 0; numDusts < 20; numDusts++)
                {
                    int DustGore = Dust.NewDust(NPC.Center, NPC.width / 2, NPC.height / 2, 4, 0f, 0f, 100, default, 2f);
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

	public class PuttySlime3 : PuttySlime1
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Putty Slime");

			var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                CustomTexturePath = "Spooky/Content/NPCs/SpookyBiome/PuttySlime3"
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
		}

		public override void HitEffect(int hitDirection, double damage) 
        {
            if (NPC.life <= 0) 
            {
                for (int numDusts = 0; numDusts < 20; numDusts++)
                {
                    int DustGore = Dust.NewDust(NPC.Center, NPC.width / 2, NPC.height / 2, 4, 0f, 0f, 100, default, 2f);
                    Main.dust[DustGore].color = Color.Chocolate;
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