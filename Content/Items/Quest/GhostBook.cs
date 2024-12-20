using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Core;
using Spooky.Content.Projectiles.Cemetery;

namespace Spooky.Content.Items.Quest
{
	public class GhostBook : ModItem
	{
		public int SummonMode = 1;

		private Asset<Texture2D> GlowTexture1;
		private Asset<Texture2D> GlowTexture2;
		private Asset<Texture2D> GlowTexture3;

		public override void SaveData(TagCompound tag)
		{
			tag["TomeSummonMode"] = SummonMode;
		}

		public override void LoadData(TagCompound tag)
		{
			SummonMode = tag.GetInt("TomeSummonMode");
		}

		public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 42;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.accessory = true;
            Item.rare = ItemRarityID.Green;  
            Item.value = Item.buyPrice(gold: 10);
        }

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			switch (SummonMode)
			{
				case 1:
				{
					player.GetModPlayer<SpookyPlayer>().GhostBookRed = true;

					player.GetDamage(DamageClass.Melee) += 0.12f;
					player.GetKnockback(DamageClass.Melee) += 0.12f;
					player.GetDamage(DamageClass.Ranged) += 0.12f;
					player.GetKnockback(DamageClass.Ranged) += 0.12f;

					bool NotSpawned = player.ownedProjectileCounts[ModContent.ProjectileType<BanditBruiserMinion>()] <= 0;
					if (NotSpawned && player.whoAmI == Main.myPlayer)
					{
						SoundEngine.PlaySound(SoundID.DD2_OgreHurt, player.Center);

						//leave the source as null for right now
						Projectile.NewProjectile(null, player.position.X + (float)(player.width / 2), player.position.Y - 25, 0f, 0f, ModContent.ProjectileType<BanditBruiserMinion>(), 40, 25f, player.whoAmI);
					}

					break;
				}

				case 2:
				{
					player.GetModPlayer<SpookyPlayer>().GhostBookGreen = true;

					player.GetDamage(DamageClass.Magic) += 0.12f;
					player.GetDamage(DamageClass.Summon) += 0.12f;
					player.manaCost -= 0.15f;

					bool NotSpawned = player.ownedProjectileCounts[ModContent.ProjectileType<BanditWizardMinion>()] <= 0;
					if (NotSpawned && player.whoAmI == Main.myPlayer)
					{
						SoundEngine.PlaySound(SoundID.DD2_GoblinScream with { Pitch = -0.25f }, player.Center);

						//leave the source as null for right now
						Projectile.NewProjectile(null, player.position.X + (float)(player.width / 2), player.position.Y - 25, 0f, 0f, ModContent.ProjectileType<BanditWizardMinion>(), 40, 5f, player.whoAmI);
					}

					break;
				}

				case 3:
				{
					player.GetModPlayer<SpookyPlayer>().GhostBookBlue = true;

					player.statDefense += 10;
					player.lifeRegen += 15;

					bool NotSpawned = player.ownedProjectileCounts[ModContent.ProjectileType<BanditPriestMinion>()] <= 0;
					if (NotSpawned && player.whoAmI == Main.myPlayer)
					{
						SoundEngine.PlaySound(SoundID.DD2_DarkMageHurt with { Pitch = 1.1f }, player.Center);

						//leave the source as null for right now
						Projectile.NewProjectile(null, player.position.X + (float)(player.width / 2), player.position.Y - 25, 0f, 0f, ModContent.ProjectileType<BanditPriestMinion>(), 0, 0, player.whoAmI);
					}

					break;
				}
			}
		}

		public override bool CanUseItem(Player player)
        {
			SoundEngine.PlaySound(SoundID.Item72, player.Center);

			SummonMode++;
			if (SummonMode > 3)
			{
				SummonMode = 1;
			}

			return true;
		}

		public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
		{
			GlowTexture1 ??= ModContent.Request<Texture2D>("Spooky/Content/Items/Quest/GhostBookGlowRed");
			GlowTexture2 ??= ModContent.Request<Texture2D>("Spooky/Content/Items/Quest/GhostBookGlowGreen");
			GlowTexture3 ??= ModContent.Request<Texture2D>("Spooky/Content/Items/Quest/GhostBookGlowBlue");

			Vector2 drawOrigin = new Vector2(Terraria.GameContent.TextureAssets.Item[Item.type].Value.Width * 0.5f, Item.height * 0.5f);

			if (SummonMode == 1)
			{
				Main.spriteBatch.Draw(GlowTexture1.Value, position, null, Color.White, 0f, drawOrigin, scale, SpriteEffects.None, 0f);
			}
			if (SummonMode == 2)
			{
				Main.spriteBatch.Draw(GlowTexture2.Value, position, null, Color.White, 0f, drawOrigin, scale, SpriteEffects.None, 0f);
			}
			if (SummonMode == 3)
			{
				Main.spriteBatch.Draw(GlowTexture3.Value, position, null, Color.White, 0f, drawOrigin, scale, SpriteEffects.None, 0f);
			}
		}

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			GlowTexture1 ??= ModContent.Request<Texture2D>("Spooky/Content/Items/Quest/GhostBookGlowRed");
			GlowTexture2 ??= ModContent.Request<Texture2D>("Spooky/Content/Items/Quest/GhostBookGlowGreen");
			GlowTexture3 ??= ModContent.Request<Texture2D>("Spooky/Content/Items/Quest/GhostBookGlowBlue");

			Vector2 drawOrigin = new Vector2(Terraria.GameContent.TextureAssets.Item[Item.type].Value.Width * 0.5f, Item.height * 0.5f);

			if (SummonMode == 1)
			{
				Main.spriteBatch.Draw(GlowTexture1.Value, Item.Center - Main.screenPosition, null, Color.White, rotation, drawOrigin, scale, SpriteEffects.None, 0f);
			}
			if (SummonMode == 2)
			{
				Main.spriteBatch.Draw(GlowTexture2.Value, Item.Center - Main.screenPosition, null, Color.White, rotation, drawOrigin, scale, SpriteEffects.None, 0f);
			}
			if (SummonMode == 3)
			{
				Main.spriteBatch.Draw(GlowTexture3.Value, Item.Center - Main.screenPosition, null, Color.White, rotation, drawOrigin, scale, SpriteEffects.None, 0f);
			}
		}
	}
}
