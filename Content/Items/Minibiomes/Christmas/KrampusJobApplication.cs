using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Core;

namespace Spooky.Content.Items.Minibiomes.Christmas
{
	public class KrampusJobApplication : ModItem
	{
		public override void Load()
		{
			On_Main.DrawMiscMapIcons += DrawKrampusMapIcon;
		}

		public override void Unload()
		{
			On_Main.DrawMiscMapIcons -= DrawKrampusMapIcon;
		}

		private static void DrawKrampusMapIcon(On_Main.orig_DrawMiscMapIcons orig, Main self, SpriteBatch spriteBatch, Vector2 mapTopLeft, Vector2 mapX2Y2AndOff, Rectangle? mapRect, float mapScale, float drawScale, ref string mouseTextString)
		{
			orig(self, spriteBatch, mapTopLeft, mapX2Y2AndOff, mapRect, mapScale, drawScale, ref mouseTextString);
			DrawMapIcon(self, spriteBatch, mapTopLeft, mapX2Y2AndOff, mapRect, mapScale, drawScale, ref mouseTextString);
		}

		private static void DrawMapIcon(Main self, SpriteBatch spriteBatch, Vector2 mapTopLeft, Vector2 mapX2Y2AndOff, Rectangle? mapRect, float mapScale, float drawScale, ref string mouseTextString)
		{
			if (Main.gameMenu)
			{
				return;
			}

			if (Flags.DrawKrampusMapIcon && Flags.KrampusPosition != Vector2.Zero)
			{
				float alphaMult = 1f;
				Vector2 vec = Flags.KrampusPosition / 16f - mapTopLeft;
				vec *= mapScale;
				vec += mapX2Y2AndOff;
				vec = vec.Floor();
				bool draw = true;
				if (mapRect.HasValue)
				{
					Rectangle value2 = mapRect.Value;
					if (!value2.Contains(vec.ToPoint()))
					{
						draw = false;
					}
				}

				if (draw)
				{
					Texture2D texture = ModContent.Request<Texture2D>("Spooky/Content/Items/Minibiomes/Christmas/KrampusJobApplicationIcon").Value;

					Rectangle rectangle = texture.Frame();

					spriteBatch.Draw(texture, vec, rectangle, Color.White * alphaMult, 0f, rectangle.Size() / 2f, drawScale, 0, 0f);
				}
			}
		}

		public override void SetDefaults()
		{
			Item.width = 32;
			Item.height = 36;
			Item.useTime = 45;
			Item.useAnimation = 45;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.rare = ItemRarityID.Quest;
		}

		public override bool? UseItem(Player player)
        {
			Flags.DrawKrampusMapIcon = true;

			if (Main.netMode == NetmodeID.Server)
			{
				NetMessage.SendData(MessageID.WorldData);
			}

			return true;
		}
	}
}