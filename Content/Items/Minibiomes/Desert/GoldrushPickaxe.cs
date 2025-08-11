using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Items.Minibiomes.Desert
{
    public class GoldrushPickaxe : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 15;
            Item.pick = 60;
			Item.DamageType = DamageClass.Melee;
			Item.autoReuse = true;
            Item.useTurn = true;
            Item.width = 38;
            Item.height = 36;
            Item.useTime = 12;
			Item.useAnimation = 12;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 2;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(gold: 2);
            Item.UseSound = SoundID.Item1;
        }
    }
}