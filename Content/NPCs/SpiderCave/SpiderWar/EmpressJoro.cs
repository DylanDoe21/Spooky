using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Items.SpiderCave.Misc;

namespace Spooky.Content.NPCs.SpiderCave.SpiderWar
{
	public class EmpressJoro : ModNPC
	{
		List<CamelColonelLeg> legs;

		private static Asset<Texture2D> NPCTexture;
		private static Asset<Texture2D> FrontLegsTexture;
		private static Asset<Texture2D> BackLegsTexture;
		private static Asset<Texture2D> FrontLegsCarryTexture;
		private static Asset<Texture2D> BackLegsCarryTexture;

		public override void SetStaticDefaults()
        {
			Main.npcFrameCount[NPC.type] = 9;

			NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                //CustomTexturePath = "Spooky/Content/NPCs/NPCDisplayTextures/OgreKingBestiary",
				//Position = new Vector2(0f, 22f),
              	//PortraitPositionYOverride = 18f
            };
        }

		public override void SetDefaults()
		{
            NPC.lifeMax = 23000;
            NPC.damage = 70;
			NPC.defense = 40;
			NPC.width = 230;
			NPC.height = 110;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0f;
			NPC.value = Item.buyPrice(0, 0, 1, 0);
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.HitSound = SoundID.NPCHit29 with { Pitch = 0.4f };
			NPC.DeathSound = SoundID.NPCDeath36 with { Pitch = 0.4f };
			NPC.aiStyle = -1;
            //SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpiderCaveBiome>().Type };
		}

		/*
		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[Type], quickUnlock: true);

			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.Harvestmen"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpiderCaveBiome>().ModBiomeBestiaryInfoElement)
			});
		}
		*/

		public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter > 4)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 9)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			NPCTexture ??= ModContent.Request<Texture2D>(Texture);
			FrontLegsTexture ??= ModContent.Request<Texture2D>(Texture + "LegsFront");
			BackLegsTexture ??= ModContent.Request<Texture2D>(Texture + "LegsBack");
			FrontLegsCarryTexture ??= ModContent.Request<Texture2D>(Texture + "LegsFrontCarry");
			BackLegsCarryTexture ??= ModContent.Request<Texture2D>(Texture + "LegsBackCarry");

			var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

			Main.EntitySpriteDraw(BackLegsTexture.Value, NPC.Center - screenPos, NPC.frame, NPC.GetNPCColorTintedByBuffs(NPC.GetAlpha(drawColor)), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            Main.EntitySpriteDraw(NPCTexture.Value, NPC.Center - screenPos, NPC.frame, NPC.GetNPCColorTintedByBuffs(NPC.GetAlpha(drawColor)), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
			Main.EntitySpriteDraw(FrontLegsTexture.Value, NPC.Center - screenPos, NPC.frame, NPC.GetNPCColorTintedByBuffs(NPC.GetAlpha(drawColor)), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

			return false;
		}

		public override void AI()
		{
			NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

			NPC.spriteDirection = NPC.direction;
			NPC.rotation = NPC.velocity.X * 0.025f;

			if (NPC.Distance(player.Center - new Vector2(0, 0)) > 100f)
			{
				Vector2 desiredVelocity = NPC.DirectionTo(player.Center) * 5;
				NPC.velocity = Vector2.Lerp(NPC.velocity, desiredVelocity, 1f / 20);
			}

			/*
			attack ideas:

			1) Go off screen, grab a giant spider egg and drop it, causing it to spawn unique minions of some sort (only uses this if there isnt any of the minion existing)
			2) Carries a giant nuke-shaped web that blows up into lingering web everywhere that slows you down
			3) Fly from the side and attempt to charge at the player in a similar fashion to tarantula hawks
			*/
		}

		public override void HitEffect(NPC.HitInfo hit) 
        {
			//on death destroy the rest of the legs
			if (NPC.life <= 0)
            {
				if (Main.netMode != NetmodeID.Server) 
				{
					Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/HarvestmenGore").Type);
				}
			}
		}
	}
}