using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Content.Projectiles.Pets;
using Spooky.Content.Buffs.Pets;

namespace Spooky.Content.Items.Pets
{
	public class FriendlyShroom : ModItem
	{
		private Asset<Texture2D> CapTexture;

		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.Fish);
			Item.width = 40;
			Item.height = 38;
			Item.noUseGraphic = true;
			Item.shoot = ModContent.ProjectileType<MushroomFriendPet>();
			Item.buffType = ModContent.BuffType<MushroomFriendPetBuff>();
		}

        public override bool? UseItem(Player player)
		{
			if (player.whoAmI == Main.myPlayer)
            {
                player.AddBuff(Item.buffType, 2, true);
            }

			return true;
        }

		public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
		{
			CapTexture ??= ModContent.Request<Texture2D>(Texture + "Cap");

			Vector2 drawOrigin = new Vector2(Terraria.GameContent.TextureAssets.Item[Item.type].Value.Width * 0.5f, Item.height * 0.5f);
			
			Main.spriteBatch.Draw(CapTexture.Value, position, null, Main.LocalPlayer.shirtColor.MultiplyRGBA(drawColor), 0f, drawOrigin, scale, SpriteEffects.None, 0f);
		}

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			CapTexture ??= ModContent.Request<Texture2D>(Texture + "Cap");

			Vector2 drawOrigin = new Vector2(Terraria.GameContent.TextureAssets.Item[Item.type].Value.Width * 0.5f, Item.height * 0.5f);

			Main.spriteBatch.Draw(CapTexture.Value, Item.Center - Main.screenPosition, null, Main.LocalPlayer.shirtColor.MultiplyRGBA(lightColor), rotation, drawOrigin, scale, SpriteEffects.None, 0f);
		}
	}
}