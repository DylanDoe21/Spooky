using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.Localization;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.UserInterfaces;

namespace Spooky.Content.NPCs.Friendly
{
	[AutoloadHead]
	public class LittleEye : ModNPC
	{
		public Vector2 modifier = new(-200, -75);

		public static readonly SoundStyle TalkSound = new("Spooky/Content/Sounds/Krampus/Talk", SoundType.Sound) { Volume = 0.35f, Pitch = 1.5f, PitchVariance = 0.75f };

		private static Asset<Texture2D> UITexture;

		private static int ShimmerHeadIndex;
        private static Profiles.StackedNPCProfile NPCProfile;

		public override void Load()
		{
			UITexture = ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/DialogueUILittleEye");
			ShimmerHeadIndex = Mod.AddNPCHeadTexture(Type, Texture + "_Shimmer_Head");
		}

		public override void SetStaticDefaults()
		{
            NPCID.Sets.ActsLikeTownNPC[Type] = true;
            NPCID.Sets.ShimmerTownTransform[Type] = true;
            NPCID.Sets.NoTownNPCHappiness[Type] = true;
            Main.npcFrameCount[NPC.type] = 5;

            NPCProfile = new Profiles.StackedNPCProfile(new Profiles.DefaultNPCProfile(Texture, NPCHeadLoader.GetHeadSlot(HeadTexture)), new Profiles.DefaultNPCProfile(Texture + "_Shimmer", ShimmerHeadIndex));
        }

        public override ITownNPCProfile TownNPCProfile()
        {
            return NPCProfile;
        }

        public override void SetDefaults()
		{
            NPC.lifeMax = 250;
			NPC.damage = 0;
			NPC.defense = 25;
            NPC.width = 20;
			NPC.height = 50;
			NPC.townNPC = true;
			NPC.friendly = true;
			NPC.immortal = true;
			NPC.dontTakeDamage = true;
			NPC.dontCountMe = true;
			TownNPCStayingHomeless = true;
            NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = 7;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpookyHellBiome>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.LittleEye"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyHellBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter > 6)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 5)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
        }

		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
		{
			return false;
		}

		public override bool CanChat() 
        {
			return true;
		}

		public override string GetChat()
		{
			LittleEyeDialogueChoiceUI.LittleEye = NPC.whoAmI;
			LittleEyeDialogueChoiceUI.UIOpen = true;
            return string.Empty;
		}

		public override bool CheckActive()
		{
			return false;
		}

		public override bool NeedSaving()
		{
			return true;
		}

		public override void AI()
		{
			NPC.TargetClosest(true);

			NPC.spriteDirection = NPC.direction;

			NPC.velocity.X = 0;
		}
	}
}