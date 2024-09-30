using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Items.SpookyHell.Misc;
using Spooky.Content.Projectiles.SpookyHell;

namespace Spooky.Content.Items.SpookyHell
{
    public class BoogerBook : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 42;
            Item.mana = 3;
			Item.DamageType = DamageClass.Magic;
            Item.noMelee = true;
            Item.channel = true;
            Item.useTurn = true;
            Item.width = 46;
            Item.height = 48;
            Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 3;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.buyPrice(gold: 15);
            Item.UseSound = SoundID.Item46;
            Item.shoot = ModContent.ProjectileType<ControllableNose>();
            Item.shootSpeed = 0f;
        }

        public override Vector2? HoldoutOffset()
		{
			return new Vector2(-3, 0);
		}

        public override void AddRecipes()
        {
            CreateRecipe()
			.AddRecipeGroup("SpookyMod:DemoniteBars", 15)
			.AddIngredient(ModContent.ItemType<SnotGlob>(), 12)
            .AddTile(TileID.Anvils)
            .Register();
        }
    }
}