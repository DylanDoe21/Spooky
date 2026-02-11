using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Content.Tiles.Pylon;
using Spooky.Content.UserInterfaces;

namespace Spooky.Content.NPCs.Friendly
{
	public class LittleEyeSleeping : ModNPC
	{
		public Vector2 modifier = new(-200, -75);
		
		private static Asset<Texture2D> UITexture;

		public static readonly SoundStyle TalkSound = new("Spooky/Content/Sounds/LittleEye/Talk", SoundType.Sound) { Volume = 2f, PitchVariance = 0.75f };

		public override void Load()
		{
			UITexture = ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/DialogueUILittleEye");
		}

		public override void SetStaticDefaults()
		{
			NPCID.Sets.ActsLikeTownNPC[Type] = true;
			NPCID.Sets.NoTownNPCHappiness[Type] = true;

			NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
		}

		public override void SetDefaults()
		{
			NPC.lifeMax = 250;
			NPC.damage = 0;
			NPC.defense = 25;
            NPC.width = 20;
			NPC.height = 40;
			NPC.townNPC = true;
			NPC.friendly = true;
			NPC.immortal = true;
			NPC.dontTakeDamage = true;
			NPC.dontCountMe = true;
            TownNPCStayingHomeless = true;
            NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
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
			DialogueChain chain = new();
			chain.Add(new(UITexture.Value, NPC,
			Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.Intro1"),
			Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.IntroPlayerResponse1"),
			TalkSound, 2f, 0f, modifier, NPCID: NPC.type))
			.Add(new(UITexture.Value, NPC,
			Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.Intro2"),
			Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.IntroPlayerResponse2"),
			TalkSound, 2f, 0f, modifier, NPCID: NPC.type))
			.Add(new(UITexture.Value, NPC,
			Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.Intro3"),
			Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.IntroPlayerResponse3"),
			TalkSound, 2f, 0f, modifier, NPCID: NPC.type))
			.Add(new(UITexture.Value, NPC,
			Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.Intro4"),
			Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.IntroPlayerResponse4"),
			TalkSound, 2f, 0f, modifier, NPCID: NPC.type))
			.Add(new(UITexture.Value, NPC,
			Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.Intro5"),
			Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.IntroPlayerResponse5"),
			TalkSound, 2f, 0f, modifier, NPCID: NPC.type))
			.Add(new(UITexture.Value, NPC, null, null, TalkSound, 2f, 0f, modifier, true));
			chain.OnPlayerResponseTrigger += PlayerResponse;
			chain.OnEndTrigger += EndDialogue;
			DialogueUI.Visible = true;
			DialogueUI.Add(chain);

			return string.Empty;
		}

		public static void PlayerResponse(Dialogue dialogue, string Text, int ID)
		{
			Dialogue newDialogue = new(ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/DialogueUIPlayer").Value, Main.LocalPlayer,
			Text, null, SoundID.Item1, 2f, 0f, default, NotPlayer: false);
			DialogueUI.Visible = true;
			DialogueUI.Add(newDialogue);
		}

		public static  void EndDialogue(Dialogue dialogue, int ID)
		{
			DialogueUI.Visible = false;
		}

        public override void AI()
		{
			if (Main.netMode != NetmodeID.MultiplayerClient) 
			{
				NPC.homeless = false;
				NPC.homeTileX = -1;
				NPC.homeTileY = -1;
				NPC.netUpdate = true;
			}

            foreach (var player in Main.player)
            {
                if (!player.active) continue;
                if (player.talkNPC == NPC.whoAmI)
                {
                    NPC.Transform(ModContent.NPCType<LittleEye>());
					SpawnItem(ModContent.ItemType<SpookyHellPylonItem>(), 1);
                    return;
                }
            }
        }

		public void SpawnItem(int Type, int Amount)
        {
            int newItem = Item.NewItem(NPC.GetSource_DropAsItem(), Main.LocalPlayer.Hitbox, Type, Amount);

			if (Main.netMode == NetmodeID.MultiplayerClient && newItem >= 0)
			{
				NetMessage.SendData(MessageID.SyncItem, -1, -1, null, newItem, 1f);
			}
        }
    }
}