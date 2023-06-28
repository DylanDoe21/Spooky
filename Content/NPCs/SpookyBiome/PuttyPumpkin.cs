using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Content.Tiles.Banner.SpookyForest;

namespace Spooky.Content.NPCs.SpookyBiome
{
	public class PuttyPumpkin : ModNPC
	{
		public override void SetStaticDefaults()
		{
			var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                CustomTexturePath = "Spooky/Content/NPCs/SpookyBiome/PuttyPumpkin"
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
		}

		public override void SetDefaults()
		{
            NPC.lifeMax = 150;
            NPC.damage = 40;
            NPC.defense = 5;
            NPC.width = 44;
            NPC.height = 34;
			NPC.knockBackResist = 0.35f;
			NPC.value = Item.buyPrice(0, 0, 1, 50);
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.Item177;
			NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = 1;
			AIType = NPCID.HoppinJack;
			Banner = NPC.type;
            BannerItem = ModContent.ItemType<PuttyPumpkinBanner>();
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpookyBiomeUg>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.PuttyPumpkin"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyBiomeUg>().ModBiomeBestiaryInfoElement)
			});
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            Player player = spawnInfo.Player;

			if (!spawnInfo.Invasion && Main.invasionType == 0 && !Main.pumpkinMoon && !Main.snowMoon && !Main.eclipse &&
            !(player.ZoneTowerSolar || player.ZoneTowerVortex || player.ZoneTowerNebula || player.ZoneTowerStardust))
            {
                if (player.InModBiome(ModContent.GetInstance<Biomes.SpookyBiomeUg>()) && Main.hardMode)
                {
                    return 10f;
                }
            }

            return 0f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
			Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;

			float stretch = NPC.velocity.Y * 0.45f;

			stretch = Math.Abs(stretch);

			//limit how much it can stretch
			if (stretch > 0.12f)
			{
				stretch = 0.12f;
			}

			//limit how much it can squish
			if (stretch < -0.12f)
			{
				stretch = -0.12f;
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
			NPC.spriteDirection = NPC.direction;
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.Gel, 1, 1, 3));
        }

		public override void HitEffect(NPC.HitInfo hit) 
        {
			//randomly spawn pumpkinlings on hit
            if (Main.rand.NextBool(3))
            {
                NPC.NewNPC(NPC.GetSource_OnHit(NPC), (int)NPC.Center.X + Main.rand.Next(-20, 20),
                (int)NPC.Center.Y + Main.rand.Next(-10, 5), ModContent.NPCType<PuttyPumpkinling>());
            }

            if (NPC.life <= 0) 
            {
				//spawn more pumpkinlings on death
				for (int numSlimes = 0; numSlimes < 3; numSlimes++)
				{
					NPC.NewNPC(NPC.GetSource_OnHit(NPC), (int)NPC.Center.X + Main.rand.Next(-20, 20), 
					(int)NPC.Center.Y + Main.rand.Next(-10, 5), ModContent.NPCType<PuttyPumpkinling>());
				}

                for (int numDusts = 0; numDusts < 15; numDusts++)
                {
                    int DustGore = Dust.NewDust(NPC.Center, 1, 1, DustID.TintableDust, 0f, 0f, 100, default, 1f);
                    Main.dust[DustGore].color = Color.Orange;
                    Main.dust[DustGore].velocity *= 1.2f;

                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[DustGore].scale = 0.5f;
                        Main.dust[DustGore].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
            }
		}
    }
}