using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Content.Items.Catacomb;
using Spooky.Content.Items.Food;

namespace Spooky.Content.NPCs.Catacomb.Layer1
{
	public class GiantPutty : ModNPC
	{
        float stretch;

		public override void SetStaticDefaults()
		{
			NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                CustomTexturePath = "Spooky/Content/NPCs/Catacomb/Layer1/GiantPutty"
            };
		}

		public override void SetDefaults()
		{
            NPC.lifeMax = 75;
            NPC.damage = 22;
            NPC.defense = 0;
            NPC.width = 56;
            NPC.height = 50;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.15f;
			NPC.value = Item.buyPrice(0, 0, 2, 50);
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.Item177;
			NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = 3;
			AIType = NPCID.ZombieMushroom;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.CatacombBiome>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.GiantPutty"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CatacombBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
			Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;

            float time = Main.GlobalTimeWrappedHourly * 3;

			stretch = Math.Abs(stretch);

			Vector2 scaleStretch = new Vector2(1f - stretch, 1f + stretch);

			var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

			spriteBatch.Draw(tex, NPC.Center + new Vector2(0, NPC.height / 2 + NPC.gfxOffY) - Main.screenPosition, 
			NPC.frame, drawColor, NPC.rotation, new Vector2(NPC.width / 2, NPC.height), scaleStretch, effects, 0f);

			return false;
        }

		public override void AI()
		{
			NPC.spriteDirection = NPC.direction;

            NPC.localAI[0] += 2;

            if (NPC.localAI[0] <= 10)
            {
                stretch -= 0.002f;
            }
            else if (NPC.localAI[0] <= 30 && NPC.localAI[0] > 10)
            {
                stretch -= 0.02f;
            }
            else if (NPC.localAI[0] <= 40)
            {
                stretch += 0.002f;
            }
            else if (NPC.localAI[0] <= 60 && NPC.localAI[0] > 40)
            {
                stretch += 0.02f;
            }

            if (NPC.localAI[0] >= 60)
            {
                NPC.localAI[0] = 0;
            }
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.Gel, 1, 2, 5));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<RustyRing>(), 20));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CandyCorn>(), 100));
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int numDusts = 0; numDusts < 25; numDusts++)
                {
                    int DustGore = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.TintableDust, 0f, 0f, 100, default, 0.85f);
                    Main.dust[DustGore].color = Color.Teal;
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