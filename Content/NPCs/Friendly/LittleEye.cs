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
		bool PlaySound = false;

		private static int ShimmerHeadIndex;
        private static Profiles.StackedNPCProfile NPCProfile;

		private static Asset<Texture2D> NPCTexture;
		private static Asset<Texture2D> CryTexture;

		public static readonly SoundStyle PokeSound = new("Spooky/Content/Sounds/SpearfishPoke", SoundType.Sound);

		public override void Load()
		{
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

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPCTexture ??= ModContent.Request<Texture2D>(Texture);
			CryTexture ??= ModContent.Request<Texture2D>(Texture + "Cry");

			Vector2 drawOrigin = new(NPCTexture.Width() * 0.5f, NPC.height * 0.5f);
			Vector2 drawPos = NPC.Center - Main.screenPosition + new Vector2(0f, NPC.gfxOffY + 3);

			var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

			//crying after eye poke
			if (Flags.PokedLittleEye)
			{
				Main.EntitySpriteDraw(CryTexture.Value, drawPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);
			}
			else
			{
				Main.EntitySpriteDraw(NPCTexture.Value, drawPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);
			}

			return NPC.IsABestiaryIconDummy;
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

		public override bool CanBeHitByNPC(NPC attacker)
		{
			return false;
		}

		public override bool? CanBeHitByProjectile(Projectile projectile)
		{
			return false;
		}

		public override bool? CanBeHitByItem(Player player, Item item)
		{
			return false;
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

		public override void AI()
		{
			NPC.TargetClosest(true);

			NPC.spriteDirection = NPC.direction;

			NPC.velocity.X = 0;

			if (Flags.PokedLittleEye && !PlaySound)
			{
				SoundEngine.PlaySound(PokeSound, NPC.Center);
				PlaySound = true;
			}
		}
	}
}