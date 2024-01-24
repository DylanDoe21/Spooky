using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.SpiderCave;

namespace Spooky.Content.Items.SpiderCave
{
    public class SpiderLegStaff : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 18;
            Item.mana = 15;
			Item.DamageType = DamageClass.Magic;
			Item.autoReuse = false;
            Item.noMelee = true;
            Item.width = 54;
            Item.height = 60;
            Item.useTime = 18;
			Item.useAnimation = 18;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 3;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(gold: 2);
            Item.UseSound = SoundID.Item1;
        }
    }
}