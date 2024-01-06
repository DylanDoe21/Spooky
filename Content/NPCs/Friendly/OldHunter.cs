using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Personalities;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Utilities;

using Spooky.Content.Biomes;

namespace Spooky.Content.NPCs.Friendly
{
	[AutoloadHead]
	public class OldHunter : ModNPC
	{
		public const string ShopName = "Shop";

        public override void SetStaticDefaults() 
        {
			Main.npcFrameCount[Type] = 25; // The total amount of frames the NPC has
			NPCID.Sets.ExtraFramesCount[Type] = 9; // Generally for Town NPCs, but this is how the NPC does extra things such as sitting in a chair and talking to other NPCs. This is the remaining frames after the walking frames.
			NPCID.Sets.AttackFrameCount[Type] = 4; // The amount of frames in the attacking animation.
			NPCID.Sets.DangerDetectRange[Type] = 700; // The amount of pixels away from the center of the NPC that it tries to attack enemies.
			NPCID.Sets.AttackType[Type] = 0; // The type of attack the Town NPC performs. 0 = throwing, 1 = shooting, 2 = magic, 3 = melee
			NPCID.Sets.AttackTime[Type] = 90; // The amount of time it takes for the NPC's attack animation to be over once it starts.
			NPCID.Sets.AttackAverageChance[Type] = 30; // The denominator for the chance for a Town NPC to attack. Lower numbers make the Town NPC appear more aggressive.
			NPCID.Sets.HatOffsetY[Type] = 4; // For when a party is active, the party hat spawns at a Y offset	.

			// Influences how the NPC looks in the Bestiary
			NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers() 
            {
				Velocity = 1f,
				Direction = 1
			};
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);

			NPC.Happiness
			.SetBiomeAffection<SpiderCaveBiome>(AffectionLevel.Love)
			.SetBiomeAffection<CemeteryBiome>(AffectionLevel.Like)
			.SetBiomeAffection<ForestBiome>(AffectionLevel.Dislike)
			.SetBiomeAffection<HallowBiome>(AffectionLevel.Hate)
			.SetNPCAffection(NPCID.Pirate, AffectionLevel.Love)
			.SetNPCAffection(NPCID.Guide, AffectionLevel.Like)
			.SetNPCAffection(NPCID.Merchant, AffectionLevel.Dislike)
			.SetNPCAffection(NPCID.Demolitionist, AffectionLevel.Hate);
		}

		public override void SetDefaults() 
		{
			NPC.lifeMax = 250;
            NPC.damage = 10;
			NPC.defense = 15;
            NPC.width = 18;
			NPC.height = 40;
            NPC.knockBackResist = 0.5f;
            NPC.townNPC = true;
			NPC.friendly = true;
			NPC.HitSound = SoundID.NPCHit2;
			NPC.DeathSound = SoundID.NPCDeath2;
            NPC.aiStyle = 7;
			AnimationType = NPCID.Merchant;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpiderCaveBiome>().Type };
		}

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.OldHunter"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpiderCaveBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override bool CanTownNPCSpawn(int numTownNPCs) 
        {
            //for now he will always move in, will be changed when the mechanic to get him is made
			return true;
		}

        public override List<string> SetNPCNameList() 
        {
			return new List<string>() 
            {
				"Gerald",
				"Bone",
				"Hunt",
				"Spooky Skeleton"
			};
		}

		public override void SetChatButtons(ref string button, ref string button2)
		{
			button = Language.GetTextValue("LegacyInterface.28");
		}

		public override void OnChatButtonClicked(bool firstButton, ref string shop) 
		{
			if (firstButton) 
			{
				shop = ShopName; // Name of the shop tab we want to open.
			}
		}

		public override string GetChat()
		{
			//default dialogue options
			List<string> Dialogue = new List<string>
			{
				Language.GetTextValue("Mods.Spooky.Dialogue.OldHunter.Default1"),
			};

			return Main.rand.Next(Dialogue);
		}

		//teleports this npc whenever the king statue is activated like vanilla town npcs
		public override bool CanGoToStatue(bool toKingStatue) 
		{
			return true;
		}

		public override void TownNPCAttackStrength(ref int damage, ref float knockback) 
		{
			damage = 20;
			knockback = 4f;
		}

		public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown) 
		{
			cooldown = 30;
			randExtraCooldown = 30;
		}

		public override void TownNPCAttackProj(ref int projType, ref int attackDelay) 
		{
			projType = ProjectileID.BoneGloveProj;
			attackDelay = 1;
		}

		public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset) 
		{
			multiplier = 12f;
			randomOffset = 2f;
			// SparklingBall is not affected by gravity, so gravityCorrection is left alone.
		}
    }
}