using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Content.Items.Pets;

namespace Spooky.Content.NPCs.SpookyBiome
{
	public class PuttySlime1 : ModNPC
	{
        private static Asset<Texture2D> NPCTexture;

        public override void SetStaticDefaults()
		{
			NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                CustomTexturePath = "Spooky/Content/NPCs/SpookyBiome/PuttySlime1"
            };
		}

		public override void SetDefaults()
		{
            NPC.lifeMax = 20;
            NPC.damage = 10;
            NPC.defense = 0;
            NPC.width = 34;
            NPC.height = 24;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.5f;
			NPC.value = Item.buyPrice(0, 0, 0, 25);
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.Item177;
			NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = 1;
			AIType = NPCID.BlueSlime;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpookyBiome>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.PuttySlime1"),
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPCTexture ??= ModContent.Request<Texture2D>(Texture);

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

			spriteBatch.Draw(NPCTexture.Value, NPC.Center + new Vector2(0, NPC.height / 2 + NPC.gfxOffY) - Main.screenPosition, 
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
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<PuttyCup>(), 100));
        }

		public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int numDusts = 0; numDusts < 20; numDusts++)
                {
                    int DustGore = Dust.NewDust(NPC.Center, 1, 1, DustID.TintableDust, 0f, 0f, 100, Color.LimeGreen, 1f);
                    Main.dust[DustGore].velocity *= 1.2f;
                }
            }
		}
    }

	public class PuttySlime2 : PuttySlime1
	{
		private static Asset<Texture2D> NPCTexture;

		public override void SetStaticDefaults()
		{
			NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                CustomTexturePath = "Spooky/Content/NPCs/SpookyBiome/PuttySlime2"
            };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.PuttySlime2"),
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyBiome>().ModBiomeBestiaryInfoElement)
			});
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPCTexture ??= ModContent.Request<Texture2D>(Texture);

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

			spriteBatch.Draw(NPCTexture.Value, NPC.Center + new Vector2(0, NPC.height / 2 + NPC.gfxOffY) - Main.screenPosition, 
            NPC.frame, drawColor, NPC.rotation, new Vector2(NPC.width / 2, NPC.height), scaleStretch, effects, 0f);

			return false;
        }

		public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int numDusts = 0; numDusts < 20; numDusts++)
                {
                    int DustGore = Dust.NewDust(NPC.Center, 1, 1, DustID.TintableDust, 0f, 0f, 100, Color.MediumPurple, 1f);
                    Main.dust[DustGore].velocity *= 1.2f;
                }
            }
		}
	}

	public class PuttySlime3 : PuttySlime1
	{
		private static Asset<Texture2D> NPCTexture;

		public override void SetStaticDefaults()
		{
			NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                CustomTexturePath = "Spooky/Content/NPCs/SpookyBiome/PuttySlime3"
            };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.PuttySlime3"),
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyBiome>().ModBiomeBestiaryInfoElement)
			});
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPCTexture ??= ModContent.Request<Texture2D>(Texture);

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

			spriteBatch.Draw(NPCTexture.Value, NPC.Center + new Vector2(0, NPC.height / 2 + NPC.gfxOffY) - Main.screenPosition, 
            NPC.frame, drawColor, NPC.rotation, new Vector2(NPC.width / 2, NPC.height), scaleStretch, effects, 0f);

			return false;
        }

		public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int numDusts = 0; numDusts < 20; numDusts++)
                {
                    int DustGore = Dust.NewDust(NPC.Center, 1, 1, DustID.TintableDust, 0f, 0f, 100, Color.Chocolate, 1f);
                    Main.dust[DustGore].velocity *= 1.2f;
                }
            }
		}
	}
}