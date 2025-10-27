using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Content.Items.SpiderCave.Misc;
using Spooky.Content.Projectiles.SpiderCave;

namespace Spooky.Content.Items.SpiderCave
{
    public class SporeWand : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 22;
            Item.mana = 20;
			Item.DamageType = DamageClass.Magic;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.width = 34;
            Item.height = 38;
            Item.useTime = 30;
			Item.useAnimation = 30;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 3;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.buyPrice(gold: 15);
            Item.UseSound = SoundID.Item69;
            Item.shoot = ModContent.ProjectileType<SporeWandBall>();
            Item.shootSpeed = 12f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<MiteMandibles>(), 10)
            .AddRecipeGroup("SpookyMod:AdamantiteBars", 8)
            .AddTile(TileID.MythrilAnvil)
            .Register();
        }
    }
}