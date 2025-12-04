using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Core;
using Spooky.Content.Projectiles.Pets;

namespace Spooky.Content.Buffs.Pets
{
	public class MushroomFriendPetBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.buffNoTimeDisplay[Type] = true;
			Main.vanityPet[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.buffTime[buffIndex] = 2;
			
			player.GetModPlayer<SpookyPlayer>().MushroomFriendPet = true;

			if (player.ownedProjectileCounts[ModContent.ProjectileType<MushroomFriendPet>()] < 1 && player.whoAmI == Main.myPlayer)
			{
				Projectile.NewProjectile(player.GetSource_Buff(buffIndex), player.Center.X, player.Center.Y, 0f, 0f, ModContent.ProjectileType<MushroomFriendPet>(), 0, 0f, player.whoAmI);
			}
		}

		public override void PostDraw(SpriteBatch spriteBatch, int buffIndex, BuffDrawParams drawParams)
        {
			spriteBatch.Draw(ModContent.Request<Texture2D>(Texture + "Cap").Value, drawParams.Position, null, 
			Main.LocalPlayer.shirtColor.MultiplyRGBA(drawParams.DrawColor), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
		}
	}
}
