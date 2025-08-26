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

using Spooky.Content.Items.Minibiomes.Ocean;

namespace Spooky.Content.NPCs.Minibiomes.Ocean
{
	public class SkeletonSunfish : ModNPC
	{
        Vector2 SavePosition = Vector2.Zero;

        private static Asset<Texture2D> NPCTexture;

		public override void SetStaticDefaults()
		{
			NPCID.Sets.CountsAsCritter[Type] = true;
		}

		public override void SetDefaults()
		{
            NPC.lifeMax = 50;
			NPC.damage = 0;
			NPC.defense = 10;
			NPC.width = 88;
			NPC.height = 50;
			NPC.npcSlots = 0.5f;
			NPC.knockBackResist = 0.35f;
			NPC.noGravity = true;
            NPC.dontTakeDamageFromHostiles = false;
			NPC.HitSound = SoundID.DD2_SkeletonHurt;
			NPC.DeathSound = SoundID.DD2_SkeletonHurt;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.ZombieOceanBiome>().Type };
		}

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>
			{
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.SkeletonSunfish"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.ZombieOceanBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPCTexture ??= ModContent.Request<Texture2D>(Texture);

			//draw aura
			Vector2 drawOrigin = new(NPCTexture.Width() * 0.5f, NPC.height * 0.5f);

            var effects = NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            //draw aura
            for (int i = 0; i < 360; i += 30)
            {
                Color color = new Color(125 - NPC.alpha, 125 - NPC.alpha, 125 - NPC.alpha, 0).MultiplyRGBA(Color.Lime);

                Vector2 circular = new Vector2(2.5f, 0).RotatedBy(MathHelper.ToRadians(i));

                spriteBatch.Draw(NPCTexture.Value, NPC.Center + circular - screenPos, NPC.frame, color, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);
            }

            spriteBatch.Draw(NPCTexture.Value, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);

            return false;
		}

		public override void AI()
		{
			if (NPC.velocity.X > 0f)
			{
				NPC.direction = 1;
			}
			if (NPC.velocity.X < 0f)
			{
				NPC.direction = -1;
			}

			NPC.rotation = NPC.velocity.Y * (NPC.direction == 1 ? 0.05f : -0.05f);

			if (SavePosition == Vector2.Zero)
			{
				NPC.ai[0] = Main.rand.Next(-400, 400);
				SavePosition = NPC.Center;
			}

			SkeletonFish.FishSwimmingAI(NPC, SavePosition, 35, 35, 0.5f, 0.5f, 0.01f, 0.01f);
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FishboneChunk>(), 3));
		}

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 4; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/SkeletonSunfishGore" + numGores).Type);
                    }
                }
            }
        }
	}
}